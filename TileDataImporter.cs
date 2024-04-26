using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace hackathon
{
    public static class TileDataImporter
    {
        private const string TileDataFile = "./terrain_tile_rules.json";

        public static List<Tile> GetTileData()
        {
            List<Tile> tiles = new();

            using FileStream stream = File.OpenRead(TileDataFile);
            using JsonDocument jsonDocument = JsonDocument.Parse(stream);

            // Populate tile data
            JsonElement data = jsonDocument.RootElement.GetProperty("tile_data");
            foreach (JsonElement tileData in data.EnumerateArray())
            {
                // Get the tile's x and y atlas coordinates
                JsonElement atlasData = tileData.GetProperty("atlas");
                int x = atlasData.GetProperty("atlas_x").GetInt32();
                int y = atlasData.GetProperty("atlas_y").GetInt32();

                // Get the tile's edge rules
                JsonElement edgeRules = tileData.GetProperty("rules");

                // Add the tile to the list
                tiles.Add(new Tile
                {
                    AtlasCoord = new(x, y),
                    Likelihood = tileData.GetProperty("likelihood").GetInt32(),
                    LeftEdgeType = JsonToTileType(edgeRules.GetProperty("left")),
                    RightEdgeType = JsonToTileType(edgeRules.GetProperty("right")),
                    TopEdgeType = JsonToTileType(edgeRules.GetProperty("top")),
                    BottomEdgeType = JsonToTileType(edgeRules.GetProperty("bottom"))
                });
            }

            // Calculate tile allowances
            for (int sourceTileIndex = 0; sourceTileIndex < tiles.Count; sourceTileIndex++)
            {
                Tile sourceTile = tiles[sourceTileIndex];
                for (int targetTileIndex = sourceTileIndex; targetTileIndex < tiles.Count; targetTileIndex++)
                {
                    Tile targetTile = tiles[targetTileIndex];
                    // t | s
                    if (sourceTile.LeftEdgeType == targetTile.RightEdgeType)
                    {
                        sourceTile.LeftAllowedTiles.Add(targetTileIndex);
                        targetTile.RightAllowedTiles.Add(sourceTileIndex);
                    }
                    // s | t
                    if (sourceTile.RightEdgeType == targetTile.LeftEdgeType)
                    {
                        sourceTile.RightAllowedTiles.Add(targetTileIndex);
                        targetTile.LeftAllowedTiles.Add(sourceTileIndex);
                    }
                    // t / s
                    if (sourceTile.TopEdgeType == targetTile.BottomEdgeType)
                    {
                        sourceTile.TopAllowedTiles.Add(targetTileIndex);
                        targetTile.BottomAllowedTiles.Add(sourceTileIndex);
                    }
                    // s / t
                    if (sourceTile.BottomEdgeType == targetTile.TopEdgeType)
                    {
                        sourceTile.BottomAllowedTiles.Add(targetTileIndex);
                        targetTile.TopAllowedTiles.Add(sourceTileIndex);
                    }
                }
            }

            // Calculate tile frequencies
            int totalLikelihood = 0;
            foreach (Tile tile in tiles)
            {
                totalLikelihood += tile.Likelihood;
            }
            foreach (Tile tile in tiles)
            {
                tile.Frequency = tile.Likelihood / (float)totalLikelihood;
            }

            // Return the populated Tiles
            return tiles;
        }

        private static EdgeType JsonToTileType(JsonElement typeJson)
        {
            return typeJson.GetString() switch
            {
                "grass" => EdgeType.Grass,
                "river" => EdgeType.River,
                _ => EdgeType.Unknown,
            };
        }
    }
}
