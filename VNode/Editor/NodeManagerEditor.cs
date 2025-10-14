using UnityEngine;
using UnityEditor;

namespace VNode
{
    [CustomEditor(typeof(NodeManager))]
    public class NodeManagerEditor : Editor
    {
        private NodeManager nodeManager;

        private void OnEnable()
        {
            nodeManager = (NodeManager)target;
            if(!nodeManager.hasInitialized) nodeManager.Initialize();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if(GUILayout.Button("Update Node List"))
            {
                nodeManager.UpdateList();
            }
        
            if(GUILayout.Button("Open Editor"))
            {
                NodeEditorWindow.Open(nodeManager);
            }

            if(GUILayout.Button("Reset graph"))
            {
                nodeManager.ResetGraph();
            }
        }
    }
}