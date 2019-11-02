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
        private string tileTypePath;//, entityNamesPath;
        public FormMain()
        {
            InitializeComponent();
            var baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            tileTypePath = Path.Combine(baseDir, "tiletypes.png");
            //entityNamesPath = Path.Combine(baseDir, "entitynames.txt");
        }

        private const string ImgFilter = "Pixel Images (*.pximg)|*.pximg";
        private const string BmpFilter = "Bitmaps (*.bmp)|*.bmp";
        private const string AllFilesFilter = "All Files (*.*)|*.*";
        private const string GuxtProjectFilter = "Guxt Mod Project (*.gux)|*.gux";
        private const string GuxtEXEFilter = "Guxt Executable (pxGame.exe)|pxGame.exe";

        private Mod? lm = null;
        private Mod? LoadedMod
        {
            get => lm;
            set
            {
                if (value != null)
                {
                    lm = value;
                    InitialiseUI();
                }
            }
        }

        private bool LoadWarning()
        {
            return LoadedMod == null ||
                MessageBox.Show("You already have a mod loaded! All unsaved progress will be lost.\n" +
                    "Are you sure you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }

        /// <summary>
        /// Initialises all lists/main ui elemets to be hooked up
        /// </summary>
        private void InitialiseUI()
        {
            //Loaded mod can't be null here, because this method is only run once LoadedMod is set to something other than null
            stagesListBox.DataSource = LoadedMod!.Stages;
            imagesListBox.DataSource = LoadedMod.Images;
            attributesListBox.DataSource = LoadedMod.Attributes;
            projectListBox.DataSource = LoadedMod.Projects;

            modPropertyGrid.SelectedObject = LoadedMod;

            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
        }

        private void ClearEditorDict(Dictionary<int, Form> dict)
        {
            foreach (var editor in dict)
                editor.Value.Close();
            dict.Clear();
        }
        private void ClearMemory()
        {
            ClearEditorDict(openStages);
            ClearEditorDict(openAttributes);
        }

        #region File

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!LoadWarning())
                return;

            using(OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Pick a Guxt exe...",
                Filter = string.Join("|", GuxtEXEFilter, AllFilesFilter)
            })
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    var dataDir = Path.Combine(Path.GetDirectoryName(ofd.FileName), "data");
                    if(Directory.Exists(dataDir))
                    {
                        ClearMemory();
                        LoadedMod = Mod.FromDataFolder(dataDir);
                    }
                    else
                    {
                        MessageBox.Show($"{dataDir} doesn't exist. Check that you're opening a stock version of Guxt.");
                    }

                }
            }
        }

        private string? savePath = null;
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!LoadWarning())
                return;

            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Pick a Guxt mod project",
                Filter = string.Join("|", GuxtProjectFilter, AllFilesFilter)
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    ClearMemory();
                    LoadedMod = Mod.Load(savePath = ofd.FileName);
                }
            }
        }
        //Save and Save As are only enabled once LoadedMod has been set to non-null
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (savePath != null)
                LoadedMod!.Save(savePath);
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
                    LoadedMod!.Save(savePath = sfd.FileName);
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
                                b.Save(sfd.FileName,ImageFormat.Bmp);
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
                                b.Save(sfd.FileName, ImageFormat.Bmp);
                        }
                    }
                }
            }
        }

        #endregion

        #region Editors

        enum AvailableEditors
        {
            Stage,
            Attribute,
            Image,
            Project
        }
        private void RemoveClosedEditor(object sender, FormClosedEventArgs e)
        {
            switch (sender)
            {
                case FormStageEditor fse:
                    openStages.Remove(fse.StageNumber);
                    break;
                case FormAttributeEditor fae:
                    openAttributes.Remove(fae.AttributeNumber);
                    break;
            }
        }

        private void AddOrOpenEditor(ListBox editorList, Dictionary<int,Form> openEditors, AvailableEditors editorType)
        {
            if(editorList.Items.Count > 0)
            {
                var sel = editorList.SelectedIndex + 1;
                if(!openEditors.ContainsKey(sel))
                {
                    Form? f = editorType switch
                    {
                        AvailableEditors.Stage => new FormStageEditor(LoadedMod!, sel, tileTypePath),
                        AvailableEditors.Attribute => new FormAttributeEditor(LoadedMod!, sel, tileTypePath),
                        _ => null
                    };
                    if (f == null)
                        return;
                    f.FormClosed += RemoveClosedEditor;
                    openEditors.Add(sel, f);
                }
                openEditors[sel].Show();
            }
        }

        Dictionary<int, Form> openStages = new Dictionary<int, Form>();
        private void StagesListBox_DoubleClick(object sender, EventArgs e)
        {
            AddOrOpenEditor(stagesListBox, openStages, AvailableEditors.Stage);
        }

        
        Dictionary<int, Form> openAttributes = new Dictionary<int, Form>();
        private void AttributesListBox_DoubleClick(object sender, EventArgs e)
        {
            AddOrOpenEditor(attributesListBox, openAttributes, AvailableEditors.Attribute);
        }

        private void ImagesListBox_DoubleClick(object sender, EventArgs e)
        {
            //TODO open image editor
        }

        private void ProjectListBox_DoubleClick(object sender, EventArgs e)
        {
            //TODO open project editor
        }

        #endregion
    }
}
