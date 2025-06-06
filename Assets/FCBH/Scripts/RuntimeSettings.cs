using UnityEngine;

namespace FCBH
{
    public class RuntimeSettings
    {
        #region Static methods

        [RuntimeInitializeOnLoadMethod]
        private static void OptimizeFrame()
        {
            Application.targetFrameRate = 0;
            Screen.SetResolution(
                Screen.currentResolution.width,
                Screen.currentResolution.height,
                FullScreenMode.FullScreenWindow,
                new RefreshRate
                {
                    numerator = 120,
                    denominator = 1,
                }
            );
        }

        #endregion
    }
}
