using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GuxtModdingFramework.Maps;
using static PixelModdingFramework.Rendering;

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

        #region Display

        #region Initialise
        /// <summary>
        /// Resets the tileset buffer, and draws the given image on it
        /// </summary>
        /// <param name="t"></param>
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
        /// <summary>
        /// Resets the tileset's tile types on a new image
        /// </summary>
        void UpdateTilesetTileTypes()
        {
            tilesetTileTypes?.Dispose();
            tilesetTileTypes = new Bitmap(16 * parentMod.TileSize, 16 * parentMod.TileSize);
            RenderTiles(tilesetTileTypes, attributes, tileTypes, parentMod.TileSize);
        }
        #endregion
        void DrawSelectedTile(Graphics g)
        {
            g.DrawRectangle(new Pen(UI.Default.SelectedTileColor),
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

        #endregion


        #region Tileset Interaction

        private void tilesetPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnTileset(e.Location);
            var value = (p.Y * 16) + p.X;
            if (value <= byte.MaxValue && value != SelectedTile)
                SelectedTile = (byte)value;
        }

        #endregion

    }
}
