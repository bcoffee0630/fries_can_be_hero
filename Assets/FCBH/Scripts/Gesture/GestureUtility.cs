using UnityEngine;

namespace FCBH
{
    public static class GestureUtility
    {
        public static Rect GetDrawArea(float x, float y, float width, float height)
        {
            return new Rect(
                Screen.width * x,
                Screen.height * y,
                Screen.width * width,
                Screen.height * height
            );
        }
    }
}