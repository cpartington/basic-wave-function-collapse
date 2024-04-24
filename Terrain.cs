using System;
using System.Collections.Generic;
using Godot;
using hackathon;

public partial class Terrain : TileMap
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Use a rolling n x n window to generate the terrain following lowest entropy
		// Weighted likelihoods for every tile

		// Test code
		List<TileType> tiles = TileDataImporter.GetTileData();
		Vector2 ScreenSize = GetViewportRect().Size;
		Random random = new();
		for (int x = 0; x < ScreenSize.X; x++)
		{
			for (int y = 0; y < ScreenSize.Y; y++)
			{
				// Pick a (pseudo) random tile
				var index = random.Next(tiles.Count);
				var randomTile = tiles[index];

				// Set the cell to the picked tile
				SetCell(0, new Vector2I(x, y), 0, randomTile.AtlasCoord);
			}
		}
	}
}
