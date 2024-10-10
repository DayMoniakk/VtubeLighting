using System.Collections.Generic;

namespace VtubeLighting.Serialization
{
    [System.Serializable]
    public class SaveData
    {
        public int resolutionWidth;
        public int resolutionHeight;
        public int maxFps;
        public float lightingRefreshRate;
        public List<LightingPreset> lightingPresets;

        public SaveData()
        {
            resolutionWidth = 1280;
            resolutionHeight = 720;
            maxFps = 60;
            lightingRefreshRate = 1.0f;
            lightingPresets = new List<LightingPreset>();
        }
    }
}