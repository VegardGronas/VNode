using System.Collections.Generic;
using UnityEngine;

namespace VNode
{
    public class NodeRegistry : MonoBehaviour
    {
        public static readonly Dictionary<string, Node> Nodes = new();
        public static readonly Dictionary<string, NodePort> Ports = new();
        public static readonly List<NodeConnection> Connections = new();

        public static Node GetNode(string id) => Nodes.TryGetValue(id, out var n) ? n : null;
        public static NodePort GetPort(string id) => Ports.TryGetValue(id, out var p) ? p : null;

        public static void Register(Node node)
        {
            if (!Nodes.ContainsKey(node.ID))
                Nodes[node.ID] = node;
        }

        public static void Register(NodePort port)
        {
            if (!Ports.ContainsKey(port.ID))
                Ports[port.ID] = port;
        }

        public static void AddConnection(NodeConnection connection)
        {
            if (!Connections.Contains(connection))
                Connections.Add(connection);
        }
    }
}