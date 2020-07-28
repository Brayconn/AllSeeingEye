using LayeredPictureBox;
using System;
using System.Drawing;

namespace GuxtEditor
{
    partial class FormStageEditor
    {
        
        #region layers

        const int baseMapLayer = 0;
        Layer<Image> baseMap { get => mapLayeredPictureBox.Layers[baseMapLayer]; }

        const int mapTileTypesLayer = baseMapLayer + 1;
        Layer<Image> mapTileTypes { get => mapLayeredPictureBox.Layers[mapTileTypesLayer]; }

        const int entityIconsLayer = mapTileTypesLayer + 1;
        Layer<Image> entityIcons { get => mapLayeredPictureBox.Layers[entityIconsLayer]; }

        const int entitySquaresLayer = entityIconsLayer + 1;
        Layer<Image> entityBoxes { get => mapLayeredPictureBox.Layers[entitySquaresLayer]; }
        
        const int selectedEntitySquaresLayer = entitySquaresLayer + 1;
        Layer<Image> selectedEntityBoxes { get => mapLayeredPictureBox.Layers[selectedEntitySquaresLayer]; }

        const int screenPreviewLayer = selectedEntitySquaresLayer + 1;
        Layer<Image> screenPreview { get => mapLayeredPictureBox.Layers[screenPreviewLayer]; }

        const int mouseOverlayLayer = screenPreviewLayer + 1;
        Layer<Image> mouseOverlay { get => mapLayeredPictureBox.Layers[mouseOverlayLayer]; }

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

                foreach(var e in entities)
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
                foreach (var e in entities)
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
        private void UpdateScreenPreviewLocation()
        {
            screenPreview.Location = new Point(hScreenPreviewScrollBar.Value, vScreenPreviewScrollBar.Value);
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
        /// Generates a rectangle from two arbitrary points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static Rectangle GetRect(Point p1, Point p2)
        {
            int x, y, width, height;
            x = Math.Min(p1.X, p2.X);
            y = Math.Min(p1.Y, p2.Y);
            width = Math.Max(p1.X, p2.X) - x + 1;
            height = Math.Max(p1.Y, p2.Y) - y + 1;
            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Returns an image containing the mouse cursor with a given size
        /// </summary>
        /// <param name="width">Width of the image in pixels</param>
        /// <param name="height">Height of the image in pixels</param>
        /// <returns>The image containing the mouse</returns>
        static Image MakeMouseImage(int width, int height, Color c)
        {
            var img = new Bitmap(width, height);
            using (var g = Graphics.FromImage(img))
                g.DrawRectangle(new Pen(c), 0, 0, img.Width - 1, img.Height - 1);
            return img;
        }

        void UpdateMouseMarquee(Point p1, Point p2)
        {
            UpdateMouseMarquee(GetRect(p1, p2));
        }
        void UpdateMouseMarquee(Rectangle rect)
        {
            mouseOverlay.Image = MakeMouseImage(rect.Size.Width * gridSize, rect.Size.Height * gridSize, UI.Default.CursorColor);
            mouseOverlay.Location = new Point(rect.Location.X * gridSize, rect.Location.Y * gridSize);
        }

        /// <summary>
        /// Sets the mouse to the default size (1 tile)
        /// </summary>
        void ResetMouseSize()
        {
            mouseOverlay.Image = MakeMouseImage(gridSize, gridSize, UI.Default.CursorColor);
        }

        void MoveMouse(Point gridPosition)
        {
            mouseOverlay.Location = new Point(gridPosition.X * gridSize, gridPosition.Y * gridSize);
        }

        #endregion
    }
}
