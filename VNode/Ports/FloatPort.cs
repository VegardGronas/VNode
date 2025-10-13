using System;
using UnityEditor;
using UnityEngine;

namespace VNode
{
    public class FloatPort : NodePort<float>
    {
        public FloatPort(Node ownerNode, string name, IO portType)
        : base(ownerNode, name, portType)
        {
        }

        public override void DrawGUIElements()
        {
            Value = EditorGUILayout.FloatField("Value", Value);
        }
    }
}