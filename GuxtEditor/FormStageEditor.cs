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
            parentMod = m;
            StageNumber = stageNum;

            InitializeComponent();            
            mapPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;            
            InitEntityList();

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

        int gridSize { get => parentMod.TileSize / (tabControl1.SelectedIndex + 1); }

        private Point GetSelectedPoint(Point p)
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

        enum AvailableActions
        {
            DrawTiles,
            PickTile,
            SelectEntities,
            MoveEntities
        }

        AvailableActions? CurrentAction = null;

        byte SelectedTile = 0;
        Point LastTileDrawn = new Point(-1, -1);
        Point EntitySelectionStart = new Point(-1, -1);

        #region Map interaction

        private void mapPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Point Selection = GetSelectedPoint(e.Location);

            switch (tabControl1.SelectedIndex)
            {
                //Map
                case 0:
                    var tile = (Selection.Y * map.Width) + Selection.X;
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            CurrentAction = AvailableActions.DrawTiles;
                                                        
                            map.Tiles[tile] = SelectedTile;
                            DrawTile(tile);
                            DisplayMap();
                            break;
                        case MouseButtons.Middle:
                            CurrentAction = AvailableActions.PickTile;

                            SelectedTile = map.Tiles[tile];
                            DisplayTileset();
                            break;
                    }
                    break;
                //Entity
                case 1:
                    switch(e.Button)
                    {
                        case MouseButtons.Left:
                            CurrentAction = AvailableActions.SelectEntities;
                            EntitySelectionStart = Selection;
                            break;
                    }
                    break;
            }
        }

        private void mapPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var p = GetSelectedPoint(e.Location);
            //if we're still on the same grid space, stop
            if (p == LastTileDrawn)
                return;
            //otherwise, we've moved, so update to new position
            LastTileDrawn = p;

            //do the actual work
            switch (CurrentAction)
            {
                case AvailableActions.DrawTiles:
                    var tile = (p.Y * map.Width) + p.X;
                    map.Tiles[tile] = SelectedTile;
                    DrawTile(tile);
                    break;
            }

            //display map either way
            //but only display cursor if not doing something already
            if (CurrentAction == null)
                DisplayMap(p);
            else
                DisplayMap();
        }

        private void mapPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            switch(CurrentAction)
            {
                case AvailableActions.SelectEntities:
                    var EntitytSelectionEnd = GetSelectedPoint(e.Location);
                    //TODO M E S S Y
                    int x = Math.Min(EntitySelectionStart.X, EntitytSelectionEnd.X);
                    int y = Math.Min(EntitySelectionStart.Y, EntitytSelectionEnd.Y);
                    int xd = Math.Max(EntitySelectionStart.X, EntitytSelectionEnd.X);
                    int yd = Math.Max(EntitySelectionStart.Y, EntitytSelectionEnd.Y);

                    var sel = entities.Where(entity => x <= entity.X && entity.X <=xd && y <= entity.Y && entity.Y <= yd);
                    if (sel.Any())
                        entityPropertyGrid.SelectedObject = sel.First();
                    break;
            }
            CurrentAction = null;            
        }

        private void mapPictureBox_MouseLeave(object sender, EventArgs e)
        {
            DisplayMap();
        }

        #endregion

        #region Tileset Interaction

        private void tilesetPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var p = GetSelectedPoint(e.Location);
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
