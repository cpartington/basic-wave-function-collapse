using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace hackathon.TileImporter
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
            for (int i = 0; i < tiles.Count; i++)
            {
                Tile sourceTile = tiles[i];
                for (int j = i; j < tiles.Count; j++)
                {
                    Tile targetTile = tiles[j];
                    // t | s
                    if (sourceTile.LeftEdgeType == targetTile.RightEdgeType)
                    {
                        sourceTile.LeftAllowedTiles.Add(targetTile);
                        targetTile.RightAllowedTiles.Add(sourceTile);
                    }
                    // s | t
                    if (sourceTile.RightEdgeType == targetTile.LeftEdgeType)
                    {
                        sourceTile.RightAllowedTiles.Add(targetTile);
                        targetTile.LeftAllowedTiles.Add(sourceTile);
                    }
                    // t / s
                    if (sourceTile.TopEdgeType == targetTile.BottomEdgeType)
                    {
                        sourceTile.TopAllowedTiles.Add(targetTile);
                        targetTile.BottomAllowedTiles.Add(sourceTile);
                    }
                    // s / t
                    if (sourceTile.BottomEdgeType == targetTile.TopEdgeType)
                    {
                        sourceTile.BottomAllowedTiles.Add(targetTile);
                        targetTile.TopAllowedTiles.Add(sourceTile);
                    }
                }
            }

            // Return the populated tiles
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
