using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace VNode
{
    [Serializable]
    public class NodePort
    {
        public string ID { get; } = Guid.NewGuid().ToString();
        public enum IO { Input, Output }
        public IO PortType { get; }
        public Type ValueType { get; }
        public Node OwnerNode { get; }
        public string Name { get; }
        public Vector2 Position { get; set; } = Vector2.zero;
        public Vector2 PositionInCanvas { get; set; } = Vector2.zero;
        public bool ShowProperty { get; set; } = true;

        public NodePort(Node ownerNode, string name, IO portType, Type valueType)
        {
            OwnerNode = ownerNode;
            PortType = portType;
            ValueType = valueType;
            Name = name;
        }

        public virtual void Connect(NodePort other)
        {
            
        }

        public virtual void Disconnect(NodePort other)
        {
            
        }

        public bool IsPointerInside(Vector2 mousePosInCanvas) 
        {
            return Vector2.Distance(Position, mousePosInCanvas) < OwnerNode.attributes.portRadius;
        }

        public virtual object GetValue() => null;

        public virtual void SetValue(object value) { }

        public virtual void DrawGUIElements() { }

        public void DrawPort(Vector2 position, int index)
        {
            Handles.BeginGUI();

            Handles.color = (PortType == IO.Output) ? NodeStyling.outputPortColor : NodeStyling.inputPortColor;

            position.y += 25f * index;

            Position = position;

            Handles.DrawSolidDisc(Position, Vector3.forward, OwnerNode.attributes.portRadius);

            DrawPortValueAt(this, position);

            Handles.EndGUI();
        }

        // Draw port value at a given absolute position
        private void DrawPortValueAt(NodePort port, Vector2 position)
        {
            float xPadding = 10f;
            float yPadding = -8f;

            if(PortType == IO.Output)
            {
                xPadding = -70f;
            }

            if (!port.ShowProperty) return;
#if UNITY_EDITOR
            if (port is NodePort<float> fPort)
                fPort.SetValue(EditorGUI.FloatField(new Rect(position.x + xPadding, position.y + yPadding, 50, 16), (float)fPort.GetValue()));
            else if (port is NodePort<int> iPort)
                iPort.SetValue(EditorGUI.IntField(new Rect(position.x + xPadding, position.y + yPadding, 50, 16), (int)iPort.GetValue()));
            else if (port is NodePort<string> sPort)
                sPort.SetValue(EditorGUI.TextField(new Rect(position.x + xPadding, position.y + yPadding, 80, 16), (string)sPort.GetValue()));
#endif
        }

        public void DrawConnectionLineWhenDragging(Vector2 mousePos)
        {
            Handles.BeginGUI();

            Vector3 startPos = new(Position.x, Position.y, 0);
            Vector3 endPos = new(mousePos.x, mousePos.y, 0);

            Handles.DrawBezier(
                startPos,
                endPos,
                startPos + Vector3.right * 50f,
                endPos + Vector3.left * 50f,
                NodeStyling.outputPortColor,
                null,
                3f
            );

            Handles.EndGUI();
        }

        #region Obsolete
        [Obsolete("Drawing lines in node editor window")]
        public void DrawConnectionLine(NodePort toPort)
        {
            if (toPort != this)
            {
                Handles.BeginGUI();

                Vector3 startPos = new(Position.x, Position.y, 0);
                Vector3 endPos = new(toPort.Position.x, toPort.Position.y, 0);

                Handles.DrawBezier(
                    startPos,
                    endPos,
                    startPos + Vector3.right * 50f,
                    endPos + Vector3.left * 50f,
                    NodeStyling.outputPortColor,
                    null,
                    3f
                );

                Handles.EndGUI();
            }
        }
        #endregion
    }

    /// <summary>
    /// Generic, strongly typed port base class.
    /// </summary>
    public class NodePort<T> : NodePort
    {
        /// <summary>
        /// The stored value (float, int, string, etc.)
        /// </summary>
        protected T Value { get; set; }

        public NodePort(Node ownerNode, string name,IO portType)
            : base(ownerNode, name, portType, typeof(T))
        {
        }

        public override void SetValue(object value)
        {
            if (value is T t)
                Value = t;
            else
                Debug.LogWarning($"Invalid value type '{value?.GetType().Name}' for port expecting '{typeof(T).Name}'.");
        }

        public override object GetValue() => Value;
    }
}