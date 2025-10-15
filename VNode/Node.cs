using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VNode
{
    [Serializable]
    public class NodeData
    {
        public string ID { get; }
        public NodeTransform NodeTransform { get; }
        public Type NodeType { get; }

        public NodeData(string id, NodeTransform nodeTransform, Type nodeType)
        {
            ID = id;
            NodeTransform = nodeTransform;
            NodeType = nodeType;
        }
    }

    public class Node : MonoBehaviour
    {
        public string ID { get; } = Guid.NewGuid().ToString();
        public Attributes attributes = new();
        public NodeTransform nodeTransform = new(new(200, 200), new(200, 100));
        public bool IsSelected { get; set; } = false;
        
        public virtual string NodeDisplayName => "Node";

        public void Initialize() 
        {
            OnInitialize(); 
        }
        
        protected virtual void OnInitialize() { }

        public virtual void Execute(HashSet<Node> visited) { }

        public void TriggerNext(NodePort port, HashSet<Node> visited = null)
        {
        }

        public virtual void NewPortConnection(NodePort port) { }

        public void DrawNode(Vector2 scrollPos)
        {
            Handles.BeginGUI();

            // Node rect
            Rect nodeRect = nodeTransform.Rect;
            nodeTransform.PositionCanvas = nodeTransform.Position + scrollPos;

            // Determine node color
            Color nodeColor = NodeStyling.defaultColor;
            if (IsSelected) // Optional: highlight selected node
                nodeColor = NodeStyling.selectedColor;

            // Draw colored background
            EditorGUI.DrawRect(nodeRect, nodeColor);

            // Draw border overlay
            GUI.Box(nodeRect, "", EditorStyles.helpBox);

            // Draw header
            DrawNodeHeader(nodeRect);

            int inputPortCount = 0;
            int outputPortCount = 0;

            foreach (NodePort port in NodeRegistry.Ports.Values)
            {
                Node node = NodeRegistry.GetNode(port.OwnerNode.ID);
                if(node == this)
                {
                    Vector2 center = port.OwnerNode.nodeTransform.Rect.center;
                    center.y -= 15f;
                    if (port.PortType == NodePort.IO.Input)
                    {
                        center.x -= port.OwnerNode.nodeTransform.Size.x / 2;
                        port.DrawPort(center, inputPortCount);
                        inputPortCount++;
                    }
                    else if (port.PortType == NodePort.IO.Output)
                    {
                        center.x += port.OwnerNode.nodeTransform.Size.x / 2;
                        port.DrawPort(center, outputPortCount);
                        outputPortCount++;
                    }
                }
            }

            Handles.EndGUI();
        }

        private void DrawNodeHeader(Rect nodeRect)
        {
            float headerHeight = 22f;
            Rect headerRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, headerHeight);

            // Draw background for the header
            EditorGUI.DrawRect(headerRect, new Color(0.2f, 0.2f, 0.2f, 1f));

            // Center the label
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            GUI.Label(headerRect, NodeDisplayName, labelStyle);
        }

        public class Attributes
        {
            public Color nodeColor = Color.white;
            public Color outPortColor = Color.white;
            public Color inPortColor = Color.white;
            public Color runningColor = Color.cyan;
            public float portRadius = 5f;
            public int width = 300;

            public Attributes() { }
        }

        #region Obsolete
        [Obsolete("Drawing ports in the window now.")]
        private void DrawPortDisc(NodePort port, Vector2 scrollPos)
        {
            Handles.color = (port.PortType == NodePort.IO.Input) ? NodeStyling.inputPortColor : NodeStyling.outputPortColor;
            Handles.DrawSolidDisc(port.Position, Vector3.forward, attributes.portRadius);
            port.PositionInCanvas = port.Position + scrollPos;
        }
        #endregion
    }
}