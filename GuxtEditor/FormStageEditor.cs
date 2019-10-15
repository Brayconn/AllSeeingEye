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
        Bitmap tileset;
        Mod parentMod;
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

        }

        void InitEntityList()
        {
            entityListView.Clear();
            entityListView.LargeImageList = parentMod.EntityIcons;
            for(int i = 0; i < 16 * 8; i++)
            {
                entityListView.Items.Add(i.ToString(), i);
            }
        }

        void InitMap()
        {
            Bitmap mapImage = new Bitmap(map.Width * 16, map.Height * 16);
            DrawWholeMap(mapImage);
            if (tileTypesToolStripMenuItem.Checked)
                DrawTileTypes(mapImage);
            if (entitiesToolStripMenuItem.Checked)
                DrawEntities(mapImage);

            mapPictureBox.Image = mapImage;
        }

        //TODO a lot of hard coded values in here. Make customizeable
        void DrawWholeMap(Image img)
        {            
            using (Graphics g = Graphics.FromImage(img))
            {
                for(int i = 0; i < map.Tiles.Count; i++)
                {
                    var tilesetX = (map.Tiles[i] % 16) * 16;
                    var tilesetY = (map.Tiles[i] / 16) * 16;

                    var tileImage = tileset.Clone(new Rectangle(
                        tilesetX, tilesetY, 16, 16
                    ), PixelFormat.DontCare);

                    var x = (i % map.Width) * 16;
                    var y = (i / map.Width) * 16;

                    g.DrawImage(tileImage, x, y);
                }
            }
        }

        void DrawTileTypes(Image img)
        {

        }

        void DrawEntities(Image img)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    var entityIcon = parentMod.EntityIcons.Images[entities[i].EntityID];
                    
                    var y = (entities[i].Y * 16) / 2;
                    var x = (entities[i].X * 16) / 2;

                    g.DrawImage(entityIcon, x, y);
                }
            }
            
        }

        void DisplayTileset()
        {
            tilesetPictureBox.Image = tileset;
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
