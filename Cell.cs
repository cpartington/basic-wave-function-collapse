using System;
using System.Collections.Generic;
using Godot;

namespace hackathon
{
    public class Cell
    {
        public Vector2I Coordinate { get; private set; }

        public bool[] TileValidity { get; private set; }
        public double Entropy { get; private set; }

        public bool IsCollapsed { get; private set; }
        public int CollapsedTileIndex { get; private set; }

        private float PossibleTileWeightsSum;
        private double PossibleTileWeightLogWeightsSum;
        private readonly double Epsilon;

        public Cell(int xCoord, int yCoord, List<Tile> tiles)
        {
            Coordinate = new Vector2I(xCoord, yCoord);

            TileValidity = new bool[tiles.Count];
            for (int i = 0; i < TileValidity.Length; i++)
            {
                TileValidity[i] = true;
            }

            // Initially, all Tiles are possible so sum of frequencies is 1
            PossibleTileWeightsSum = 1f;

            // Calculate w1*log(w1) + w2*log(w2) + ... + wn*log(wn)
            double totalWeightLogWeight = 0;
            for (int i = 0; i < tiles.Count; i++)
            {
                totalWeightLogWeight += tiles[i].FreqLogFreq;
            }
            PossibleTileWeightLogWeightsSum = totalWeightLogWeight;

            // Randomly determine entropy noise for this cell
            Random random = new();
            Epsilon = random.NextDouble() * Math.Pow(10, -5);

            // Calculate initial entropy
            UpdateEntropy();
        }

        public bool TryRemovePossibleTile(int tileIndex, List<Tile> tiles)
        {
            TileValidity[tileIndex] = false;
            if (CheckForContradiction()) return false;

            // Remove the tile's frequency and freq * log(freq) from the totals
            PossibleTileWeightsSum -= tiles[tileIndex].Frequency;
            PossibleTileWeightLogWeightsSum -= tiles[tileIndex].FreqLogFreq;

            // Update entropy
            UpdateEntropy();

            return true;
        }

        public void CollapseCell(List<Tile> tiles)
        {
            // Pick a random value between 0 and the sum of possible tile weights
            // This gives us a weighted random choice between the remaining possible Tiles
            Random random = new();
            float remainingWeight = random.NextSingle() * PossibleTileWeightsSum;

            // Start subtracting weights until we go negative
            for (int i = 0; i < tiles.Count; i++)
            {
                if (TileValidity[i])
                {
                    float tileWeight = tiles[i].Frequency;
                    if (remainingWeight > tileWeight)
                    {
                        remainingWeight -= tileWeight;
                    }
                    else
                    {
                        IsCollapsed = true;
                        CollapsedTileIndex = i;
                        break;
                    }
                }
            }

            // Set all possible Tiles to false except the chosen tile
            for (int i = 0; i < tiles.Count; i++)
            {
                if (i != CollapsedTileIndex)
                {
                    TileValidity[i] = false;
                }
            }

            Logger.Log($"Collapsed cell {Coordinate}, selected tile: {CollapsedTileIndex}");
        }

        private void UpdateEntropy()
        {
            Entropy = Math.Log2(PossibleTileWeightsSum) - (PossibleTileWeightLogWeightsSum / PossibleTileWeightsSum) + Epsilon;
        }

        private bool CheckForContradiction()
        {
            for (int i = 0; i < TileValidity.Length; i++)
            {
                if (TileValidity[i]) return false;
            }

            // If we reach this point, no tile can be placed in this cell
            return true;
        }
    }
}
