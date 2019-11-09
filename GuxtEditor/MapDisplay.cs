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
            baseMap = new Bitmap(map.Width * parentMod.TileSize, map.Height * parentMod.TileSize);
            DrawTiles(baseMap, map, baseTileset);

        }
        void InitMapTileTypes()
        {
            mapTileTypes?.Dispose();
            mapTileTypes = new Bitmap(map.Width * parentMod.TileSize, map.Height * parentMod.TileSize);
            DrawTiles(mapTileTypes, map, tilesetTileTypes);
        }
        #endregion

        void DrawEntities(Graphics g)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var entityIcon = parentMod.EntityIcons.Images[entities[i].EntityID];

                var y = (entities[i].Y * parentMod.TileSize) / 2;
                var x = (entities[i].X * parentMod.TileSize) / 2;

                g.DrawImage(entityIcon, x, y, parentMod.IconSize, parentMod.IconSize);

                var isSelected = selectedEntities.Contains(entities[i]);
                if (entityBoxesToolStripMenuItem.Checked || isSelected)
                    g.DrawRectangle(isSelected ? Pens.Aqua : Pens.Lime,
                        x, y, (parentMod.TileSize / 2) - 1, (parentMod.TileSize / 2) - 1);
            }
        }
        private void DrawMouseOverlay(Graphics g, Point p)
        {
            int x = p.X * gridSize;
            int y = p.Y * gridSize;

            int width = gridSize - 1;
            int height = gridSize - 1;

            g.DrawRectangle(new Pen(Color.LightGray), x, y, width, height);
        }
        void DisplayMap(Point? p = null)
        {
            Bitmap mapImage = new Bitmap(baseMap);
            using (Graphics g = Graphics.FromImage(mapImage))
            {
                if (tileTypesToolStripMenuItem.Checked)
                    g.DrawImage(mapTileTypes,0,0,mapTileTypes.Width,mapTileTypes.Height);
                if (entitySpritesToolStripMenuItem.Checked)
                    DrawEntities(g);
                if (p != null)
                    DrawMouseOverlay(g, (Point)p);
            }
            mapPictureBox.Image?.Dispose();
            mapPictureBox.Image = mapImage;
        }
    }
}
