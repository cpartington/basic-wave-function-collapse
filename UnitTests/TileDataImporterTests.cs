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
    }
}