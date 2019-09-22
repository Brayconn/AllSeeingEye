using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GuxtModdingFramework
{
    public class Mod
    {
        public bool ImagesScrambeled { get; set; } = true;

        public string DataPath { get; set; }

        public List<string> Maps { get; set; }
        public List<string> Entities { get; set; }
        public List<string> Images { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Projects { get; set; }

        public static Mod FromDataFolder(string path)
        {
            return new Mod()
            {
                DataPath = path,

                Maps = Directory.GetFiles(path, "*.pxmap").Select(x => Path.GetFileName(x)).ToList(),

                Entities = Directory.GetFiles(path, "*.pxeve").Select(x => Path.GetFileName(x)).ToList(),

                Images = Directory.GetFiles(path, "*.pximg").Select(x => Path.GetFileName(x)).ToList(),

                Attributes = Directory.GetFiles(path, "*.pxatrb").Select(x => Path.GetFileName(x)).ToList(),

                Projects = Directory.GetFiles(path, "*.stgprj").Select(x => Path.GetFileName(x)).ToList()
            };
        }

        public void Save(string path)
        {
            File.WriteAllText(path, (ImagesScrambeled ? "1" : "0") + DataPath);
        }

        public static Mod Load(string path)
        {
            string projFile = File.ReadAllText(path);
            Mod m = FromDataFolder(projFile.Substring(1));
            m.ImagesScrambeled = projFile[0] == '1';
            return m;
        }
    }
}
