using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using GuxtModdingFramework.Maps;
using System.Drawing.Imaging;

namespace GuxtEditor
{
    static class CommonDraw
    {
        /// <summary>
        /// Scales an image up
        /// </summary>
        /// <param name="i">Source image</param>
        /// <param name="scale">Scale multiplier</param>
        /// <param name="mode">How the upscale will look</param>
        /// <returns></returns>
        public static Image Scale(Image i, int scale, InterpolationMode mode = InterpolationMode.NearestNeighbor)
        {
            Bitmap o = new Bitmap(i.Width * scale, i.Height * scale);
            using (Graphics g = Graphics.FromImage(o))
            {
                g.InterpolationMode = mode;
                g.DrawImage(i, 0, 0, o.Width, o.Height);
            }
            return o;
        }

        public static Image RenderTiles(Map tileSource, Bitmap tileset, int tileSize, CompositingMode mode = CompositingMode.SourceOver)
        {
            Image img = new Bitmap(tileSource.Width * tileSize, tileSource.Height * tileSize);
            RenderTiles(img, tileSource, tileset, tileSize, mode);
            return img;
        }
        public static void RenderTiles(Image i, Map tileSource, Bitmap tileset, int tileSize, CompositingMode mode = CompositingMode.SourceOver)
        {
            using(Graphics g = Graphics.FromImage(i))
            {
                g.CompositingMode = mode;
                RenderTiles(g, tileSource, tileset, tileSize);
            }
        }
        public static void RenderTiles(Graphics g, Map tileSource, Bitmap tileset, int tileSize)
        {
            for (int i = 0; i < tileSource.Tiles.Count; i++)
            {
                DrawTile(g, tileSource, i, tileset, tileSize);
            }
        }

        public static void DrawTile(Image img, Map tileSource, int i, Bitmap tileset, int tileSize, CompositingMode mode = CompositingMode.SourceOver)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                g.CompositingMode = mode;
                DrawTile(g, tileSource, i, tileset, tileSize);
            }
        }
        public static void DrawTile(Graphics g, Map tileSource, int i, Bitmap tileset, int tileSize)
        {
            var x = (i % tileSource.Width) * tileSize;
            var y = (i / tileSource.Width) * tileSize;
            g.DrawImage(GetTile(tileSource.Tiles[i], tileset, tileSize), x, y, tileSize, tileSize);
        }

        public const int TilesetWidth = 16;
        public const int TilesetHeight = 16;
        /// <summary>
        /// Gets a single tile from a tileset
        /// </summary>
        /// <param name="tileSource"></param>
        /// <param name="i"></param>
        /// <param name="tileset"></param>
        /// <param name="tileSize"></param>
        /// <returns></returns>
        public static Image GetTile(byte tileValue, Bitmap tileset, int tileSize)
        {
            //yes, this is intentional to not use TilesetHeight
            var tilesetX = (tileValue % TilesetWidth) * tileSize;
            var tilesetY = (tileValue / TilesetWidth) * tileSize;
            return tileset.Clone(new Rectangle(tilesetX, tilesetY, tileSize, tileSize), PixelFormat.DontCare);
        }
    }
}
