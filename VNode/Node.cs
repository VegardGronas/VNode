using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VNode
{
    public class Node : MonoBehaviour
    {
        public Attributes attributes = new();
        public Dictionary<string, NodePort> ports = new();
        public NodeTransform nodeTransform = new(new(200, 200), new(200, 100));
        [HideInInspector] public bool hasInitialized = false;
        public bool IsSelected { get; set; } = false;

        public void Initialize(bool force = false) { if (hasInitialized && !force) return; ports.Clear(); OnInitialize(); hasInitialized = true; }
        
        protected virtual void OnInitialize() { }

        public virtual void Execute(HashSet<Node> visited) { }

        public void TriggerNext(NodePort port, HashSet<Node> visited = null)
        {
            visited ??= new HashSet<Node>();
            if (!visited.Add(this))
                return; // already executed in this chain, stop recursion

            foreach (NodePort p in port.connections)
            {
                if (p.OwnerNode != this)
                    p.OwnerNode.Execute(visited);
            }
        }

        public virtual void NewPortConnection(NodePort port) { }

        public List<NodePort> GetPorts(NodePort.IO portType)
        {
            List<NodePort> result = new();

            foreach (var port in ports.Values) // iterate over the dictionary's values
            {
                if (port.PortType == portType)
                    result.Add(port);
            }

            return result;
        }

        public void RemoveConnections()
        {
            foreach (NodePort port in ports.Values)
            {
                // Make a copy of the list since Disconnect modifies it
                foreach (var conn in port.connections.ToList())
                {
                    port.Disconnect(conn);
                }
            }
        }

        public void DrawNode(Vector2 scrollPos)
        {
            float portStartGap = 30f;
            float portGap = 25f;
            float portPadding = 10f;
            float valueLift = 6f;

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

            // Draw input ports
            for (int i = 0; i < GetPorts(NodePort.IO.Input).Count; i++)
            {
                NodePort port = GetPorts(NodePort.IO.Input)[i];

                Vector2 pos = nodeRect.position;
                pos.y += portStartGap + portGap * i;
                pos.x += portPadding;

                port.Position = pos;
                DrawPortDisc(port, scrollPos);
                DrawPortValueAt(port, new Vector2(port.Position.x + 10, port.Position.y - valueLift));
            }

            // Draw output ports
            for (int i = 0; i < GetPorts(NodePort.IO.Output).Count; i++)
            {
                NodePort port = GetPorts(NodePort.IO.Output)[i];

                Vector2 pos = nodeRect.position;
                pos.x += nodeRect.width - portPadding;
                pos.y += portStartGap + portGap * i;

                port.Position = pos;
                DrawPortDisc(port, scrollPos);
                port.DrawConnectionLine();
                DrawPortValueAt(port, new Vector2(port.Position.x - 65f, port.Position.y - valueLift));
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

            GUI.Label(headerRect, GetType().ToString() ?? "Node", labelStyle);
        }

        private void DrawPortDisc(NodePort port, Vector2 scrollPos)
        {
            Handles.color = (port.PortType == NodePort.IO.Input) ? NodeStyling.inputPortColor : NodeStyling.outputPortColor;
            Handles.DrawSolidDisc(port.Position, Vector3.forward, attributes.portRadius);
            port.PositionInCanvas = port.Position + scrollPos;
        }

        // Draw port value at a given absolute position
        private void DrawPortValueAt(NodePort port, Vector2 position)
        {
            if(!port.ShowProperty) return;
#if UNITY_EDITOR
            if (port is NodePort<float> fPort)
                fPort.SetValue(EditorGUI.FloatField(new Rect(position.x, position.y, 50, 16), (float)fPort.GetValue()));
            else if (port is NodePort<int> iPort)
                iPort.SetValue(EditorGUI.IntField(new Rect(position.x, position.y, 50, 16), (int)iPort.GetValue()));
            else if (port is NodePort<string> sPort)
                sPort.SetValue(EditorGUI.TextField(new Rect(position.x, position.y, 80, 16), (string)sPort.GetValue()));
#endif
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
    }
}