using System;
using System.Collections.Generic;
using Godot;

namespace hackathon
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
        private float _frequency;
        public float Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                _frequency = value;
                FreqLogFreq = _frequency * Math.Log2(_frequency);
            }
        }
        public double FreqLogFreq { get; private set; }

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
