using System.Collections.Generic;
using UnityEngine;
using VNode;

namespace VNode
{
    public class StartNode : Node
    {
        protected override void OnInitialize()
        {
            NodePort outPort = new(this, "Start", NodePort.IO.Output, null);
            ports.Add(outPort.Name, outPort);

            Debug.Log("Added port: " + outPort.Name);
        }

        public override void Execute(HashSet<Node> visited)
        {
            TriggerNext(ports["Start"], visited);
        }
    }
}