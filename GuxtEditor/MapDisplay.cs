using GuxtModdingFramework.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuxtEditor
{
    partial class FormStageEditor
    {
        #region Display

        /// <summary>
        /// Image of the loaded map
        /// </summary>
        Bitmap baseMap;
        /// <summary>
        /// Image of the loaded map's tile types
        /// </summary>
        Bitmap mapTileTypes;

        #region Initialise
        void InitMap()
        {
            baseMap?.Dispose();
            baseMap = (Bitmap)CommonDraw.RenderTiles(map, baseTileset, parentMod.TileSize);
        }
        void InitMapTileTypes()
        {
            mapTileTypes?.Dispose();
            mapTileTypes = (Bitmap)CommonDraw.RenderTiles(map, tilesetTileTypes, parentMod.TileSize);
        }
        #endregion

        void DrawEntities(Graphics g)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var y = (entities[i].Y * parentMod.TileSize) / 2;
                var x = (entities[i].X * parentMod.TileSize) / 2;

                if(entitySpritesToolStripMenuItem.Checked && entities[i].EntityID < EntityIcons.Images.Count)
                    g.DrawImage(EntityIcons.Images[entities[i].EntityID],
                        x, y, parentMod.IconSize, parentMod.IconSize);

                var isSelected = selectedEntities.Contains(entities[i]);
                if (entityBoxesToolStripMenuItem.Checked || isSelected)
                    g.DrawRectangle(new Pen(isSelected ? UI.Default.SelectedEntityBoxColor : UI.Default.EntityBoxColor),
                        x, y, (parentMod.TileSize / 2) - 1, (parentMod.TileSize / 2) - 1);
            }
        }

        const int ScreenWidth = 8;
        const int ScreenHeight = 10;
        private void DrawScreenPreview(Graphics g)
        {
            g.DrawRectangle(new Pen(UI.Default.ScreenPreviewColor),
                hScreenPreviewScrollBar.Value,
                vScreenPreviewScrollBar.Value,
                (ScreenWidth * parentMod.TileSize) - 1,
                (ScreenHeight * parentMod.TileSize) - 1);
        }

        /// <summary>
        /// Draws the mouse selection overlay from grid spaces p to p2 (or p to p, if nothing is provided for p2)
        /// </summary>
        /// <param name="g">Graphics to draw to</param>
        /// <param name="p">Start grid space</param>
        /// <param name="p2">End grid space</param>
        private void DrawMouseOverlay(Graphics g, Point p, Point? p2 = null)
        {
            int x,y, width, height;
            if(p2 != null)
            {
                x = Math.Min(p.X, ((Point)p2).X);
                y = Math.Min(p.Y, ((Point)p2).Y);                               
                width = Math.Max(p.X, ((Point)p2).X) - x + 1;
                height = Math.Max(p.Y, ((Point)p2).Y) - y + 1;
            }
            else
            {
                x = p.X;
                y = p.Y;
                width = 1;
                height = 1;
            }
            x *= gridSize;
            y *= gridSize;
            width = (width * gridSize) - 1;
            height = (height * gridSize) - 1;

            g.DrawRectangle(new Pen(UI.Default.CursorColor), x, y, width, height);
        }
        /// <summary>
        /// Displays the map to the user, with the selection box at point p to p2
        /// </summary>
        /// <param name="p">Start grid space</param>
        /// <param name="p2">End grid space</param>
        void DisplayMap(Point? p = null, Point? p2 = null)
        {
            Bitmap mapImage = new Bitmap(baseMap);
            using (Graphics g = Graphics.FromImage(mapImage))
            {
                if (tileTypesToolStripMenuItem.Checked)
                    g.DrawImage(mapTileTypes,0,0,mapTileTypes.Width,mapTileTypes.Height);
                if (entitySpritesToolStripMenuItem.Checked || entityBoxesToolStripMenuItem.Checked || userHasSelectedEntities)
                    DrawEntities(g);
                if (screenPreviewToolStripMenuItem.Checked)
                    DrawScreenPreview(g);
                if (p != null)
                    DrawMouseOverlay(g, (Point)p, p2);
            }            
            mapPictureBox.Image?.Dispose();
            mapPictureBox.Image = CommonDraw.Scale(mapImage, ZoomLevel);
            mapImage.Dispose();
        }               

        #endregion


    }
}
