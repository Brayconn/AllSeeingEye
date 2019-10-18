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
        
        byte SelectedTile = 0;
        Point LastHover = new Point(-1, -1);

        
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

        #region map stuff
        
        void InitMap()
        {
            baseMap = new Bitmap(map.Width * parentMod.TileSize, map.Height * parentMod.TileSize);
            DrawTiles(baseMap);
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

        void DisplayMap(Point? p = null)
        {
            Bitmap mapImage = new Bitmap(baseMap);
            if (tileTypesToolStripMenuItem.Checked)
                DrawTileTypes(mapImage);
            if (entitiesToolStripMenuItem.Checked)
                DrawEntities(mapImage);
            if(p != null)
                DrawMouseOverlay(mapImage, (Point)p);

            mapPictureBox.Image?.Dispose();
            mapPictureBox.Image = mapImage;
        }

        int gridSize { get => parentMod.TileSize / (tabControl1.SelectedIndex + 1); }

        private Point GetSelectedPoint(Point p)
        {
            return new Point(p.X / gridSize, p.Y / gridSize);
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

        #endregion

        void DrawTileTypes(Image img)
        {
            //TODO implement
        }

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

        private void mapPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            Point Selection = GetSelectedPoint(e.Location);

            switch (tabControl1.SelectedIndex)
            {
                //Map
                case 0:
                    var tile = (Selection.Y * map.Width) + Selection.X;
                    map.Tiles[tile] = SelectedTile;
                    using (Graphics g = Graphics.FromImage(baseMap))
                        DrawTile(g, tile);
                    break;
                //Entity
                case 1:
                    var sel = entities.Where(entity => entity.X == Selection.X && entity.Y == Selection.Y);
                    if (sel.Any())
                        entityPropertyGrid.SelectedObject = sel.First();
                    break;
            }
        }

        private void mapPictureBox_MouseLeave(object sender, EventArgs e)
        {
            DisplayMap();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.Save(mapPath);
            //attributes.Save();
            PXEVE.Write(entities, entityPath);
        }

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

        private void mapPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var p = GetSelectedPoint(e.Location);
            if (p != LastHover)
                DisplayMap(LastHover = p);
                
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
