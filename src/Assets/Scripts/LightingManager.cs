using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VtubeLighting.Utility;

namespace VtubeLighting.Core
{
    public class LightingManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RawImage avatarDisplayOutput;
        [SerializeField] private RawImage lightingDisplay;
        [SerializeField] private GameObject tempBackground;
        [SerializeField] private Material lightingMaterial;
        [SerializeField] private Button btnStartCapture;
        [SerializeField] private Button btnStopCapture;
        [SerializeField] private Slider sliderLightOpacity;
        [SerializeField] private TMP_InputField sliderLightOpacityField;
        [SerializeField] private Slider sliderLightIntensity;
        [SerializeField] private TMP_InputField sliderLightIntensityField;
        [SerializeField] private Slider sliderLightUpdate;
        [SerializeField] private TMP_InputField sliderLightUpdateField;
        [SerializeField] private SpoutInputManager avatarInputManager;
        [SerializeField] private SpoutInputManager lightInputManager;

        private bool lightingEnabled = false;
        private Color32[] newPixels;
        private Texture2D previousOutputTexture;
        private Texture2D lightingTextureResized; // The webcam output is copied to this texture
        private readonly Vector2Int lightingTextureSize = new(32, 18); // The width and height of the lighting texture used to light up the avatar (we don't need a huge resolution to get lighting and it will help with performance)
        private float lightingUpdateRate = 8; // How many times per second should we update the lighting texture
        private float nextTimeToUpdateLighting; // Holds the next time to update the lighting texture

        public float LightingOpacity => sliderLightOpacity.value;
        public float LightingIntensity => sliderLightIntensity.value;
        public float LightingUpdateRate => lightingUpdateRate;

        public bool IsLightingEnabled => lightingEnabled;

        public void SetLightingOpacity(float value) => sliderLightOpacity.value = value;
        public void SetLightingIntensity(float value) => sliderLightIntensity.value = value;
        public void SetLightingUpdateRate(float value)
        {
            lightingUpdateRate = value;
            sliderLightUpdate.value = value;
        }

        public delegate void OnLightingStateChanged(bool state);
        public OnLightingStateChanged onLightingStateChanged;

        private void Start()
        {
            avatarInputManager.onSourceUpdated += RefreshButtons;
            lightInputManager.onSourceUpdated += RefreshButtons;

            sliderLightOpacity.onValueChanged.AddListener(value =>
           {
               avatarDisplayOutput.material.SetFloat("_LightOpacity", value);
               sliderLightOpacityField.text = value.ToString("F2");
           });
            sliderLightOpacityField.onValueChanged.AddListener(value => { sliderLightOpacity.value = float.Parse(value); });
            sliderLightOpacityField.onEndEdit.AddListener(value => { sliderLightOpacityField.text = sliderLightOpacity.value.ToString("F2"); });

            sliderLightIntensity.onValueChanged.AddListener(value =>
            {
                avatarDisplayOutput.material.SetFloat("_LightIntensity", value);
                sliderLightIntensityField.text = value.ToString("F2");
            });
            sliderLightIntensityField.onValueChanged.AddListener(value => { sliderLightIntensity.value = float.Parse(value); });
            sliderLightIntensityField.onEndEdit.AddListener(value => { sliderLightIntensityField.text = sliderLightIntensity.value.ToString("F2"); });

            sliderLightUpdate.onValueChanged.AddListener(value =>
            {
                lightingUpdateRate = value;

                sliderLightUpdateField.text = value.ToString();
            });
            sliderLightUpdateField.onValueChanged.AddListener(value => { sliderLightUpdate.value = float.Parse(value); });
            sliderLightUpdateField.onEndEdit.AddListener(value => { sliderLightUpdateField.text = sliderLightUpdate.value.ToString(); });

            btnStartCapture.onClick.AddListener(() => { SetLighting(true); });
            btnStopCapture.onClick.AddListener(() => { SetLighting(false); });

            SetLighting(false);
        }

        private void OnDestroy()
        {
            avatarInputManager.onSourceUpdated -= RefreshButtons;
            lightInputManager.onSourceUpdated -= RefreshButtons;
        }

        private void Update()
        {
            if (lightingEnabled)
            {
                if (Time.time >= nextTimeToUpdateLighting)
                {
                    nextTimeToUpdateLighting = Time.time + 1f / lightingUpdateRate;
                    UpdateLighting();
                }
            }
        }

        private void SetLighting(bool state)
        {
            lightingEnabled = state;
            onLightingStateChanged?.Invoke(state);
            RefreshButtons();

            if (state)
            {
                if (lightingTextureResized != null)
                {
                    Destroy(lightingTextureResized);
                }

                lightingTextureResized = new(lightingTextureSize.x, lightingTextureSize.y);

                // Assigning the lighting effect shader to the Vtuber Avatar source
                Material spoutMat = new(lightingMaterial);
                avatarDisplayOutput.material = spoutMat;
                spoutMat.SetTexture("_MainTex", lightingDisplay.mainTexture.ConvertToTexture2D());
                spoutMat.SetTexture("_VirtualWebcamTex", lightingTextureResized);
                spoutMat.SetFloat("_LightOpacity", sliderLightOpacity.value);
                spoutMat.SetFloat("_LightIntensity", sliderLightIntensity.value);

                tempBackground.SetActive(false);
            }
            else
            {
                tempBackground.SetActive(true);
                avatarDisplayOutput.material = null;
            }
        }

        private void UpdateLighting()
        {
            if (previousOutputTexture != null)
            {
                Destroy(previousOutputTexture);
                previousOutputTexture = null;
            }

            previousOutputTexture = lightingDisplay.mainTexture.ConvertToTexture2D();

            if (newPixels == null || newPixels.Length != lightingTextureSize.x * lightingTextureSize.y)
            {
                newPixels = new Color32[lightingTextureSize.x * lightingTextureSize.y];
            }

            Color32[] pixels = previousOutputTexture.GetPixels32();

            for (int y = 0; y < lightingTextureSize.y; y++)
            {
                for (int x = 0; x < lightingTextureSize.x; x++)
                {
                    float xFrac = x / (float)lightingTextureSize.x;
                    float yFrac = y / (float)lightingTextureSize.y;

                    int xOrig = (int)(xFrac * previousOutputTexture.width);
                    int yOrig = (int)(yFrac * previousOutputTexture.height);

                    newPixels[y * lightingTextureSize.x + x] = pixels[yOrig * previousOutputTexture.width + xOrig];
                }
            }

            lightingTextureResized.SetPixels32(newPixels);
            lightingTextureResized.Apply();
        }

        private void RefreshButtons()
        {
            if (!avatarInputManager.HasSpoutSource || !lightInputManager.HasSpoutSource)
            {
                btnStartCapture.interactable = false;
                btnStopCapture.interactable = false;
                sliderLightOpacity.interactable = false;
                sliderLightOpacityField.interactable = false;
                sliderLightIntensity.interactable = false;
                sliderLightIntensityField.interactable = false;
                sliderLightUpdate.interactable = false;
                sliderLightUpdateField.interactable = false;
            }
            else
            {
                btnStartCapture.interactable = !lightingEnabled;
                btnStopCapture.interactable = lightingEnabled;
                sliderLightOpacity.interactable = lightingEnabled;
                sliderLightOpacityField.interactable = lightingEnabled;
                sliderLightIntensity.interactable = lightingEnabled;
                sliderLightIntensityField.interactable = lightingEnabled;
                sliderLightUpdate.interactable = lightingEnabled;
                sliderLightUpdateField.interactable = lightingEnabled;
            }
        }
    }
}