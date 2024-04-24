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
            
            JsonElement data = jsonDocument.RootElement.GetProperty("tile_data");
            foreach (JsonElement tileData in data.EnumerateArray())
            {
                // Get the tile's x and y atlas coordinates
                JsonElement atlasData = tileData.GetProperty("atlas");
                int x = atlasData.GetProperty("atlas_x").GetInt32();
                int y = atlasData.GetProperty("atlas_y").GetInt32();

                // Get the tile's edge rules
                JsonElement edgeRules = tileData.GetProperty("rules");

                tiles.Add(new Tile
                {
                    AtlasCoord = new(x, y),
                    LeftEdgeType = JsonToTileType(edgeRules.GetProperty("left")),
                    RightEdgeType = JsonToTileType(edgeRules.GetProperty("right")),
                    TopEdgeType = JsonToTileType(edgeRules.GetProperty("top")),
                    BottomEdgeType = JsonToTileType(edgeRules.GetProperty("bottom"))
                });
            }

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
