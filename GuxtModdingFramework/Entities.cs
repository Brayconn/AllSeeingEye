using System.Collections.Generic;
using System.IO;

namespace GuxtModdingFramework.Entities
{
    /// <summary>
    /// Represents a Guxt entity stored in a .PXEVE file
    /// </summary>
    public class Entity
    {
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

        public Entity(int x, int y, int type1, int type2)
        {
            X = x;
            Y = y;
            EntityID = type1;
            ExtraInfo = type2;
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
            List<Entity> contents = new List<Entity>();
            using (PXEVEReader pxr = new PXEVEReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                while (pxr.BaseStream.Position != pxr.BaseStream.Length)
                {
                    //contents.Add(new Entity(ReadVarInt(fs), ReadVarInt(fs), ReadVarInt(fs), ReadVarInt(fs)));
                    contents.Add(new Entity(pxr.Read7BitEncodedInt(), pxr.Read7BitEncodedInt(), pxr.Read7BitEncodedInt(), pxr.Read7BitEncodedInt()));
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
                foreach (var e in input)
                {
                    pxw.Write7BitEncodedInt(e.X);
                    pxw.Write7BitEncodedInt(e.Y);
                    pxw.Write7BitEncodedInt(e.EntityID);
                    pxw.Write7BitEncodedInt(e.ExtraInfo);
                }
            }
        }
    }
}
