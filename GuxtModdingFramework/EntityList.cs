using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace GuxtModdingFramework.Entities
{
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

    abstract class StringTypeConverter : TypeConverter
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
                return text.IndexOf(s);
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
                return (0 <= i && i < text.Count) ? text[i] : errorText;
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public StringTypeConverter(string e, params string[] t)
        {
            errorText = e;
            text = new List<string>(t);
            svc = new StandardValuesCollection(text);
        }
    }

    public class Powerup : EntityShell
    {
        class PowerUpTypeConverter : StringTypeConverter
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

    //11, 12
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
    public class Bullet : EntityShell
    {
        [Description("Angle, in 1/256th of a circle. 0 = Right, 64 = Down, 128 = Left, 192 = Up, etc.")]
        public int Angle
        {
            get => base.ExtraInfo;
            set => base.ExtraInfo = value;
        }

        public Bullet(Entity e) : base(e) { }
    }

    //19
    public class BGM : EntityShell
    {
        class MusicTypeConverter : StringTypeConverter
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

    //35
    public class B2Rocket : EntityShell
    {
        class RocketBehaviorTypeConverter : StringTypeConverter
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

    //45
    public class Bonus : EntityShell
    {
        public Bonus(Entity e) : base(e)
        {

        }
    }

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


    public static class EntityList
    {
        public readonly static ReadOnlyDictionary<int, Type> EntityTypes =
        new ReadOnlyDictionary<int, Type>(new Dictionary<int, Type>()
        {
            {007, typeof(Powerup) },
            {011, typeof(Wing) },
            {012, typeof(Wing) },
            {013, typeof(Bullet) },
            {014, typeof(Bullet) },
            {019, typeof(BGM) },
            {022, typeof(ScrollSpeed) },
            {035, typeof(B2Rocket) },
            {059, typeof(Powerup) },
            {060, typeof(Powerup) },
            {061, typeof(Powerup) },
            {064, typeof(Powerup) },
            {067, typeof(GuxtTank) },
            {070, typeof(Blendy) },
            {074, typeof(Powerup) },
            {075, typeof(Powerup) },
            {084, typeof(Square) },
            {094, typeof(SandStamper) },
            {107, typeof(ToggleShadow) },
            {117, typeof(CreditDelete) },
            {118, typeof(CreditLoadImg) },
            {119, typeof(CreditPhoto) },
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
