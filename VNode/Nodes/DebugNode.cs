using System.Collections.Generic;
using UnityEngine;

namespace VNode
{
    public class DebugNode : Node
    {
        public override string NodeDisplayName => "Debug";

        protected override void OnInitialize()
        {
            NodePort inPort = new(this, "Input", NodePort.IO.Input, null);
            NodePort outPort = new(this, "Output", NodePort.IO.Output, null);
            NodeRegistry.Register(inPort);
            NodeRegistry.Register(outPort);
        }

        public override void Execute(HashSet<Node> visited)
        {

        }
    }
}