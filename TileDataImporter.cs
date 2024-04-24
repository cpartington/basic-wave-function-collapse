using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace hackathon
{
    internal static class TileDataImporter
    {
        private const string TileDataFile = "./terrain_tile_rules.json";

        public static List<TileType> GetTileData()
        {
            List<TileType> tileTypes = new();

            using FileStream stream = File.OpenRead(TileDataFile);
            using JsonDocument jsonDocument = JsonDocument.Parse(stream);
            
            JsonElement data = jsonDocument.RootElement.GetProperty("tile_data");
            foreach (JsonElement tileData in data.EnumerateArray())
            {
                JsonElement atlasData = tileData.GetProperty("atlas");
                int x = atlasData.GetProperty("atlas_x").GetInt32();
                int y = atlasData.GetProperty("atlas_y").GetInt32();

                tileTypes.Add(new TileType(x, y));
            }

            return tileTypes;
        }
    }
}
