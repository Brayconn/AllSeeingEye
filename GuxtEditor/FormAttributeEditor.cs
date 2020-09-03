using GuxtModdingFramework;
using GuxtModdingFramework.Images;
using GuxtModdingFramework.Maps;
using LayeredPictureBox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using static PixelModdingFramework.Rendering;
using static GuxtEditor.SharedGraphics;

namespace GuxtEditor
{
    //TODO not sure what to do with this class anymore tbh
    //it used to be really similar to FormStageEditor, but with all the changes made to that class, this one is now like a time capsule
    //still kinda want to merge them, but it might be a bit infeasable now with undo
    public partial class FormAttributeEditor : Form
    {
        readonly Mod parentMod;
        public string AttributeFilename { get; private set; }

        readonly IDictionary<WinFormsKeybinds.KeyInput, string> Keybinds;

        bool unsavedEdits = false;
        public bool UnsavedEdits
        {
            get => unsavedEdits;
            private set
            {
                if (unsavedEdits != value)
                {
                    unsavedEdits = value;
                    UpdateTitle();
                }
            }
        }

        private void UpdateTitle()
        {
            this.Text = AttributeFilename;
            if (UnsavedEdits)
                this.Text += "*";
        }

        /// <summary>
        /// Location to save this attribute to
        /// </summary>
        readonly string attributePath;

        /// <summary>
        /// The loaded attributes
        /// </summary>
        readonly Map attributes;

        /// <summary>
        /// All tile types
        /// </summary>
        readonly Bitmap tileTypes;

        //everything get initialised, just not in this method
        #nullable disable
        public FormAttributeEditor(Mod m, string filename, string tileTypePath, IDictionary<WinFormsKeybinds.KeyInput,string> keybinds)
        #nullable restore
        {
            parentMod = m;
            AttributeFilename = filename;
            Keybinds = keybinds;

            InitializeComponent();
            UpdateTitle();
            attributesLayeredPictureBox.MouseWheel += attributesLayeredPictureBox_MouseWheel;
            attributesLayeredPictureBox.AddLayers(mouseOverlayLayer + 1);
            availableTileTypesLayeredPictureBox.AddLayers(availableTileTypesMouseOverlayLayer + 1);

            tileTypes = new Bitmap(tileTypePath);
            InitAvailableTileTypes(tileTypes);

            //attributes file
            attributePath = Path.Combine(parentMod.DataPath, AttributeFilename);
            attributes = new Map(attributePath);
            attributes.MapResized += delegate { InitAttributeTileTypes(); attributesResizeControl.InitSize(attributes.Width, attributes.Height, attributes.CurrentBufferSize); };
            attributesResizeControl.InitSize(attributes.Width, attributes.Height, attributes.CurrentBufferSize);
            
            //tileset image
            var t = new Bitmap(Path.ChangeExtension(attributePath, parentMod.ImageExtension));
            if (parentMod.ImagesScrambeled)
                t = Scrambler.Unscramble(t);
            InitAttributes(t);
        }

        #region attributes

        const int baseAttributesLayer = 0;
        Layer<Image> baseAttributes => attributesLayeredPictureBox.Layers[baseAttributesLayer];

        const int attributesTileTypesLayer = baseAttributesLayer + 1;
        Layer<Image> attributesTileTypes => attributesLayeredPictureBox.Layers[attributesTileTypesLayer];

        const int mouseOverlayLayer = attributesTileTypesLayer + 1;
        Layer<Image> mouseOverlay => attributesLayeredPictureBox.Layers[mouseOverlayLayer];

        void InitAttributes(Image t)
        {
            attributesLayeredPictureBox.UnlockCanvasSize();
            
            var attrib = new Bitmap(16 * parentMod.TileSize, 16 * parentMod.TileSize);
            using (Graphics g = Graphics.FromImage(attrib))
            {
                g.Clear(Color.Black);
                g.DrawImage(t, 0, 0, t.Width, t.Height);
            }

            baseAttributes.Image = attrib;
            InitAttributeTileTypes();
            mouseOverlay.Image = MakeMouseImage(parentMod.TileSize, parentMod.TileSize, UI.Default.CursorColor);
            
            attributesLayeredPictureBox.LockCanvasSize();
        }
        void InitAttributeTileTypes()
        {
            var tiletypes = new Bitmap(16 * parentMod.TileSize, 16 * parentMod.TileSize);
            RenderTiles(tiletypes, attributes, tileTypes, parentMod.TileSize);
            attributesTileTypes.Image = tiletypes;
        }


        #endregion


        #region available tile types

        const int availableTileTypesLayer = 0;
        Layer<Image> availableTileTypes => availableTileTypesLayeredPictureBox.Layers[availableTileTypesLayer];

        const int availableTileTypesMouseOverlayLayer = availableTileTypesLayer + 1;
        Layer<Image> availableTileTypesMouseOverlay => availableTileTypesLayeredPictureBox.Layers[availableTileTypesMouseOverlayLayer];

        /// <summary>
        /// Resets the tileset buffer, and draws the given image on it
        /// </summary>
        /// <param name="t"></param>
        void InitAvailableTileTypes(Image t)
        {
            availableTileTypesLayeredPictureBox.UnlockCanvasSize();

            var tileset = new Bitmap(16 * parentMod.TileSize, 16 * parentMod.TileSize);
            using (Graphics g = Graphics.FromImage(tileset))
            {
                g.Clear(Color.Black);
                g.DrawImage(t, 0, 0, t.Width, t.Height);
            }            
            availableTileTypes.Image = tileset;
            availableTileTypesMouseOverlay.Image = MakeMouseImage(parentMod.TileSize, parentMod.TileSize, UI.Default.SelectedTileColor);

            availableTileTypesLayeredPictureBox.LockCanvasSize();
        }
        void MoveTileSelection()
        {
            availableTileTypesMouseOverlay.Location = new Point((SelectedTile % 16) * parentMod.TileSize, (SelectedTile / 16) * parentMod.TileSize);
        }

        #endregion
        
        byte selectedTile = 0;
        byte SelectedTile
        {
            get => selectedTile;
            set
            {
                selectedTile = value;
                MoveTileSelection();
            }
        }
        void SetTile(Point p)
        {
            UnsavedEdits = true;
            //auto-resize
            if (attributes.Width <= p.X || attributes.Height <= p.Y)
            {
                attributes.Resize((short)Math.Max(attributes.Width, p.X + 1), (short)Math.Max(attributes.Height, p.Y + 1), ResizeModes.Logical);
            }
            
            var tile = (p.Y * attributes.Width) + p.X;
            attributes.Tiles[tile] = SelectedTile;

            DrawTile(attributesTileTypes.Image, attributes, tile, tileTypes, parentMod.TileSize, CompositingMode.SourceCopy);
        }
                
        #region Mouse

        Point MousePositionOnGrid = new Point(-1, -1);

        /// <summary>
        /// The bottom right point of the map
        /// </summary>
        Point maxGridPoint { get => new Point(attributes.Width - 1, attributes.Height - 1); }

        bool Draw = false;
        private Point GetMousePointOnTileTypes(Point p)
        {
            return new Point(p.X / parentMod.TileSize, p.Y / parentMod.TileSize);
        }
        private Point GetMousePointOnAttributes(Point p)
        {
            return new Point(p.X / (parentMod.TileSize * ZoomLevel), p.Y / (parentMod.TileSize * ZoomLevel));
        }

        private void availableTileTypesPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnTileTypes(e.Location);
            var value = (p.Y * 16) + p.X;
            if (value <= byte.MaxValue && value != SelectedTile)
            {
                SelectedTile = (byte)value;
            }
        }

        private void attributesLayeredPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBoxPanel.Select();
            Draw = e.Button == MouseButtons.Left;
            mouseOverlay.Shown = !Draw;
            var p = GetMousePointOnAttributes(e.Location);
            SetTile(p);
            attributesLayeredPictureBox.Invalidate();
        }

        private void attributesLayeredPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnAttributes(e.Location);
            //TODO make clamp?
            p.X = Math.Max(0, Math.Min(p.X, maxGridPoint.X));
            p.Y = Math.Max(0, Math.Min(p.Y, maxGridPoint.Y));
            //if we're still on the same grid space, stop
            if (p == MousePositionOnGrid)
                return;

            if (Draw)
            {
                SetTile(p);
                attributesLayeredPictureBox.Invalidate();
            }
            MoveMouse(p);
            MousePositionOnGrid = p;
        }

        void MoveMouse(Point gridPosition)
        {
            mouseOverlay.Location = new Point(gridPosition.X * parentMod.TileSize, gridPosition.Y * parentMod.TileSize);
        }

        private void attributesLayeredPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            Draw = false;
            mouseOverlay.Shown = true;
        }

        private void attributesLayeredPictureBox_MouseLeave(object sender, EventArgs e)
        {
            Draw = false;
            mouseOverlay.Shown = false;
        }

        #endregion

        #region Keyboard

        private void FormAttributeEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (attributesResizeControl.IsBeingEdited)
                return;            

            var input = new WinFormsKeybinds.KeyInput(e.KeyData);
            if(Keybinds.ContainsKey(input))
            {
                switch(Keybinds[input])
                {
                    case "ZoomIn":
                        ZoomLevel++;
                        break;
                    case "ZoomOut":
                        ZoomLevel--;
                        break;
                    case "Save":
                        Save();
                        break;
                }
            }
        }

        #endregion

        #region zoom

        const int MaxZoom = 10;
        int ZoomLevel
        {
            get => attributesLayeredPictureBox.CanvasScale;
            set
            {
                if (1 <= value && value <= MaxZoom)
                {
                    attributesLayeredPictureBox.CanvasScale = value;
                }
            }
        }
        private void attributesLayeredPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
                ZoomLevel += (e.Delta > 0) ? 1 : -1;
        }

        #endregion

        private void Save()
        {
            UnsavedEdits = false;
            attributes.Save(attributePath);
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void tileTypesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
             attributesTileTypes.Shown = tileTypesToolStripMenuItem.Checked;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && UnsavedEdits)
            {
                switch (MessageBox.Show("You have unsaved changes! Would you like to save?", "Warning", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        Save();
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }
            }
        }

        private void attributesLayeredPictureBox_MouseEnter(object sender, EventArgs e)
        {
            mouseOverlay.Shown = true;
        }

        private void attributesResizeControl_MapResizeInitialized(object sender, MapResizeInitiatedEventArgs e)
        {
            attributes.Resize(e.NewWidth, e.NewHeight, e.ResizeMode, e.ShrinkBuffer);
            e.NewWidth = attributes.Width;
            e.NewHeight = attributes.Height;
            e.NewBufferSize = attributes.CurrentBufferSize;
        }
    }
}
