using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libMBIN;
using PSArcHandler;
using System.IO;

namespace NMSMerge
{
    struct MBINMemory
    {
        public MBINFile mbin;
        public MemoryStream mem;
    }
    class MBINDatabase : IDisposable
    {
        private delegate MBINMemory LoadMBINDelegate();

        readonly Dictionary<string, LoadMBINDelegate> _lazyMBINs = new Dictionary<string, LoadMBINDelegate>();
        readonly Dictionary<string, MBINMemory> _loadedMBINs = new Dictionary<string, MBINMemory>();
        private readonly List<PSARC> _paks = new List<PSARC>();

        public MBINMemory this[string name]
        {
            get
            {
                if (_loadedMBINs.ContainsKey(name))
                    return _loadedMBINs[name];
                else if (_lazyMBINs.ContainsKey(name))
                {
                    var mbin = _loadedMBINs[name] = _lazyMBINs[name]();
                    _lazyMBINs.Remove(name);
                    return mbin;
                }
                else
                    throw new KeyNotFoundException();
            }
        }

        public void LoadMBINsInFolder(string folder, bool includeSubdirectories = false) => LoadMBINsFromPaks(Directory.EnumerateFiles(folder, "*.pak",
            includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

        public void LoadMBINsFromPaks(IEnumerable<string> paths)
        {
            foreach(var pakPath in paths)
                LoadMBINsFromPak(pakPath);
        }

        public void LoadMBINsFromPak(string pakPath)
        {
            var pak = new PSARC(pakPath);
            _paks.Add(pak);
            foreach (var entry in pak.TOC)
            {
                if (Path.GetExtension(entry.FileName)?.ToLower() == ".mbin")
                    _lazyMBINs[entry.FileName] = delegate
                    {
                        var decompressed = pak.DecompressFile(entry.FileName);
                        var stream = new MemoryStream(decompressed.BinaryFile);
                        return new MBINMemory
                        {
                            mbin = new MBINFile(stream),
                            mem = stream
                        };
                    };
            }
        }

        public void AddMBIN(string name, MBINMemory mbin)
        {
            _loadedMBINs[name] = mbin;
        }

        public void SaveAllToPak(string pakPath)
        {
            foreach (var lazy in _lazyMBINs)
            {
                var mem = this[lazy.Key];
            }

            PSARC pak = new PSARC();
            foreach (var mbin in _loadedMBINs)
            {
                mbin.Value.mbin.Save();
                var bytes = mbin.Value.mem.GetBuffer();
                pak.CompressFile(mbin.Key, bytes);
            }

            // TODO: Fix PSArcHandler to handle creating brand new PAK files
        }

        public void Dispose()
        {
            foreach (var mbin in _loadedMBINs)
                mbin.Value.mbin.Dispose();

            foreach(var pak in _paks)
                pak.Dispose();
        }
    }
}
