using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace GuxtModdingFramework.Entities
{
    #region Common

    public class MultiEntityShell
    {
        readonly Entity[] hosts;
        enum Properties
        {
            Unused,
            X,
            Y,
            EntityID,
            ExtraInfo
        }
        private int GetProperty(Entity e, Properties p)
        {
            return p switch
            {
                Properties.Unused => e.Unused,
                Properties.X => e.X,
                Properties.Y => e.Y,
                Properties.EntityID => e.EntityID,
                Properties.ExtraInfo => e.ExtraInfo,
                _ => throw new ArgumentException("Invalid property", nameof(p))
            };
        }
        private int? GetProperty(Properties p)
        {
            int val = GetProperty(hosts[0], p);
            for(int i = 1; i < hosts.Length; i++)
                if (val != GetProperty(hosts[i], p))
                    return null;
            return val;
        }

        private void SetProperty(int? value, Properties p)
        {
            if (value == null)
                return;
            foreach(var entity in hosts)
            {
                switch(p)
                {
                    case Properties.Unused:
                        entity.Unused = (int)value;
                        break;
                    case Properties.X:
                        entity.X = (int)value;
                        break;
                    case Properties.Y:
                        entity.Y = (int)value;
                        break;
                    case Properties.EntityID:
                        entity.EntityID = (int)value;
                        break;
                    case Properties.ExtraInfo:
                        entity.ExtraInfo = (int)value;
                        break;
                    default:
                        throw new ArgumentException("Invalid property", nameof(p));
                }
            }
        }

        [Description(Entity.UnusedDescription)]
        public int? Unused { get => GetProperty(Properties.Unused); set => SetProperty(value, Properties.Unused); }
        [Description(Entity.XDescription)]
        public int? X { get => GetProperty(Properties.X); set => SetProperty(value, Properties.X); }
        [Description(Entity.YDescription)]
        public int? Y { get => GetProperty(Properties.Y); set => SetProperty(value, Properties.Y); }
        [Description(Entity.EntityIDDescription)]
        public int? EntityID { get => GetProperty(Properties.EntityID); set => SetProperty(value, Properties.EntityID); }
        [Description(Entity.ExtraInfoDescription)]
        public int? ExtraInfo { get => GetProperty(Properties.ExtraInfo); set => SetProperty(value, Properties.ExtraInfo); }

        public MultiEntityShell(params Entity[] ents)
        {
            if (ents.Length < 1)
                throw new ArgumentException("You must supply at least one entity.", nameof(ents));
            hosts = ents;
        }
    }

    /// <summary>
    /// Shell over an entity to allow for adding entity specific interfaces
    /// </summary>
    public abstract class EntityShell
    {
        readonly Entity host;

        [Description(Entity.UnusedDescription)]
        public int Unused { get => host.Unused; set => host.Unused = value; }

        [Description(Entity.XDescription)]
        public int X { get => host.X; set => host.X = value; }

        [Description(Entity.YDescription)]
        public int Y { get => host.Y; set => host.Y = value; }

        [Description(Entity.EntityIDDescription)]
        public int EntityID { get => host.EntityID; set => host.EntityID = value; }

        [Description(Entity.ExtraInfoDescription)]
        public int ExtraInfo { get => host.ExtraInfo; set => host.ExtraInfo = value; }

        protected EntityShell(Entity e)
        {
            host = e;
        }
    }

    abstract class StringTypeConverter<T> : TypeConverter where T : struct
    {
        readonly List<string> text;
        readonly StandardValuesCollection svc;
        readonly string errorText;
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return svc;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                var t = typeof(T);
                if (t == typeof(int))
                    return text.IndexOf(s);
                else if (t == typeof(bool))
                    return text.IndexOf(s) == 0;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(int) || destinationType == typeof(bool) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value switch
            {
                int i => (0 <= i && i < text.Count) ? text[i] : errorText,
                bool b => text[b ? 1 : 0],
                _ => base.ConvertTo(context, culture, value, destinationType)
            };
        }

        public StringTypeConverter(string e, params string[] t)
        {
            errorText = e;
            text = new List<string>(t);

            //bool mode
            if (typeof(T) == typeof(bool))
            {
                //either provide just the two as seperate params
                if (!string.IsNullOrWhiteSpace(e) && t.Length == 1)
                    text.Add(errorText);
                //or leave e blank and provide two values to t
                else if (!(string.IsNullOrWhiteSpace(e) && t.Length == 2))
                    throw new ArgumentException("You must provide only 2 options for bool mode.", nameof(e) + ", " + nameof(t));
            }
            //if it's not bool, it better be int, otherwise...
            else if (typeof(T) != typeof(int))
                throw new NotSupportedException("Supplied type is not supported. Please use bool or int.");
                                  
            svc = new StandardValuesCollection(text);
        }
    }

    #endregion

    #region Entities

    public class Powerup : EntityShell
    {
        class PowerUpTypeConverter : StringTypeConverter<int>
        {
            PowerUpTypeConverter() : base("(Invalid)",
                "(Nothing)",
                "Speedup",
                "Shield",
                "Twin",
                "Machine",
                "Triangle",
                "Back",
                "Wide",
                "3Line (unused)",
                "Star",
                "Harpoon",
                "Anchor",
                "Rocket",
                "WMachine"
                )
            { }
        }

        [TypeConverter(typeof(PowerUpTypeConverter)), Description("What powerup you get")]
        public int PowerupID
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }

        public Powerup(Entity e) : base(e) { }
    }

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

    //13, 14
    public class AngleEntity : EntityShell
    {
        [Description("Angle, in 1/256th of a circle. 0 = Right, 64 = Down, 128 = Left, 192 = Up, etc.")]
        public int Angle
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }

        public AngleEntity(Entity e) : base(e) { }
    }

    //18
    public class BulletLong : EntityShell
    {
        class SpawnDirectionTypeConverter : StringTypeConverter<int>
        {
            public SpawnDirectionTypeConverter() : base("Right with glitchy graphics",
                "Straight",
                "Left",
                "Right")
            { }
        }
        [TypeConverter(typeof(SpawnDirectionTypeConverter)), Description("What direction to spawn in")]
        public int SpawnDirection
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }
        public BulletLong(Entity e) : base(e) { }
    }

    //19
    public class BGM : EntityShell
    {
        class MusicTypeConverter : StringTypeConverter<int>
        {
            MusicTypeConverter() : base("(Invalid)",
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
                    "ending")
            { }
        }

        [TypeConverter(typeof(MusicTypeConverter)), Description("What song to play")]
        public int MusicID
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }

        public BGM(Entity e) : base(e) { }
    }

    //22
    public class ScrollSpeed : EntityShell
    {
        [Description("How fast the screen scrolls in pixels per frame(?)")]
        public int ScreenScrollingSpeed
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }

        public ScrollSpeed(Entity e) : base(e) { }
    }

    public class AsteroidSGravity : EntityShell
    {
        class SpawnDirectionTypeConverter : StringTypeConverter<int>
        {
            public SpawnDirectionTypeConverter() : base("Straight down",
                "Slightly Left",
                "Slightly Right",
                "More Left",
                "More Right")
            { }
        }
        [TypeConverter(typeof(SpawnDirectionTypeConverter)), Description("What direction to spawn in")]
        public int SpawnDirection
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }
        public AsteroidSGravity(Entity e) : base(e) { }
    }

    //25
    public class HangarShoot : EntityShell
    {
        [Description("When set, takes only 1 hit and gives 0 score (instead of 2 hits and 10 points)")]
        public bool LowHealth
        {
            get => base.ExtraInfo != 0;
            set => base.ExtraInfo = value ? 1 : 0;
        }

        public HangarShoot(Entity e) : base(e) { }
    }

    //26
    public class RockHugger : EntityShell
    {
        class RockTypeTypeConverter : StringTypeConverter<int>
        {
            public RockTypeTypeConverter() : base("Invisible",
                "Big",
                "Small",
                "No Rock")
            { }
        }
        [TypeConverter(typeof(RockTypeTypeConverter)), Description("What rock this hugger will be holding")]
        public int RockType
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }
        public RockHugger(Entity e) : base(e) { }
    }

    //29
    public class Entity29 : EntityShell
    {
        [Description("Whether or not to spawn closer to the top of the screen")]
        public bool SpawnHigher
        {
            get => base.ExtraInfo != 0;
            set => base.ExtraInfo = value ? 1 : 0;
        }

        public Entity29(Entity e) : base(e) { }
    }

    //35
    public class B2Rocket : EntityShell
    {
        class RocketBehaviorTypeConverter : StringTypeConverter<int>
        {
            RocketBehaviorTypeConverter() : base("(Invalid/Frozen In Place)",
                "Down",
                "45° Down Left, Down",
                "45° Down Right, Down",
                "Down, Bounce Up",
                "Down, Bounce 45° Up Left, Up",
                "Down, Bounce 45° Up Right, Up")
            { }
        }

        [TypeConverter(typeof(RocketBehaviorTypeConverter)), Description("How the rocket will behave on appearence")]
        public int RocketBehavior
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }

        public B2Rocket(Entity e) : base(e) { }
    }
    
    //37
    public class Stars : EntityShell
    {
        class StarTypeConverter : StringTypeConverter<int>
        {
            public StarTypeConverter() : base("(Invalid/Invisible)",
                "One Big",
                "Two Small",
                "One Small")
            { }
        }
        [TypeConverter(typeof(StarTypeConverter)), Description("What rock this hugger will be holding")]
        public int Star
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }
        public Stars(Entity e) : base(e) { }
    }

    //41
    public class CatEye : EntityShell
    {
        [Description("How many frames to wait before activating. ExtraInfo * 50")]
        public string Delay
        {
            get => base.ExtraInfo > 0 ? (base.ExtraInfo * 50).ToString() : "Disabled";
        }
        public CatEye(Entity e) : base(e) { }
    }

    //43
    public class Cycloid : EntityShell
    {
        class SpawnLocationTypeConverter : StringTypeConverter<bool>
        {
            public SpawnLocationTypeConverter() : base("Bottom", "Top") { }
        }

        [TypeConverter(typeof(SpawnLocationTypeConverter)), Description("What or not the Sand Stamper will stamp faster")]
        public bool SpawnLocation
        {
            get => base.ExtraInfo != 0;
            set => base.ExtraInfo = value ? 1 : 0;
        }

        public Cycloid(Entity e) : base(e) { }
    }

    //45, 121
    public class Bonus : EntityShell
    {
        [Description("What stage's bonus graphics to use")]
        public int BonusType
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }

        public Bonus(Entity e) : base(e) { }
    }

    //46, 106
    public class BonusDisable : EntityShell
    {
        [Description("Whether or not this bonus will spawn")]
        public bool Disabled
        {
            get => base.ExtraInfo != 0;
            set => base.ExtraInfo = value ? 1 : 0;
        }

        public BonusDisable(Entity e) : base(e) { }
    }

    //48
    public class Gimmick : EntityShell
    {
        class DirectionTypeConverter : StringTypeConverter<int>
        {
            public DirectionTypeConverter() : base("(Invalid)",
                "Left",
                "Up",
                "Right",
                "Down")
            { }
        }

        [TypeConverter(typeof(DirectionTypeConverter)), Description("What direction to face.")]
        public int Direction
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }
        public Gimmick(Entity e) : base(e) { }
    }

    //67
    public class GuxtTank : EntityShell
    {
        [Description("Offset in the entity grid. Positive values bring them closer to the bottom, negative values closer to the top")]
        public int Offset
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }

        public GuxtTank(Entity e) : base(e) { }
    }

    //68,99
    public class DelayActivate : EntityShell
    {
        [Description("How many frames to wait before activating. ExtraInfo * 25")]
        public string Delay
        {
            get => base.ExtraInfo > 0 ? (base.ExtraInfo * 25).ToString() : "Disabled";
        }
        public DelayActivate(Entity e) : base(e) { }
    }

    //70
    public class Blendy : EntityShell
    {
        [Description("What position for the arm to start in, in 1/8ths")]
        public byte Rotation
        {
            get => (byte)(base.ExtraInfo & 7);
            set
            {
                if (value >= 8)
                    return;
                base.ExtraInfo = (base.ExtraInfo & ~7) | value;
            }
        }

        public enum SpinDirections
        {
            Clockwise = 0,
            Counterclockwise = 8,
        }

        [Description("What direction to spin in")]
        public SpinDirections SpinDirection
        {
            get => (SpinDirections)(base.ExtraInfo & (int)SpinDirections.Counterclockwise);
            set
            {
                base.ExtraInfo = value switch
                {
                    SpinDirections.Clockwise => base.ExtraInfo &= ~(int)SpinDirections.Counterclockwise,
                    SpinDirections.Counterclockwise => base.ExtraInfo |= (int)SpinDirections.Counterclockwise,
                    _ => base.ExtraInfo
                };
            }
        }

        [Description("Makes the enemy go faster")]
        public bool MoveFaster
        {
            get => (base.ExtraInfo & 256) != 0;
            set
            {
                if (value)
                    base.ExtraInfo |= 256;
                else
                    base.ExtraInfo &= ~256;
            }
        }

        public Blendy(Entity e) : base(e) { }
    }

    //84
    public class Square : EntityShell
    {
        [Description("Spawn offset from the top of the screen in tiles")]
        public int Offset
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }
        public Square(Entity e) : base(e) { }
    }

    //88
    public class MissilePot : EntityShell
    {
        class OffsetTypeConverter : StringTypeConverter<bool>
        {
            public OffsetTypeConverter() : base("Low", "High") { }
        }

        [TypeConverter(typeof(OffsetTypeConverter)), Description("Spawn either a bit higher, or a bit lower")]
        public bool Offset
        {
            get => base.ExtraInfo != 0;
            set => base.ExtraInfo = value ? 1 : 0;
        }
        public MissilePot(Entity e) : base(e) { }
    }

    //90
    public class Missile : EntityShell
    {
        class PatternTypeConverter : StringTypeConverter<int>
        {
            public PatternTypeConverter() : base("Hard Left", "Hard Left",
                "Hard Right",
                "Medium Left",
                "Medium Right",
                "Small Left",
                "Small Right")
            { }
        }
        [TypeConverter(typeof(PatternTypeConverter)),Description("What flight pattern to use. The difference between these is VERY slight")]
        public int Pattern
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }

        public Missile(Entity e) : base(e) { }
    }

    //91
    public class Rock : EntityShell
    {
        [Description("What framerects to use. The values used in game are 1 and 2, which line up with tiles 44/45 and 43 respectively")]
        public int RectIndex
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }
        public Rock(Entity e) : base(e) { }
    }

    //94
    public class SandStamper : EntityShell
    {
        [Description("Whether or not the Sand Stamper will stamp faster")]
        public bool StampFaster
        {
            get => base.ExtraInfo != 0;
            set => base.ExtraInfo = value ? 1 : 0;
        }

        public SandStamper(Entity e) : base(e) { }
    }

    //107
    public class ToggleShadow : EntityShell
    {
        [Description("Whether or not to show a shadow underneath the player's ship")]
        public bool ShowShadow
        {
            get => base.ExtraInfo != 0;
            set => base.ExtraInfo = value ? 1 : 0;
        }

        public ToggleShadow(Entity e) : base(e) { }
    }

    //117
    public class CreditDelete : EntityShell
    {
        [Description("What entity ID to delete")]
        public int SelectedID
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }
        public CreditDelete(Entity e) : base(e) { }
    }

    //118
    public class CreditLoadImg : EntityShell
    {
        [Description("What spritesheet to load")]
        public int SpriteSheetNumber
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }
        public CreditLoadImg(Entity e) : base(e) { }
    }

    //119
    public class CreditPhoto : EntityShell
    {
        [Description("What photo to display")]
        public int PhotoNumber
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }
        public enum Direction
        {
            Right = 0,
            Left = 1
        }
        [Description("What direction the photo will slide in")]
        public Direction PhotoDirection
        {
            //Every second photo goes left
            get => (Direction)(base.ExtraInfo & 1);
        }

        public CreditPhoto(Entity e) : base(e) { }
    }

    #endregion

    public static class EntityList
    {
        public readonly static ReadOnlyDictionary<int, Type> EntityTypes =
        new ReadOnlyDictionary<int, Type>(new Dictionary<int, Type>()
        {
            {007, typeof(Powerup) },
            {011, typeof(Wing) },
            {013, typeof(AngleEntity) },
            {014, typeof(AngleEntity) },
            {018, typeof(BulletLong) },
            {019, typeof(BGM) },
            {022, typeof(ScrollSpeed) },
            {023, typeof(AsteroidSGravity) },
            {025, typeof(HangarShoot) },
            {026, typeof(RockHugger) },
            {029, typeof(Entity29) },
            {035, typeof(B2Rocket) },
            {037, typeof(Stars) },
            {041, typeof(CatEye) },
            {043, typeof(Cycloid) },
            {045, typeof(Bonus) },
            {046, typeof(BonusDisable) },
            {048, typeof(Gimmick) },
            {059, typeof(Powerup) },
            {060, typeof(Powerup) },
            {061, typeof(Powerup) },
            {063, typeof(AngleEntity) },
            {064, typeof(Powerup) },
            {067, typeof(GuxtTank) },
            {068, typeof(DelayActivate) },
            {070, typeof(Blendy) },
            {074, typeof(Powerup) },
            {075, typeof(Powerup) },
            {084, typeof(Square) },
            {088, typeof(MissilePot) },
            {090, typeof(Missile) },
            {091, typeof(Rock) },
            {094, typeof(SandStamper) },
            {098, typeof(AngleEntity) },
            {099, typeof(DelayActivate) },
            {102, typeof(AngleEntity) },
            {104, typeof(AngleEntity) },
            {106, typeof(BonusDisable) },
            {107, typeof(ToggleShadow) },
            {117, typeof(CreditDelete) },
            {118, typeof(CreditLoadImg) },
            {119, typeof(CreditPhoto) },
            {121, typeof(Bonus) },
        });

        public readonly static ReadOnlyDictionary<int, string> EntityNames =
        new ReadOnlyDictionary<int, string>(new Dictionary<int, string>()
        {
            {001, "Explode"},
            {002, "CloudL"},
            {003, "CloudS"},
            {004, "Puff"},
            {005, "Hanger"},
            {006, "CloudGen"},
            {007, "Powerup"},
            {008, "Climber"},
            {009, "Kagome"},
            {011, "Wing"},
            {012, "WingsDead"},
            {013, "Bullet"},
            {014, "BulletSlow"},
            {015, "Boss1"},
            {018, "BulletLong"},
            {019, "BGM"},
            {020, "Asteroid L"},
            {021, "Asteroid S"},
            {022, "ScrollSpdSet"},
            {023, "Asteroid SGravity"},
            {024, "Hanger Wave"},
            {025, "Hanger Shoot"},
            {026, "RockHugger"},
            {027, "RHAsteroidL"},
            {028, "RHAsteroidS"},
            //{029, "Something's Jet Trail???"},
            {030, "Elka"},
            {031, "Sodi"},
            {032, "Boss2"},
            {035, "B2Rocket"},
            {037, "Stars?"},
            {038, "StarGen"},
            {039, "Elka2"},
            {040, "GuxtFort"},
            {041, "CatEye"},
            {042, "Slider"},
            {043, "Cycloid"},
            {044, "Chester"},
            {045, "Bonus"},
            {046, "BonusGen"},
            {047, "CloudXL"},
            {048, "Gimmick"},
            {049, "Boss3"},
            {050, "62"},
            {054, "BulletBlocker?"},
            {056, "Checkpoint"},
            {058, "B3Turret"},
            {059, "PowerupSpinnerShield"},
            {060, "PowerupBox"},
            {061, "PowerupStatic"},
            {063, "BulletWhite"},
            {064, "PowerupSpinner"},
            {065, "ClimberShoot"},
            {066, "Tri"},
            {067, "GuxtTank"},
            {068, "MedusaEye"},
            {069, "GuxtMine"},
            {070, "Blendy"},
            {072, "Cycloid2"},
            {073, "Cycloid2Gen"},
            {074, "PowerupHidden"},
            {075, "PowerupAsteroid"},
            {076, "Boss4"},
            {081, "SpaceAmeba"},
            {083, "B4Bullet"},
            {084, "Square"},
            {085, "CloudDarkL"},
            {086, "CloudDarkS"},
            {087, "CloudDarkGen"},
            {088, "MissilePot"},
            {090, "Missile"},
            {091, "Rock" },
            {092, "BGRock"},
            {093, "ClearRockCloudGen"},
            {094, "SandStamper"},
            {096, "Stamp"},
            {097, "Brick"},
            {098, "BulletSquare"},
            {099, "BulletSquareGen"},
            {100, "Boss5"},
            {102, "B5Bullet"},
            {104, "B5Laser"},
            {105, "B5LaserTrail"},
            {106, "AzaBonusSpawner"},
            {107, "ToggleShadow" },
            {108, "Boss6"},
            {111, "B6Laser"},
            {114, "B6Ball"},
            {115, "B6Caret"},
            {116, "CreditText"},
            {117, "CreditReplace"},
            {118, "CreditLoadImg"},
            {119, "CreditPhoto"},
            {120, "CreditGimmick"},
            {121, "BonusHidden"},
        });
    }
}
