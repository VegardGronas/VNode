using UnityEditor;
using UnityEngine;

namespace VNode
{
    public partial class NodeEditorWindow : EditorWindow
    {
        private NodeManager nodeManager;
        private NodePort selectedPort;
        private Node selectedNode;
        private Vector2 mousePositionInCanvas;
        private Vector2 mousePosition;
        private Vector2 dragOffset;
        private Vector2 scrollPos;
        private float panningSpeed = .5f;
        private NodeInputManager input;
        private NodeContextMenu contextMenu;

        public static void Open(NodeManager nodeManager)
        {
            NodeEditorWindow window = GetWindow<NodeEditorWindow>("VNode Editor");
            window.nodeManager = nodeManager;

            Node[] nodes = nodeManager.GetComponents<Node>();
            foreach (Node node in nodes)
            {
                NodeRegistry.Register(node);
                node.Initialize();
            }

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
                    if(selectedPort.PortType != otherPort.PortType)
                    {
                        NodeRegistry.AddConnection(new(selectedPort.OwnerNode.ID, selectedPort.ID, otherPort.OwnerNode.ID, otherPort.ID));
                    }
                }
            }
            selectedPort = null;
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
            int i = 0;
            foreach(Node node in NodeRegistry.Nodes.Values)
            {
                node.nodeTransform.Position = new(100, 100 + (100 * i));
                i++;
            }
        }

        private NodePort GetPort()
        {
            foreach(NodePort port in NodeRegistry.Ports.Values)
            {
                if (port.IsPointerInside(mousePosition))
                {
                    return port;
                }
            }
            return null;
        }

        private Node GetNodeIfPointerInside()
        {
            foreach(Node node in NodeRegistry.Nodes.Values)
            {
               if(node.nodeTransform.Rect.Contains(mousePosition))
               {
                    return node;
               }
            }
            return null;
        }
    }
}