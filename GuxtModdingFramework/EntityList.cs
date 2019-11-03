using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuxtModdingFramework.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace GuxtModdingFramework.Entities
{
    public class EntityList
    {
        /// <summary>
        /// Shell over an entity to allow for adding entity specific interfaces
        /// </summary>
        public abstract class EntityShell
        {
            Entity host;

            [Description("Unused variable")]
            public int Unused { get => host.Unused; set => host.Unused = value; }

            [Description("Horizontal position of the entity")]
            public int X { get => host.X; set => host.X = value; }

            [Description("Vertical position of the entity")]
            public int Y { get => host.Y; set => host.Y = value; }

            [Description("What entity this is")]
            public int EntityID { get => host.EntityID; set => host.EntityID = value; }

            [Description("The entity's properties. Contents/interpretation depends on the entity")]
            public int ExtraInfo { get => host.ExtraInfo; set => host.ExtraInfo = value; }

            public EntityShell(Entity e)
            {
                host = e;
            }
        }

        public readonly static ReadOnlyDictionary<int, Func<Entity, EntityShell>> ClassDictionary =
        new ReadOnlyDictionary<int, Func<Entity, EntityShell>>(new Dictionary<int, Func<Entity, EntityShell>>()
        {
            {11, (Entity e) => new Wing(e) },
            {19, (Entity e) => new BGM(e) },
        });

        //11
        public class Wing : EntityShell
        {
            [Description("If set, falls twice as fast")]
            public bool FallFaster
            {
                get => base.ExtraInfo != 0;
                set => base.ExtraInfo = value ? 1 : 0;
            }
            public Wing(Entity e) : base(e) { }
        }

        //19
        public class BGM : EntityShell
        {
            class MusicTypeConverter : TypeConverter
            {
                static List<string> musicList = new List<string>()
            {
                "(Nothing/Fade Out)",
                "opening",
                "boss",
                "gameover",
                "stage1",
                "stage2",
                "stage3",
                "stage4",
                "stage5",
                "boss2",
                "ending"
            };
                static StandardValuesCollection standardValues = new StandardValuesCollection(musicList);

                public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
                public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
                {
                    return standardValues;
                }

                public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
                {
                    return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
                }

                public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
                {
                    if (value is string s)
                        return musicList.IndexOf(s);
                    else
                        return base.ConvertFrom(context, culture, value);
                }

                public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
                {
                    return destinationType == typeof(int) || base.CanConvertTo(context, destinationType);
                }

                public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
                {
                    if (value is int i)
                        return (0 <= i && i < musicList.Count) ? musicList[i] : "(Invalid)";
                    else
                        return base.ConvertTo(context, culture, value, destinationType);
                }
            }

            [TypeConverter(typeof(MusicTypeConverter)), Description("What song to play")]
            public int MusicID
            {
                get => base.ExtraInfo;
                set => base.ExtraInfo = value;
            }

            public BGM(Entity e) : base(e) { }
        }
    }
}
