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
        Map map, attributes;
        Bitmap baseTileset, baseMap, tileset;

        public FormStageEditor(Mod m, int stageNum)
        {
            //everything needs this stuff
            parentMod = m;
            StageNumber = stageNum;

            //UI init
            InitializeComponent();            
            mapPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;            
            InitEntityList();

            //Tool strip menu items init

            createEntityToolStripMenuItem = new ToolStripMenuItem("Insert Entity");
            createEntityToolStripMenuItem.Click += CreateNewEntity;

            //base map setup
            mapPath = Path.Combine(parentMod.DataPath, parentMod.MapName + StageNumber + "." + parentMod.MapExtension);
            map = new Map(mapPath);
            mapPropertyGrid.SelectedObject = map;

            var tilesetPath = Path.Combine(parentMod.DataPath, parentMod.AttributeName + StageNumber + "." + parentMod.ImageExtension);
            tileset = new Bitmap(tilesetPath);
            if (parentMod.ImagesScrambeled)
                tileset = Scrambler.Unscramble(tileset); 

            attributes = new Map(Path.ChangeExtension(tilesetPath, parentMod.AttributeExtension));

            entityPath = Path.Combine(parentMod.DataPath, parentMod.EntityName + StageNumber + "." + parentMod.EntityExtension);
            entities = PXEVE.Read(entityPath);

            InitTileset();
            DisplayTileset();

            InitMap();
            DisplayMap();

        }

        void InitEntityList()
        {
            entityListView.Clear();
            entityListView.LargeImageList = parentMod.EntityIcons;
            for (int i = 0; i < parentMod.EntityIcons.Images.Count; i++)
            {
                entityListView.Items.Add(i.ToString(), i);
            }
        }

        int gridSize { get => parentMod.TileSize / (editModeTabControl.SelectedIndex + 1); }

        private Point GetMousePointOnGrid(Point p)
        {
            return new Point(p.X / gridSize, p.Y / gridSize);
        }

        #region Map display

        void InitMap()
        {
            baseMap = new Bitmap(map.Width * parentMod.TileSize, map.Height * parentMod.TileSize);
            DrawTiles(baseMap);
        }
        void DrawTiles(Image img)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                for (int i = 0; i < map.Tiles.Count; i++)
                {
                    DrawTile(g, i);
                }
            }
        }
        void DrawTile(int i)
        {
            using (Graphics g = Graphics.FromImage(baseMap))
                DrawTile(g, i);
        }
        void DrawTile(Graphics g, int i)
        {
            var tilesetX = (map.Tiles[i] % 16) * parentMod.TileSize;
            var tilesetY = (map.Tiles[i] / 16) * parentMod.TileSize;

            var x = (i % map.Width) * parentMod.TileSize;
            var y = (i / map.Width) * parentMod.TileSize;
            
            using (var tileImage = tileset.Clone(new Rectangle(
                tilesetX, tilesetY, parentMod.TileSize, parentMod.TileSize
            ), PixelFormat.DontCare))
            {
                g.DrawImage(tileImage, x, y, parentMod.TileSize, parentMod.TileSize);
            }
        }        
        void DrawEntities(Image img)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    var entityIcon = parentMod.EntityIcons.Images[entities[i].EntityID];

                    var y = (entities[i].Y * parentMod.TileSize) / 2;
                    var x = (entities[i].X * parentMod.TileSize) / 2;

                    g.DrawImage(entityIcon, x, y, parentMod.IconSize, parentMod.IconSize);
                }
            }

        }
        private void DrawMouseOverlay(Image img, Point p)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                int x = p.X * gridSize;
                int y = p.Y * gridSize;

                int width = gridSize - 1;
                int height = gridSize - 1;
                
                g.DrawRectangle(new Pen(Color.LightGray), x, y, width, height);
            }
        }
        void DisplayMap(Point? p = null)
        {
            Bitmap mapImage = new Bitmap(baseMap);
            if (tileTypesToolStripMenuItem.Checked)
                DrawTileTypes(mapImage);
            if (entitiesToolStripMenuItem.Checked)
                DrawEntities(mapImage);
            if (p != null)
                DrawMouseOverlay(mapImage, (Point)p);

            mapPictureBox.Image?.Dispose();
            mapPictureBox.Image = mapImage;
        }

        #endregion

        void DrawTileTypes(Image img)
        {
            //TODO implement
        }

        #region Tileset display

        void InitTileset()
        {
            baseTileset = new Bitmap(parentMod.TileSize * 16, parentMod.TileSize * 16);
            using (Graphics g = Graphics.FromImage(baseTileset))
            {
                //init to black
                g.DrawRectangle(new Pen(Color.Black), 0, 0, baseTileset.Width, baseTileset.Height);
            }
        }
        void DrawTileset(Image img)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                g.DrawImage(tileset, 0, 0, tileset.Width, tileset.Height);
            }            
        }
        void DrawSelectedTile(Image img)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                g.DrawRectangle(new Pen(Color.LightGray),
                    (SelectedTile % 16) * parentMod.TileSize,
                    (SelectedTile / 16) * parentMod.TileSize,
                    parentMod.TileSize - 1,
                    parentMod.TileSize - 1);
            }
        }
        void DisplayTileset()
        {
            var thing = new Bitmap(baseTileset);
            DrawTileset(thing);
            DrawSelectedTile(thing);

            tilesetPictureBox.Image?.Dispose();
            tilesetPictureBox.Image = thing;
        }

        #endregion

        void SetTile(Point p)
        {
            SetTile((p.Y * map.Width) + p.X);
        }
        void SetTile(int tile)
        {
            map.Tiles[tile] = SelectedTile;
            DrawTile(tile);
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
        private void CreateNewEntity(object sender, EventArgs e)
        {
            if (entityListView.SelectedIndices.Count > 0)
            {
                entities.Add(new Entity(0, MousePositionOnGrid.X, MousePositionOnGrid.Y, entityListView.SelectedIndices[0], 0));
                DisplayMap(MousePositionOnGrid);
            }
        }
        private void SelectEntity(object sender, EventArgs e)
        {
            if(sender is ToolStripMenuItem tsmi)
                entityPropertyGrid.SelectedObject = entities[int.Parse(tsmi.Name)];
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
                        case MouseButtons.Right:
                            ContextMenuStrip entityContextMenu = new ContextMenuStrip();
                            entityContextMenu.Items.Add(createEntityToolStripMenuItem);
                            //TODO add paste

                            var c = hoveredEntities.Count();
                            //add copy and delete
                            if (c == 1)
                            {
                                //TODO add copy and delete
                            }
                            //anything else is bigger, so we gotta check what entity to select
                            else if(c > 1)
                            {
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

        private void mapPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var p = GetMousePointOnGrid(e.Location);
            //if we're still on the same grid space, stop
            if (p == MousePositionOnGrid)
                return;
            
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
                    selectedEntities = new HashSet<Entity>(sel);
                    entityPropertyGrid.SelectedObject = sel.Any() ? sel.First() : null;
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.Save(mapPath);
            //attributes.Save();
            PXEVE.Write(entities, entityPath);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            else
                base.OnFormClosing(e);
        }
    }
}
