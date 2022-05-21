using UnityEngine;

public class ControlNode : Node
{
        #region Variables

        public bool IsActive { get; }
        public Node Above { get; }
        public Node Right { get; }

        #endregion
        
        public ControlNode(Vector3 pos, bool isActive, float squareSize): base(pos)
        {
                IsActive = isActive;
                Above = new Node(pos + Vector3.forward * squareSize / 2);
                Right = new Node(pos + Vector3.right * squareSize / 2);
        }
}