using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VNode
{
    public class NodeContextMenu
    {
        public void OpenContextMenu(Vector2 mousePosition, NodeManager nodeCollector)
        {
            GenericMenu menu = new GenericMenu();

            var nodeTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
                })
                .Where(t => typeof(Node).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            if (nodeTypes.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No node types found"));
            }
            else
            {
                foreach (var type in nodeTypes)
                {
                    string name = ObjectNames.NicifyVariableName(type.Name);
                    menu.AddItem(new GUIContent(name), false, () => CreateNode(type, mousePosition, nodeCollector));
                }
            }

            menu.ShowAsContext();
        }

        private void CreateNode(Type nodeType, Vector2 position, NodeManager nodeCollector)
        {
            if (nodeCollector == null)
            {
                Debug.LogError("No NodeCollector found — cannot create node!");
                return;
            }

            // Create the node as a MonoBehaviour component
            Node newNode = (Node)nodeCollector.gameObject.AddComponent(nodeType);
            newNode.nodeTransform.Position = position;

            newNode.Initialize();

            Debug.Log($"Created node of type {nodeType.Name} at {position}");
        }
    }
}