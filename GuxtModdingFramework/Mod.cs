using GuxtModdingFramework.Images;
using GuxtModdingFramework.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace GuxtModdingFramework
{
    public class ExtensionChangedEventArgs : EventArgs
    {
        public string Previous { get; set; }
        public string Current { get; set; }

        public ExtensionChangedEventArgs(string prev, string curr)
        {
            Previous = prev;
            Current = curr;
        }
    }

    public class StageCountChangedEventArgs : EventArgs
    {
        public int Previous { get; set; }
        public int Current { get; set; }

        public StageCountChangedEventArgs(int prev, int curr)
        {
            Previous = prev;
            Current = curr;
        }
    }

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
                }
            }
        }

        #endregion
        
        /// <summary>
        /// Size of each icon (pixels)
        /// </summary>
        [Category("Images"), DefaultValue(16), Description("The size of each editor icon.")]
        public int IconSize { get; set; } = 16;
        
        [Category("Entities"), Description()]
        public Dictionary<int, string> EntityNames { get; private set; } = new Dictionary<int, string>();

        [Category("Entities"), Description("The type of each entity. Only edit this if you've applied hacks to the exe")]
        public Dictionary<int, Type> EntityTypes { get; private set; } = new Dictionary<int, Type>();

        /// <summary>
        /// Size of each tile (pixels)
        /// </summary>
        [Category("Images"), DefaultValue(16), Description("The size of each tile.")]
        public int TileSize { get; set; } = 16;

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
        
        public event EventHandler<ExtensionChangedEventArgs> MapExtensionChanged =
            new EventHandler<ExtensionChangedEventArgs>((p, c) => { });

        [Category("File Extensions"), DefaultValue(DefaultMapExtension)]
        public string MapExtension
        {
            get => mapExtension;
            set
            {
                if (mapExtension != value)
                    MapExtensionChanged(this, new ExtensionChangedEventArgs(mapExtension, mapExtension = value));                
            }
        }

        public event EventHandler<ExtensionChangedEventArgs> EntityExtensionChanged =
            new EventHandler<ExtensionChangedEventArgs>((p, c) => { });

        [Category("File Extensions"), DefaultValue(DefaultEntityExtension)]
        public string EntityExtension
        {
            get => entityExtension;
            set
            {
                if (entityExtension != value)
                    EntityExtensionChanged(this, new ExtensionChangedEventArgs(entityExtension, entityExtension = value));                
            }
        }

        public event EventHandler<ExtensionChangedEventArgs> ImageExtensionChanged =
            new EventHandler<ExtensionChangedEventArgs>((p, c) => { });

        [Category("File Extensions"), DefaultValue(DefaultImageExtension)]
        public string ImageExtension
        {
            get => imageExtension;
            set
            {
                if(imageExtension != value)
                    ImageExtensionChanged(this, new ExtensionChangedEventArgs(imageExtension, imageExtension = value));
            }
        }

        public event EventHandler<ExtensionChangedEventArgs> AttributeExtensionChanged =
            new EventHandler<ExtensionChangedEventArgs>((p, c) => { });

        [Category("File Extensions"), DefaultValue(DefaultAttributeExtension)]
        public string AttributeExtension
        {
            get => attributeExtension;
            set
            {
                if (attributeExtension != value)
                    AttributeExtensionChanged(this, new ExtensionChangedEventArgs(attributeExtension, attributeExtension = value));
            }
        }

        public event EventHandler<ExtensionChangedEventArgs> ProjectExtensionChanged =
            new EventHandler<ExtensionChangedEventArgs>((p, c) => { });

        [Category("File Extensions"), DefaultValue(DefaultProjectExtension)]
        public string ProjectExtension
        {
            get => projectExtension;
            set
            {
                if(projectExtension != value)
                    ProjectExtensionChanged(this, new ExtensionChangedEventArgs(projectExtension, projectExtension = value));
            }
        }

        #endregion

        [Category("Images"), DefaultValue(true), Description("Whether or not the images are scrambeled or not. Only turn this off if you've patched your exe and converted the images.")]
        public bool ImagesScrambeled { get; set; } = true;

        [Category("General"), ReadOnly(true), Description("Path to the data folder of your mod. This should only be changed in the event something gets desynced.")]
        public string DataPath { get; set; }

        #region Stage stuff

        public event EventHandler<StageCountChangedEventArgs> StageCountChanged =
            new EventHandler<StageCountChangedEventArgs>((o, e) => { });

        private int stageCount = 6;
        [Category("General"), DefaultValue(6)]
        public int StageCount
        {
            get => stageCount;
            set
            {
                if (stageCount != value)
                    StageCountChanged(this, new StageCountChangedEventArgs(stageCount, stageCount = value));
            }
        }

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

            foreach (var val in EntityList.EntityTypes)
                m.EntityTypes.Add(val.Key, val.Value);
            
            foreach (var val in EntityList.EntityNames)
                m.EntityNames.Add(val.Key, val.Value);

            return m;
        }

        public void Save(string path)
        {
            var relativeDataPath = new Uri(path).MakeRelativeUri(new Uri(DataPath));

            var entityNames = new XElement("EntityNames",
                EntityNames.Select(x => new XElement("Name", new XAttribute("key", x.Key.ToString()), x.Value)));

            var entityTypes = new XElement("EntityTypes");
            var baseAssembly = Assembly.GetAssembly(typeof(Mod));
            foreach (var ent in EntityTypes)
            {
                var item = new XElement("Type", new XAttribute("key", ent.Key.ToString()), ent.Value.FullName);
                //If this is a custom type (IE, not a part of GuxtModdingFramework), store the dll it's a part of
                Assembly a = Assembly.GetAssembly(ent.Value);
                if (a != baseAssembly)
                    item.Add(new XAttribute("dll", a.GetName()));

                entityTypes.Add(item);                
            }
            
            new XDocument(
                new XElement("GuxtMod",
                    new XElement("DataPath", relativeDataPath),
                    new XElement("Stages", StageCount),
                    new XElement("Images",
                        new XElement("Scrambeled", ImagesScrambeled),
                        new XElement("TileSize", TileSize),
                        new XElement("IconSize", IconSize)                    
                    ),
                    new XElement("FileNames",
                        new XElement("Map", MapName),
                        new XElement("Entity", EntityName),
                        new XElement("Images", ImageName),
                        new XElement("Attributes", AttributeName),
                        new XElement("IconName", EditorIconName)
                    ),
                    new XElement("FileExtensions",
                        new XElement("Map", MapExtension),
                        new XElement("Entity", EntityExtension),
                        new XElement("Images", ImageExtension),
                        new XElement("Attributes", AttributeExtension),
                        new XElement("Projects", ProjectExtension)
                   ),
                   entityNames,
                   entityTypes
                )
            ).Save(path);
        }

        public static Mod Load(string path)
        {
            //Basic init/check that this is actually a guxt project
            var doc = new XmlDocument();
            doc.Load(path);
            var root = doc["GuxtMod"];
            if (root == null)
                throw new FileLoadException("The given file wasn't a Guxt project. Make sure it has the root XML tag as \"GuxtMod\".", path);

            //Datapath check
            string? dataFolder = root["DataPath"]?.InnerText;
            if(string.IsNullOrWhiteSpace(dataFolder))
                throw new ArgumentNullException("DataPath","The selected Guxt Mod doesn't have a data folder.");
            dataFolder = Path.Combine(Path.GetDirectoryName(path), dataFolder);
            if (!Directory.Exists(dataFolder))
                //TODO don't make me xml edit
                throw new DirectoryNotFoundException($"The directory \"{dataFolder}\" was not found. Please fix this project file using an xml editor.");
            
            Mod m = new Mod(dataFolder);
            
            string? sc = root["Stages"]?.InnerText;
            if(sc != null)
                m.StageCount = int.Parse(sc);

            var images = root["Images"];
            m.ImagesScrambeled = images["Scrambeled"]?.InnerText == "true";
            var tilesize = images["TileSize"]?.InnerText;
            if (tilesize != null)
                m.TileSize = int.Parse(tilesize);
            var iconsize = images["IconSize"]?.InnerText;
            if (iconsize != null)
                m.IconSize = int.Parse(iconsize);

            #region Filenames

            var names = root["FileNames"];
            m.mapName = names["Map"]?.InnerText ?? m.MapName;
            m.entityName = names["Entity"]?.InnerText ?? m.EntityName;
            m.imageName = names["Image"]?.InnerText ?? m.ImageName;
            m.attributeName = names["Attribute"]?.InnerText ?? m.AttributeName;
            m.editorIconName = names["IconName"]?.InnerText ?? m.editorIconName;

            #endregion

            #region File extensions

            var extensions = root["FileExtensions"];
            m.mapExtension = extensions["Map"]?.InnerText ?? m.MapExtension;
            m.entityExtension = extensions["Entity"]?.InnerText ?? m.EntityExtension;
            m.imageExtension = extensions["Image"]?.InnerText ?? m.ImageExtension;
            m.attributeExtension = extensions["Attribute"]?.InnerText ?? m.AttributeExtension;
            m.projectExtension = extensions["Projects"]?.InnerText ?? m.ProjectExtension;

            #endregion

            #region Entity names

            var entityNames = root["EntityNames"];
            foreach(XmlElement element in entityNames)
            {
                int key = int.Parse(element.Attributes["key"].Value);
                string value = element.InnerText;
                m.EntityNames.Add(key, value);
            }

            #endregion

            #region Entity Types

            Dictionary<string, Assembly>? externalTypes = null;
            string? dllSearchDir = null;

            var entityTypes = root["EntityTypes"];
            foreach (XmlElement element in entityTypes)
            {
                int key = int.Parse(element.Attributes["key"].Value);
                Type value;
                string? dll = element.Attributes["dll"]?.Value;
                if (!string.IsNullOrWhiteSpace(dll))
                {
                    externalTypes ??= new Dictionary<string, Assembly>();
                    dllSearchDir ??= Path.GetDirectoryName(path);

                    if (!externalTypes.ContainsKey(dll!))
                    {
                        string dllFullPath = Path.Combine(dllSearchDir, dll!);
                        if (!File.Exists(dllFullPath))
                            throw new FileNotFoundException($"The required library {dll!} could not be found. Make sure it exists in the same directory as the project file.", dllFullPath);
                        externalTypes.Add(dll!, Assembly.LoadFile(dllFullPath));
                    }
                    value = externalTypes[dll!].GetType(element.InnerText);
                }
                else
                    value = Type.GetType(element.InnerText);

                if (!typeof(EntityShell).IsAssignableFrom(value))
                    throw new ArgumentException("All types must inherit from the EntityShell class.", element.InnerText);

                m.EntityTypes.Add(key, value);
            }

            #endregion

            return m;
        }
    }
}
