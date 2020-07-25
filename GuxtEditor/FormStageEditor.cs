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
using static PixelModdingFramework.Rendering;

namespace GuxtEditor
{
    public partial class FormStageEditor : Form
    {
        /// <summary>
        /// What stage this editor is editing
        /// </summary>
        public int StageNumber { get; private set; }

        #region unsaved edits

        bool unsavedEdits = false;
        public bool UnsavedEdits
        {
            get => unsavedEdits;
            private set
            {
                if(unsavedEdits != value)
                {
                    unsavedEdits = value;
                    UpdateTitle();
                }
            }
        }
        private void SetUnsavedEdits(object o, EventArgs e)
        {
            UnsavedEdits = true;
        }
        #endregion

        /// <summary>
        /// Updates the window title
        /// </summary>
        private void UpdateTitle()
        {
            this.Text = $"Stage {StageNumber}";
            if (UnsavedEdits)
                this.Text += "*";
        }

        private readonly ImageList EntityIcons = new ImageList();

        readonly Mod parentMod;
        readonly IDictionary<WinFormsKeybinds.KeyInput, string> Keybinds;

        readonly string mapPath, entityPath;

        /// <summary>
        /// List of loaded entities
        /// </summary>
        readonly List<Entity> entities;        
        /// <summary>
        /// The map object this is editing
        /// </summary>
        readonly Map map;
        /// <summary>
        /// The attributes for this map's tiles
        /// </summary>
        readonly Map attributes;
        /// <summary>
        /// Image for all tile types
        /// </summary>
        readonly Bitmap tileTypes;

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
            UpdateTitle();
            mapLayeredPictureBox.MouseWheel += mapPictureBox_MouseWheel;
            //add layers, and init the ones we can
            mapLayeredPictureBox.AddLayers(mouseOverlayLayer + 1);
            ResetMouseSize();
            InitScreenPreview();

            InitEntityList();

            //entities
            entityPath = Path.Combine(parentMod.DataPath, parentMod.EntityName + StageNumber + "." + parentMod.EntityExtension);
            entities = PXEVE.Read(entityPath);
            foreach (var ent in entities)
                ent.PropertyChanged += SetUnsavedEdits;
            
            //attributes
            var attributePath = Path.Combine(parentMod.DataPath, parentMod.AttributeName + StageNumber + "." + parentMod.AttributeExtension);
            attributes = new Map(attributePath);
            tileTypes = new Bitmap(tileTypePath);

            //tileset
            var t = new Bitmap(Path.ChangeExtension(attributePath, parentMod.ImageExtension));
            if (parentMod.ImagesScrambeled)
                t = Scrambler.Unscramble(t);
            InitTileset(t);
            UpdateTilesetTileTypes();

            //base map setup
            mapPath = Path.Combine(parentMod.DataPath, parentMod.MapName + StageNumber + "." + parentMod.MapExtension);
            map = new Map(mapPath);
            //need to re-display the map from scratch in the case of a resize
            map.MapResized += delegate { InitMap(); };
            mapPropertyGrid.SelectedObject = map;            

            DisplayTileset();
            InitMap();

            //need to init entity images after the map has been initialized so we actually have a good size
            //need to init them each seperately since otherwise I think it would all be a reference to the same image...
            entityIcons.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
            entityBoxes.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
            selectedEntityBoxes.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
            DrawEntityIcons();
            DrawEntityBoxes();

            tileTypesToolStripMenuItem_CheckedChanged(this, new EventArgs());
            entitySpritesToolStripMenuItem_CheckedChanged(this, new EventArgs());
            entityBoxesToolStripMenuItem_CheckedChanged(this, new EventArgs());
            screenPreviewToolStripMenuItem_CheckedChanged(this, new EventArgs());

            //Init screen preview to the bottom of the screen            
            vScreenPreviewScrollBar.Value = vScreenPreviewScrollBar.Maximum - 1;
            UpdateScreenPreviewLocation();
        }

        /// <summary>
        /// Saves the loaded map/entities
        /// </summary>
        private void Save()
        {
            UnsavedEdits = false;
            map.Save(mapPath);
            //attributes.Save();
            PXEVE.Write(entities, entityPath);
        }

        #region Zoom

        const int MaxZoom = 10;

        int ZoomLevel
        {
            get => mapLayeredPictureBox.CanvasScale;
            set
            {
                if (1 <= value && value <= MaxZoom)
                {
                    //changing the scroll value in here doesn't work, it gets set to something else shortly after
                    //so it has to wait until later...
                    scrollBarNeedsUpdate = true;
                    scrollBarMultiplier = panel1.VerticalScroll.Value / (decimal)(panel1.VerticalScroll.Maximum - panel1.VerticalScroll.LargeChange - 1);
                    //setting this last so the map gets updated at the right time
                    mapLayeredPictureBox.CanvasScale  = value;
                }
            }
        }
        private void mapPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if(ModifierKeys == Keys.Control)
                ZoomLevel += (e.Delta > 0) ? 1 : -1;
        }

        bool scrollBarNeedsUpdate = false;
        decimal scrollBarMultiplier = 0;
        //HACK need to find a better event to hook into than paint
        void UpdateMapScrollPosition()
        {
            if (scrollBarNeedsUpdate)
            {
                panel1.VerticalScroll.Value = (int)((panel1.VerticalScroll.Maximum - panel1.VerticalScroll.LargeChange - 1) * scrollBarMultiplier);
                panel1.PerformLayout();
                scrollBarNeedsUpdate = false;
            }
        }

        #endregion

        #region map and entity init

        /// <summary>
        /// Initializes the entire map from scratch.
        /// only used on startup and resize
        /// </summary>
        void InitMap()
        {
            mapLayeredPictureBox.UnlockCanvasSize();

            baseMap.Image = RenderTiles(map, baseTileset, parentMod.TileSize);
            
            //init screen preview max
            vScreenPreviewScrollBar.Maximum = ((map.Height - GuxtScreenHeight) * parentMod.TileSize) + vScreenPreviewScrollBar.LargeChange - 1;
            hScreenPreviewScrollBar.Maximum = ((map.Width - GuxtScreenWidth) * parentMod.TileSize) + hScreenPreviewScrollBar.LargeChange - 1;

            mapTileTypes.Image = RenderTiles(map, tilesetTileTypes, parentMod.TileSize);

            mapLayeredPictureBox.LockCanvasSize();
        }

        void UpdateEntityIcons()
        {
            var iconImage = new Bitmap(Path.Combine(parentMod.DataPath, parentMod.EditorIconName + "." + parentMod.ImageExtension));
            if (parentMod.ImagesScrambeled)
                iconImage = Scrambler.Unscramble(iconImage);
            iconImage.MakeTransparent(Color.Black);

            EntityIcons.Images.Clear();
            var IconsPerRow = iconImage.Width / parentMod.IconSize;
            try
            {
                for (int i = 0; i < (iconImage.Width * iconImage.Height) / (parentMod.IconSize * parentMod.IconSize); i++)
                {
                    var iconX = (i % IconsPerRow) * parentMod.IconSize;
                    var iconY = (i / IconsPerRow) * parentMod.IconSize;

                    var entityIcon = iconImage.Clone(new Rectangle(
                        iconX, iconY, parentMod.IconSize, parentMod.IconSize
                        ), PixelFormat.DontCare);

                    EntityIcons.Images.Add(parentMod.EntityNames.ContainsKey(i) ? parentMod.EntityNames[i] : i.ToString(), entityIcon);
                }
            }
            //Clone() likes to throw this when you do most things wrong
            //but here it's most likley that the image isn't big enough
            catch (OutOfMemoryException)
            {
                //TODO uh...
            }

            iconImage.Dispose();
        }

        /// <summary>
        /// Initializes the entity list
        /// </summary>
        void InitEntityList()
        {
            UpdateEntityIcons();
            entityListView.Clear();
            entityListView.LargeImageList = EntityIcons;
            for (int i = 0; i < EntityIcons.Images.Count; i++)
            {
                entityListView.Items.Add(new ListViewItem()
                {
                    Text = parentMod.EntityNames.TryGetValue(i, out string val) ? val : i.ToString(),
                    ImageIndex = i,
                    //ToolTipText = ;)
                });
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
            UnsavedEdits = true;
            map.Tiles[tileNum] = tileValue;
            DrawTile(baseMap.Image, map, tileNum, baseTileset, parentMod.TileSize);
            DrawTile(mapTileTypes.Image, map, tileNum, tilesetTileTypes, parentMod.TileSize, System.Drawing.Drawing2D.CompositingMode.SourceCopy);
        }

        #endregion

        #region entity stuff

        /// <summary>
        /// Whether or not the user has any entities selected
        /// </summary>
        bool userHasSelectedEntities { get => selectedEntities.Count > 0; }
        readonly HashSet<Entity> selectedEntities = new HashSet<Entity>();

        /// <summary>
        /// Whether or not the user as copied any entities
        /// </summary>
        bool entitiesInClipboard { get => entityClipboard.Count > 0; }
        readonly HashSet<Entity> entityClipboard = new HashSet<Entity>();
                     
        private IEnumerable<Entity> GetEntitiesAtLocation(int x, int y)
        {
            return GetEntitiesAtLocation(x, y, x, y);
        }
        private IEnumerable<Entity> GetEntitiesAtLocation(int x, int y, int x2, int y2)
        {
            for(int i = 0; i< entities.Count; i++)
                if (x <= entities[i].X && entities[i].X <= x2 && y <= entities[i].Y && entities[i].Y <= y2)
                    yield return entities[i];
        }                
        void CreateNewEntity(Point pos)
        {
            UnsavedEdits = true;
            entities.Add(new Entity(0, pos.X, pos.Y, entityListView.SelectedIndices[0], 0));
            entities[entities.Count - 1].PropertyChanged += SetUnsavedEdits;
            SelectEntities(entities[entities.Count - 1]);
            RedrawAllEntityLayers();
        }
        void DeleteSelectedEntities()
        {
            UnsavedEdits = true;
            foreach (var ent in selectedEntities)
            {
                entities.Remove(ent);
                ent.PropertyChanged -= SetUnsavedEdits;
            }
            SelectEntities();
            RedrawAllEntityLayers();
        }

        /// <summary>
        /// Set what entities are being edited in the entityPorpertyGrid
        /// </summary>
        /// <param name="ents">Entities to edit</param>
        private void SetEditingEntity(params Entity[] ents)
        {
            if (ents.Length == 0)
                entityPropertyGrid.SelectedObject = null;
            else if(ents.Length == 1)
            {
                //if the entity has a custom type, use that, otherwise edit the entity directly
                entityPropertyGrid.SelectedObject = parentMod.EntityTypes.TryGetValue(ents[0].EntityID, out Type t) ? Activator.CreateInstance(t, ents) : ents[0];
            }
            else
            {
                entityPropertyGrid.SelectedObject = new MultiEntityShell(ents);
            }
        }
        /// <summary>
        /// Sets the given entities as selected. Passing no args will deselect
        /// </summary>
        /// <param name="ents">Entities to select</param>
        private void SelectEntities(params Entity[] ents)
        {
            //clear everything
            selectedEntities.Clear();
            SetEditingEntity();

            //if nothing was passed, this will be skipped
            foreach (var e in ents)
                selectedEntities.Add(e);
            //and this check would fail
            if (userHasSelectedEntities)
            {
                //try to scroll to the selected entity type, if only one is selected
                if (selectedEntities.Count == 1)
                {
                    var e = selectedEntities.First();
                    //can't scroll to an entity that doesn't exist
                    if (0 <= e.EntityID && e.EntityID < entityListView.Items.Count)
                    {
                        entityListView.SelectedIndices.Clear();
                        entityListView.SelectedIndices.Add(e.EntityID);
                        entityListView.EnsureVisible(e.EntityID);
                    }
                }
                SetEditingEntity(ents);
            }
            //so there would be nothing selected for editing, or in the selection list
            DrawSelectedEntityBoxes();
        }
        /// <summary>
        /// Copies the given entities to the clipboard
        /// </summary>
        /// <param name="ents">Entities to copy</param>
        void CopyEntities(params Entity[] ents)
        {
            Point topLeft = new Point(ents.Select(x => x.X).Min(), ents.Select(x => x.Y).Min());
            entityClipboard.Clear();
            foreach (var e in ents)
            {
                entityClipboard.Add(new Entity(e) {
                X = e.X - topLeft.X,
                Y = e.Y - topLeft.Y
                });
            }
        }
        /// <summary>
        /// Pastes the entities currently in the clipboard to the given grid position
        /// </summary>
        /// <param name="gridPos">Where on the grid to paste the entities</param>
        void PasteEntities(Point gridPos)
        {
            if (!entitiesInClipboard)
                return;
            UnsavedEdits = true;
            foreach (var e in entityClipboard)
            {
                entities.Add(new Entity(e)
                {
                    X = gridPos.X + e.X,
                    Y = gridPos.Y + e.Y
                });
                entities[entities.Count - 1].PropertyChanged += SetUnsavedEdits;
            }
            RedrawAllEntityLayers();
        }
        #endregion

        #region Mouse

        /// <summary>
        /// The mouse's current position on the grid
        /// </summary>
        Point MousePositionOnGrid = new Point(-1, -1);

        /// <summary>
        /// The grid position the user started selecting entities from
        /// </summary>
        Point EntitySelectionStart = new Point(-1, -1);
        /// <summary>
        /// The grid position the user was last selecting entities from
        /// </summary>
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
        Point maxGridPoint => new Point((map.Width * (int)editMode) - 1, (map.Height * (int)editMode) - 1);

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

        /// <summary>
        /// Whether or not the mouse is currently on the map
        /// </summary>
        bool mouseOnMap { get => mapLayeredPictureBox.ClientRectangle.Contains(mapLayeredPictureBox.PointToClient(Cursor.Position)); }
        /// <summary>
        /// Whether or not the user has selected an entity from the list of all entities
        /// </summary>
        bool entitySelected { get => entityListView.SelectedIndices.Count > 0; }

        enum HoldActions
        {
            DrawTiles,
            SelectEntities,
            MoveEntities
        }

        HoldActions? HoldAction = null;

        #region context menu methods n stuff

        /// <summary>
        /// The delete context menu item, just to save on creating a new one every time
        /// </summary>
        ToolStripMenuItem? delete = null;
        ContextMenuStrip? entityContextMenu = null;

        //can't do one for insert because it relies on the location of the click...
        void EntityContectMenu_CopyEntity(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                CopyEntities(entities[int.Parse(tsmi.Name)]);
            }
        }
        //paste also relies on that location...
        void EntityContextMenu_Delete(object sender, EventArgs e)
        {
            DeleteSelectedEntities();
            MoveMouse(MousePositionOnGrid);
        }
        void EntityContextMenu_SelectEntity(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                SelectEntities(entities[int.Parse(tsmi.Name)]);
            }
        }

        void EntityContextMenu_VisibleChanged(object sender, EventArgs e)
        {
            if(sender is ContextMenuStrip cms && cms.Visible == false && !mouseOnMap)
            {
                mapPictureBox_MouseLeave(sender, e);
            }
        }
        #endregion

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
                            mouseOverlay.Shown = false;
                            SetTile(tile);
                            mapLayeredPictureBox.Invalidate();
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
                                mouseOverlay.Shown = false;
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
                            entityContextMenu = new ContextMenuStrip();

                            //Insert
                            var insert = new ToolStripMenuItem("Insert Entity");
                            insert.Enabled = entitySelected;
                            insert.Click += delegate { if (entitySelected) { CreateNewEntity(p); MoveMouse(MousePositionOnGrid); } };
                            entityContextMenu.Items.Add(insert);
                                                        
                            //Copy
                            //TODO expand on copy/paste functionality
                            var copy = new ToolStripMenuItem("Copy");
                            //copy enabled if only one entity selected, other stuff only initiallized then too
                            if (copy.Enabled = hoveredEntitiesCount == 1)
                            {
                                copy.Name = entities.IndexOf(entitiesWhereClicked.First()).ToString();
                                copy.Click += EntityContectMenu_CopyEntity;
                            }
                            entityContextMenu.Items.Add(copy);

                            //Paste
                            var paste = new ToolStripMenuItem("Paste");
                            paste.Enabled = entitiesInClipboard;
                            paste.Click += delegate { PasteEntities(p); MoveMouse(MousePositionOnGrid); };
                            entityContextMenu.Items.Add(paste);

                            //Delete
                            if(delete == null)
                            {
                                delete = new ToolStripMenuItem();
                                delete.Click += EntityContextMenu_Delete;
                            }                            
                            delete.Text = $"Delete Entit{(selectedEntities.Count > 1 ? "ies" : "y")}";
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
                                    parentMod.EntityNames.TryGetValue(entities[index].EntityID, out entityName);
                                    var tsmi = new ToolStripMenuItem(index.ToString() + " - " + entityName);
                                    tsmi.Name = index.ToString();
                                    tsmi.Click += EntityContextMenu_SelectEntity;
                                    entityContextMenu.Items.Add(tsmi);
                                }
                            }
                            entityContextMenu.VisibleChanged += EntityContextMenu_VisibleChanged;
                            entityContextMenu.Show(mapLayeredPictureBox, e.Location);
                            #endregion
                            break;
                    }
                    break;
            }
        }
        private void mapLayeredPictureBox_MouseEnter(object sender, EventArgs e)
        {
            mouseOverlay.Shown = true;
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
                    mapLayeredPictureBox.Invalidate();
                    break;
                case HoldActions.MoveEntities:
                    int xd = MousePositionOnGrid.X - p.X;
                    int yd = MousePositionOnGrid.Y - p.Y;
                    foreach(var ent in selectedEntities)
                    {
                        ent.X -= xd;
                        ent.Y -= yd;
                    }
                    RedrawAllEntityLayers();
                    mapLayeredPictureBox.Invalidate();
                    break;
                case HoldActions.SelectEntities:
                    UpdateMouseMarquee(EntitySelectionStart, EntitySelectionEnd = p);
                    break;
                default:
                    MoveMouse(p);
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

                    SelectEntities(GetEntitiesAtLocation(x, y, xd, yd).ToArray());
                    ResetMouseSize();
                    break;
            }
            MoveMouse(MousePositionOnGrid);
            mouseOverlay.Shown = true;
            HoldAction = null;
        }

        private void mapPictureBox_MouseLeave(object sender, EventArgs e)
        {
            HoldAction = null;
            //this check is here to stop the mouse hiding when the context menu appears, since that triggers MouseLeave
            if (entityContextMenu != null && !entityContextMenu.Visible)
            {
                MousePositionOnGrid = new Point(-1, -1);
                mouseOverlay.Shown = false;
            }
            ResetMouseSize();
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
                        mapLayeredPictureBox.Invalidate();
                        break;
                    case "InsertEntity" when editMode == EditModes.Entity && mouseOnMap:
                        CreateNewEntity(MousePositionOnGrid);
                        mapLayeredPictureBox.Invalidate();
                        break;
                    case "Copy" when editMode == EditModes.Entity && userHasSelectedEntities:
                        CopyEntities(selectedEntities.ToArray());
                        break;
                    case "Paste" when editMode == EditModes.Entity && entitiesInClipboard && mouseOnMap:
                        PasteEntities(MousePositionOnGrid);
                        mapLayeredPictureBox.Invalidate();
                        break;
                    case "Save":
                        Save();
                        break;
                }
            }
        }

        #endregion

        #region menu buttons

        private void tileTypesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mapTileTypes.Shown = tileTypesToolStripMenuItem.Checked;
            DisplayTileset();
        }

        private void entitySpritesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            entityIcons.Shown = entitySpritesToolStripMenuItem.Checked;
        }

        private void entityBoxesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            entityBoxes.Shown = entityBoxesToolStripMenuItem.Checked;
        }


        private void deleteAllEntitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete EVERY entity?\nIf you save after this, there's no coming back.", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SelectEntities();
                entities.Clear();

                DrawEntityIcons();
                DrawEntityBoxes();
                DrawSelectedEntityBoxes();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        #endregion

        #region screen preview scroll bars

        private void ScreenPreviewScrollChanged(object sender, ScrollEventArgs e)
        {
            UpdateScreenPreviewLocation();
        }

        private void screenPreviewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            vScreenPreviewScrollBar.Enabled = vScreenPreviewScrollBar.Maximum > 1 && screenPreviewToolStripMenuItem.Checked;
            hScreenPreviewScrollBar.Enabled = hScreenPreviewScrollBar.Maximum > 1 && screenPreviewToolStripMenuItem.Checked;
            screenPreview.Shown = screenPreviewToolStripMenuItem.Checked;
        }

        #endregion
        
        //HACK bad event to hook into for this
        private void mapPictureBox_Paint(object sender, PaintEventArgs e)
        {
            UpdateMapScrollPosition();
        }

        private void editModeTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetMouseSize();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && UnsavedEdits)
            {
                switch(MessageBox.Show("You have unsaved changes! Would you like to save?", "Warning", MessageBoxButtons.YesNoCancel))
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
