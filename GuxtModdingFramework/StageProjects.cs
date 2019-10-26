using GuxtModdingFramework.Maps;
using GuxtModdingFramework.Entities;
using GuxtModdingFramework.Images;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GuxtModdingFramework.StageProjects
{
    /// <summary>
    /// Represents a .stgprj file
    /// 
    /// Format of the files is:
    /// map:MapPath
    /// eve:EntityPath
    /// prt:TilesetPath
    /// att:TilesetPropertiesPath
    /// uni:SpritesheetPath 
    /// 
    /// </summary>
    public class StageProject
    {
        public Map? Map { get; set; }
        public List<Entity>? Entity { get; set; }
        public Bitmap? Tileset { get; set; }
        public Map? TilesetProperties { get; set; }
        public Bitmap? Spritesheet { get; set; }

        public string? MapPath { get; set; }
        public string? EntityPath { get; set; }
        public string? TilesetPath { get; set; }
        public string? TilesetPropertiesPath { get; set; }
        public string? SpritesheetPath { get; set; }

        /// <summary>
        /// Creates a new StageProject from a .stgprj file
        /// </summary>
        /// <param name="path">Path to the .stgproj file</param>
        public StageProject(string path, bool imagesScrambled = true)
        {
            string dir = Path.GetDirectoryName(path);
            string[] text = File.ReadAllLines(path);
            //Get paths from file
            foreach (var line in text)
            {
                if (line.Length <= 0)
                    continue;
                string[] parts = line.Split(':');
                string fullPath = Path.Combine(dir, parts[1]);
                switch (parts[0])
                {
                    case "map":
                        MapPath = fullPath;
                        break;
                    case "eve":
                        EntityPath = fullPath;
                        break;
                    case "prt":
                        TilesetPath = fullPath;
                        break;
                    case "att":
                        TilesetPropertiesPath = fullPath;
                        break;
                    case "uni":
                        SpritesheetPath = fullPath;
                        break;
                }
            }
            //Load in valid ones
            if (!string.IsNullOrWhiteSpace(MapPath))
                Map = new Map(MapPath!);

            if (!string.IsNullOrWhiteSpace(EntityPath))
                Entity = PXEVE.Read(EntityPath!);

            if (!string.IsNullOrWhiteSpace(TilesetPath))
                using (Bitmap b = new Bitmap(TilesetPath))
                    Tileset = imagesScrambled ? Scrambler.Unscramble(b) : new Bitmap(b);

            if (!string.IsNullOrWhiteSpace(TilesetPropertiesPath))
                TilesetProperties = new Map(TilesetPropertiesPath!);

            if (!string.IsNullOrWhiteSpace(SpritesheetPath))
                using (Bitmap b = new Bitmap(SpritesheetPath))
                    Spritesheet = imagesScrambled ? Scrambler.Unscramble(b) : new Bitmap(b);
        }

        public override string ToString()
        {
            return $"map:{Path.GetFileName(MapPath)}\n" +
                   $"eve:{Path.GetFileName(EntityPath)}\n" +
                   $"prt:{Path.GetFileName(TilesetPath)}\n" +
                   $"att:{Path.GetFileName(TilesetPropertiesPath)}\n" +
                   $"uni:{Path.GetFileName(SpritesheetPath)}";
        }

        /// <summary>
        /// Saves the StageProject as a .stgprj
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            File.WriteAllText(path, this.ToString());
        }

        /// <summary>
        /// Saves all items in the StageProject to their respective parts
        /// </summary>
        public void SaveAll(bool imagesScrambled = true)
        {
            //These have all been checked
            Map?.Save(MapPath!);

            if(Entity != null)
                PXEVE.Write(Entity, EntityPath!);

            if (Tileset != null)
            {
                if (imagesScrambled)
                    using (Bitmap b = Scrambler.Scramble(Tileset))
                        b.Save(TilesetPath);
                else Tileset.Save(TilesetPath);
            }
            
            TilesetProperties?.Save(TilesetPropertiesPath!);

            if (Spritesheet != null)
            {
                if (imagesScrambled)
                    using (Bitmap b = Scrambler.Scramble(Spritesheet))
                        b.Save(SpritesheetPath);
                else
                    Spritesheet.Save(SpritesheetPath);
            }
        }
    }
}
