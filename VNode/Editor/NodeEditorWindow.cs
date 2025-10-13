using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
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
        private bool middleClickPressed = false;
        private Vector2 mousePositionInCanvas;
        private Vector2 mousePosition;
        private Vector2 dragOffset;
        private Vector2 scrollPos;
        private float panningSpeed = .5f;
        
        public static void Open(NodeCollector collector)
        {
            NodeEditorWindow window = GetWindow<NodeEditorWindow>("VNode Editor");
            window.nodeCollector = collector;
            window.Show();
        }

        private void OnGUI()
        {
            DrawGrid();

            if (nodeCollector == null)
            {
                EditorGUILayout.HelpBox("No NodeCollector assigned!", MessageType.Warning);
                return;
            }

            // --- Toolbar ---
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Run nodes", EditorStyles.toolbarButton))
                nodeCollector.Run();

            if (GUILayout.Button("Clean node positions", EditorStyles.toolbarButton))
                CleanNodePositions();
            EditorGUILayout.EndHorizontal();

            // --- Scrollable node canvas ---
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true);

            // Reserve layout space
            Rect canvasRect = GUILayoutUtility.GetRect(4000, 4000);

            // Draw background
            EditorGUI.DrawRect(canvasRect, NodeStyling.backgroundColor * 0.5f);

            // Adjust mouse position to canvas space
            Event e = Event.current;
            mousePositionInCanvas = e.mousePosition + scrollPos; // Keep this
            mousePosition = e.mousePosition;

            ProcessEvents(e);

            // Draw nodes directly
            foreach (Node node in nodeCollector.nodes)
                node.DrawNode(scrollPos);

            if(middleClickPressed)
            {
                Pan(e);
            }

            if (selectedPort != null && isDragging)
                DragNodePortConnection(e);
            else if (selectedNode != null && isDragging)
                DragNode(e);

            EditorGUILayout.EndScrollView();

            Repaint();
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                        LeftMouseDown(e);
                    else if (e.button == 2) // Middle click
                        OnMiddleMouseDown(e);
                    break;

                case EventType.MouseUp:
                    if (e.button == 0)
                        LeftMouseUp(e);
                    else if (e.button == 2) // Middle click
                        OnMiddleMouseUp(e);
                    break;
                
                case EventType.ContextClick:
                    RightMouseDown(e);
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                        MouseDrag(e);
                    break;
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.LeftControl)
                        leftControlPressed = true;

                    if (e.keyCode == KeyCode.Delete)
                    {
                        if (selectedNode != null) nodeCollector.DeleteNode(selectedNode);
                        selectedNode = null;
                    }
                    break;

                case EventType.KeyUp:
                    if (e.keyCode == KeyCode.LeftControl)
                        leftControlPressed = false;
                    break;
            }
        }

        private void RightMouseDown(Event e)
        {
            OpenContextMenu();
        }

        private void Pan(Event e)
        {
            scrollPos -= e.delta * panningSpeed;
        }

        private void OpenContextMenu()
        {
            GenericMenu menu = new GenericMenu();

            var nodeTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
                })
                .Where(t => typeof(Node).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            if (nodeTypes.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No node types found"));
            }
            else
            {
                foreach (var type in nodeTypes)
                {
                    string name = ObjectNames.NicifyVariableName(type.Name);
                    menu.AddItem(new GUIContent(name), false, () => CreateNode(type, mousePosition));
                }
            }

            menu.ShowAsContext();
        }

        private void CreateNode(Type nodeType, Vector2 position)
        {
            if (nodeCollector == null)
            {
                Debug.LogError("No NodeCollector found — cannot create node!");
                return;
            }

            // Create the node as a MonoBehaviour component
            Node newNode = (Node)nodeCollector.gameObject.AddComponent(nodeType);
            newNode.nodeTransform.Position = position;

            nodeCollector.UpdateList();

            Debug.Log($"Created node of type {nodeType.Name} at {position}");
        }

        private void MouseDrag(Event e)
        {
            isDragging = true;
        }

        private void OnMiddleMouseDown(Event e)
        {
            middleClickPressed = true;
        }
        
        private void OnMiddleMouseUp(Event e)
        {
            middleClickPressed = false;
        }

        private void LeftMouseDown(Event e)
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
            selectedNode.nodeTransform.Position = GetSnappedPosition(mousePositionInCanvas - dragOffset);
            GUI.changed = true;
        }

        private void DragNodePortConnection(Event e)
        {
            selectedPort.DrawConnectionLineWhenDragging(e.mousePosition);
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

        private void DrawGrid(float gridSpacing = 20f)
        {
            int width = (int)EditorGUIUtility.currentViewWidth;
            int height = (int)EditorGUIUtility.singleLineHeight * 50; // Fallback height

            // Get the rect of the editor window (or area)
            Rect viewRect = new Rect(0, 0, width, height);

            // Use Handles for performance and crisp lines
            Handles.BeginGUI();
            Handles.color = NodeStyling.gridColor;

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