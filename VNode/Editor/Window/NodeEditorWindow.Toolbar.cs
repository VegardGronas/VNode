using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace VNode
{
    public partial class NodeEditorWindow : EditorWindow
    {
        private void DrawToolbar()
        {
            if (nodeManager == null)
            {
                EditorGUILayout.HelpBox("No NodeCollector assigned!", MessageType.Warning);
                return;
            }

            // --- Toolbar ---
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Run nodes", EditorStyles.toolbarButton))
                nodeManager.Run();
            if (GUILayout.Button("Clean node positions", EditorStyles.toolbarButton))
                CleanNodePositions();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCanvas()
        {
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

            input.Update(e);

            DrawPortConnections();

            foreach (Node node in NodeRegistry.Nodes.Values)
            {
                node.DrawNode(scrollPos);
            }

            if (selectedPort != null && input.dragInputAction.IsDragging)
            {
                DragNodePortConnection(input.dragInputAction.DragValues.position);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawPortConnections()
        {
            foreach (NodeConnection connection in NodeRegistry.Connections)
            {
                NodePort fromPort = NodeRegistry.GetPort(connection.FromPortID);
                NodePort toPort = NodeRegistry.GetPort(connection.ToPortID);

                Handles.BeginGUI();

                Vector3 startPos = new(fromPort.Position.x, fromPort.Position.y, 0);
                Vector3 endPos = new(toPort.Position.x, toPort.Position.y, 0);

                Handles.DrawBezier(
                    startPos,
                    endPos,
                    startPos + Vector3.right * 50f,
                    endPos + Vector3.left * 50f,
                    NodeStyling.outputPortColor,
                    null,
                    3f
                );

                Handles.EndGUI();
            }
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