using LayeredPictureBox;
using System;
using System.Drawing;
using static GuxtEditor.SharedGraphics;

namespace GuxtEditor
{
    partial class FormStageEditor
    {
        
        #region layers

        const int baseMapLayer = 0;
        Layer<Image> baseMap => mapLayeredPictureBox.Layers[baseMapLayer];

        const int mapTileTypesLayer = baseMapLayer + 1;
        Layer<Image> mapTileTypes => mapLayeredPictureBox.Layers[mapTileTypesLayer];

        const int mapTileGridLayer = mapTileTypesLayer + 1;
        Layer<Image> mapTileGrid => mapLayeredPictureBox.Layers[mapTileGridLayer];

        const int entityIconsLayer = mapTileGridLayer + 1;
        Layer<Image> entityIcons => mapLayeredPictureBox.Layers[entityIconsLayer];

        const int entitySquaresLayer = entityIconsLayer + 1;
        Layer<Image> entityBoxes => mapLayeredPictureBox.Layers[entitySquaresLayer];

        const int selectedEntitySquaresLayer = entitySquaresLayer + 1;
        Layer<Image> selectedEntityBoxes => mapLayeredPictureBox.Layers[selectedEntitySquaresLayer];

        const int screenPreviewLayer = selectedEntitySquaresLayer + 1;
        Layer<Image> screenPreview => mapLayeredPictureBox.Layers[screenPreviewLayer];

        const int mouseOverlayLayer = screenPreviewLayer + 1;
        Layer<Image> mouseOverlay => mapLayeredPictureBox.Layers[mouseOverlayLayer];

        #endregion

        #region grid

        void RedrawGrid()
        {
            using(var g = Graphics.FromImage(mapTileGrid.Image))
            {
                g.Clear(Color.Transparent);

                var width = map.Width * parentMod.TileSize;
                var height = map.Height * parentMod.TileSize;

                //vertical lines
                for (int i = 0; i < map.Width; i++)
                {
                    var x = (i * parentMod.TileSize) - 1;
                    g.DrawLine(new Pen(UI.Default.GridColor), x, 0, x, height);
                }
                //horizontal lines
                for (int i = 1; i < map.Height; i++)
                {
                    var y = (i * parentMod.TileSize) - 1;
                    g.DrawLine(new Pen(UI.Default.GridColor), 0, y, width, y);
                }
            }
        }

#endregion

#region entity

        void RedrawAllEntityLayers()
        {
            DrawEntityIcons();
            DrawEntityBoxes();
            DrawSelectedEntityBoxes();
        }

        void DrawEntityIcons()
        {
            using (var g = Graphics.FromImage(entityIcons.Image))
            {
                g.Clear(Color.Transparent);

                foreach(var e in Entities)
                {                    
                    if (e.EntityID < EntityIcons.Images.Count)
                    {
                        var y = (e.Y * parentMod.TileSize) / 2;
                        var x = (e.X * parentMod.TileSize) / 2;

                        g.DrawImage(EntityIcons.Images[e.EntityID], x, y, parentMod.IconSize, parentMod.IconSize);
                    }
                }
            }
        }
        void DrawEntityBoxes()
        {
            using (var g = Graphics.FromImage(entityBoxes.Image))
            {
                g.Clear(Color.Transparent);

                //used to not draw selected entities, but that broke inserting new entities
                foreach (var e in Entities)
                {
                    var y = (e.Y * parentMod.TileSize) / 2;
                    var x = (e.X * parentMod.TileSize) / 2;

                    g.DrawRectangle(new Pen(UI.Default.EntityBoxColor), x, y, (parentMod.TileSize / 2) - 1, (parentMod.TileSize / 2) - 1);
                }
            }
        }
        void DrawSelectedEntityBoxes()
        {
            using (var g = Graphics.FromImage(selectedEntityBoxes.Image))
            {
                g.Clear(Color.Transparent);

                //don't need to draw selected entities on this layer
                foreach (var e in selectedEntities)
                {
                    var y = (e.Y * parentMod.TileSize) / 2;
                    var x = (e.X * parentMod.TileSize) / 2;

                    g.DrawRectangle(new Pen(UI.Default.SelectedEntityBoxColor), x, y, (parentMod.TileSize / 2) - 1, (parentMod.TileSize / 2) - 1);
                }
            }
        }
#endregion

#region screen preview
        const int GuxtScreenWidth = 8;
        const int GuxtScreenHeight = 10;
        private void UpdateScreenPreviewLocation(int h, int v)
        {
            hScreenPreviewScrollBar.Value = h;
            vScreenPreviewScrollBar.Value = v;
            screenPreview.Location = new Point(h, v);
        }
        private void InitScreenPreview()
        {
            var sp = new Bitmap(GuxtScreenWidth * parentMod.TileSize, GuxtScreenHeight * parentMod.TileSize);
            using (var g = Graphics.FromImage(sp))
            {
                g.DrawRectangle(new Pen(UI.Default.ScreenPreviewColor), 0, 0, sp.Width - 1, sp.Height - 1);
            }
            screenPreview.Image = sp;
        }
#endregion

#region mouse stuff

        /// <summary>
        /// Sets the mouse back to the right size for whatever edit mode we're in
        /// </summary>
        void RestoreMouseSize()
        {
            mouseOverlay.Image = editMode switch
            {
                EditModes.Tile => MakeMouseImage(SelectedTiles.Width * parentMod.TileSize, SelectedTiles.Height * parentMod.TileSize, UI.Default.CursorColor),
                _ => MakeMouseImage(gridSize, gridSize, UI.Default.CursorColor),
            };
        }

        void MoveMouse(Point gridPosition)
        {
            mouseOverlay.Location = new Point(gridPosition.X * gridSize, gridPosition.Y * gridSize);
        }

#endregion
    }
}
