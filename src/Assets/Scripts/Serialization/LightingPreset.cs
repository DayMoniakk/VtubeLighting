namespace VtubeLighting.Serialization
{
    [System.Serializable]
    public class LightingPreset
    {
        public string name = "Unamed Preset";
        public int resolutionIndex = 0;
        public float opacity = 0.0f;
        public float brightness = 1.0f;
        public float updateRate = 8.0f;

        public LightingPreset(string name, int resolutionIndex, float opacity, float brightness, float updateRate)
        {
            this.name = name;
            this.resolutionIndex = resolutionIndex;
            this.opacity = opacity;
            this.brightness = brightness;
            this.updateRate = updateRate;
        }

        public static LightingPreset GetDefault() => new LightingPreset("Default", 0, 0.0f, 1.0f, 8.0f);
    }
}