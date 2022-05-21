using System;
using System.Collections.Generic;
using TileScripts;
using UnityEngine;

namespace RoomScripts
{
    public class Room : IComparable<Room>
    {
        #region Variables
        private int RoomSize { get; }
        private List<Room> ConnectedRooms { get; }
        private List<Tile> Tiles { get; }
        private List<Tile> EdgeTiles { get; }
        private bool IsAccessibleFromTheMainRoom { get; set; }
        private bool IsMainRoom { get; set; }
        private const int BorderSize = 4;

        #endregion

        #region Constructors

        private Room()
        {
            //Empty room
        }

        public Room(List<Tile> roomTiles, int[,] map) {
            Tiles = roomTiles;
            RoomSize = Tiles.Count;
            ConnectedRooms = new List<Room>();

            EdgeTiles = new List<Tile>();
            foreach (var tile in Tiles) {
                for (int x = tile.TileCoordX-1; x <= tile.TileCoordX+1; x++) {
                    for (int y = tile.TileCoordY-1; y <= tile.TileCoordY + 1; y++)
                    {
                        if (x != tile.TileCoordX && y != tile.TileCoordY) 
                            continue;
                        
                        if (map[x,y] == 1) {
                            EdgeTiles.Add(tile);
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.IsAccessibleFromTheMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }

            if (roomB.IsAccessibleFromTheMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.ConnectedRooms.Add(roomB);
            roomB.ConnectedRooms.Add(roomA);
        }

        private bool IsConnected(Room room)
        {
            return ConnectedRooms.Contains(room);
        }
        
        private static void CreatePassage(Room roomA, Room roomB, Tile tileA, Tile tileB, int width, int height, int[,] map)
        {
            ConnectRooms(roomA, roomB);
            var line = GetLine(tileA, tileB);
            foreach (var t in line)
            {
                DrawCircle(t, 3, width, height, map);
            }
        }

        private static void DrawCircle(Tile t, int r, int width, int height, int[,] map)
        {
            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    if (x * x + y * y <= r * r)
                    {
                        var drawX = t.TileCoordX + x;
                        var drawY = t.TileCoordY + y;

                        if (!Boundaries.IsWall(drawX, drawY, height, width, BorderSize))
                        {
                            map[drawX, drawY] = 0;
                        }
                    }
                }
            }
        }
        
        private static List<Tile> GetLine(Tile from, Tile to)
        {
            var line = new List<Tile>();
            var x = from.TileCoordX;
            var y = from.TileCoordY;
            var dx = to.TileCoordX - x;
            var dy = to.TileCoordY - y;
            var step = (int)Mathf.Sign(dx);
            var gradientStep = (int)Mathf.Sign(dy);
            var longest = Mathf.Abs(dx);
            var shortest = Mathf.Abs(dy);
            var inverted = longest < shortest;

            if (inverted)
            {
                longest = Mathf.Abs(dy);
                shortest = Mathf.Abs(dx);
                step = (int)Mathf.Sign(dy);
                gradientStep = (int)Mathf.Sign(dx);
            }

            var gradientAccumulation = longest / 2;

            for (int i = 0; i < longest; i++)
            {
                line.Add(new Tile(x, y));

                if (inverted)
                {
                    y += step;
                }
                else
                {
                    x += step;
                }

                gradientAccumulation += shortest;
                if (gradientAccumulation < longest) 
                    continue;
                
                switch (inverted)
                {
                    case true:
                        x += gradientStep;
                        break;
                    case false:
                        y += gradientStep;
                        break;
                }

                gradientAccumulation += longest;
            }
            return line;
        }

        private void SetAccessibleFromMainRoom()
        {
            if (IsAccessibleFromTheMainRoom) return;
            IsAccessibleFromTheMainRoom = true;

            foreach (var room in ConnectedRooms)
            {
                room.SetAccessibleFromMainRoom();
            }
        }

        #endregion

        #region Public Methods

        public static void SortRooms(List<Room> allRooms)
        {
            allRooms.Sort();
            allRooms[0].IsMainRoom = true;
            allRooms[0].IsAccessibleFromTheMainRoom = true;
        }
        
        public static void ConnectClosestRooms(List<Room> allRooms, int width, int height, int[,] map, bool forceAccessibilityFromMainRoom = false)
        {
            var roomListA = new List<Room>();
            var roomListB = new List<Room>();

            if (forceAccessibilityFromMainRoom)
            {
                foreach (var room in allRooms)
                {
                    if (room.IsAccessibleFromTheMainRoom)
                    {
                        roomListB.Add(room);
                        continue;
                    }
                    roomListA.Add(room);
                }
            }

            if (!forceAccessibilityFromMainRoom)
            {
                roomListA = allRooms;
                roomListB = allRooms;
            }
            
            var bestDistance = 0;
            var bestTileA = new Tile();
            var bestTileB = new Tile();
            var bestRoomA = new Room();
            var bestRoomB = new Room();
            var possibleConnectionFound = false;

            
            foreach (var roomA in roomListA)
            {
                if (!forceAccessibilityFromMainRoom)
                {
                    possibleConnectionFound = false;

                    if (roomA.ConnectedRooms.Count > 0)
                    {
                        continue;
                    }
                }

                foreach (var roomB in roomListB)
                {
                    if (roomA == roomB || roomA.IsConnected(roomB))
                        continue;
                    
                    foreach (var roomATile in roomA.EdgeTiles)
                    {
                        foreach (var roomBTile in roomB.EdgeTiles)
                        {
                            var distanceBetweenRooms = (int)(Mathf.Pow(roomATile.TileCoordX - roomBTile.TileCoordX, 2) 
                                                             + Mathf.Pow(roomATile.TileCoordY - roomBTile.TileCoordY, 2));

                            if (bestDistance <= distanceBetweenRooms && possibleConnectionFound) 
                                continue;
                            
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = roomATile;
                            bestTileB = roomBTile;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
                if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
                {
                    CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB, width, height, map);
                }
            }
            
            if (possibleConnectionFound && forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB, width, height, map);
                ConnectClosestRooms(allRooms, width, height, map, true);
            }

            if (!forceAccessibilityFromMainRoom)
            {
                ConnectClosestRooms(allRooms, width, height, map, true);
            }
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.RoomSize.CompareTo(RoomSize);
        }

        #endregion
    }
}