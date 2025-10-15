using UnityEditor;
using UnityEngine;

namespace VNode
{
    public partial class NodeEditorWindow : EditorWindow
    {
        private void OnInputActionPressed(NodeInputManager.InputAction action)
        {
            switch (action.Type)
            {
                case NodeInputManager.InputType.LeftClick:
                    LeftMouseDown();
                    break;
                case NodeInputManager.InputType.DeleteClick:
                    if (selectedNode != null)
                    {
                        if (selectedNode is StartNode) break;
                        nodeManager.DeleteNode(selectedNode);
                    }
                    selectedNode = null;
                    break;
                case NodeInputManager.InputType.ContextClick:
                    contextMenu.OpenContextMenu(mousePosition, nodeManager);
                    break;
            }
        }

        private void OnInputActionReleased(NodeInputManager.InputAction action)
        {
            switch (action.Type)
            {
                case NodeInputManager.InputType.LeftClick:
                    LeftMouseUp();
                    break;
            }
        }

        private void OnDrag(NodeInputManager.DragInputAction action)
        {
            if (input.IsPressed(NodeInputManager.InputType.MiddleClick))
            {
                Pan(action.DragValues.delta);
            }
            else if (selectedNode != null)
                DragNode();
        }
    }
}