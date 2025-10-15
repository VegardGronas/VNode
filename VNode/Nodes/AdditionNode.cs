using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace VNode
{
    public class AdditionNode : Node
    {
        public override string NodeDisplayName => "Addition";

        protected override void OnInitialize()
        {
            NodePort<float> outputPort = new(this, "Execute", NodePort.IO.Output);
            NodePort inputPort = new(this, "Input", NodePort.IO.Input, null);
            NodePort<float> inputValue1 = new(this, "Value 1", NodePort.IO.Input);
            NodePort<float> inputValue2 = new(this, "Value 2", NodePort.IO.Input);
            NodeRegistry.Register(outputPort);
            NodeRegistry.Register(inputPort);   
            NodeRegistry.Register(inputValue1);
            NodeRegistry.Register(inputValue2);
        }

        public override void Execute(HashSet<Node> visited)
        {
            
        }

        public override void NewPortConnection(NodePort port)
        {
            base.NewPortConnection(port);
        }
    }
}