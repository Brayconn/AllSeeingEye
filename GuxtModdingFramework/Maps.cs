using System.Collections.Generic;
using System.IO;

namespace GuxtModdingFramework.Maps
{
    public class Map
    {
        public ushort Width { get; set; }

        public ushort Height { get; set; }

        public List<byte> Tiles { get; set; }

        /// <summary>
        /// Clears the map and fills with 0s
        /// </summary>
        public void Init()
        {
            Tiles.Clear();
            while (Tiles.Count < Width * Height)
                Tiles.Add(0x00);
        }

        /// <summary>
        /// Creates a new Map. Use Init() to initialise the tiles
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public Map(ushort w, ushort h)
        {
            Width = w;
            Height = h;
            Tiles = new List<byte>(w * h);
        }

        public Map(string path)
        {
            using (BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                Width = br.ReadUInt16();
                Height = br.ReadUInt16();
                Tiles = new List<byte>(Width * Height);
                while (br.BaseStream.Position < br.BaseStream.Length)
                    Tiles.Add(br.ReadByte());
            }
        }

        public void Save(string path)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
            {
                bw.Write(Width);
                bw.Write(Height);
                bw.Write(Tiles.ToArray());
            }
        }
    }
}
