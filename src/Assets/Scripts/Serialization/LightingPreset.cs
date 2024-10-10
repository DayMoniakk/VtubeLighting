namespace VtubeLighting.Serialization
{
    [System.Serializable]
    public class LightingPreset
    {
        public string name = "Unamed Preset";
        public float opacity = 0.0f;
        public float intensity = 1.0f;
        public float updateRate = 8.0f;

        public LightingPreset(string name, float opacity, float intensity, float updateRate)
        {
            this.name = name;
            this.opacity = opacity;
            this.intensity = intensity;
            this.updateRate = updateRate;
        }

        public static LightingPreset GetDefault() => new LightingPreset("Default", 0.0f, 1.0f, 8.0f);
    }
}