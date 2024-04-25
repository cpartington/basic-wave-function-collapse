using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hackathon
{
    public class Cell
    {
        private static readonly List<Tile> tiles = TileDataImporter.GetTileData();

        public bool[] IsPossibleTile { get; set; }
        public double Entropy { get; set; }

        private float PossibleTileWeightsSum;
        private double PossibleTileWeightLogWeightsSum;

        public Cell()
        {
            IsPossibleTile = new bool[tiles.Count];
            for (int i = 0; i < IsPossibleTile.Length; i++)
            {
                IsPossibleTile[i] = true;
            }

            // Initially, all tiles are possible so sum of frequencies is 1
            PossibleTileWeightsSum = 1f;

            // Calculate w1*log(w1) + w2*log(w2) + ... + wn*log(wn)
            double totalWeightLogWeight = 0;
            for (int i = 0; i < tiles.Count; i++)
            {
                totalWeightLogWeight += tiles[i].FreqLogFreq;
            }
            PossibleTileWeightLogWeightsSum = totalWeightLogWeight;
        }

        public void RemovePossibleTile(int tileIndex)
        {
            IsPossibleTile[tileIndex] = false;
            
            // Remove the tile's frequency and freq * log(freq) from the totals
            PossibleTileWeightsSum -= tiles[tileIndex].Frequency;
            PossibleTileWeightLogWeightsSum -= tiles[tileIndex].FreqLogFreq;

            // Update entropy
            Entropy = Math.Log2(PossibleTileWeightsSum) - (PossibleTileWeightLogWeightsSum / PossibleTileWeightsSum);
        }
    }
}
