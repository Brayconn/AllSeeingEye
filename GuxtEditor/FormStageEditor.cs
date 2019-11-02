using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GuxtModdingFramework;
using GuxtModdingFramework.Images;
using GuxtModdingFramework.Entities;
using GuxtModdingFramework.Maps;

namespace GuxtEditor
{
    public partial class FormStageEditor : Form
    {
        Mod parentMod;
        public int StageNumber { get; private set; }
        
        List<Entity> entities;

        readonly string mapPath, entityPath;
        /// <summary>
        /// The map object this is editing
        /// </summary>
        Map map { get; set; }
        /// <summary>
        /// The attributes for this map's tiles
        /// </summary>
        Map attributes { get; set; }
        
        /// <summary>
        /// Image for all tile types
        /// </summary>
        Bitmap tileTypes;

        //Everything gets set, just not directly in this method
        #nullable disable
        public FormStageEditor(Mod m, int stageNum, string tileTypePath)
        #nullable restore
        {
            //everything needs this stuff
            parentMod = m;
            StageNumber = stageNum;

            //UI init
            InitializeComponent();       
            InitEntityList();

            //Tool strip menu items init
            createEntityToolStripMenuItem = new ToolStripMenuItem("Insert Entity");
            createEntityToolStripMenuItem.Click += CreateNewEntity;
            deleteEntityToolStripMenuItem = new ToolStripMenuItem();
            deleteEntityToolStripMenuItem.Click += DeleteEntities;

            //entities
            entityPath = Path.Combine(parentMod.DataPath, parentMod.EntityName + StageNumber + "." + parentMod.EntityExtension);
            entities = PXEVE.Read(entityPath);

            //attributes
            var attributePath = Path.Combine(parentMod.DataPath, parentMod.AttributeName + StageNumber + "." + parentMod.AttributeExtension);
            attributes = new Map(attributePath);
            tileTypes = new Bitmap(tileTypePath);

            //tileset            
            var t = new Bitmap(Path.ChangeExtension(attributePath, parentMod.ImageExtension));
            if (parentMod.ImagesScrambeled)
                t = Scrambler.Unscramble(t);
            InitTileset(t);
            InitTilesetTileTypes();            

            //base map setup
            mapPath = Path.Combine(parentMod.DataPath, parentMod.MapName + StageNumber + "." + parentMod.MapExtension);
            map = new Map(mapPath);
            map.MapResized += () => InitMapAndDisplay();
            mapPropertyGrid.SelectedObject = map;

            //display everything
            DisplayTileset();

            InitMapAndDisplay();
        }
        void InitMapAndDisplay()
        {
            InitMap();
            InitMapTileTypes();
            DisplayMap();
        }

        void InitEntityList()
        {
            entityListView.Clear();
            entityListView.LargeImageList = parentMod.EntityIcons;
            for (int i = 0; i < parentMod.EntityIcons.Images.Count; i++)
            {
                entityListView.Items.Add(parentMod.EntityNames.ContainsKey(i) ? parentMod.EntityNames[i] : i.ToString() , i);
            }
        }

        int gridSize { get => parentMod.TileSize / (editModeTabControl.SelectedIndex + 1); }

        private Point GetMousePointOnGrid(Point p)
        {
            return new Point(p.X / gridSize, p.Y / gridSize);
        }

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
                for (int i = 0; i < tileSource.Tiles.Count; i++)
                {
                    DrawTile(g, tileSource, i, tileset);
                }
            }
        }
        void DrawTile(Image img, Map tileSource, int i, Bitmap tileset)
        {
            using (Graphics g = Graphics.FromImage(img))
                DrawTile(g, tileSource, i, tileset);
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
                g.DrawImage(tileImage, x, y, parentMod.TileSize, parentMod.TileSize);
            }
        }

        void SetTile(Point p)
        {
            SetTile((p.Y * map.Width) + p.X);
        }
        void SetTile(int tile)
        {
            map.Tiles[tile] = SelectedTile;
            DrawTile(baseMap, map, tile, baseTileset);
            DrawTile(mapTileTypes, map, tile, tilesetTileTypes);
        }

        enum HoldActions
        {
            DrawTiles,
            SelectEntities,
            MoveEntities
        }

        HoldActions? HoldAction = null;

        byte SelectedTile = 0;
        HashSet<Entity> selectedEntities = new HashSet<Entity>();
        Entity? entityClipboard;
        Point MousePositionOnGrid = new Point(-1, -1);
        Point EntitySelectionStart = new Point(-1, -1);

        #region Map interaction

        private IEnumerable<Entity> GetEntitiesAtLocation(int x, int y)
        {
            return GetEntitiesAtLocation(x, y, x, y);
        }
        private IEnumerable<Entity> GetEntitiesAtLocation(int x, int y, int x2, int y2)
        {
            //TODO maybe remove LINQ?
            return entities.Where(entity => x <= entity.X && entity.X <= x2
                                         && y <= entity.Y && entity.Y <= y2);
        }

        ToolStripMenuItem createEntityToolStripMenuItem;
        ToolStripMenuItem deleteEntityToolStripMenuItem;
        private void CreateNewEntity(object sender, EventArgs e)
        {
            if (entityListView.SelectedIndices.Count > 0)
            {
                entities.Add(new Entity(0, MousePositionOnGrid.X, MousePositionOnGrid.Y, entityListView.SelectedIndices[0], 0));
                DisplayMap(MousePositionOnGrid);
            }
        }
        private void DeleteEntities(object sender, EventArgs e)
        {
            foreach (var ent in selectedEntities)
                entities.Remove(ent);
            selectedEntities.Clear();
        }

        private void SetSelectedEntity(IEnumerable<Entity> entities)
        {
            selectedEntities = new HashSet<Entity>(entities);
            entityPropertyGrid.SelectedObject = entities.Any() ? entities.First() : null;
        }
        private void SelectEntity(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                SetSelectedEntity(new[] { entities[int.Parse(tsmi.Name)] });
            }
        }
        private void CopyEntity(object sender, EventArgs e)
        {
            if(sender is ToolStripMenuItem tsmi)
                entityClipboard = new Entity(entities[int.Parse(tsmi.Name)]);             
        }
        private void PasteEntity(object sender, EventArgs e)
        {
            if (entityClipboard == null)
                return;
            entities.Add(new Entity(entityClipboard) { 
            X = MousePositionOnGrid.X,
            Y = MousePositionOnGrid.Y
            });
            DisplayMap(MousePositionOnGrid);
        }

        private void mapPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = GetMousePointOnGrid(e.Location);

            switch (editModeTabControl.SelectedIndex)
            {
                //Map
                case 0:
                    var tile = (p.Y * map.Width) + p.X;
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            HoldAction = HoldActions.DrawTiles;

                            SetTile(tile);
                            DisplayMap();
                            break;
                        case MouseButtons.Middle:
                            SelectedTile = map.Tiles[tile];
                            DisplayTileset();
                            break;
                    }
                    break;
                //Entity
                case 1:
                    var hoveredEntities = GetEntitiesAtLocation(p.X, p.Y);
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            //if the user is clicking on an entity that's already selected
                            if(hoveredEntities.Intersect(selectedEntities).Any())
                            {
                                //start moving
                                HoldAction = HoldActions.MoveEntities;
                            }
                            //if the user isn't, that means they're either:
                            // a. clicking on a new entity, or
                            // b. clicking on an empty space
                            // either way, we're selecting something new
                            else
                            {
                                HoldAction = HoldActions.SelectEntities;
                                EntitySelectionStart = p;
                            }                            
                            break;
                        //Context menu
                        case MouseButtons.Right:
                            //basic init
                            var c = hoveredEntities.Count();
                            ContextMenuStrip entityContextMenu = new ContextMenuStrip();
                            
                            //Insert
                            entityContextMenu.Items.Add(createEntityToolStripMenuItem);
                                                        
                            //Copy
                            //TODO expand on copy/paste functionality
                            var copy = new ToolStripMenuItem("Copy");
                            //copy enabled if only one entity selected, other stuff only initiallized then too
                            if (copy.Enabled = c == 1)
                            {
                                copy.Name = entities.IndexOf(hoveredEntities.First()).ToString();
                                copy.Click += CopyEntity;
                            }
                            entityContextMenu.Items.Add(copy);

                            //Paste
                            var paste = new ToolStripMenuItem("Paste");
                            //paste enabled if you have something on the clipboard
                            paste.Enabled = entityClipboard != null;
                            paste.Click += PasteEntity;
                            entityContextMenu.Items.Add(paste);

                            //Delete
                            deleteEntityToolStripMenuItem.Text = $"Delete Entit{(c > 1 ? "ies" : "y")}";
                            deleteEntityToolStripMenuItem.Enabled = c >= 1;
                            entityContextMenu.Items.Add(deleteEntityToolStripMenuItem);


                            //anything else is bigger, so we gotta check what entity to select
                            if (c > 1)
                            {
                                entityContextMenu.Items.Add(new ToolStripSeparator());
                                foreach(var ent in hoveredEntities)
                                {
                                    var index = entities.IndexOf(ent);

                                    //TODO temp text
                                    var tsmi = new ToolStripMenuItem(index.ToString());
                                    tsmi.Name = index.ToString();
                                    tsmi.Click += SelectEntity;
                                    entityContextMenu.Items.Add(tsmi);
                                }                                    
                            }
                            entityContextMenu.Show(mapPictureBox, new Point(MousePositionOnGrid.X * gridSize, MousePositionOnGrid.Y * gridSize));
                            break;
                    }
                    break;
            }
        }
        bool InRange = false;
        private void mapPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnGrid(e.Location);
            //if we're still on the same grid space, stop
            if (p == MousePositionOnGrid)
                return;

            var ir = (0 <= p.X && p.X < map.Width) && (0 <= p.Y && p.Y < map.Height);                        
            //If mouse is not in range, but it was in range last time...
            if(!ir && InRange)
            {
                //that means the mouse has left. I don't know why it couldn't figure this out, but...
                mapPictureBox_MouseLeave(sender, e);
            }
            //If these two aren't equal, that means the mouse is either leaving or entering, and we need to update
            if (ir != InRange)
            {
                InRange = ir;
            }
            //If both are false the mouse is just off the map entirely, so stop
            if (!ir && !InRange)
            {
                return;
            }

            switch (HoldAction)
            {
                case HoldActions.DrawTiles:
                    SetTile(p);
                    break;
                case HoldActions.MoveEntities:
                    int xd = MousePositionOnGrid.X - p.X;
                    int yd = MousePositionOnGrid.Y - p.Y;
                    foreach(var ent in selectedEntities)
                    {
                        ent.X -= xd;
                        ent.Y -= yd;
                    }
                    break;
            }

            MousePositionOnGrid = p;
            //TODO this won't work for an expanding selection square
            //display map not matter what, but only display cursor if not doing something already
            if (HoldAction == null)
                DisplayMap(p);
            else
                DisplayMap();
        }

        private void mapPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            switch(HoldAction)
            {
                case HoldActions.SelectEntities:
                    var EntitySelectionEnd = GetMousePointOnGrid(e.Location);
                    //TODO M E S S Y
                    int x = Math.Min(EntitySelectionStart.X, EntitySelectionEnd.X);
                    int y = Math.Min(EntitySelectionStart.Y, EntitySelectionEnd.Y);
                    int xd = Math.Max(EntitySelectionStart.X, EntitySelectionEnd.X);
                    int yd = Math.Max(EntitySelectionStart.Y, EntitySelectionEnd.Y);

                    var sel = GetEntitiesAtLocation(x, y, xd, yd);
                    SetSelectedEntity(sel);
                    break;
            }
            HoldAction = null;            
        }

        private void mapPictureBox_MouseLeave(object sender, EventArgs e)
        {
            HoldAction = null;
            DisplayMap();
        }

        #endregion

        #region Tileset Interaction

        private void tilesetPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnGrid(e.Location);
            var value = (p.Y * 16) + p.X;
            if (value <= byte.MaxValue && value != SelectedTile)
            {
                SelectedTile = (byte)value;
                DisplayTileset();
            }
        }

        #endregion
        
        private void RefreshDisplay(object sender, EventArgs e)
        {
            DisplayTileset();
            DisplayMap();
        }

        private void Save()
        {
            map.Save(mapPath);
            //attributes.Save();
            PXEVE.Write(entities, entityPath);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void FormStageEditor_KeyDown(object sender, KeyEventArgs e)
        {
            //Kinda hacky way of checking if the user is editing something in a property grid
            if (entityPropertyGrid.ActiveControl?.GetType().Name == "GridViewEdit" ||
                mapPropertyGrid.ActiveControl?.GetType().Name == "GridViewEdit")
                return;

            switch(e.KeyCode)
            {
                case Keys.Delete when editModeTabControl.SelectedIndex == 1 && selectedEntities.Count > 0:
                    DeleteEntities(sender, e);
                    DisplayMap(MousePositionOnGrid);
                    break;
            }
        }

        private void deleteAllEntitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete EVERY entity?\nIf you save after this, there's no coming back.", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                entityPropertyGrid.SelectedObject = null;
                selectedEntities.Clear();
                entities.Clear();
                DisplayMap();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                switch(MessageBox.Show("Would you like to save?", "Warning", MessageBoxButtons.YesNoCancel))
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
            base.OnFormClosing(e);
        }
    }
}
