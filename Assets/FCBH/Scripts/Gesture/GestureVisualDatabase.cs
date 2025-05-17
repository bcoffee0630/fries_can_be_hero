using System;
using System.Collections.Generic;
using UnityEngine;

namespace FCBH
{
    [CreateAssetMenu(menuName = "FCBH/Gesture/Create VisualDatabase")]
    public class GestureVisualDatabase : ScriptableObject
    {
        #region Structure

        [Serializable]
        public class GestureVisual
        {
            public string gestureName;
            public Sprite gestureTexture;
        }

        #endregion
        
        [SerializeField] private List<GestureVisual> gestureVisuals = new();
        
        public GestureVisual GetGestureVisual(string gestureName) => gestureVisuals.Find(x => x.gestureName == gestureName);
    }
}
