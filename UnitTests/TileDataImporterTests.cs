namespace hackathon.Tests
{
    [TestClass()]
    public class TileDataImporterTests
    {
        [TestMethod()]
        public void GetTileData_NoUnknowns()
        {
            List<Tile> tiles = TileDataImporter.GetTileData();

            static string GetError(string edgeType)
            {
                return $"{edgeType} should not be {nameof(EdgeType)} {nameof(EdgeType.Unknown)}";
            }

            foreach (Tile tile in tiles)
            {
                Assert.IsTrue(tile.LeftEdgeType != EdgeType.Unknown, GetError(nameof(tile.LeftEdgeType)));
                Assert.IsTrue(tile.RightEdgeType != EdgeType.Unknown, GetError(nameof(tile.RightEdgeType)));
                Assert.IsTrue(tile.TopEdgeType != EdgeType.Unknown, GetError(nameof(tile.TopEdgeType)));
                Assert.IsTrue(tile.BottomEdgeType != EdgeType.Unknown, GetError(nameof(tile.BottomEdgeType)));
            }
        }

        [TestMethod()]
        public void GetTileData_CheckTileMappings()
        {
            List<Tile> tiles = TileDataImporter.GetTileData();

            foreach (Tile tile in tiles)
            {
                // Every tile should have at least one tile type that can border it on each side
                Assert.IsTrue(tile.LeftAllowedTiles.Count > 0);
                Assert.IsTrue(tile.RightAllowedTiles.Count > 0);
                Assert.IsTrue(tile.TopAllowedTiles.Count > 0);
                Assert.IsTrue(tile.BottomAllowedTiles.Count > 0);

                // Every tile should match up correctly
                foreach (Tile leftTile in tile.LeftAllowedTiles)
                {
                    Assert.AreEqual(tile.LeftEdgeType, leftTile.RightEdgeType);
                }
                foreach (Tile rightTile in tile.RightAllowedTiles)
                {
                    Assert.AreEqual(tile.RightEdgeType, rightTile.LeftEdgeType);
                }
                foreach (Tile topTile in tile.TopAllowedTiles)
                {
                    Assert.AreEqual(tile.TopEdgeType, topTile.BottomEdgeType);
                }
                foreach (Tile bottomTile in tile.BottomAllowedTiles)
                {
                    Assert.AreEqual(tile.BottomEdgeType, bottomTile.TopEdgeType);
                }
            }
        }
    }
}