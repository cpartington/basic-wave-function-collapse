using Godot;

namespace hackathon.Tests
{
    [TestClass()]
    public class TileDataImporterTests
    {
        [TestMethod()]
        public void GetTileData_NoNulls()
        {
            List<Tile> tiles = TileDataImporter.GetTileData();

            static string GetUnknownError(Vector2I coord, string edgeType)
            {
                return $"{edgeType} should not be {nameof(EdgeType.Unknown)} for tile ({coord})";
            }

            static string GetCountError(Vector2I coord, string listName)
            {
                return $"{listName} should have count greater than zero for tile ({coord})";
            }

            foreach (Tile tile in tiles)
            {
                // Check frequency
                Assert.IsNotNull(tile.Likelihood);

                // Check edge types
                Assert.IsTrue(tile.LeftEdgeType != EdgeType.Unknown, GetUnknownError(tile.AtlasCoord, nameof(tile.LeftEdgeType)));
                Assert.IsTrue(tile.RightEdgeType != EdgeType.Unknown, GetUnknownError(tile.AtlasCoord, nameof(tile.RightEdgeType)));
                Assert.IsTrue(tile.TopEdgeType != EdgeType.Unknown, GetUnknownError(tile.AtlasCoord, nameof(tile.TopEdgeType)));
                Assert.IsTrue(tile.BottomEdgeType != EdgeType.Unknown, GetUnknownError(tile.AtlasCoord, nameof(tile.BottomEdgeType)));

                // Check allowed tiles
                Assert.IsTrue(tile.LeftAllowedTiles.Count > 0, GetCountError(tile.AtlasCoord, nameof(tile.LeftAllowedTiles)));
                Assert.IsTrue(tile.RightAllowedTiles.Count > 0, GetCountError(tile.AtlasCoord, nameof(tile.RightAllowedTiles)));
                Assert.IsTrue(tile.TopAllowedTiles.Count > 0, GetCountError(tile.AtlasCoord, nameof(tile.TopAllowedTiles)));
                Assert.IsTrue(tile.BottomAllowedTiles.Count > 0, GetCountError(tile.AtlasCoord, nameof(tile.BottomAllowedTiles)));
            }
        }

        [TestMethod()]
        public void GetTileData_CheckTileMappings()
        {
            List<Tile> tiles = TileDataImporter.GetTileData();

            foreach (Tile tile in tiles)
            {
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