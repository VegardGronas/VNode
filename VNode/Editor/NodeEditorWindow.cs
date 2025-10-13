using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VNode
{
    public class NodeEditorWindow : EditorWindow
    {
        private NodeCollector nodeCollector;
        private NodePort selectedPort;
        private Node selectedNode;
        private bool leftControlPressed = false;
        private bool isDragging = false;
        private Vector2 mousePosition;
        private Vector2 dragOffset;

        public static void Open(NodeCollector collector)
        {
            NodeEditorWindow window = GetWindow<NodeEditorWindow>("VNode Editor");
            window.nodeCollector = collector;
            window.Show();
        }

        private void OnGUI()
        {
            if (nodeCollector == null)
            {
                EditorGUILayout.HelpBox("No NodeCollector assigned!", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("Test run the nodes", GUILayout.Width(100)))
            {
                nodeCollector.Run();
            }

            if(GUILayout.Button("Clean node positions"))
            {
                CleanNodePositions();
            }

            Event e = Event.current;
            mousePosition = e.mousePosition;

            ProcessEvents(e);

            DrawGrid();

            foreach (Node node in nodeCollector.nodes)
            {
                node.DrawNode();
            }

            if (selectedPort != null && isDragging) DragNodePortConnection(e);
            else if (selectedNode != null && isDragging) DragNode(e);

            Repaint();
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                        LeftMouseDown(e);
                    break;

                case EventType.MouseUp:
                    if (e.button == 0)
                        LeftMouseUp(e);
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                        MouseDrag(e);
                    break;

                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.LeftControl)
                        leftControlPressed = true;
                    break;

                case EventType.KeyUp:
                    if (e.keyCode == KeyCode.LeftControl)
                        leftControlPressed = false;
                    break;
            }
        }

        private void MouseDrag(Event e)
        {
            isDragging = true;
        }

        private void LeftMouseDown(Event e)
        {
            selectedPort = null;
            selectedNode = null;

            selectedPort = GetPort();
            if(selectedPort == null)
            {
                selectedNode = GetNodeIfPointerInside();
                if(selectedNode != null)
                {
                    dragOffset = mousePosition - selectedNode.nodeTransform.Position;
                }
            }
        }

        private void LeftMouseUp(Event e)
        {
            isDragging = false;
            if (selectedPort != null)
            {
                NodePort otherPort = GetPort();
                if (otherPort != null)
                {
                    selectedPort.Connect(otherPort);
                }
            }
        }

        private void DragNode(Event e)
        {
            selectedNode.nodeTransform.Position = GetSnappedPosition(mousePosition - dragOffset);
            GUI.changed = true;
        }

        private void DragNodePortConnection(Event e)
        {
            selectedPort.DrawConnectionLineWhenDragging(mousePosition);
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
                    if (port.IsPointerInside(mousePosition))
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
                if(node.nodeTransform.IsPointerInside(mousePosition))   
                    return node;
            }

            return null;
        }

        private void DrawGrid(float gridSpacing = 20f, float gridOpacity = 0.2f, Color gridColor = default)
        {
            if (gridColor == default)
                gridColor = Color.gray;

            int width = (int)EditorGUIUtility.currentViewWidth;
            int height = (int)EditorGUIUtility.singleLineHeight * 50; // Fallback height

            // Get the rect of the editor window (or area)
            Rect viewRect = new Rect(0, 0, width, height);

            // Use Handles for performance and crisp lines
            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            // Vertical lines
            for (float x = 0; x < viewRect.width; x += gridSpacing)
                Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, viewRect.height, 0));

            // Horizontal lines
            for (float y = 0; y < viewRect.height; y += gridSpacing)
                Handles.DrawLine(new Vector3(0, y, 0), new Vector3(viewRect.width, y, 0));

            Handles.color = Color.white;
            Handles.EndGUI();
        }
    }
}