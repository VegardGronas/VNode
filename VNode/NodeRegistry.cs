using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VNode
{
    public static class NodeRegistry
    {
        public static string NodeManagerInstanceID = string.Empty;

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

        // Save registry to disk
        public static void SaveRegistry(string filePath = null)
        {
            List<NodeConnection> connections = new();
            connections.AddRange(Connections); 

            NodeRegistryData data = new(NodeManagerInstanceID, connections);

            string json = JsonUtility.ToJson(data, true);

            filePath ??= Application.dataPath + "/VNode/Save/registry.json";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, json);

            Debug.Log($"[VNode] Registry saved to {filePath}");
        }

        // Load registry from disk
        public static void LoadRegistry(string filePath = null)
        {
            filePath ??= Application.dataPath + "/VNode/Save/registry.json";
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("[VNode] No registry file found to load.");
                return;
            }

            string json = File.ReadAllText(filePath);
            NodeRegistryData data = JsonUtility.FromJson<NodeRegistryData>(json);

            // Clear existing data
            Nodes.Clear();
            Ports.Clear();
            Connections.Clear();

            NodeManagerInstanceID = data.nodeManagerID;

            NodeManager[] managers = GameObject.FindObjectsByType<NodeManager>(FindObjectsSortMode.None);
            foreach (NodeManager manager in managers)
            {
                if(manager.ID == NodeManagerInstanceID)
                {
                    manager.Load();
                    break;
                }
            }

            Connections.AddRange(data.connections);

            Debug.Log($"[VNode] Registry loaded with {Nodes.Count} nodes, {Ports.Count} ports, {Connections.Count} connections.");
        }
    }

    [Serializable]
    public class NodeRegistryData
    {
        public string nodeManagerID = string.Empty;
        public List<NodeConnection> connections = new();

        public NodeRegistryData(string nodeManagerId, List<NodeConnection> connections)
        {
            this.nodeManagerID = nodeManagerId;
            this.connections = connections;
        }
    }
}