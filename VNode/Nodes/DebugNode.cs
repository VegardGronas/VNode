using System.Collections.Generic;
using UnityEngine;

namespace VNode
{
    public class DebugNode : Node
    {
        protected override void OnInitialize()
        {
            NodePort inPort = new(this, "Input", NodePort.IO.Input, null);
            NodePort outPort = new(this, "Output", NodePort.IO.Output, null);
            ports.Add("Input", inPort);
            ports.Add("Output", outPort);
        }

        public override void Execute(HashSet<Node> visited)
        {
            if (ports["Input"].connections.Count > 0)
            {
                Debug.Log(ports["Input"].connections[0].GetValue());
                
                TriggerNext(ports["Input"], visited);
            }
        }
    }
}