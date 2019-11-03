using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace GuxtModdingFramework.Entities
{
    /// <summary>
    /// Represents a Guxt entity stored in a .PXEVE file
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Unused variable
        /// </summary>
        public int Unused { get; set; }
        /// <summary>
        /// X position of the entity (in tiles)
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Y position of the entity (in ???)
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// The entity's type
        /// </summary>
        public int EntityID { get; set; }
        /// <summary>
        /// Extra info (music for music switcher)
        /// </summary>
        public int ExtraInfo { get; set; }

        public Entity(Entity e) : this(e.Unused, e.X, e.Y, e.EntityID, e.ExtraInfo) { }
        public Entity(int u, int x, int y, int id, int info)
        {
            Unused = u;
            X = x;
            Y = y;
            EntityID = id;
            ExtraInfo = info;
        }

        //rough list of "everything" that belongs in an Entity struct?
        //public int cond;
        //position / velocity?
        //public int x, y, xm, ym;
        //spritesheet stuff?
        //public int xoff, yoff, w, h;

        //public int surf, type, state;

        //public int count1, count2, count3;

        //public int health, damage, flag, shock, score;

        //public int type2, child, rot1, count4, num, destroyHitVoice;

        //public Rectangle rect;
    }

    public static class PXEVE
    {
        /// <summary>
        /// BinaryReader + access to Read7BitEncodedInt()
        /// </summary>
        public class PXEVEReader : BinaryReader
        {
            public PXEVEReader(Stream stream) : base(stream) { }
            public new int Read7BitEncodedInt()
            {
                return base.Read7BitEncodedInt();
            }
        }
        /// <summary>
        /// Parses a PXEVE file to a list of Entities
        /// </summary>
        /// <param name="path">Path of the file to parse</param>
        /// <returns>List of entities in the PXEVE file</returns>
        public static List<Entity> Read(string path)
        {
            List<Entity> contents; 
            using (PXEVEReader pxr = new PXEVEReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                contents = new List<Entity>(pxr.Read7BitEncodedInt());
                try
                {
                    while (pxr.BaseStream.Position != pxr.BaseStream.Length)
                    {
                        //contents.Add(new Entity(ReadVarInt(fs), ReadVarInt(fs), ReadVarInt(fs), ReadVarInt(fs)));
                        contents.Add(new Entity(pxr.Read7BitEncodedInt(), pxr.Read7BitEncodedInt(), pxr.Read7BitEncodedInt(), pxr.Read7BitEncodedInt(), pxr.Read7BitEncodedInt()));
                    }
                }
                catch(EndOfStreamException)
                {
                    //TODO maybe warn?
                }
            }
            return contents;
        }

        /// <summary>
        /// BinaryWriter + access to Write7BitEncodedInt()
        /// </summary>
        public class PXEVEWriter : BinaryWriter
        {
            public PXEVEWriter(Stream stream) : base(stream) { }
            public new void Write7BitEncodedInt(int i)
            {
                base.Write7BitEncodedInt(i);
            }
        }
        /// <summary>
        /// Creates a PXEVE file using the given list of Entities
        /// </summary>
        /// <param name="input"></param>
        /// <param name="path"></param>
        public static void Write(IList<Entity> input, string path)
        {
            using (PXEVEWriter pxw = new PXEVEWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
            {
                pxw.Write7BitEncodedInt(input.Count);
                foreach (var e in input)
                {
                    pxw.Write7BitEncodedInt(e.Unused);
                    pxw.Write7BitEncodedInt(e.X);
                    pxw.Write7BitEncodedInt(e.Y);
                    pxw.Write7BitEncodedInt(e.EntityID);
                    pxw.Write7BitEncodedInt(e.ExtraInfo);
                }
            }
        }
    }
}
