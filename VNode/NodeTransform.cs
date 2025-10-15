using UnityEngine;

namespace VNode
{
    [System.Serializable]
    public class NodeTransform
    {
        public Vector2 Position { get; set; } // top-left of node in canvas space
        public Vector2 Size { get; set; }

        public Vector2 PositionLocal => Position; // for now same as canvas
        public Vector2 PositionCanvas { get; set; } // can add parent offsets later

        public Rect Rect => new(Position, Size);

        public NodeTransform(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        // Helper: convert local position to canvas
        public Vector2 LocalToCanvas(Vector2 localPos) => Position + localPos;

        // Helper: convert canvas position to local
        public Vector2 CanvasToLocal(Vector2 canvasPos) => canvasPos - Position;

        public bool IsPointerInside(Vector2 mousePos)
        {
            // Define the node rectangle in canvas space
            Rect nodeRect = new(PositionCanvas, Size);

            // Return true if the mouse is inside this rectangle
            return nodeRect.Contains(mousePos);
        }
    }
}