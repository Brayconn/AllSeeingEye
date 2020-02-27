using GuxtModdingFramework;
using GuxtModdingFramework.Images;
using GuxtModdingFramework.Maps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuxtEditor
{
    //TODO this entire class is a mess
    //it's just FormStageEditor, but without entities, and a slight simplification of the actual editor
    //Really should merge the two
    public partial class FormAttributeEditor : Form
    {
        readonly Mod parentMod;
        public int AssetNumber { get; private set; }

        readonly IDictionary<WinFormsKeybinds.KeyInput, string> Keybinds;

        protected bool unsavedEdits = false;
        public bool UnsavedEdits
        {
            get => unsavedEdits;
            protected set
            {
                if (unsavedEdits != value)
                {
                    unsavedEdits = value;
                    UpdateTitle();
                }
            }
        }

        protected virtual void UpdateTitle()
        {
            this.Text = $"Attribute {AssetNumber}";
            if (UnsavedEdits)
                this.Text += "*";
        }

        /// <summary>
        /// Location to save this attribute to
        /// </summary>
        string attributePath;

        /// <summary>
        /// The loaded attributes
        /// </summary>
        Map attributes;

        /// <summary>
        /// All tile types
        /// </summary>
        Bitmap tileTypes;

        /// <summary>
        /// The current tileset (image)
        /// </summary>
        Bitmap baseTileset;
        /// <summary>
        /// The current tileset (tile types)
        /// </summary>
        Bitmap tilesetTileTypes;

        private FormAttributeEditor()
        {
            InitializeComponent();
        }
        //everything get initialised, just not in this method
        #nullable disable
        public FormAttributeEditor(Mod m, int attributeNumber, string tileTypePath, IDictionary<WinFormsKeybinds.KeyInput,string> keybinds)
            : base()
        #nullable restore
        {
            parentMod = m;
            AssetNumber = attributeNumber;
            Keybinds = keybinds;

            tileTypes = new Bitmap(tileTypePath);

            //designer, events, other UI
            //InitializeComponent();
            mainPictureBox.MouseWheel += mainPictureBox_MouseWheel;
            UpdateTitle();


            InitAssets();
        }
        protected virtual void InitAssets()
        {
            //attributes
            attributePath = Path.Combine(parentMod.DataPath, parentMod.AttributeName + AssetNumber + "." + parentMod.AttributeExtension);
            attributes = new Map(attributePath);
            attributes.MapResized += () => InitTilesetTileTypes();
            

            tilePropertyGrid.SelectedObject = attributes;

            //tileset            
            var t = new Bitmap(Path.ChangeExtension(attributePath, parentMod.ImageExtension));
            if (parentMod.ImagesScrambeled)
                t = Scrambler.Unscramble(t);
            InitTileset(t);
            InitTilesetTileTypes();

            DisplayTileTypes();
            DisplayTileset();
        }

        #region init images

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
            CommonDraw.RenderTiles(tilesetTileTypes, attributes, tileTypes, parentMod.TileSize);
        }

        #endregion

        #region Drawing tile types
        void DrawSelectedTile(Graphics g)
        {
            g.DrawRectangle(new Pen(UI.Default.SelectedTileColor),
                (SelectedTile % 16) * parentMod.TileSize,
                (SelectedTile / 16) * parentMod.TileSize,
                parentMod.TileSize - 1,
                parentMod.TileSize - 1);
        }
        void DisplayTileTypes()
        {
            var workingTileTypes = new Bitmap(tileTypes.Width, tileTypes.Height);
            using (Graphics g = Graphics.FromImage(workingTileTypes))
            {
                g.Clear(Color.Black);
                g.DrawImage(tileTypes, 0, 0, tileTypes.Width, tileTypes.Height);
                DrawSelectedTile(g);
            }
            tilesetPictureBox.Image?.Dispose();
            tilesetPictureBox.Image = workingTileTypes;
        }

        #endregion

        #region draw tileset

        private void DrawMouseOverlay(Graphics g, Point p)
        {
            int x = p.X * parentMod.TileSize;
            int y = p.Y * parentMod.TileSize;

            int width = parentMod.TileSize - 1;
            int height = parentMod.TileSize - 1;

            g.DrawRectangle(new Pen(UI.Default.CursorColor), x, y, width, height);
        }
        void DisplayTileset(Point? p = null)
        {
            Bitmap tilesetImage = new Bitmap(baseTileset);
            if (tileTypesToolStripMenuItem.Checked)
            {
                using (Graphics g = Graphics.FromImage(tilesetImage))
                {
                    if (tileTypesToolStripMenuItem.Checked)
                        g.DrawImage(tilesetTileTypes, 0, 0, tilesetTileTypes.Width, tilesetTileTypes.Height);
                    if (p != null)
                        DrawMouseOverlay(g, (Point)p);
                }
            }
            mainPictureBox.Image?.Dispose();
            mainPictureBox.Image = CommonDraw.Scale(tilesetImage, ZoomLevel);
            tilesetImage.Dispose();
        }

        #endregion

        #region edit tileset

        byte SelectedTile = 0;
        void SetTile(Point p)
        {
            UnsavedEdits = true;
            //auto-resize
            if(p.X >= attributes.Width)
                attributes.Width = (ushort)(p.X + 1);
            if(p.Y >= attributes.Height)
                attributes.Height = (ushort)(p.Y + 1);
            
            var tile = (p.Y * attributes.Width) + p.X;
            attributes.Tiles[tile] = SelectedTile;

            CommonDraw.DrawTile(tilesetTileTypes, attributes, tile, tileTypes, parentMod.TileSize, CompositingMode.SourceCopy);
        }

        #endregion

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
        private Point GetMousePointOnTileset(Point p)
        {
            return new Point(p.X / (parentMod.TileSize * ZoomLevel), p.Y / (parentMod.TileSize * ZoomLevel));
        }

        private void tileTypesPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnTileTypes(e.Location);
            var value = (p.Y * 16) + p.X;
            if (value <= byte.MaxValue && value != SelectedTile)
            {
                SelectedTile = (byte)value;
                DisplayTileTypes();
            }
        }

        private void tilesetPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Draw = e.Button == MouseButtons.Left;
            var p = GetMousePointOnTileset(e.Location);
            SetTile(p);
            DisplayTileset(p);
        }

        private void tilesetPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnTileset(e.Location);
            //TODO make clamp?
            p.X = Math.Max(0, Math.Min(p.X, maxGridPoint.X));
            p.Y = Math.Max(0, Math.Min(p.Y, maxGridPoint.Y));
            //if we're still on the same grid space, stop
            if (p == MousePositionOnGrid)
                return;

            if (Draw)
                SetTile(p);

            MousePositionOnGrid = p;
            DisplayTileset(p);
        }

        private void tilesetPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            Draw = false;
        }

        private void tilesetPictureBox_MouseLeave(object sender, EventArgs e)
        {
            Draw = false;
            DisplayTileset();
        }

        #endregion

        #region Keyboard

        private void FormAttributeEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (tilePropertyGrid.ActiveControl?.GetType().Name == "GridViewEdit")
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

        int zoomLevel = 1;
        int ZoomLevel
        {
            get => zoomLevel;
            set
            {
                if (1 <= value && value <= MaxZoom)
                {
                    zoomLevel = value;
                    DisplayTileset();
                }
            }
        }
        protected virtual void mainPictureBox_MouseWheel(object sender, MouseEventArgs e)
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
            DisplayTileset();
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
    }
}
