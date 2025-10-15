using System;
using System.Collections.Generic;
using UnityEngine;

namespace VNode
{
    [RequireComponent (typeof (StartNode))]
    public class NodeManager : MonoBehaviour
    {
        public string ID { get; } = Guid.NewGuid().ToString();
        public void Initialize()
        {
            NodeRegistry.NodeManagerInstanceID = ID;
            Node[] nodes = GetComponentsInChildren<Node>();
            foreach (Node node in nodes)
            {
                if (node is StartNode) return;
            }
            CreateStartNode();
        }

        public void Load()
        {
            Node[] nodes = GetComponentsInChildren<Node>();
            foreach (Node node in nodes)
            {
                NodeRegistry.Register(node);
                node.Initialize();
            }
        }

        private void CreateStartNode()
        {
            StartNode node = gameObject.AddComponent<StartNode>();
            node.Initialize();
        }

        public void UpdateList()
        {
            
        }

        public void DeleteNode(Node node)
        {
            
        }

        public void ResetGraph()
        {
            
        }

        public void Run()
        {
            
        }
    }
}