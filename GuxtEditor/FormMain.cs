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

namespace GuxtEditor
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private const string ImgFilter = "Pixel Images (*.pximg)|*.pximg";
        private const string BmpFilter = "Bitmaps (*.bmp)|*.bmp";
        private const string AllFilesFilter = "All Files (*.*)|*.*";
        private const string GuxtProjectFilter = "Guxt Mod Project (*.gux)|*.gux";

        private Mod lm = null;
        private Mod LoadedMod
        {
            get => lm;
            set
            {
                lm = value;
                InitialiseLists();
            }
        }

        private void InitialiseLists()
        {
            mapsListBox.Items.AddRange(LoadedMod.Maps.ToArray());
            entitiesListBox.Items.AddRange(LoadedMod.Entities.ToArray());
            imagesListBox.Items.AddRange(LoadedMod.Images.ToArray());
            attributesListBox.Items.AddRange(LoadedMod.Attributes.ToArray());
            projectListBox.Items.AddRange(LoadedMod.Projects.ToArray());
        }

        #region File

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Pick a Guxt exe...",
                Filter = "pxGame.exe (pxGame.exe)|pxGame.exe|All Files (*.*)|*.*"
            })
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    var dataDir = Path.Combine(Path.GetDirectoryName(ofd.FileName), "data");
                    if(Directory.Exists(dataDir))
                    {
                        LoadedMod = Mod.FromDataFolder(dataDir);
                        InitialiseLists();
                    }
                    else
                    {
                        MessageBox.Show($"{dataDir} doesn't exist. Check that you're opening a stock version of Guxt.");
                    }

                }
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Pick a Guxt mod project",
                Filter = string.Join("|", GuxtProjectFilter, AllFilesFilter)
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    LoadedMod = Mod.Load(ofd.FileName);
            }
        }

        private string savePath = null;
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (savePath != null)
                LoadedMod.Save(savePath);
            else
                SaveAsToolStripMenuItem_Click(sender, e);
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Choose a save location for this project...",
                Filter = string.Join("|", GuxtProjectFilter, AllFilesFilter)
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    LoadedMod.Save(savePath = sfd.FileName);
                }
            }
        }

        #endregion

        #region Edit

        private void ScrambleImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Choose a file to scramble...",
                Filter = string.Join("|", BmpFilter, ImgFilter, AllFilesFilter)
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (SaveFileDialog sfd = new SaveFileDialog()
                    {
                        Title = "Choose where to output the file...",
                        Filter = string.Join("|", ImgFilter, BmpFilter, AllFilesFilter)
                    })
                    {
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            using (Bitmap b = Scrambler.Scramble(new Bitmap(ofd.FileName)))
                                b.Save(sfd.FileName);
                        }
                    }
                }
            }
        }

        private void UnscrambleImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Choose a file to unscramble...",
                Filter = string.Join("|", ImgFilter, BmpFilter, AllFilesFilter)
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (SaveFileDialog sfd = new SaveFileDialog()
                    {
                        Title = "Choose where to output the file...",
                        Filter = string.Join("|", BmpFilter, ImgFilter, AllFilesFilter)
                    })
                    {
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            using (Bitmap b = Scrambler.Unscramble(new Bitmap(ofd.FileName)))
                                b.Save(sfd.FileName);
                        }
                    }
                }
            }
        }

        #endregion

        #region Open editors

        private void MapsListBox_DoubleClick(object sender, EventArgs e)
        {
            //TODO open map editor
        }

        private void EntitiesListBox_DoubleClick(object sender, EventArgs e)
        {
            //TODO open entity editor
        }

        private void ImagesListBox_DoubleClick(object sender, EventArgs e)
        {
            //TODO open image editor
        }

        private void AttributesListBox_DoubleClick(object sender, EventArgs e)
        {
            //TODO open attribute editor
        }

        private void ProjectListBox_DoubleClick(object sender, EventArgs e)
        {
            //TODO open project editor
        }

        #endregion

    }
}
