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
        public int AttributeNumber { get; private set; }

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
            this.Text = $"Attribute {AttributeNumber}";
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

        /// <summary>
        /// The current tileset (image)
        /// </summary>
        Bitmap baseTileset;
        /// <summary>
        /// The current tileset (tile types)
        /// </summary>
        Bitmap tilesetTileTypes;

        //everything get initialised, just not in this method
        #nullable disable
        public FormAttributeEditor(Mod m, int attributeNumber, string tileTypePath, IDictionary<WinFormsKeybinds.KeyInput,string> keybinds)
        #nullable restore
        {
            parentMod = m;
            AttributeNumber = attributeNumber;
            Keybinds = keybinds;

            InitializeComponent();
                        
            //attributes
            attributePath = Path.Combine(parentMod.DataPath, parentMod.AttributeName + AttributeNumber + "." + parentMod.AttributeExtension);
            attributes = new Map(attributePath);
            attributes.MapResized += () => InitTilesetTileTypes();
            tileTypes = new Bitmap(tileTypePath);

            attributePropertyGrid.SelectedObject = attributes;

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
            DrawTiles(tilesetTileTypes, attributes, tileTypes);
        }

        #endregion

        #region generic draw

        /// <summary>
        /// Draws all the tiles from the given map onto the given image, using the given tileset
        /// </summary>
        /// <param name="toDraw"></param>
        /// <param name="tileSource"></param>
        /// <param name="tileset"></param>
        void DrawTiles(Image toDraw, Map tileSource, Bitmap tileset)
        {
            using (Graphics g = Graphics.FromImage(toDraw))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                for (int i = 0; i < tileSource.Tiles.Count; i++)
                {
                    DrawTile(g, tileSource, i, tileset);
                }
            }
        }
        void DrawTile(Image img, Map tileSource, int i, Bitmap tileset)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                DrawTile(g, tileSource, i, tileset);
            }
        }

        /// <summary>
        /// Draws the "i"th tile of the given map, using the given Graphics and tileset
        /// </summary>
        /// <param name="g"></param>
        /// <param name="tileSource"></param>
        /// <param name="i"></param>
        /// <param name="tileset"></param>
        void DrawTile(Graphics g, Map tileSource, int i, Bitmap tileset)
        {
            var tilesetX = (tileSource.Tiles[i] % 16) * parentMod.TileSize;
            var tilesetY = (tileSource.Tiles[i] / 16) * parentMod.TileSize;

            var x = (i % tileSource.Width) * parentMod.TileSize;
            var y = (i / tileSource.Width) * parentMod.TileSize;

            using (var tileImage = tileset.Clone(new Rectangle(
                tilesetX, tilesetY, parentMod.TileSize, parentMod.TileSize
            ), PixelFormat.DontCare))
            {
                g.FillRectangle(Brushes.Transparent, x, y, parentMod.TileSize, parentMod.TileSize);
                g.DrawImage(tileImage, x, y, parentMod.TileSize, parentMod.TileSize);
            }
        }

        #endregion

        #region Drawing tile types
        void DrawSelectedTile(Graphics g)
        {
            g.DrawRectangle(new Pen(Color.LightGray),
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
            tileTypesPictureBox.Image?.Dispose();
            tileTypesPictureBox.Image = workingTileTypes;
        }

        #endregion

        #region draw tileset

        private void DrawMouseOverlay(Graphics g, Point p)
        {
            int x = p.X * parentMod.TileSize;
            int y = p.Y * parentMod.TileSize;

            int width = parentMod.TileSize - 1;
            int height = parentMod.TileSize - 1;

            g.DrawRectangle(new Pen(Color.LightGray), x, y, width, height);
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
            tilesetPictureBox.Image?.Dispose();
            tilesetPictureBox.Image = tilesetImage;
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

            DrawTile(tilesetTileTypes, attributes, tile, tileTypes);
        }

        #endregion

        #region Mouse

        Point MousePositionOnGrid = new Point(-1, -1);

        /// <summary>
        /// The bottom right point of the map
        /// </summary>
        Point maxGridPoint { get => new Point(attributes.Width - 1, attributes.Height - 1); }

        bool Draw = false;

        private Point GetMousePointOnGrid(Point p)
        {
            return new Point(p.X / parentMod.TileSize, p.Y / parentMod.TileSize);
        }

        private void tileTypesPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnGrid(e.Location);
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
            SetTile(GetMousePointOnGrid(e.Location));
        }

        private void tilesetPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnGrid(e.Location);
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
            if (attributePropertyGrid.ActiveControl?.GetType().Name == "GridViewEdit")
                return;

            var input = new WinFormsKeybinds.KeyInput(e.KeyData);
            if(Keybinds.ContainsKey(input))
            {
                switch(Keybinds[input])
                {
                    case "Save":
                        Save();
                        break;
                }
            }
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
