using System;
using System.Collections.Generic;
using Godot;
using hackathon;

public partial class Terrain : TileMap
{
	[Export]
	public Timer timer;

	[Export]
	public int XLength;
	[Export]
	public int YLength;

	private List<(Vector2I cellCoord, Vector2I atlasCoord)> tileLog;
	private int currentLogIndex = 0;

	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		WaveFunctionCollapseGeneration();
	}

	private void WaveFunctionCollapseGeneration()
	{
		var generator = new TerrainGenerator(XLength, YLength);
		tileLog = generator.CollapsedTiles;
	}

	private void OnTimerTimeout()
	{
		SetCell(0, tileLog[currentLogIndex].cellCoord, 0, tileLog[currentLogIndex].atlasCoord);
		currentLogIndex++;
		if (currentLogIndex == tileLog.Count)
		{
			timer.QueueFree();
		}
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
