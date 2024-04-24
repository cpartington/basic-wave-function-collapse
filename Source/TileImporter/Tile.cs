using System.Collections.Generic;
using Godot;

namespace hackathon.TileImporter
{
    public enum EdgeType
    {
        Grass,
        River,
        Unknown
    }

    public class Tile
    {
        public Vector2I AtlasCoord { get; set; }
        public int Likelihood { get; set; }

        public EdgeType LeftEdgeType { get; set; }
        public EdgeType RightEdgeType { get; set; }
        public EdgeType TopEdgeType { get; set; }
        public EdgeType BottomEdgeType { get; set; }

        public List<Tile> LeftAllowedTiles { get; set; } = new();
        public List<Tile> RightAllowedTiles { get; set; } = new();
        public List<Tile> TopAllowedTiles { get; set; } = new();
        public List<Tile> BottomAllowedTiles { get; set; } = new();

        public Tile() { }
    }
}
