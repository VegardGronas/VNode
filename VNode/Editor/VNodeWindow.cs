using UnityEditor;
using UnityEngine;

namespace VNode
{
    public class VNodeWindow : EditorWindow
    {
        private static VNodeWindow window;
        private Vector2 scrollPos;
        private string readmeContent = "";
        private bool readmeOpen = false;

        [MenuItem("VNode/Help window")]
        public static void OpenWindow()
        {
            // If already open, focus it
            if (window != null)
            {
                window.Focus();
                return;
            }

            // Otherwise, create and show a new one
            window = CreateWindow<VNodeWindow>("VNode");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Welcome to VNode!", EditorStyles.boldLabel);
        
            if(GUILayout.Button("Create VNode Graph"))
            {
                GameObject nodeObject = new("VNode Graph");
                nodeObject.AddComponent<NodeCollector>();
                Selection.activeObject = nodeObject;
                NodeCollector collector = nodeObject.gameObject.GetComponent<NodeCollector>();
                NodeEditorWindow.Open(collector);
            }

            GUILayout.Space(10);
            
            if (!readmeOpen && GUILayout.Button("Open README file"))
            {
                readmeOpen = true;
            }
            else if(readmeOpen && GUILayout.Button("Close README file"))
            {
                readmeOpen = false;
            }

            if (readmeOpen)
            {
                string readmePath = Application.dataPath + "/VNode/README.md";
                if (System.IO.File.Exists(readmePath))
                {
                    readmeContent = System.IO.File.ReadAllText(readmePath);
                    readmeOpen = true;
                }

                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
                if (!string.IsNullOrEmpty(readmeContent))
                {
                    GUILayout.TextArea(readmeContent, GUILayout.ExpandHeight(true));
                }
                GUILayout.EndScrollView();
            }
        }
    }
}