using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GuxtModdingFramework;
using GuxtModdingFramework.Images;
using GuxtModdingFramework.Entities;
using GuxtModdingFramework.Maps;
using static PixelModdingFramework.Rendering;
using System.Reflection;

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

            tilesetLayeredPictureBox.AddLayers(tilesetMouseOverlayLayer + 1);


            InitEntityList();

            //entities
            entityPath = Path.Combine(parentMod.DataPath, parentMod.EntityName + StageNumber + "." + parentMod.EntityExtension);
            entities = PXEVE.Read(entityPath);
            foreach (var ent in entities)
            {
                ent.PropertyChanged += SetUnsavedEdits;
                entityListBox.Items.Add(ent);
            }
            //attributes
            var attributePath = Path.Combine(parentMod.DataPath, parentMod.AttributeName + StageNumber + "." + parentMod.AttributeExtension);
            attributes = new Map(attributePath);
            tileTypes = new Bitmap(tileTypePath);

            //tileset
            var t = new Bitmap(Path.ChangeExtension(attributePath, parentMod.ImageExtension));
            if (parentMod.ImagesScrambeled)
                t = Scrambler.Unscramble(t);
            InitTilesetAndTileTypes(t);

            //base map setup
            mapPath = Path.Combine(parentMod.DataPath, parentMod.MapName + StageNumber + "." + parentMod.MapExtension);
            map = new Map(mapPath);
            //need to re-display the map from scratch in the case of a resize
            map.MapResized += delegate { UnsavedEdits = true; InitMap(); };
            mapPropertyGrid.SelectedObject = map;            

            InitMap();
                        
            //need to init entity images after the map has been initialized so we actually have a good size
            //need to init them each seperately since otherwise I think it would all be a reference to the same image...
            entityIcons.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
            entityBoxes.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
            selectedEntityBoxes.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
            DrawEntityIcons();
            DrawEntityBoxes();

            tileTypesToolStripMenuItem_CheckedChanged(this, new EventArgs());
            gridToolStripMenuItem_CheckedChanged(this, new EventArgs());
            entitySpritesToolStripMenuItem_CheckedChanged(this, new EventArgs());
            entityBoxesToolStripMenuItem_CheckedChanged(this, new EventArgs());
                        
            //Init screen preview to the bottom of the screen            
            UpdateScreenPreviewLocation(0, vScreenPreviewScrollBar.Maximum - vScreenPreviewScrollBar.LargeChange + 1);
        }

        private void FormStageEditor_Load(object sender, EventArgs e)
        {
            //need to update these scrollbars once the form has loaded, otherwise they are permanently hidden
            screenPreviewToolStripMenuItem_CheckedChanged(this, new EventArgs());
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
                    scrollBarMultiplier = pictureBoxPanel.VerticalScroll.Value / (decimal)(pictureBoxPanel.VerticalScroll.Maximum - pictureBoxPanel.VerticalScroll.LargeChange + 1);
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
                pictureBoxPanel.VerticalScroll.Value = (int)Math.Round((pictureBoxPanel.VerticalScroll.Maximum - pictureBoxPanel.VerticalScroll.LargeChange + 1) * scrollBarMultiplier);
                pictureBoxPanel.PerformLayout();
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
            //hiding both of these so they can't affect the canvas size
            bool spShown = screenPreview.Shown;
            screenPreview.Shown = false;
            bool mShown = mouseOverlay.Shown;
            mouseOverlay.Shown = false;
            
            mapLayeredPictureBox.UnlockCanvasSize();
            {   
                baseMap.Image = RenderTiles(map, (Bitmap)baseTileset.Image, parentMod.TileSize);

                mapTileGrid.Image = new Bitmap(baseMap.Image.Width, baseMap.Image.Height);
                RedrawGrid();

                //init screen preview max
                var vMax = (map.Height - GuxtScreenHeight) * parentMod.TileSize;
                var hMax = (map.Width - GuxtScreenWidth) * parentMod.TileSize;
                vScreenPreviewScrollBar.Maximum = vMax + vScreenPreviewScrollBar.LargeChange - 1;
                hScreenPreviewScrollBar.Maximum = hMax + hScreenPreviewScrollBar.LargeChange - 1;
                //clamp the screen preview inside the map
                UpdateScreenPreviewLocation(Math.Min(screenPreview.Location.X, hMax), Math.Min(screenPreview.Location.Y, vMax));
                //this call won't do anything the first time this function is called, but it will work on subsequent calls
                UpdateScreenPreviewScrollbars();

                mapTileTypes.Image = RenderTiles(map, (Bitmap)tilesetTileTypes.Image, parentMod.TileSize);
            }
            mapLayeredPictureBox.LockCanvasSize();

            //show the ones that were shown again
            screenPreview.Shown = spShown;
            mouseOverlay.Shown = mShown;
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
                MoveTileSelection();
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
            DrawTile(baseMap.Image, map, tileNum, (Bitmap)baseTileset.Image, parentMod.TileSize);
            DrawTile(mapTileTypes.Image, map, tileNum, (Bitmap)tilesetTileTypes.Image, parentMod.TileSize, System.Drawing.Drawing2D.CompositingMode.SourceCopy);
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

        private string GetEntityIndexAndName(Entity ent)
        {
            string entityName = "<no name>";
            parentMod.EntityNames.TryGetValue(ent.EntityID, out entityName);
            return $"{entities.IndexOf(ent)} - {entityName}";
        }
        private void AddEntity(Entity ent)
        {
            ent.PropertyChanged += SetUnsavedEdits;
            entities.Add(ent);
            entityListBox.Items.Add(ent);
        }
        private void InsertEntity(int index, Entity ent)
        {
            ent.PropertyChanged += SetUnsavedEdits;
            entities.Insert(index, ent);
            entityListBox.Items.Insert(index, ent);
        }
        private void MoveEntity(Entity ent, int difference)
        {
            if(difference != 0)
            {
                var currentIndex = entities.IndexOf(ent);
                if (currentIndex == -1)
                    throw new ArgumentException("Entity not found!", nameof(ent));
                var newIndex = currentIndex + difference;
                RemoveEntity(ent);
                if (newIndex > entities.Count)
                    AddEntity(ent);
                else
                    InsertEntity(newIndex, ent);
            }
        }
        private void RemoveEntity(Entity ent)
        {
            entities.Remove(ent);
            entityListBox.Items.Remove(ent);
            ent.PropertyChanged -= SetUnsavedEdits;
        }

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
            if (entityListView.SelectedIndices.Count <= 0)
                return;
            UnsavedEdits = true;
            
            var ent = new Entity(0, pos.X, pos.Y, entityListView.SelectedIndices[0], 0);
            AddEntity(ent);
            SelectEntities(ent);

            RedrawAllEntityLayers();
        }
        void DeleteSelectedEntities()
        {
            UnsavedEdits = true;
            foreach (var ent in selectedEntities.ToList())
            {
                RemoveEntity(ent);
            }
            entityListBox.SelectedItems.Clear();
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
            entityListBox.SelectedItems.Clear();
            SetEditingEntity();

            //only select things if there's something to select
            if(ents.Length > 0)
            { 
                foreach (var e in ents)
                {
                    selectedEntities.Add(e);
                    entityListBox.SelectedItems.Add(e);
                }
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
            if (entitiesInClipboard)
            {
                UnsavedEdits = true;
                foreach (var e in entityClipboard)
                {
                    AddEntity(new Entity(e)
                    {
                        X = gridPos.X + e.X,
                        Y = gridPos.Y + e.Y
                    });
                }
                RedrawAllEntityLayers();
            }
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
                2 => EditModes.Entity,
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
                                    
                                    var tsmi = new ToolStripMenuItem(GetEntityIndexAndName(ent));
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
            if (entityContextMenu == null || !entityContextMenu.Visible)
                mouseOverlay.Shown = false;            
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
            tilesetTileTypes.Shown = tileTypesToolStripMenuItem.Checked;
        }

        private void gridToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mapTileGrid.Shown = gridToolStripMenuItem.Checked;
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
            int h = hScreenPreviewScrollBar.Value;
            int v = vScreenPreviewScrollBar.Value;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                v = e.NewValue;
            else if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                h = e.NewValue;
            else
                throw new ArgumentException();
            UpdateScreenPreviewLocation(h,v);
        }
                
        /// <summary>
        /// Toggles the visisbility of a scrollbar, and adjusts the size of the pictureboxpanel and the scrollbars to fit
        /// </summary>
        /// <param name="visible">Whether or not to show this scrollbar</param>
        /// <param name="sb1">The scrollbar to show/hide</param>
        /// <param name="sb2">A second scrollbar that may need to move out of the way, or that could be expanded</param>
        void ToggleScrollbarVisible(bool visible, ScrollBar sb1, ScrollBar sb2)
        {
            int GetScrollBarLength(ScrollBar sb)
            {
                if (sb is VScrollBar v)
                    return v.Height;
                else if (sb is HScrollBar h)
                    return h.Width;
                else
                    throw new ArgumentException();
            }
            void SetScrollBarLength(ScrollBar sb, int value)
            {
                if (sb is VScrollBar v)
                {
                    v.Height = value;
                    pictureBoxPanel.Height = value;
                }
                else if (sb is HScrollBar h)
                {
                    h.Width = value;
                    pictureBoxPanel.Width = value;
                }
                else
                    throw new ArgumentException();
            }
            int GetScrollBarWidth(ScrollBar sb)
            {
                if (sb is VScrollBar v)
                    return v.Width;
                else if (sb is HScrollBar h)
                    return h.Height;
                else
                    throw new ArgumentException();
            }
            //int SetScrollBarWidth(ScrollBar sb); //never need to set the scrollbar width
                        
            if (visible)
                //move the other one out of the way to make room for us
                SetScrollBarLength(sb2, GetScrollBarLength(sb2) - GetScrollBarWidth(sb1));
            else
                //the other one can take up all the space now that we're gone
                SetScrollBarLength(sb2, GetScrollBarLength(sb2) + GetScrollBarWidth(sb1));
            
            sb1.Visible = visible;
        }
        void UpdateScreenPreviewScrollbars()
        {
            bool showV = vScreenPreviewScrollBar.Maximum > vScreenPreviewScrollBar.LargeChange && screenPreviewToolStripMenuItem.Checked;
            if(vScreenPreviewScrollBar.Visible != showV)
                ToggleScrollbarVisible(showV, vScreenPreviewScrollBar, hScreenPreviewScrollBar);

            bool showH = hScreenPreviewScrollBar.Maximum > hScreenPreviewScrollBar.LargeChange && screenPreviewToolStripMenuItem.Checked;
            if(hScreenPreviewScrollBar.Visible != showH)
                ToggleScrollbarVisible(showH, hScreenPreviewScrollBar, vScreenPreviewScrollBar);
        }
        private void screenPreviewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            UpdateScreenPreviewScrollbars();
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

        #region entity list box

        private void entityListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            IEnumerable<Entity> listboxSelectedEntities()
            {
                var iter = entityListBox.SelectedItems.GetEnumerator();
                while (iter.MoveNext())
                    yield return (Entity)iter.Current;
            }            
            if (!selectedEntities.SetEquals(listboxSelectedEntities()))
            {
                SelectEntities(listboxSelectedEntities().ToArray());
                /*
                selectedEntities.Clear();
                foreach (var ent in listboxSelectedEntities())
                    selectedEntities.Add(ent);
                */
            }
        }

        private void entityListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is Entity ent && e.DesiredType == typeof(string))
                e.Value = GetEntityIndexAndName(ent);
        }

        bool isHolding = false;
        int startIndex = -1;
        int endIndex = -1;

        private void entityListBox_MouseDown(object sender, MouseEventArgs e)
        {
            var clickedIndex = entityListBox.IndexFromPoint(new Point(e.X, e.Y));
            if (clickedIndex == -1)
                return;
            if (e.Button == MouseButtons.Left && !isHolding)
            {
                startIndex = clickedIndex;
                isHolding = true;
            }
        }

        private void entityListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isHolding)
            {
                //hopefully stopping unnessecary entity selections
                if(entityListBox.SelectedIndex != startIndex)
                    entityListBox.SelectedIndex = startIndex;
                endIndex = entityListBox.IndexFromPoint(new Point(e.X, e.Y));
                if (startIndex != endIndex)
                    DoDragDrop(selectedEntities, DragDropEffects.Move);
            }
        }

        private void entityListBox_MouseUp(object sender, MouseEventArgs e)
        {
            isHolding = false;
        }

        private void entityListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetFormats().Any(x => e.Data.GetData(x) is HashSet<Entity>) && e.AllowedEffect == DragDropEffects.Move)
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void entityListBox_DragDrop(object sender, DragEventArgs e)
        {
            int GetIndex()
            {
                var p = entityListBox.PointToClient(new Point(e.X, e.Y));
                int index = entityListBox.IndexFromPoint(p);
                //default to adding to the end of the list
                if (index < 0)
                    index = entityListBox.Items.Count;
                return index;
            }
            int index = Math.Min(entityListBox.Items.Count - 1, GetIndex());
            var entitiesToMove = e.Data.GetFormats().Select(x => e.Data.GetData(x) as HashSet<Entity>).FirstOrDefault().ToArray();
            
            //sort list by index
            var entsInOrder = (IEnumerable<Entity>)entitiesToMove.OrderBy(x => entities.IndexOf(x));
            if (startIndex < index) //always start from the end closest to where we're moving
                entsInOrder = entsInOrder.Reverse();
            //calculate how much to move each entity by
            var difference = index - startIndex;
            
            foreach (var entity in entsInOrder)
            {
                MoveEntity(entity, difference);
            }
            SelectEntities(entitiesToMove);
            //HACK need to refresh the entity list, and for whatever reason that method is private
            typeof(ListBox).InvokeMember("RefreshItems",
              BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
              null, entityListBox, Array.Empty<object>());

            isHolding = false;
        }

        #endregion

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
