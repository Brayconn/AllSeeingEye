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
using GuxtEditor.Properties;

namespace GuxtEditor
{
    public partial class FormStageEditor : Form
    {
        /// <summary>
        /// What stage this editor is editing
        /// </summary>
        public int StageNumber { get; private set; }

        Mod parentMod;
        readonly IDictionary<WinFormsKeybinds.KeyInput, string> Keybinds;

        readonly string mapPath, entityPath;

        /// <summary>
        /// List of loaded entities
        /// </summary>
        List<Entity> entities;        
        /// <summary>
        /// The map object this is editing
        /// </summary>
        Map map;
        /// <summary>
        /// The attributes for this map's tiles
        /// </summary>
        Map attributes;
        /// <summary>
        /// Image for all tile types
        /// </summary>
        Bitmap tileTypes;

        //Everything gets set, just not directly in this method
#nullable disable
        public FormStageEditor(Mod m, int stageNum, string tileTypePath, IDictionary<WinFormsKeybinds.KeyInput,string> keybinds)
        #nullable restore
        {
            //everything needs this stuff
            parentMod = m;
            StageNumber = stageNum;
            Keybinds = keybinds;

            //UI init
            InitializeComponent();
            mapPictureBox.MouseWheel += mapPictureBox_MouseWheel;
            InitEntityList();

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

        /// <summary>
        /// Saves the loaded map/entities
        /// </summary>
        private void Save()
        {
            map.Save(mapPath);
            //attributes.Save();
            PXEVE.Write(entities, entityPath);
        }

        #region Zoom

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
                    DisplayMap();
                }
            }
        }
        private void mapPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if(ModifierKeys == Keys.Control)
                ZoomLevel += (e.Delta > 0) ? 1 : -1;
        }

        #endregion

        #region map and entity init

        /// <summary>
        /// Initializes the entire map from scratch, then displays.
        /// only used on startup and resize
        /// </summary>
        void InitMapAndDisplay()
        {
            InitMap();
            InitMapTileTypes();
            DisplayMap();
        }

        /// <summary>
        /// Initializes the entity list
        /// </summary>
        void InitEntityList()
        {
            entityListView.Clear();
            entityListView.LargeImageList = parentMod.EntityIcons;
            for (int i = 0; i < parentMod.EntityIcons.Images.Count; i++)
            {
                entityListView.Items.Add(EntityList.EntityNames.ContainsKey(i) ? EntityList.EntityNames[i] : i.ToString() , i);
            }
        }

        #endregion

        #region generic draw functions

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
        /// <summary>
        /// Draws the "i"th tile of the given map, using the given Image and tileset
        /// </summary>
        /// <param name="img"></param>
        /// <param name="tileSource"></param>
        /// <param name="i"></param>
        /// <param name="tileset"></param>
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

        #endregion

        #region edit map

        byte selectedTile = 0;
        byte SelectedTile
        {
            get => selectedTile;
            set
            {
                selectedTile = value;
                DisplayTileset();
            }
        }

        void SetTile(Point p)
        {
            SetTile((p.Y * map.Width) + p.X);
        }
        void SetTile(int tileNum)
        {
            SetTile(tileNum, SelectedTile);
        }
        void SetTile(int tileNum, byte tileValue)
        {
            map.Tiles[tileNum] = tileValue;
            DrawTile(baseMap, map, tileNum, baseTileset);
            DrawTile(mapTileTypes, map, tileNum, tilesetTileTypes);
        }

        #endregion

        #region entity stuff

        HashSet<Entity> selectedEntities = new HashSet<Entity>();
        bool entitiesCopied { get => entityClipboard != null; }
        Entity? entityClipboard = null;
                     
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
        void CreateNewEntity(Point pos)
        {
            entities.Add(new Entity(0, pos.X, pos.Y, entityListView.SelectedIndices[0], 0));
            DisplayMap(MousePositionOnGrid);
        }
        void DeleteSelectedEntities()
        {
            foreach (var ent in selectedEntities)
                entities.Remove(ent);
            selectedEntities.Clear();
        }

        private void SetEditingEntity(IEnumerable<Entity> entities)
        {
            selectedEntities = new HashSet<Entity>(entities);
            var ent = entities.Any() ? entities.First() : null;
            if(ent != null && EntityList.ClassDictionary.ContainsKey(ent.EntityID))
                entityPropertyGrid.SelectedObject = EntityList.ClassDictionary[ent.EntityID](ent);
            else
                entityPropertyGrid.SelectedObject = ent;
        }
        private void SelectEntity(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                SetEditingEntity(new[] { entities[int.Parse(tsmi.Name)] });
            }
        }
        void CopyEntity(int index)
        {
            CopyEntity(entities[index]);
        }
        void CopyEntity(Entity e)
        {
            entityClipboard = new Entity(e);
        }
        void PasteEntity(Point pos)
        {
            if (!entitiesCopied)
                return;
            entities.Add(new Entity(entityClipboard!) //what do you think the above check was for >:(
            {
                X = pos.X,
                Y = pos.Y
            });
            DisplayMap(MousePositionOnGrid); //Yes, this is on purpose
        }
        #endregion

        #region Mouse


        Point MousePositionOnGrid = new Point(-1, -1);

        Point EntitySelectionStart = new Point(-1, -1);
        Point EntitySelectionEnd = new Point(-1, -1);

        /// <summary>
        /// All possible editing modes
        /// </summary>
        enum EditModes
        {
            Tile = 1,
            Entity = 2
        }
        /// <summary>
        /// What the user is currently editing
        /// </summary>
        EditModes editMode
        {
            get => editModeTabControl.SelectedIndex switch
            {
                0 => EditModes.Tile,
                1 => EditModes.Entity,
                _ => EditModes.Tile //Expand new tabs here
            };
        }

        /// <summary>
        /// The bottom right point of the map
        /// </summary>
        Point maxGridPoint { get => new Point((map.Width * (int)editMode) - 1, (map.Height * (int)editMode) - 1); }

        /// <summary>
        /// How big the grid is right now
        /// </summary>
        int gridSize { get => parentMod.TileSize / (int)editMode; }

        /// <summary>
        /// converts a cursor location to a point on the tileset
        /// </summary>
        /// <param name="p">Cursor location</param>
        /// <returns>The coords of the tile the cursor is hovering over</returns>
        private Point GetMousePointOnTileset(Point p)
        {
            return new Point(p.X / gridSize, p.Y / gridSize);
        }
        /// <summary>
        /// converts a cursor location to a point on the map
        /// </summary>
        /// <param name="p">Cursor location</param>
        /// <returns>The coords of the tile the cursor is hovering over</returns>
        private Point GetMousePointOnMap(Point p)
        {
            return new Point(p.X / (gridSize * ZoomLevel), p.Y / (gridSize * ZoomLevel));
        }

        bool mouseOnMap { get => MousePositionOnGrid != new Point(-1, -1); }
        /// <summary>
        /// Whether or not the user has selected an entity from the list of all entities
        /// </summary>
        bool entitySelected { get => entityListView.SelectedIndices.Count > 0; }

        /// <summary>
        /// Whether or not the user has any entities selected
        /// </summary>
        bool userHasSelectedEntities { get => selectedEntities.Count > 0; }

        enum HoldActions
        {
            DrawTiles,
            SelectEntities,
            MoveEntities
        }

        HoldActions? HoldAction = null;

        ToolStripMenuItem? delete = null;
        private void mapPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = GetMousePointOnMap(e.Location);

            switch (editMode)
            {
                //Map
                case EditModes.Tile:
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
                            break;
                    }
                    break;
                //Entity
                case EditModes.Entity:
                    var entitiesWhereClicked = GetEntitiesAtLocation(p.X, p.Y);
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            //if the user is clicking on an entity that's already selected
                            if(entitiesWhereClicked.Intersect(selectedEntities).Any())
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
                                EntitySelectionStart = EntitySelectionEnd = p;
                            }                            
                            break;
                        //Context menu
                        case MouseButtons.Right:
                            #region Context menu stuff
                            //basic init
                            var hoveredEntitiesCount = entitiesWhereClicked.Count();
                            ContextMenuStrip entityContextMenu = new ContextMenuStrip();

                            //Insert
                            var insert = new ToolStripMenuItem("Insert Entity");
                            insert.Enabled = entitySelected;
                            insert.Click += delegate { if (entitySelected) CreateNewEntity(p); };
                            entityContextMenu.Items.Add(insert);
                                                        
                            //Copy
                            //TODO expand on copy/paste functionality
                            var copy = new ToolStripMenuItem("Copy");
                            //copy enabled if only one entity selected, other stuff only initiallized then too
                            if (copy.Enabled = hoveredEntitiesCount == 1)
                            {
                                copy.Name = entities.IndexOf(entitiesWhereClicked.First()).ToString();
                                copy.Click += (o,e) => { CopyEntity(int.Parse(((ToolStripMenuItem)o).Name)); };
                            }
                            entityContextMenu.Items.Add(copy);

                            //Paste
                            var paste = new ToolStripMenuItem("Paste");
                            paste.Enabled = entitiesCopied;
                            paste.Click += delegate { PasteEntity(p); };
                            entityContextMenu.Items.Add(paste);

                            //Delete
                            if(delete == null)
                            {
                                delete = new ToolStripMenuItem();
                                delete.Click += delegate { DeleteSelectedEntities(); };
                            }                            
                            delete.Text = $"Delete Entit{(userHasSelectedEntities ? "ies" : "y")}";
                            delete.Enabled = userHasSelectedEntities;
                            entityContextMenu.Items.Add(delete);

                            //Add buttons to select stacked entities
                            if (hoveredEntitiesCount > 1)
                            {
                                entityContextMenu.Items.Add(new ToolStripSeparator());
                                foreach(var ent in entitiesWhereClicked)
                                {
                                    var index = entities.IndexOf(ent);

                                    //TODO temp text
                                    string entityName = "<no name>";
                                    EntityList.EntityNames.TryGetValue(entities[index].EntityID, out entityName);
                                    var tsmi = new ToolStripMenuItem(index.ToString() + " - " + entityName);
                                    tsmi.Name = index.ToString();
                                    tsmi.Click += SelectEntity;
                                    entityContextMenu.Items.Add(tsmi);
                                }                                    
                            }
                            entityContextMenu.Show(mapPictureBox, e.Location);
                            #endregion
                            break;
                    }
                    break;
            }
        }
        private void mapPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnMap(e.Location);
            //TODO could do with making a Clamp method
            p.X = Math.Max(0, Math.Min(p.X, maxGridPoint.X));
            p.Y = Math.Max(0, Math.Min(p.Y, maxGridPoint.Y));
            //if we're still on the same grid space, stop
            if (p == MousePositionOnGrid)
                return;

            switch (HoldAction)
            {
                case HoldActions.DrawTiles:
                    SetTile(p);
                    DisplayMap();
                    break;
                case HoldActions.MoveEntities:
                    int xd = MousePositionOnGrid.X - p.X;
                    int yd = MousePositionOnGrid.Y - p.Y;
                    foreach(var ent in selectedEntities)
                    {
                        ent.X -= xd;
                        ent.Y -= yd;
                    }
                    DisplayMap();
                    break;
                case HoldActions.SelectEntities:
                    DisplayMap(EntitySelectionStart, EntitySelectionEnd = p);
                    break;
                default:
                    DisplayMap(p);
                    break;

            }
            MousePositionOnGrid = p;
        }

        private void mapPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            switch(HoldAction)
            {
                case HoldActions.SelectEntities:
                    
                    //TODO M E S S Y
                    int x = Math.Min(EntitySelectionStart.X, EntitySelectionEnd.X);
                    int y = Math.Min(EntitySelectionStart.Y, EntitySelectionEnd.Y);
                    int xd = Math.Max(EntitySelectionStart.X, EntitySelectionEnd.X);
                    int yd = Math.Max(EntitySelectionStart.Y, EntitySelectionEnd.Y);

                    var sel = GetEntitiesAtLocation(x, y, xd, yd);
                    SetEditingEntity(sel);
                    DisplayMap(MousePositionOnGrid);
                    break;
            }
            HoldAction = null;
        }

        private void mapPictureBox_MouseLeave(object sender, EventArgs e)
        {
            HoldAction = null;
            MousePositionOnGrid = new Point(-1, -1);
            DisplayMap();
        }

        #endregion

        #region keyboard
        private void FormStageEditor_KeyDown(object sender, KeyEventArgs e)
        {
            //Kinda hacky way of checking if the user is editing something in a property grid
            if (entityPropertyGrid.ActiveControl?.GetType().Name == "GridViewEdit" ||
                mapPropertyGrid.ActiveControl?.GetType().Name == "GridViewEdit")
                return;

            var input = new WinFormsKeybinds.KeyInput(e.KeyData);
            if (Keybinds.ContainsKey(input))
            {
                switch (Keybinds[input])
                {
                    case "ZoomIn":
                        ZoomLevel++;
                        break;
                    case "ZoomOut":
                        ZoomLevel--;
                        break;
                    case "PickTile" when editMode == EditModes.Tile && mouseOnMap:
                        SelectedTile = map.Tiles[(MousePositionOnGrid.Y * map.Width) + MousePositionOnGrid.X];
                        break;
                    case "DeleteEntities" when editMode == EditModes.Entity && userHasSelectedEntities:
                        DeleteSelectedEntities();
                        DisplayMap(MousePositionOnGrid);
                        break;
                    case "InsertEntity" when editMode == EditModes.Entity && mouseOnMap:
                        CreateNewEntity(MousePositionOnGrid);
                        break;
                    case "Copy" when editMode == EditModes.Entity && userHasSelectedEntities:
                        CopyEntity(selectedEntities.First());
                        break;
                    case "Paste" when editMode == EditModes.Entity && entitiesCopied && mouseOnMap:
                        PasteEntity(MousePositionOnGrid);
                        break;
                }
            }
        }

        #endregion

        #region menu buttons

        /// <summary>
        /// Used when checkboxes for entities/grid n stuff are changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDisplay(object sender, EventArgs e)
        {
            DisplayTileset();
            DisplayMap();
        }

        private void deleteAllEntitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete EVERY entity?\nIf you save after this, there's no coming back.", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                entityPropertyGrid.SelectedObject = null;
                selectedEntities.Clear();
                entities.Clear();
                DisplayMap();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        #endregion

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
