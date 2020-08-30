using GuxtModdingFramework.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GuxtEditor
{
    interface IUndo { }

    class EntitiesMoved : IUndo
    {
        public class MovedEntity
        {
            public Point OldLocation { get; }

            public Point NewLocation { get; set; }

            public MovedEntity(Point old, Point @new)
            {
                OldLocation = old;
                NewLocation = @new;
            }
        }

        public Dictionary<Entity, MovedEntity> Entities { get; } = new Dictionary<Entity, MovedEntity>();
    }

    class EntityPropertiesChanged : IUndo
    {
        public class EntityPropertyChanged
        {
            public int OldValue { get; }
            public int NewValue { get; set; }
            public EntityPropertyChanged(int old, int @new)
            {
                OldValue = old;
                NewValue = @new;
            }
        }
        public string Property { get; }

        public Dictionary<Entity, EntityPropertyChanged> Entities { get; } = new Dictionary<Entity, EntityPropertyChanged>();

        public EntityPropertiesChanged(string prop)
        {
            Property = prop;
        }

    }

    class EntityListChanged : IUndo
    {
        public Entity[] OldEntities { get; }
        public Entity[] NewEntities { get; set; }

        public EntityListChanged(Entity[] old)
        {
            OldEntities = old;
            NewEntities = Array.Empty<Entity>();
        }
    }

    class TilesPlaced : IUndo
    {
        public class TileChanged
        {
            public byte OldValue { get; }
            public byte NewValue { get; set; }

            public TileChanged(byte prev, byte @new)
            {
                OldValue = prev;
                NewValue = @new;
            }
        }

        public Dictionary<int, TileChanged> Tiles { get; } = new Dictionary<int, TileChanged>();
    }

    class MapResized : IUndo
    {
        public short OldWidth { get; }
        public short OldHeight { get; }
        public byte[] OldTiles { get; }

        public short NewWidth { get; set; }
        public short NewHeight { get; set; }
        public byte[] NewTiles { get; set; }

        public MapResized(short oldWidth, short oldHeight, byte[] old)
        {
            OldWidth = oldWidth;
            OldHeight = oldHeight;
            OldTiles = old;

            NewTiles = Array.Empty<byte>();
        }
    }
}
