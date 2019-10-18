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
    public partial class FormStageEditor: Form
    {
        List<Entity> entities;
        Map map, attributes;
        Bitmap tileset, baseMapImage;
        Mod parentMod;

        Point LastSelected = new Point(-1, -1);

        public FormStageEditor(Mod m, int stageNum)
        {
            parentMod = m;

            InitializeComponent();            
            mapPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;            
            InitEntityList();

            

            //base map setup
            map = new Map(Path.Combine(parentMod.DataPath, parentMod.MapName + stageNum + "." + parentMod.MapExtension));
            mapPropertyGrid.SelectedObject = map;

            var tilesetPath = Path.Combine(parentMod.DataPath, parentMod.AttributeName + stageNum + "." + parentMod.ImageExtension);
            tileset = new Bitmap(tilesetPath);
            if (parentMod.ImagesScrambeled)
                tileset = Scrambler.Unscramble(tileset);

            attributes = new Map(Path.ChangeExtension(tilesetPath, parentMod.AttributeExtension));

            entities = PXEVE.Read(Path.Combine(parentMod.DataPath, parentMod.EntityName + stageNum + "." + parentMod.EntityExtension));

            
            DisplayTileset();

            InitMap();
            DrawTiles();

        }

        void InitEntityList()
        {
            entityListView.Clear();
            entityListView.LargeImageList = parentMod.EntityIcons;
            for(int i = 0; i < parentMod.EntityIcons.Images.Count; i++)
            {
                entityListView.Items.Add(i.ToString(), i);
            }
        }

        void InitMap()
        {
            baseMapImage = new Bitmap(map.Width * parentMod.TileSize, map.Height * parentMod.TileSize);
            DrawWholeMap(baseMapImage);
        }

        void DrawWholeMap(Image img)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                for (int i = 0; i < map.Tiles.Count; i++)
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
            }
        }

        void DrawTiles(Point? p = null)
        {
            Bitmap mapImage = new Bitmap(baseMapImage);
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

        void DrawTileTypes(Image img)
        {
            //TODO implement
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

        void DisplayTileset()
        {
            tilesetPictureBox.Image = tileset;
        }

        private void mapPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            Point Selection = GetSelectedPoint(e.Location);

            switch (tabControl1.SelectedIndex)
            {
                //Map
                case 0:
                    //get slection as tile

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
            DrawTiles();
        }

        private void mapPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var p = GetSelectedPoint(e.Location);
            if (p != LastSelected)
                DrawTiles(LastSelected = p);
                
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
