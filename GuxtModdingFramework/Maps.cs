using PixelModdingFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GuxtModdingFramework.Maps
{
    public class Map : IMap<List<byte>>
    {
        public event Action MapResized = new Action(() => { });
        public int Size { get => Width * Height; }

        private short width;
        public short Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    width = value;
                    Resize();
                    MapResized();
                }
            }
        }

        private short height;
        public short Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    height = value;
                    Resize();
                    MapResized();
                }
            }
        }

        public List<byte> Tiles { get; set; }

        /// <summary>
        /// Clears the map and fills with 0s
        /// </summary>
        public void Init()
        {
            Tiles.Clear();
            Resize();
        }
        public void Resize()
        {
            while (Tiles.Count != Size)
            {
                if (Tiles.Count < Size)
                    Tiles.Add(0x00);
                else
                    Tiles.RemoveAt(Tiles.Count - 1);
            }            
        }

        /// <summary>
        /// Creates a new Map. Use Init() to initialise the tiles
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public Map(short w, short h)
        {
            Width = w;
            Height = h;
            Tiles = new List<byte>(w * h);
        }

        public Map(string path)
        {
            using (BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                width = br.ReadInt16();
                height = br.ReadInt16();
                Tiles = new List<byte>(Width * Height);
                while (br.BaseStream.Position < br.BaseStream.Length)
                    Tiles.Add(br.ReadByte());
                Resize();
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
