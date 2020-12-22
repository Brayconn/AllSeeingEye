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
    public class Map : IMap<short, List<byte>, byte>
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

        public int CurrentBufferSize => Tiles.Count;
        public int PrefferedBufferSize => Width * Height;

        public short Width { get; private set; }

        public short Height { get; private set; }

        public List<byte> Tiles { get; set; }

        public void Resize(short width, short height, ResizeModes mode, bool shrinkBuffer = true)
        {
            NotifyMapResizing();
            int newBufferLength;
            switch (mode)
            {
                case ResizeModes.Buffer:
                    newBufferLength = width * height;
                    if (CurrentBufferSize != newBufferLength)
                    {
                        if (CurrentBufferSize < newBufferLength)
                        {
                            Tiles.AddRange(new byte[newBufferLength - CurrentBufferSize]);
                        }
                        else if (shrinkBuffer)
                        {
                            Tiles.RemoveRange(newBufferLength, CurrentBufferSize - newBufferLength);
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
                        newBufferLength = width * height;
                        //any bytes after this point were not visible before this resize (or don't exist yet)
                        var hiddenStart = width * Height;
                        //any bytes after this point are still not visible (or don't exist yet)
                        var hiddenEnd = Math.Min(newBufferLength, CurrentBufferSize);

                        //if the buffer size needs to change...
                        if (newBufferLength != CurrentBufferSize)
                        {
                            //add visible bytes
                            if (CurrentBufferSize < newBufferLength)
                            {
                                Tiles.AddRange(new byte[newBufferLength - CurrentBufferSize]);
                            }
                            //remove non-visible bytes
                            else if (shrinkBuffer)
                            {
                                Tiles.RemoveRange(newBufferLength, CurrentBufferSize - newBufferLength);
                            }
                        }
                        //clear any previously hidden bytes
                        for (int i = hiddenStart; i < hiddenEnd; i++)
                            Tiles[i] = 0x00;
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
