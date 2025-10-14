using UnityEditor;
using UnityEngine;

namespace VNode
{
    public partial class NodeEditorWindow : EditorWindow
    {
        private void DrawToolbar()
        {
            if (nodeCollector == null)
            {
                EditorGUILayout.HelpBox("No NodeCollector assigned!", MessageType.Warning);
                return;
            }

            // --- Toolbar ---
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Run nodes", EditorStyles.toolbarButton))
                nodeCollector.Run();
            if (GUILayout.Button("Reset Graph", EditorStyles.toolbarButton))
                nodeCollector.ResetGraph();
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

            // Draw nodes directly
            foreach (Node node in nodeCollector.nodes)
                node.DrawNode(scrollPos);

            if(selectedPort != null && input.dragInputAction.IsDragging)
            {
                DragNodePortConnection(input.dragInputAction.DragValues.position);
            }

            EditorGUILayout.EndScrollView();
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