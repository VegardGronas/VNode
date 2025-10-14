using UnityEditor;
using UnityEngine;

namespace VNode
{
    public partial class NodeEditorWindow : EditorWindow
    {
        private NodeManager nodeCollector;
        private NodePort selectedPort;
        private Node selectedNode;
        private Vector2 mousePositionInCanvas;
        private Vector2 mousePosition;
        private Vector2 dragOffset;
        private Vector2 scrollPos;
        private float panningSpeed = .5f;
        private NodeInputManager input;
        private NodeContextMenu contextMenu;

        public static void Open(NodeManager collector)
        {
            NodeEditorWindow window = GetWindow<NodeEditorWindow>("VNode Editor");
            window.nodeCollector = collector;
            window.Show();
        }

        private void OnEnable()
        {
            input = new();
            input.OnActionPressed += OnInputActionPressed;
            input.OnActionReleased += OnInputActionReleased;
            input.OnDragging += OnDrag;
            contextMenu = new();
        }

        private void OnGUI()
        {
            DrawGrid();

            DrawToolbar();

            DrawCanvas();

            Repaint();
        }

        private void Pan(Vector2 delta)
        {
            scrollPos -= delta * panningSpeed;
        }

        private void LeftMouseDown()
        {
            if(selectedNode != null) selectedNode.IsSelected = false;
            selectedPort = null;
            selectedNode = null;

            selectedPort = GetPort();
            if(selectedPort == null)
            {
                selectedNode = GetNodeIfPointerInside();
                if(selectedNode != null)
                {
                    dragOffset = mousePositionInCanvas - selectedNode.nodeTransform.Position;
                    selectedNode.IsSelected = true;
                }
            }
        }

        private void LeftMouseUp()
        {
            if (selectedPort != null)
            {
                NodePort otherPort = GetPort();
                if (otherPort != null)
                {
                    selectedPort.Connect(otherPort);
                }
            }
        }

        private void DragNode()
        {
            selectedNode.nodeTransform.Position = GetSnappedPosition(mousePositionInCanvas - dragOffset);
            GUI.changed = true;
        }

        private void DragNodePortConnection(Vector2 position)
        {
            selectedPort.DrawConnectionLineWhenDragging(position);
            GUI.changed = true;
        }

        private Vector2 GetSnappedPosition(Vector2 position, float gridSpacing = 20f)
        {
            float x = Mathf.Round(position.x / gridSpacing) * gridSpacing;
            float y = Mathf.Round(position.y / gridSpacing) * gridSpacing;
            return new Vector2(x, y);
        }

        private void CleanNodePositions()
        {
            for(int i = 0; i < nodeCollector.nodes.Count; i++)
            {
                nodeCollector.nodes[i].nodeTransform.Position = new(100, 100 * (i + 1));
            }
        }

        private NodePort GetPort()
        {
            foreach (Node node in nodeCollector.nodes)
            {
                foreach (NodePort port in node.ports.Values)
                {
                    if (port.IsPointerInside(mousePositionInCanvas))
                    {
                        Debug.Log("Clicked node port");
                        return port;
                    }
                }
            }
            return null;
        }

        private Node GetNodeIfPointerInside()
        {
            foreach(Node node in nodeCollector.nodes)
            {
                if(node.nodeTransform.IsPointerInside(mousePositionInCanvas))   
                    return node;
            }

            return null;
        }
    }
}