using System;
using System.Collections.Generic;
using RoomScripts;
using TileScripts;
using UnityEngine;

namespace Generators
{
    public class MapGenerator : MonoBehaviour
    {
        #region Variables
    
        private int[,] _map;

        [SerializeField]
        [Range(0, 100)]
        private int randomFillPercent;
    
        [SerializeField]
        private int width, height;

        [SerializeField]
        private int smoothLevel;

        private const int SmoothEdge = 4;

        private const int BorderSize = 4;
        
        private const int WallThresholdSize = 50;
        
        private const int RoomThresholdSize = 50;

        private List<Room> _survivedRooms;
    
        #endregion
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GenerateMap();
            }
        }

        private void GenerateMap()
        {
            _map = new int[width, height];
            _survivedRooms = new List<Room>();
            FillMap();

            for (int i = 0; i < smoothLevel; i++)
            {
                SmoothMap();
            }
            
            ProcessMap(1, WallThresholdSize);
            ProcessMap(0, RoomThresholdSize);
            
            var meshGenerator = GetComponent<MeshGenerator>();
            meshGenerator.GenerateMesh(_map, 1);
        }

        private void FillMap()
        {
            var rnd = new System.Random();
            
            var pseudoRandom = new System.Random(rnd.Next(0, int.MaxValue));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (Boundaries.IsWall(x, y, height, width, BorderSize))
                    {
                        _map[x, y] = 1;
                        continue;
                    }
                    _map[x, y] = pseudoRandom.Next(0, 100) < randomFillPercent ? 1: 0;
                }
            }
        }

        private void SmoothMap()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var neighboursWallTiles = CountWallsInNeighbours(x, y);

                    if (neighboursWallTiles > SmoothEdge)
                    {
                        _map[x, y] = 1;
                        continue;
                    }

                    if (neighboursWallTiles < SmoothEdge)
                    {
                        _map[x, y] = 0;
                    }
                }
            }
        }

        private int CountWallsInNeighbours(int x, int y)
        {
            int wallCount = 0;

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (Boundaries.InBoundaries(i, j, height, width))
                    {
                        if (i != x || j != y)
                        {
                            wallCount += _map[i, j];
                        }
                        continue;
                    }
                    wallCount++;
                }
            }
            return wallCount;
        }
        
        private void ProcessMap(int tileType, int thresholdSize)
        {
            var regions = Regions.GetRegions(tileType, width, height, _map);
            var tileValue = tileType == 0 ? 1 : 0;
            foreach (var region in regions)
            {
                if (region.Count >= thresholdSize)
                {
                    if (tileType == 0) //room
                    {
                        _survivedRooms.Add(new Room(region, _map));
                    }
                    continue;
                }
                
                foreach (var tile in region)
                {
                    _map[tile.TileCoordX, tile.TileCoordY] = tileValue;
                }
            }

            if (_survivedRooms.Count <= 0) 
                return;
            
            Room.SortRooms(_survivedRooms);
            Room.ConnectClosestRooms(_survivedRooms, width, height, _map);
        }
    }
}
