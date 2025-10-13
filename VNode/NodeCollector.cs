using System.Collections.Generic;
using UnityEngine;

namespace VNode
{
    [RequireComponent (typeof (StartNode))]
    public class NodeCollector : MonoBehaviour
    {
        public List<Node> nodes = new();

        public void Initialize()
        {
            nodes.Clear();
            Node[] childNodes = GetComponentsInChildren<Node>();
            nodes.AddRange(childNodes);
        }

        public void UpdateList()
        {
            List<Node> updatedList = new();
            foreach (Node node in nodes)
            {
                if (node == null) continue;
                updatedList.Add(node);
            }
            Node[] childNodes = GetComponentsInChildren<Node>();
            foreach (Node node in childNodes)
            {
                if (updatedList.Contains(node)) continue;
                updatedList.Add(node);
            }
            nodes = updatedList;
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