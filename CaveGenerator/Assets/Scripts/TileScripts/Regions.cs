using System.Collections.Generic;

namespace TileScripts
{
    public static class Regions
    {
        public static List<List<Tile>> GetRegions(int tileType, int width, int height, int[,] map)
        {
            var regions = new List<List<Tile>>();
            var mapFlags = new int[width][];
            for (int index = 0; index < width; index++)
            {
                mapFlags[index] = new int[height];
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (mapFlags[x][y] == 0 && map[x, y] == tileType)
                    {
                        var newRegion = GetRegionTiles(x, y, width, height, map);
                        regions.Add(newRegion);
                        foreach (var tile in newRegion)
                        {
                            mapFlags[tile.TileCoordX][tile.TileCoordY] = 1;
                        }
                    }
                }
            }
            return regions;
        }
        
        private static List<Tile> GetRegionTiles(int startX, int startY, int width, int height, int[,] map)
        {
            var tiles = new List<Tile>();
            var mapFlags = new int[width][];
            for (int index = 0; index < width; index++)
            {
                mapFlags[index] = new int[height];
            }

            var tileType = map[startX, startY];
            var queue = new Queue<Tile>();
            queue.Enqueue(new Tile(startX, startY));
            mapFlags[startX][startY] = 1;

            while (queue.Count > 0)
            {
                var curTile = queue.Dequeue();
                tiles.Add(curTile);

                for (int x = curTile.TileCoordX - 1; x <= curTile.TileCoordX + 1; x++)
                {
                    for (int y = curTile.TileCoordY- 1; y <= curTile.TileCoordY + 1; y++)
                        
                    {
                        if (Boundaries.InBoundaries(x, y, height, width) &&
                            (y == curTile.TileCoordY || x == curTile.TileCoordX))
                        {
                            if (mapFlags[x][y] == 0 && map[x, y] == tileType)
                            {
                                mapFlags[x][y] = 1;
                                queue.Enqueue(new Tile(x, y));
                            }
                        }
                    }
                }
            }
            return tiles;
        }
    }
}