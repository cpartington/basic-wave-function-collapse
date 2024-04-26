using System;
using System.Collections.Generic;
using Godot;

namespace hackathon
{
	public class TerrainGenerator
	{
		private enum Direction
		{
			LEFT,
			RIGHT,
			TOP,
			BOTTOM
		}

		private struct UpdateData
		{
			public int XCoord;
			public int YCoord;
			public List<int> UpdatedTileIndexes;
			public Direction UpdateDirection;
		}

		// Tile data
		private readonly List<Tile> Tiles = TileDataImporter.GetTileData();

		// Grid data
		private readonly Cell[,] TerrainGrid;
		private readonly int XLength;
		private readonly int YLength;

		// Algorithm data
		private readonly Queue<UpdateData> UpdateQueue;
		private readonly PriorityQueue<(int x, int y), double> EntropyHeap;
		
		public List<(Vector2I cellCoord, Vector2I atlasCoord)> CollapsedTiles { get; private set; }

		public TerrainGenerator(int x, int y)
		{
			XLength = x;
			YLength = y;
			TerrainGrid = new Cell[YLength, XLength];
			UpdateQueue = new();
			EntropyHeap = new(XLength * YLength);
			CollapsedTiles = new();

			bool success = RunGeneration();
			while (!success)
			{
				Logger.LogWarning("Hit a contradiction, restarting.");
				success = RunGeneration();
			}
		}
		
		public bool RunGeneration()
		{
			//Logger.Log($"Entering {nameof(RunGeneration)}");
			Initialize();

			while (true)
			{
				// Pick the tile with the lowest entropy
				// If the cell has already been collapsed, ignore it and pick the next one
				Cell cellToCollapse;
				do
				{
					// If the heap is empty, we've collapsed all the Tiles and we can finish generating
					if (EntropyHeap.Count < 1)
					{
						//Logger.Log($"Exiting {nameof(RunGeneration)}");
						return true;
					}

					(int x, int y) = EntropyHeap.Dequeue();
					cellToCollapse = TerrainGrid[y, x];
				}
				while (cellToCollapse.IsCollapsed);

				// Collapse the cell
				cellToCollapse.CollapseCell(Tiles);
				CollapsedTiles.Add((cellToCollapse.Coordinate, GetAtlasCoordinate(cellToCollapse.CollapsedTileIndex)));
				AddNeighborsToQueue(cellToCollapse.Coordinate.X, cellToCollapse.Coordinate.Y);

				// Propagate changes
				while (UpdateQueue.Count > 0)
				{
					UpdateData updatedCellData = UpdateQueue.Dequeue();
					Cell cellToUpdate = TerrainGrid[updatedCellData.YCoord, updatedCellData.XCoord];
					//Logger.Log($"Removed {cellToUpdate.Coordinate} from the queue for processing, updated queue length: {UpdateQueue.Count}");
					bool madeAnUpdate = false;

					// Iterate over all the possible tiles for the current cell
					for (int possibleTileIndex = 0; possibleTileIndex < cellToUpdate.TileValidity.Length; possibleTileIndex++)
					{
						// Only check tile types that are still possible in the current cell
						if (cellToUpdate.TileValidity[possibleTileIndex])
						{
							// Get the list of tiles allowed by the current tile type in the direction of the updated cell
							List<int> allowedEdgeTiles = GetAllowedTilesInDirection(possibleTileIndex, updatedCellData.UpdateDirection);

							// Iterate over the tile validity of the updated cell
							// We want to know if at least one of the provided tile indexes is present in the allo list
							bool atLeastOneMatch = false;
							foreach (int allowedIndex in updatedCellData.UpdatedTileIndexes)
							{
								if (allowedEdgeTiles.Contains(allowedIndex))
								{
									atLeastOneMatch = true;
								}

							}

							// If none of the possible indexes are present in the cell's allowed tiles, this tile is no longer possible
							if (!atLeastOneMatch)
							{
								// Remove the tile type from the possible tiles for the current cell
								bool success = cellToUpdate.TryRemovePossibleTile(possibleTileIndex, Tiles);
								// If TryRemovePossibleTile returns false, we've hit a contradiction and need to start over
								if (!success) return false;
								madeAnUpdate = true;
							}
						}
					}

					// If we've made a change to the possible tiles for this cell, we need to propagate to its neighbors
					if (madeAnUpdate)
					{
						AddNeighborsToQueue(updatedCellData.XCoord, updatedCellData.YCoord); 
					}
				}
			}
		}

		private void Initialize()
		{
			// Clear the entropy heap and the update queue
			EntropyHeap.Clear();
			UpdateQueue.Clear();
			CollapsedTiles.Clear();

			// Initialize the grid and build the entropy heap
			for (int y = 0; y < YLength; y++)
			{
				for (int x = 0; x < XLength; x++)
				{
					Cell cell = new(x, y, Tiles);
					TerrainGrid[y, x] = cell;
					EntropyHeap.Enqueue((x, y), cell.Entropy);
				}
			}
		}

		private void AddNeighborsToQueue(int x, int y)
		{
			// Convert validity array to a list of allowed indexes
			bool[] tileValidity = TerrainGrid[y, x].TileValidity;
			List<int> allowedTiles = new();
			for (int i = 0; i < tileValidity.Length; i++)
			{
				if (tileValidity[i]) allowedTiles.Add(i);
			}

			(int newX, int newY) = GetNewCoord((x, y), Direction.LEFT);
			if (newX >= 0 && !TerrainGrid[newY, newX].IsCollapsed)
			{
				// Update the cell to the left (i.e., original cell is to its right)
				UpdateQueue.Enqueue(new UpdateData { XCoord = newX, YCoord = newY, UpdatedTileIndexes = allowedTiles, UpdateDirection = Direction.RIGHT });
				//Logger.Log($"Added cell {TerrainGrid[newY, newX].Coordinate} to the queue for processing, updated queue length: {UpdateQueue.Count}");
			}

			(newX, newY) = GetNewCoord((x, y), Direction.TOP);
			if (newY >= 0 && !TerrainGrid[newY, newX].IsCollapsed)
			{
				// Update the cell above
				UpdateQueue.Enqueue(new UpdateData { XCoord = newX, YCoord = newY, UpdatedTileIndexes = allowedTiles, UpdateDirection = Direction.BOTTOM });
				//Logger.Log($"Added cell {TerrainGrid[newY, newX].Coordinate} to the queue for processing, updated queue length: {UpdateQueue.Count}");
			}

			(newX, newY) = GetNewCoord((x, y), Direction.RIGHT);
			if (newX < XLength && !TerrainGrid[newY, newX].IsCollapsed)
			{
				// Update the cell to the right
				UpdateQueue.Enqueue(new UpdateData { XCoord = newX, YCoord = newY, UpdatedTileIndexes = allowedTiles, UpdateDirection = Direction.LEFT });
				//Logger.Log($"Added cell {TerrainGrid[newY, newX].Coordinate} to the queue for processing, updated queue length: {UpdateQueue.Count}");
			}

			(newX, newY) = GetNewCoord((x, y), Direction.BOTTOM);
			if (newY < YLength && !TerrainGrid[newY, newX].IsCollapsed)
			{
				// Update the cell below
				UpdateQueue.Enqueue(new UpdateData { XCoord = newX, YCoord = newY, UpdatedTileIndexes = allowedTiles, UpdateDirection = Direction.TOP });
				//Logger.Log($"Added cell {TerrainGrid[newY, newX].Coordinate} to the queue for processing, updated queue length: {UpdateQueue.Count}");
			}
		}

		private static (int, int) GetNewCoord((int x, int y) coord, Direction direction)
		{
			return direction switch
			{
				Direction.LEFT => (coord.x - 1, coord.y),
				Direction.RIGHT => (coord.x + 1, coord.y),
				Direction.TOP => (coord.x, coord.y - 1),
				Direction.BOTTOM => (coord.x, coord.y + 1),
				_ => throw new NotImplementedException()
			};
		}

		private Vector2I GetAtlasCoordinate(int index)
		{
			return Tiles[index].AtlasCoord;
		}

		private List<int> GetAllowedTilesInDirection(int tileIndex, Direction direction)
		{
			return direction switch
			{
				Direction.LEFT => Tiles[tileIndex].LeftAllowedTiles,
				Direction.RIGHT => Tiles[tileIndex].RightAllowedTiles,
				Direction.TOP => Tiles[tileIndex].TopAllowedTiles,
				Direction.BOTTOM => Tiles[tileIndex].BottomAllowedTiles,
				_ => throw new NotImplementedException()
			};
		}
	}
}
