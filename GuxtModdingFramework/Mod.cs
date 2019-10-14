using GuxtModdingFramework.Images;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace GuxtModdingFramework
{
    public class Mod
    {
        #region File Name stuff
        public const string DefaultMapName = "map";
        public const string DefaultEntityName = "event";
        public const string DefaultImageName = "enemy";
        public const string DefaultAttributeName = "parts";
        public const string DefaultEditorIconName = "units";
        
        private string mapName = DefaultMapName;
        private string entityName = DefaultEntityName;
        private string imageName = DefaultImageName;
        private string attributeName = DefaultAttributeName;
        private string editorIconName = DefaultEditorIconName;

        [Category("File Names"), DefaultValue(DefaultMapName)]
        public string MapName
        {
            get => mapName;
            set
            {
                mapName = value;
                //FillStagesList();
            }
        }
        [Category("File Names"), DefaultValue(DefaultEntityName)]
        public string EntityName
        {
            get => entityName;
            set
            {
                entityName = value;
                //FillStagesList();
            }
        }
        [Category("File Names"), DefaultValue(DefaultImageName)]
        public string ImageName
        {
            get => imageName;
            set
            {
                imageName = value;
                //FillWithFileNames(Images, DataPath, ImageExtension);
            }
        }
        [Category("File Names"), DefaultValue(DefaultAttributeName)]
        public string AttributeName
        {
            get => attributeName;
            set
            {
                attributeName = value;
                //FillStagesList();
            }
        }
        [Category("File Names"), DefaultValue(DefaultEditorIconName)]
        public string EditorIconName
        {
            get => editorIconName;
            set
            {
                if(File.Exists(value))
                {
                    editorIconName = value;
                    UpdateIconImage();
                }
            }
        }

        #endregion

        Bitmap iconImage;

        void UpdateIconImage()
        {
            iconImage = new Bitmap(Path.Combine(DataPath, EditorIconName + "." + ImageExtension));
            if(ImagesScrambeled)
                iconImage = Scrambler.Unscramble(iconImage);
        }

        #region File Extension stuff

        public const string DefaultMapExtension = "pxmap";
        public const string DefaultEntityExtension = "pxeve";
        public const string DefaultImageExtension = "pximg";
        public const string DefaultAttributeExtension = "pxatrb";
        public const string DefaultProjectExtension = "stgprj";

        private string mapExtension = DefaultMapExtension;
        private string entityExtension = DefaultEntityExtension;
        private string imageExtension = DefaultImageExtension;
        private string attributeExtension = DefaultAttributeExtension;
        private string projectExtension = DefaultProjectExtension;

        [Category("File Extensions"), DefaultValue(DefaultMapExtension)]
        public string MapExtension
        {
            get => mapExtension;
            set
            {
                mapExtension = value;
                //FillStagesList();
            }
        }

        [Category("File Extensions"), DefaultValue(DefaultEntityExtension)]
        public string EntityExtension
        {
            get => entityExtension;
            set
            {
                entityExtension = value;
                //FillStagesList();
            }
        }

        /// <summary>
        /// Fills up the given list with the file names from the given directory, that match the given extension, but don't match the given filter
        /// </summary>
        /// <param name="list">The list to fill</param>
        /// <param name="dir">Directory to search</param>
        /// <param name="ext">Extension the files must have</param>
        /// <param name="filter">Filter the files must NOT meet</param>
        private static void FillWithFileNames(IList<string> list, string dir, string ext, string? filter = null)
        {
            list.Clear();
            foreach (var f in Directory.EnumerateFiles(dir, "*." + ext))
                if (filter == null || !Regex.Match(f, $@"^{filter}\d+\.{ext}$").Success)
                    list.Add(Path.GetFileName(f));
        }

        [Category("File Extensions"), DefaultValue(DefaultImageExtension)]
        public string ImageExtension
        {
            get => imageExtension;
            set
            {
                imageExtension = value;
                FillWithFileNames(Images, DataPath, ImageExtension);
            }
        }
        
        [Category("File Extensions"), DefaultValue(DefaultAttributeExtension)]
        public string AttributeExtension
        {
            get => attributeExtension;
            set
            {
                attributeExtension = value;
                FillWithFileNames(Attributes, DataPath, AttributeExtension);
            }
        }
        
        [Category("File Extensions"), DefaultValue(DefaultProjectExtension)]
        public string ProjectExtension
        {
            get => projectExtension;
            set
            {
                projectExtension = value;
                FillWithFileNames(Projects, DataPath, ProjectExtension);
            }
        }

        #endregion

        [Category("General"), DefaultValue(true), Description("Whether or not the images are scrambeled or not. Only turn this off if you've patched your exe and converted the images.")]
        public bool ImagesScrambeled { get; set; } = true;

        [Category("General"), ReadOnly(true), Description("Path to the data folder of your mod. This should only be changed in the event something gets desynced.")]
        public string DataPath { get; set; }

        #region Stage stuff
        
        /// <summary>
        /// Fills the stage list with all the right numbers
        /// </summary>
        private void FillStagesList()
        {
            Stages.Clear();
            for (int i = 1; i <= stageCount; i++)
                Stages.Add($"Stage {i}");
        }

        private int stageCount = 6;
        [Category("General"), DefaultValue(6)]
        public int StageCount
        {
            get => stageCount;
            set
            {
                if (stageCount != value)
                {
                    stageCount = value;
                    FillStagesList();
                }
            }
        }

        /// <summary>
        /// Used for editing "stages" (collection of map + entites + backgrounds)
        /// </summary>
        [Browsable(false)]
        public BindingList<string> Stages { get; } = new BindingList<string>();

        #endregion

        #region Internal lists

        /// <summary>
        /// Images (spritesheets + backgrounds)
        /// </summary>
        [Browsable(false)]
        public BindingList<string> Images { get; } = new BindingList<string>();
        
        /// <summary>
        /// Tileset/backgroud tile attributes
        /// </summary>
        [Browsable(false)]
        public BindingList<string> Attributes { get; } = new BindingList<string>();
        
        /// <summary>
        /// Project files (kinda useless to edit...)
        /// </summary>
        [Browsable(false)]
        public BindingList<string> Projects { get; } = new BindingList<string>();

        #endregion

        private Mod(string path)
        {
            DataPath = path;
        }

        public static Mod FromDataFolder(string path)
        {
            if(!Directory.Exists(path))
                //TODO don't make me xml edit
                throw new DirectoryNotFoundException($"The directory \"{path}\" was not found. Please fix this project file using an xml editor.");
            
            var m = new Mod(path);
            m.FillStagesList();
            FillWithFileNames(m.Images, m.DataPath, m.ImageExtension, m.ImageName);
            FillWithFileNames(m.Attributes, m.DataPath, m.AttributeExtension);
            FillWithFileNames(m.Projects, m.DataPath, m.ProjectExtension);
            m.UpdateIconImage();
            
            return m;
        }

        public void Save(string path)
        {
            //TODO save icon name
            var relativeDataPath = new Uri(path).MakeRelativeUri(new Uri(DataPath));            
            new XDocument(
                new XElement("GuxtMod",
                    new XElement("DataPath", relativeDataPath),
                    new XElement("ImagesScrambeled", ImagesScrambeled),
                    new XElement("Stages", StageCount),
                    new XElement("FileNames",
                        new XElement("Map", MapName),
                        new XElement("Entity", EntityName),
                        new XElement("Images", ImageName),
                        new XElement("Attributes", AttributeName)
                    ),
                    new XElement("FileExtensions",
                        new XElement("Map", MapExtension),
                        new XElement("Entity", EntityExtension),
                        new XElement("Images", ImageExtension),
                        new XElement("Attributes", AttributeExtension),
                        new XElement("Projects", ProjectExtension)
                   )
                )
            ).Save(path);
        }

        public static Mod Load(string path)
        {
            var doc = new XmlDocument();
            doc.Load(path);
            var root = doc["GuxtMod"];
            if (root == null)
                throw new FileLoadException("The given file wasn't a Guxt project. Make sure it has the root XML tag as \"GuxtMod\".", path);

            string? dataFolder = root["DataPath"]?.InnerText;
            if(string.IsNullOrWhiteSpace(dataFolder))
                throw new ArgumentNullException("DataPath","The selected Guxt Mod doesn't have a data folder.");
            dataFolder = Path.Combine(Path.GetDirectoryName(path), dataFolder);
            if (!Directory.Exists(dataFolder))
                //TODO don't make me xml edit
                throw new DirectoryNotFoundException($"The directory \"{dataFolder}\" was not found. Please fix this project file using an xml editor.");
            
            //Already have a null check earlier
            #nullable disable
            Mod m = FromDataFolder(dataFolder);
            #nullable restore
            
            m.ImagesScrambeled = root["ImagesScrambeled"]?.InnerText == "true";
            
            string? sc = root["Stages"]?.InnerText;
            if(sc != null)
                m.StageCount = int.Parse(sc);

            var names = root["FileNames"];
            m.mapName = names["Map"]?.InnerText ?? m.MapName;
            m.entityName = names["Entity"]?.InnerText ?? m.EntityName;
            m.imageName = names["Image"]?.InnerText ?? m.ImageName;
            m.attributeName = names["Attribute"]?.InnerText ?? m.AttributeName;

            var extensions = root["FileExtensions"];
            m.mapExtension = extensions["Map"]?.InnerText ?? m.MapExtension;
            m.entityExtension = extensions["Entity"]?.InnerText ?? m.EntityExtension;
            m.imageExtension = extensions["Image"]?.InnerText ?? m.ImageExtension;
            m.attributeExtension = extensions["Attribute"]?.InnerText ?? m.AttributeExtension;
            m.projectExtension = extensions["Projects"]?.InnerText ?? m.ProjectExtension;
            
            return m;
        }
    }
}
