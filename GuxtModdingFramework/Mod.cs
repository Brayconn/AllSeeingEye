using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        
        private string mapName = DefaultMapName;
        private string entityName = DefaultEntityName;
        private string imageName = DefaultImageName;
        private string attributeName = DefaultAttributeName;

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

        #endregion

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

        [Category("General"), Description("Whether or not the images are scrambeled or not. Only turn this off if you've patched your exe and converted the images.")]
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
        public int StageCount
        {
            get => stageCount;
            set
            {
                if (stageCount != value)
                {
                    stageCount = value;

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
                throw new DirectoryNotFoundException($"The directory \"{path}\" was not found. Please fix it using an xml editor.");
            
            var m = new Mod(path);
            FillWithFileNames(m.Stages, m.DataPath, m.mapExtension);
            FillWithFileNames(m.Images, m.DataPath, m.ImageExtension, m.ImageName);
            FillWithFileNames(m.Attributes, m.DataPath, m.AttributeExtension);
            FillWithFileNames(m.Projects, m.DataPath, m.ProjectExtension);
            return m;
        }

        public void Save(string path)
        {
            new XDocument(
                new XElement("GuxtMod",
                    new XElement("ImagesScrambeled", ImagesScrambeled),
                    new XElement("DataPath", DataPath),
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

            string? dataFolder = doc.SelectSingleNode("GuxtMod/DataPath")?.InnerText;
            if(string.IsNullOrWhiteSpace(dataFolder))
                throw new ArgumentNullException("DataPath","The selected Guxt Mod doesn't have a data folder.");
            if (!Directory.Exists(dataFolder))
                //TODO don't make me xml edit
                throw new DirectoryNotFoundException($"The directory \"{dataFolder}\" was not found. Please fix it using an xml editor.");
            
            //Already have a null check earlier
            #nullable disable
            Mod m = FromDataFolder(dataFolder);
            #nullable restore
            
            m.ImagesScrambeled = doc.SelectSingleNode("GuxtMod/ImagesScrambeled")?.InnerText == "true";

            var extensions = doc.SelectSingleNode("GuxtMod/FileExtensions");
            m.mapExtension = extensions.SelectSingleNode("Map")?.InnerText ?? m.MapExtension;
            m.entityExtension = extensions.SelectSingleNode("Entity")?.InnerText ?? m.EntityExtension;
            m.imageExtension = extensions.SelectSingleNode("Image")?.InnerText ?? m.ImageExtension;
            m.attributeExtension = extensions.SelectSingleNode("Attribute")?.InnerText ?? m.AttributeExtension;
            m.projectExtension = extensions.SelectSingleNode("Projects")?.InnerText ?? m.ProjectExtension;
            
            return m;
        }
    }
}
