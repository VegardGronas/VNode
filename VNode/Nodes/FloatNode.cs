using UnityEngine;

namespace VNode
{
    public class FloatNode : Node
    {
        protected override void OnInitialize()
        {
            NodePort<float> outPut = new(this, "Value", NodePort.IO.Output);
            ports.Add("OutExecute", outPut);
        }
    }
}