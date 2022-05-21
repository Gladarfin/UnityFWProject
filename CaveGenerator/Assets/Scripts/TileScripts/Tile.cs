using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

public class Tile 
{
    #region Variables

    public int TileCoordX { get; set; }
    public int TileCoordY { get; set; }

    #endregion

    #region Constructors

    public Tile(int x, int y)
    {
        TileCoordX = x;
        TileCoordY = y;
    }

    public Tile()
    {
        
    }

    #endregion
    
}
