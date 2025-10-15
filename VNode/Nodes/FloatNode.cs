using UnityEngine;

namespace VNode
{
    public class FloatNode : Node
    {
        public override string NodeDisplayName => "Float";

        protected override void OnInitialize()
        {
            NodePort<float> outPut = new(this, "Value", NodePort.IO.Output);
            NodeRegistry.Register(outPut);
        }
    }
}