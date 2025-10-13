using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace VNode
{
    public class NodePort
    {
        public enum IO { Input, Output }
        public IO PortType { get; }
        public Type ValueType { get; }
        public Node OwnerNode { get; }
        public string Name { get; }
        public Vector2 Position { get; set; } = Vector2.zero;
        public Vector2 PositionInCanvas { get; set; } = Vector2.zero;
        
        public List<NodePort> connections = new();
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
            if(connections.Contains(other))
            {
                Debug.Log("Already connected");
                return;
            }
            connections.Add(other);
            other.connections.Add(this);

            OwnerNode.NewPortConnection(other);
            other.OwnerNode.NewPortConnection(this);
        }

        public virtual void Disconnect(NodePort other)
        {
            if (connections.Contains(other))
            {
                connections.Remove(other);
                other.connections.Remove(this);
            }
        }

        public bool IsPointerInside(Vector2 mousePosInCanvas) 
        {
            return Vector2.Distance(PositionInCanvas, mousePosInCanvas) < OwnerNode.attributes.portRadius;
        }

        public virtual object GetValue() => null;

        public virtual void SetValue(object value) { }

        public virtual void DrawGUIElements() { }

        public void DrawConnectionLine()
        {
            foreach(NodePort port in connections)
            {
                if(port != this)
                {
                    Vector3 startPos = new(Position.x, Position.y, 0);
                    Vector3 endPos = new(port.Position.x, port.Position.y, 0);

                    Handles.DrawBezier(
                        startPos,
                        endPos,
                        startPos + Vector3.right * 50f,
                        endPos + Vector3.left * 50f,
                        NodeStyling.outputPortColor,
                        null,
                        3f
                    );
                }
            }
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