using UnityEngine;

namespace VNode
{
    public static class NodeStyling
    {
        // Node background
        public static Color defaultColor = new Color(0.18f, 0.18f, 0.18f, 1f);  // dark gray
        public static Color selectedColor = new Color(0.26f, 0.48f, 0.85f, 1f);  // soft blue highlight

        // Ports
        public static Color inputPortColor = new Color(0.2f, 0.9f, 0.9f, 1f);     // cyan
        public static Color outputPortColor = new Color(0.9f, 0.3f, 0.9f, 1f);     // magenta/purple

        // Background + grid
        public static Color backgroundColor = new Color(0.12f, 0.12f, 0.12f, 1f);  // dark editor background
        public static Color gridColor = new Color(0.25f, 0.25f, 0.25f, 1f);  // subtle gray grid
    }
}