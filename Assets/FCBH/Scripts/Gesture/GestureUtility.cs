using UnityEngine;

namespace FCBH
{
    public static class GestureUtility
    {
        private static string[] SupportedGestures = new[]
        {
            "Circle",
            "Triangle",
            "Square",
            "M",
            "X",
            "Tick",
            "Thunder",
            "Up",
            "Down",
            "Left",
            "Right",
        };
        
        public static Rect GetDrawArea(float x, float y, float width, float height)
        {
            return new Rect(
                Screen.width * x,
                Screen.height * y,
                Screen.width * width,
                Screen.height * height
            );
        }
        
        public static string GetRandomGesture() => SupportedGestures[Random.Range(0, SupportedGestures.Length)];
    }
}