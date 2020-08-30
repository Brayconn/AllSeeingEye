using PixelModdingFramework;
using System;
using System.Collections.Generic;
using System.IO;

namespace GuxtModdingFramework.Maps
{
    public enum ResizeModes
    {
        Buffer,
        Logical
    }
    public class Map : IMap<List<byte>>
    {
        public event EventHandler? MapResized;
        public event EventHandler? MapResizing;

        private void NotifyMapResized()
        {
            MapResized?.Invoke(this, new EventArgs());
        }
        private void NotifyMapResizing()
        {
            MapResizing?.Invoke(this, new EventArgs());
        }

        public int PrefferedBufferSize => Width * Height;

        public short Width { get; private set; }

        public short Height { get; private set; }

        public List<byte> Tiles { get; set; }

        public void Resize(short width, short height, ResizeModes mode, bool shrinkBuffer = true)
        {
            NotifyMapResizing();
            switch (mode)
            {
                case ResizeModes.Buffer:
                    var newBuffer = width * height;
                    if (Tiles.Count != newBuffer)
                    {
                        if (Tiles.Count < newBuffer)
                        {
                            Tiles.AddRange(new byte[newBuffer - Tiles.Count]);
                        }
                        else if (shrinkBuffer)
                        {
                            Tiles.RemoveRange(newBuffer, Tiles.Count - newBuffer);
                        }
                    }
                    break;
                case ResizeModes.Logical:
                    if (width != Width)
                    {
                        if (width < Width)
                        {
                            for (int row = 0; row < Height; row++)
                                Tiles.RemoveRange((row * width) + width, Width - width);
                        }
                        else
                        {
                            var diff = new byte[width - Width];
                            for(int row = 0; row < Height; row++)
                                Tiles.InsertRange((row * width) + Width, diff);
                        }
                    }
                    if(height != Height)
                    {
                        if(height < Height)
                        {
                            for(int i =0; i < Height - height; i++)
                                Tiles.RemoveRange(Tiles.Count - Height, width);
                        }
                        else
                        {
                            var buff = new byte[width];
                            for (int i = 0; i < height - Height; i++)
                                Tiles.AddRange(buff);
                        }
                    }
                    break;
            }
            Width = width;
            Height = height;
            NotifyMapResized();
        }

        /// <summary>
        /// Creates a new Map.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public Map(short w, short h)
        {
            Width = w;
            Height = h;
            Tiles = new List<byte>(new byte[w * h]);
        }

        public Map(string path)
        {
            using (BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                Width = br.ReadInt16();
                Height = br.ReadInt16();
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
