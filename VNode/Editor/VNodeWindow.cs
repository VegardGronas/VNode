using UnityEditor;
using UnityEngine;

namespace VNode
{
    public class VNodeWindow : EditorWindow
    {
        private static VNodeWindow window;

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
            }
        }
    }
}