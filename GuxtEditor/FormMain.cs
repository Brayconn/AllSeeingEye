using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GuxtModdingFramework;
using GuxtModdingFramework.Images;
using WinFormsKeybinds;

namespace GuxtEditor
{
    public partial class FormMain : Form
    {
        private readonly ReadOnlyDictionary<KeyInput, string> stageEditorKeybinds;
        private readonly string tileTypePath;//, entityNamesPath;
        private FormMain(string ttp, IDictionary<KeyInput, string> kb)
        {
            tileTypePath = ttp;
            stageEditorKeybinds = new ReadOnlyDictionary<KeyInput, string>(kb);

            InitializeComponent();
        }

        public static FormMain Create(string tileTypePath, KeybindCollection kc)
        {
            var kb = kc.ToDictionary();
            return new FormMain(tileTypePath, kb);
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
        /// Fills the stage list with all the right numbers
        /// </summary>
        private void FillStagesList(int value)
        {
            stagesListBox.Items.Clear();
            for (int i = 1; i <= value; i++)
                stagesListBox.Items.Add($"Stage {i}");
        }

        /// <summary>
        /// Fills up the given list with the file names from the given directory, that match the given extension, but don't match the given filter
        /// </summary>
        /// <param name="list">The list to fill</param>
        /// <param name="dir">Directory to search</param>
        /// <param name="ext">Extension the files must have</param>
        private void FillWithFileNames(ListBox.ObjectCollection list, string ext)//, string? filter = null)
        {
            if (LoadedMod == null)
                return;
            list.Clear();
            foreach (var f in Directory.EnumerateFiles(LoadedMod.DataPath, "*." + ext))
                //if (filter == null || !Regex.Match(f, $@"^{filter}\d+\.{ext}$").Success)
                    list.Add(Path.GetFileName(f));
        }

        /// <summary>
        /// Initialises all lists/main ui elemets to be hooked up
        /// </summary>
        private void InitialiseUI()
        {
            //Loaded mod can't be null here, because this method is only run once LoadedMod is set to something other than null
            LoadedMod!.StageCountChanged += (o,e) => { FillStagesList(e.Current); };
            FillStagesList(LoadedMod.StageCount);

            LoadedMod.ImageExtensionChanged += (o, e) => { FillWithFileNames(imagesListBox.Items, e.Current); };
            FillWithFileNames(imagesListBox.Items, LoadedMod.ImageExtension);

            LoadedMod.AttributeExtensionChanged += (o, e) => { FillWithFileNames(attributesListBox.Items, e.Current); };
            FillWithFileNames(attributesListBox.Items, LoadedMod.AttributeExtension);

            LoadedMod.ProjectExtensionChanged += (o, e) => { FillWithFileNames(projectListBox.Items, e.Current); };
            FillWithFileNames(projectListBox.Items, LoadedMod.ProjectExtension);

            modPropertyGrid.SelectedObject = LoadedMod;

            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;

            openBPPToolStripMenuItem.Enabled = true;
        }

        private static void ClearEditorDict(Dictionary<int, Form> dict)
        {
            foreach (var editor in dict)
                editor.Value.Close();
            dict.Clear();
        }
        private void ClearMemory()
        {
            guxtEXEPath = null;
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

        private void MassUnscrambleToBMP(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to unscramble all selected images to <filename>.bmp?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            foreach(string item in imagesListBox.SelectedItems)
            {
                //this method can only run when images are in the list which means loadedmod is not null
                string filepath = Path.Combine(LoadedMod!.DataPath, item);
                Scrambler.Unscramble(new Bitmap(filepath)).Save(Path.ChangeExtension(filepath,"bmp"),ImageFormat.Bmp);
            }
        }

        private void MassReplaceWithBMP(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to replace all selected images with the version from <filename>.bmp?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            foreach(string item in imagesListBox.SelectedItems)
            {
                //see a few lines above about the LoadedMod!
                string pximg = Path.Combine(LoadedMod!.DataPath, item);
                string bmp = Path.ChangeExtension(pximg, "bmp");
                Scrambler.Scramble(new Bitmap(bmp)).Save(pximg, ImageFormat.Bmp);
            }
        }

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
                    //HACK this can probably be done better, but it's going to require redoing the AddOrRemoveEditor function
                    foreach(var kvp in openAttributes)
                    {
                        if(kvp.Value == fae)
                        {
                            openAttributes.Remove(kvp.Key);
                            break;
                        }
                    }
                    break;
            }
        }

        private void AddOrOpenEditor(ListBox editorList, Dictionary<int,Form> openEditors, AvailableEditors editorType)
        {
            if(editorList.Items.Count > 0)
            {
                var selectedMap = editorList.SelectedIndex + 1;
                if(!openEditors.ContainsKey(selectedMap))
                {
                    Form? f = editorType switch
                    {
                        AvailableEditors.Stage => new FormStageEditor(LoadedMod!, selectedMap, tileTypePath, stageEditorKeybinds),
                        AvailableEditors.Attribute => new FormAttributeEditor(LoadedMod!, editorList.SelectedItem.ToString(), tileTypePath, stageEditorKeybinds),
                        _ => null
                    };
                    if (f == null)
                        return;
                    f.FormClosed += RemoveClosedEditor;
                    openEditors.Add(selectedMap, f);
                }
                openEditors[selectedMap].Show();
            }
        }

        readonly Dictionary<int, Form> openStages = new Dictionary<int, Form>();
        private void StagesListBox_DoubleClick(object sender, EventArgs e)
        {
            AddOrOpenEditor(stagesListBox, openStages, AvailableEditors.Stage);
        }

        
        readonly Dictionary<int, Form> openAttributes = new Dictionary<int, Form>();
        private void AttributesListBox_DoubleClick(object sender, EventArgs e)
        {
            AddOrOpenEditor(attributesListBox, openAttributes, AvailableEditors.Attribute);
        }
        private void imagesListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip imagesCMS = new ContextMenuStrip();

                var enabled = imagesListBox.SelectedIndices.Count > 0;

                ToolStripMenuItem unscramble = new ToolStripMenuItem("Unscramble to bmp...");
                unscramble.Click += MassUnscrambleToBMP;
                unscramble.Enabled = enabled;
                imagesCMS.Items.Add(unscramble);

                ToolStripMenuItem replace = new ToolStripMenuItem("Replace with bmp...");
                replace.Click += MassReplaceWithBMP;
                replace.Enabled = enabled;
                imagesCMS.Items.Add(replace);

                imagesCMS.Show(imagesListBox, e.Location);
            }
        }

        private void ProjectListBox_DoubleClick(object sender, EventArgs e)
        {
            //TODO open project editor
        }

        #endregion

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing
                && (openStages.Any(x => ((FormStageEditor)x.Value).UnsavedEdits)
                || openAttributes.Any(x => ((FormAttributeEditor)x.Value).UnsavedEdits)))
            {
                if(MessageBox.Show("You still have editors open! Are you sure you want to close without saving them?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        e.Cancel = true;
            }
        }

        string? guxtEXEPath;
        private void openBPPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!File.Exists(Patcher.Default.ExePath))
            {
                MessageBox.Show("You haven't configured your patcher path! Please fill it in the settings file.", "Error");
                return;
            }
            //base address for guxt is 0x400000, not 0x40100 as RAIN thought it was 😤
            var args = "-b 4194304";
            
            if (Directory.Exists(Patcher.Default.HackFolder))
                args += $" -h \"{Patcher.Default.HackFolder}\"";

            //If you've saved
            if (File.Exists(savePath))
                args += $" -l \"{Path.ChangeExtension(savePath, "ph")}\"";

            var exeDir = Path.GetDirectoryName(LoadedMod!.DataPath);
            if (!File.Exists(guxtEXEPath))
            {
                var exes = Directory.GetFiles(exeDir, "*.exe");
                if (exes.Length == 1)
                    guxtEXEPath = exes[0];
                else
                {
                    MessageBox.Show("Multiple exes were found in your guxt folder! Please choose which is the right one:", this.Text);
                    using (var ofd = new OpenFileDialog()
                    {
                        Title = "Choose your exe...",
                        InitialDirectory = exeDir,
                        Filter = string.Join("|", GuxtEXEFilter, AllFilesFilter)
                    })
                    {
                        if (ofd.ShowDialog() == DialogResult.OK)
                            guxtEXEPath = ofd.FileName;
                        else
                            return;
                    }
                }
            }
            args += $" -e \"{guxtEXEPath}\"";

            Process.Start(Patcher.Default.ExePath, args);
        }
    }
}
