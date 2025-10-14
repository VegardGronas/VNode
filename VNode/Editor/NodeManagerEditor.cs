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
            nodeManager.Initialize();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if(GUILayout.Button("Save Registry"))
            {
                NodeRegistry.SaveRegistry();
            }

            if (GUILayout.Button("Load Registry"))
            {
                NodeRegistry.LoadRegistry();
            }

            if (GUILayout.Button("Open Editor"))
            {
                NodeEditorWindow.Open(nodeManager);
            }
        }
    }
}