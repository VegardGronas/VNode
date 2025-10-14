using UnityEngine;

namespace VNode
{
    public class NodeConnection
    {
        public string FromNodeID { get; }
        public string FromPortID { get; }
        public string ToNodeID { get; }
        public string ToPortID { get; }

        public NodeConnection(string fromNodeID, string fromPortID, string toNodeID, string toPortID)
        {
            FromNodeID = fromNodeID;
            FromPortID = fromPortID;
            ToNodeID = toNodeID;
            ToPortID = toPortID;
        }
    }
}