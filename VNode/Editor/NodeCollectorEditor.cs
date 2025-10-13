using UnityEngine;
using UnityEditor;

namespace VNode
{
    [CustomEditor(typeof(NodeCollector))]
    public class NodeCollectorEditor : Editor
    {
        private NodeCollector collector;

        private void OnEnable()
        {
            collector = (NodeCollector)target;
            if(!collector.hasInitialized) collector.Initialize();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if(GUILayout.Button("Update Node List"))
            {
                collector.UpdateList();
            }
        
            if(GUILayout.Button("Open Editor"))
            {
                NodeEditorWindow.Open(collector);
            }

            if(GUILayout.Button("Reset graph"))
            {
                collector.nodes.Clear();
                collector.UpdateList();
                foreach (Node node in collector.nodes)
                {
                    node.Initialize(true);
                }
            }
        }
    }
}