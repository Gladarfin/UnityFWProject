using UnityEngine;

public class SquareGrid
{
    public Square[,] Squares { get; set; }

    #region Constructors

    public SquareGrid(int[,] map, float squareSize)
    {
        var nodeCountX = map.GetLength(0);
        var nodeCountY = map.GetLength(1);
        var controlNodes = CreateControlNodesArray(map, nodeCountX, nodeCountY, squareSize);
        Squares = new Square[nodeCountX - 1, nodeCountY - 1];
        Squares = CreateSquaresArray(controlNodes, nodeCountX, nodeCountY);
    }
    
    #endregion"

    private static ControlNode[,] CreateControlNodesArray(int[,] map, int nodeCountX, int nodeCountY, float squareSize)
    {
        var mapHeight = nodeCountY * squareSize;
        var mapWidth = nodeCountX * squareSize;
        var contNodes = new ControlNode[nodeCountX, nodeCountY];

        for (int x = 0; x < nodeCountX; x++)
        {
            for (int y = 0; y < nodeCountY; y++)
            {
                var pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 
                    0,
                    -mapHeight / 2 + y * squareSize + squareSize / 2);
                
                contNodes[x, y] = new ControlNode(pos, map[x,y] == 1, squareSize);
            }
        }
        return contNodes;
    }

    private static Square[,] CreateSquaresArray(ControlNode[,] controlNodes, int nodeCountX, int nodeCountY)
    {
        Square[,] squares = new Square[nodeCountX - 1, nodeCountY - 1];

        for (int x = 0; x < nodeCountX - 1; x++)
        {
            for (int y = 0; y < nodeCountY - 1; y++)
            {
                squares[x, y] = new Square(controlNodes[x, y + 1], 
                    controlNodes[x + 1, y + 1], 
                    controlNodes[x + 1, y],
                    controlNodes[x, y]);
            }
        }
        return squares;
    }
}
