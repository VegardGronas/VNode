using System.Collections.Generic;
using UnityEngine;

namespace VNode
{
    [RequireComponent (typeof (StartNode))]
    public class NodeCollector : MonoBehaviour
    {
        public List<Node> nodes = new();
        [HideInInspector] public bool hasInitialized = false;

        public void Initialize()
        {
            nodes.Clear();
            Node[] childNodes = GetComponentsInChildren<Node>();
            nodes.AddRange(childNodes);
            hasInitialized = true;
        }

        public void UpdateList()
        {
            Node[] childNodes = GetComponentsInChildren<Node>();
           
            foreach(Node node in childNodes)
            {
                if(!nodes.Contains(node)) nodes.Add(node);  
            }
        }

        public void DeleteNode(Node node)
        {
            if (nodes.Contains(node))
            {
                node.RemoveConnections();
                nodes.Remove(node);
                DestroyImmediate(node);
            }
        }

        public void Run()
        {
            foreach (Node node in nodes)
            {
                if (node is StartNode)
                {
                    node.TriggerNext(node.GetPorts(NodePort.IO.Output)[0]);
                    return;
                }
            }

            Debug.LogWarning("Could not find a stat node. Unable to run");
        }
    }
}