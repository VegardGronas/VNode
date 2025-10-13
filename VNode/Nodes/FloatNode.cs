using UnityEngine;

namespace VNode
{
    public class FloatNode : Node
    {
        public override void Initialize()
        {
            NodePort<float> outPut = new(this, "Value", NodePort.IO.Output);
            ports.Add("OutExecute", outPut);
        }
    }
}