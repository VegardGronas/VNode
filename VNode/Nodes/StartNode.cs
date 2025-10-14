using System.Collections.Generic;
using UnityEngine;
using VNode;

namespace VNode
{
    public class StartNode : Node
    {
        public override string NodeDisplayName => "Start";

        protected override void OnInitialize()
        {
            NodePort outPort = new(this, "Start", NodePort.IO.Output, null);
            NodeRegistry.Register(outPort);

            Debug.Log("Added port: " + outPort.Name);
        }

        public override void Execute(HashSet<Node> visited)
        {

        }
    }
}