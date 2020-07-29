using System.Drawing;
using System.Windows.Forms;
using LayeredPictureBox;
using static PixelModdingFramework.Rendering;
using static GuxtEditor.SharedGraphics;

namespace GuxtEditor
{
    partial class FormStageEditor
    {
        const int baseTilesetLayer = 0;
        Layer<Image> baseTileset => tilesetLayeredPictureBox.Layers[baseTilesetLayer];

        const int tilesetTileTypesLayer = baseTilesetLayer + 1;
        Layer<Image> tilesetTileTypes => tilesetLayeredPictureBox.Layers[tilesetTileTypesLayer];

        const int tilesetMouseOverlayLayer = tilesetTileTypesLayer + 1;
        Layer<Image> tilesetMouseOverlay => tilesetLayeredPictureBox.Layers[tilesetMouseOverlayLayer];

        /// <summary>
        /// Resets the tileset buffer, and draws the given image on it
        /// </summary>
        /// <param name="t"></param>
        void InitTilesetAndTileTypes(Image t)
        {
            tilesetLayeredPictureBox.UnlockCanvasSize();

            var tileset = new Bitmap(16 * parentMod.TileSize, 16 * parentMod.TileSize);
            using (Graphics g = Graphics.FromImage(tileset))
            {
                g.Clear(Color.Black);
                g.DrawImage(t, 0, 0, t.Width, t.Height);
            }
            
            var tiletypes = new Bitmap(16 * parentMod.TileSize, 16 * parentMod.TileSize);
            RenderTiles(tiletypes, attributes, tileTypes, parentMod.TileSize);

            baseTileset.Image = tileset;
            tilesetTileTypes.Image = tiletypes;
            tilesetMouseOverlay.Image = MakeMouseImage(parentMod.TileSize, parentMod.TileSize, UI.Default.SelectedTileColor);

            tilesetLayeredPictureBox.LockCanvasSize();
        }
        void MoveTileSelection()
        {
            tilesetMouseOverlay.Location = new Point((SelectedTile % 16) * parentMod.TileSize, (SelectedTile / 16) * parentMod.TileSize);
        }

        private void tilesetPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnTileset(e.Location);
            var value = (p.Y * 16) + p.X;
            if (value <= byte.MaxValue && value != SelectedTile)
                SelectedTile = (byte)value;
        }
    }
}
