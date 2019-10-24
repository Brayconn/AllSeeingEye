using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuxtModdingFramework.Maps;

namespace GuxtEditor
{
    partial class FormStageEditor
    {
        /// <summary>
        /// Image of the loaded tileset
        /// </summary>
        Bitmap baseTileset;
        /// <summary>
        /// The tile types of the tileset
        /// </summary>
        Bitmap tilesetTileTypes;


        #region Initialise
        void InitTileset(Image t)
        {
            baseTileset?.Dispose();
            baseTileset = new Bitmap(16 * parentMod.TileSize, 16 * parentMod.TileSize);
            using (Graphics g = Graphics.FromImage(baseTileset))
            {
                g.Clear(Color.Black);
                g.DrawImage(t, 0, 0, t.Width, t.Height);
            }
        }
        void InitTilesetTileTypes()
        {
            tilesetTileTypes?.Dispose();
            tilesetTileTypes = new Bitmap(16 * parentMod.TileSize, 16 * parentMod.TileSize);
            DrawTiles(tilesetTileTypes, attributes, tileTypes);
        }
        #endregion
        void DrawSelectedTile(Graphics g)
        {
            g.DrawRectangle(new Pen(Color.LightGray),
                (SelectedTile % 16) * parentMod.TileSize,
                (SelectedTile / 16) * parentMod.TileSize,
                parentMod.TileSize - 1,
                parentMod.TileSize - 1);
        }
        void DisplayTileset()
        {
            var workingTileset = new Bitmap(baseTileset);
            using (Graphics g = Graphics.FromImage(workingTileset))
            {
                if (tileTypesToolStripMenuItem.Checked)
                    g.DrawImage(tilesetTileTypes,0,0, tilesetTileTypes.Width, tilesetTileTypes.Height);
                DrawSelectedTile(g);
            }
            tilesetPictureBox.Image?.Dispose();
            tilesetPictureBox.Image = workingTileset;
        }
    }
}
