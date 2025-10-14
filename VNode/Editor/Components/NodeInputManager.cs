using System;
using System.Collections.Generic;
using UnityEngine;
using static VNode.NodeInputManager;

namespace VNode
{
    public class NodeInputManager
    {
        public event Action<InputAction> OnActionPressed;
        public event Action<InputAction> OnActionReleased;
        public event Action<DragInputAction> OnDragging;

        private readonly Dictionary<InputType, InputAction> actions = new();
        public DragInputAction dragInputAction { get; private set; }

        private const float DRAG_THRESHOLD = 4f; // pixels before we call it a drag

        public NodeInputManager()
        {
            foreach (InputType type in Enum.GetValues(typeof(InputType)))
                actions[type] = new InputAction(type);

            dragInputAction = new();
        }

        public void Update(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    dragInputAction.StartDragPosition = e.mousePosition;
                    dragInputAction.IsDragging = false;
                    HandleMouseDown(e);
                    break;

                case EventType.MouseUp:
                    HandleMouseUp(e);
                    if (dragInputAction.IsDragging)
                    {
                        dragInputAction.EndDragPosition = e.mousePosition;
                        dragInputAction.IsDragging = false;
                    }
                    break;

                case EventType.MouseDrag:
                    HandleDrag(e);
                    break;

                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.Delete)
                        TriggerAction(InputType.DeleteClick);
                    break;
            }
        }

        private void HandleMouseDown(Event e)
        {
            switch (e.button)
            {
                case 0: SetActionState(InputType.LeftClick, true); break;
                case 1: SetActionState(InputType.ContextClick, true); break;
                case 2: SetActionState(InputType.MiddleClick, true); break;
            }
        }

        private void HandleMouseUp(Event e)
        {
            switch (e.button)
            {
                case 0: SetActionState(InputType.LeftClick, false); break;
                case 1: SetActionState(InputType.ContextClick, false); break;
                case 2: SetActionState(InputType.MiddleClick, false); break;
            }
        }

        private void HandleDrag(Event e)
        {
            Vector2 currentPos = e.mousePosition;
            Vector2 delta = currentPos - dragInputAction.StartDragPosition;

            dragInputAction.DragValues = (delta, currentPos);

            if (!dragInputAction.IsDragging && delta.magnitude > DRAG_THRESHOLD)
            {
                dragInputAction.IsDragging = true;
            }

            if (dragInputAction.IsDragging)
            {
                dragInputAction.StartDragPosition = currentPos; // for smooth continuous dragging
                OnDragging?.Invoke(dragInputAction);
            }
        }

        private void SetActionState(InputType type, bool pressed)
        {
            var action = actions[type];

            if (action.IsPressed == pressed)
                return;

            action.IsPressed = pressed;

            if (pressed)
                OnActionPressed?.Invoke(action);
            else
                OnActionReleased?.Invoke(action);
        }

        private void TriggerAction(InputType type)
        {
            OnActionPressed?.Invoke(actions[type]);
        }

        public bool IsPressed(InputType type) => actions.ContainsKey(type) && actions[type].IsPressed;
        public bool IsCtrl => Event.current != null && Event.current.control;
        public bool IsShift => Event.current != null && Event.current.shift;
        public bool IsAlt => Event.current != null && Event.current.alt;

        public enum InputType { LeftClick, ContextClick, DeleteClick, MiddleClick }

        public class InputAction
        {
            public InputType Type { get; }
            public bool IsPressed { get; internal set; }
            public InputAction(InputType type) => Type = type;
        }

        public class DragInputAction
        {
            public Vector2 StartDragPosition { get; internal set; }
            public Vector2 EndDragPosition { get; internal set; }
            public (Vector2 delta, Vector2 position) DragValues {  get; internal set; }
            public bool IsDragging { get; internal set; }
            public DragInputAction() { }
        }
    }
}