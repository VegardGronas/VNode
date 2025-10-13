using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace VNode
{
    public class AdditionNode : Node
    {
        protected override void OnInitialize()
        {
            NodePort<float> outputPort = new(this, "Execute", NodePort.IO.Output);
            NodePort inputPort = new(this, "Input", NodePort.IO.Input, null);
            NodePort<float> inputValue1 = new(this, "Value 1", NodePort.IO.Input);
            NodePort<float> inputValue2 = new(this, "Value 2", NodePort.IO.Input);
            ports.Add("Execute", outputPort);
            ports.Add("Input", inputPort);
            ports.Add("InputValue1", inputValue1);
            ports.Add("InputValue2", inputValue2);
        }

        public override void Execute(HashSet<Node> visited)
        {
            NodePort val1Port = ports["InputValue1"];
            if(val1Port != null)
            {
                if (val1Port.connections.Count > 0)
                {
                    val1Port.SetValue(val1Port.connections[0].GetValue());
                }
            }

            NodePort val2Port = ports["InputValue2"];
            if (val2Port != null)
            {
                if (val2Port.connections.Count > 0)
                {
                    val2Port.SetValue(val2Port.connections[0].GetValue());
                }
            }

            float value1 = (float)ports["InputValue1"].GetValue();
            float value2 = (float)ports["InputValue2"].GetValue();

            float res = value1 + value2;

            ports["Execute"].SetValue(res);

            TriggerNext(ports["Execute"], visited);
        }

        public override void NewPortConnection(NodePort port)
        {
            base.NewPortConnection(port);

            Debug.Log("Addition node got new connection");

            if (ports["InputValue1"].connections.Contains(port))
            {
                ports["InputValue1"].ShowProperty = false;
            }
            else if (ports["InputValue2"].connections.Contains(port))
            {
                ports["InputValue2"].ShowProperty = false;
            }
        }
    }
}