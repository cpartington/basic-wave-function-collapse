using System;
using System.Collections.Generic;
using Godot;

namespace hackathon
{
    internal class TileType
    {
        public Guid Id;
        public Vector2I AtlasCoord;
        public List<Guid> AllowedTypesLeft;
        public List<Guid> AllowedTypesRight;
        public List<Guid> AllowedTypesAbove;
        public List<Guid> AllowedTypesBelow;

        public TileType(int x, int y)
        {
            Id = Guid.NewGuid();
            AtlasCoord = new(x, y);
        }
    }
}
