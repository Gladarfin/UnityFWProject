using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square
{
    #region Variables

    public Node CenterTop { get; set; }
    public Node CenterRight { get; set; }
    public Node CenterBottom { get; set; }
    public Node CenterLeft { get; set; }
    public ControlNode TopLeft { get; set; }
    public ControlNode TopRight { get; set; }
    public ControlNode BottomRight { get; set; }
    public ControlNode BottomLeft { get; set; }
    public int Configuration { get; set; }

    #endregion

    #region Constructors

    public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
    {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
        CenterTop = TopLeft.Right;
        CenterRight = BottomRight.Above;
        CenterBottom = BottomLeft.Right;
        CenterLeft = BottomLeft.Above;
        SetConfiguration();
    }

    #endregion
    
    private void SetConfiguration()
    {
        if (TopLeft.IsActive)
        {
            Configuration += 8;
        }

        if (TopRight.IsActive)
        {
            Configuration += 4;
        }

        if (BottomRight.IsActive)
        {
            Configuration += 2;
        }

        if (BottomLeft.IsActive)
        {
            Configuration += 1;
        }
    }
}
