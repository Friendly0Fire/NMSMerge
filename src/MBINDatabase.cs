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
    class MBINDatabase
    {
        public delegate MBINFile LoadMBINDelegate(PSARC pak, int offset);

        public Dictionary<string, PSARC> Paks { get; } = new Dictionary<string, PSARC>();
        public Dictionary<string, LoadMBINDelegate> LazyMBINs { get; } = new Dictionary<string, LoadMBINDelegate>();

        public void LoadMINs(string folder) => LoadMBINs(Directory.EnumerateFiles(folder, "*.pak"));

        public void LoadMBINs(IEnumerable<string> paths)
        {
            foreach(var pakPath in paths)
            {
                var pak = new PSARC(pakPath);
                Paks[pakPath] = pak;
                foreach (var entry in pak.TOC)
                {
                    if (Path.GetExtension(entry.FileName)?.ToLower() == ".mbin")
                        LazyMBINs[entry.FileName] = delegate
                        {
                            var decompressed = pak.DecompressFile(entry.FileName);
                            var stream = new MemoryStream(decompressed.BinaryFile);
                            return new MBINFile(stream, true);
                        };
                }
            }
        }
    }
}
