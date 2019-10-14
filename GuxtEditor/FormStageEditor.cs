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
        public FormStageEditor(Mod m, int stageNum)
        {
            InitializeComponent();

            map = new Map(Path.Combine(m.DataPath, m.MapName + stageNum + "." + m.MapExtension));

            var backgroundPath = Path.Combine(m.DataPath, m.AttributeName + stageNum + "." + m.ImageExtension);
            tileset = new Bitmap(backgroundPath);
            if (m.ImagesScrambeled)
                tileset = Scrambler.Unscramble(tileset);

            attributes = new Map(Path.ChangeExtension(backgroundPath, m.AttributeExtension));

            entities = PXEVE.Read(Path.Combine(m.DataPath, m.EntityName + stageNum + "." + m.EntityExtension));

            pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            DisplayTileset();
            DisplayMap();

        }

        void DisplayMap()
        {
            Bitmap mapImage = new Bitmap(map.Width * 16, map.Height * 16);
            using (Graphics g = Graphics.FromImage(mapImage))
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
            pictureBox2.Image = mapImage;
        }

        void DisplayTileset()
        {
            pictureBox1.Image = tileset;
        }
    }
}
