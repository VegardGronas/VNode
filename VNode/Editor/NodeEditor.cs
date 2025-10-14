using UnityEngine;
using UnityEditor;

namespace VNode
{
    [CustomEditor(typeof(Node), true)]
    public class NodeEditor : Editor
    {
        Node node;

        private void OnEnable()
        {
            node = (Node)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}