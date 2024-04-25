using System;
using System.Collections.Generic;
using Godot;
using hackathon;

public partial class Terrain : TileMap
{
	// Called when the node enters the scene tree for the first time
	// Also called when window is resized
	public override void _Ready()
	{
        WaveFunctionCollapseGeneration();
    }

    private void WaveFunctionCollapseGeneration()
    {
        // Make a terrain grid that matches the size of the view window
        Vector2 screenSize = GetViewportRect().Size;
        var gridX = (int)screenSize.X;
        var gridY = (int)screenSize.Y;

        Cell[,] terrainGrid = new Cell[gridY, gridX];

        // Keep track of cell with lowest entropy
        double lowestEntropy = double.MaxValue;
        Cell lowestEntropyCell = null;

        // Create a queue for cells that need updating


        // Pick a random cell to start with since everything is initialized equally
        Random random = new();
    }

	private void BasicGeneration()
	{
        // Test code
        List<Tile> tiles = TileDataImporter.GetTileData();
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
