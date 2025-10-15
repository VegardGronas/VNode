using System;
using UnityEngine;

namespace VNode
{
    public static class VNodeSaving
    {
        public static event Action<string> OnSavingEvent;
        public static event Action<string> OnLoadingEvent;

        public static void Save()
        {
            OnSavingEvent?.Invoke(Application.dataPath + "/VNode/Save/registry.json");
        }

        public static void Load() 
        {
            OnLoadingEvent?.Invoke(Application.dataPath + "/VNode/Save/registry.json");
        }
    }
}