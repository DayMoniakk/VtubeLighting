using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// I have to admit that this class got really messy but basically VirtualWebcamCapture is handling the Virtual Webcam stuff for the Lighting Effect

public class VirtualWebcamCapture : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown deviceDropdown; // The dropdown that allows the user to select the OBS Virtual Camera to get the background
    [SerializeField] private RawImage spoutDisplayImage; // The RawImage that displays the Vtuber Avatar source from Spout2
    [SerializeField] private RawImage lightBackground; // The RawImage that displays the content sent by the OBS Virtual Camera
    [SerializeField] private GameObject tempBackground; // The placerholder checkerboard background displayed when the lighting effect is disabled
    [SerializeField] private Material lightingMaterial; // The shader material used to create the lighting effect (added and remove at runtime)
    [SerializeField] private Button btnStartCapture; // The button to start the lighting effect
    [SerializeField] private Button btnStopCapture; // The button to stop the lighting effect
    [SerializeField] private Button btnRefreshWebcams; // The button to refresh the webcam list in the deviceDropdown
    [SerializeField] private Slider sliderLightIntensity; // The slider that allows the user to control the lighting intensity
    [SerializeField] private TMP_InputField sliderLightIntensityField; // The InputField that allows the user to type a value to control the lighting intensity
    [SerializeField] private Slider sliderLightUpdate; // The slider that allows the user to control the blurriness of the background
    [SerializeField] private TMP_InputField sliderLightUpdateField; // The InputField that allows the user to type a value to control the burriness of the background

    [Header("Script References")]
    [SerializeField] private AppManager appManager; // The script that manages the application
    [SerializeField] private SpoutInputManager spoutInputManager; // Our custom script that controls the Vtuber Avatar rendering
    [SerializeField] private TranslationsManager translationsManager; // Used to localize strings

    private WebCamDevice[] webcams; // The webcams currently plugged in
    private WebCamTexture currentWebcamTexture; // The current webcam texture used to display the OBS Virtual Camera

    private Texture2D lightingTextureResized; // The webcam output is copied to this texture
    private readonly Vector2Int lightingTextureSize = new(32, 18); // The width and height of the lighting texture used to light up the avatar (we don't need a huge resolution to get lighting and it will help with performance)
    private float lightingUpdateRate = 8; // How many times per second should we update the lighting texture
    private float nextTimeToUpdateLighting; // Holds the next time to update the lighting texture

    private bool hasAvatarSource; // Returns true if the Avatar source is getting something via Spout2
    private bool webcamRunning; // Returns true if the webcam is rendering  (so the lighting effect is currently enabled)
    private bool checkedState; // Used to check whenever the Spout2 signal is lost

    private void Reset()  // Used when the script is added to the gameobject (utility only)
    {
        spoutInputManager = FindObjectOfType<SpoutInputManager>();
        translationsManager = FindObjectOfType<TranslationsManager>();
    }

    private void Start()
    {
        spoutInputManager.onSourceUpdated += OnSourceUpdated; // Subscribe to our Spout2 controller script

        RefreshWebcamsList();

        // Here i'm just setting up all the UI callbacks so I don't have to drag and drop every events by hand
        sliderLightIntensity.onValueChanged.AddListener(value =>
        {
            spoutDisplayImage.material.SetFloat("_LightIntensity", value);
            sliderLightIntensityField.text = value.ToString("F2");
        });
        sliderLightIntensityField.onValueChanged.AddListener(value => { sliderLightIntensity.value = float.Parse(value); });
        sliderLightIntensityField.onEndEdit.AddListener(value => { sliderLightIntensityField.text = sliderLightIntensity.value.ToString("F2"); });

        // Read the update rate from the save file and apply it to the slider and it's input field
        lightingUpdateRate = appManager.GetSavedData().lightingRefreshRate;
        sliderLightUpdate.value = lightingUpdateRate;
        sliderLightUpdateField.text = lightingUpdateRate.ToString();

        sliderLightUpdate.onValueChanged.AddListener(value =>
        {
            lightingUpdateRate = value;

            sliderLightUpdateField.text = value.ToString();
        });
        sliderLightUpdateField.onValueChanged.AddListener(value => { sliderLightUpdate.value = float.Parse(value); });
        sliderLightUpdateField.onEndEdit.AddListener(value => { sliderLightUpdateField.text = sliderLightUpdate.value.ToString(); });

        // This might be confusing but basically i'm just creating a Event Trigger through script and assign the Pointer Up event to a custom method.
        // Why ? Because I want to detect when the user let go of the slider, I don't want to spam the saving system.
        EventTrigger ev = sliderLightUpdate.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new()
        {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener(data =>
        {
            SaveData saveData = appManager.GetSavedData();
            if (saveData.lightingRefreshRate == lightingUpdateRate) return; // We don't want to start a save procedure if the settings haven't changed !

            saveData.lightingRefreshRate = lightingUpdateRate;
            appManager.SaveData(saveData);
        });
        ev.triggers.Add(entry);

        btnStartCapture.onClick.AddListener(() => { SetWebcamState(true); });
        btnStopCapture.onClick.AddListener(() => { SetWebcamState(false); });
        btnRefreshWebcams.onClick.AddListener(RefreshWebcamsList);

        SetWebcamState(false); // Webcam rendering should be turned off by default
    }

    private void OnDestroy() => spoutInputManager.onSourceUpdated -= OnSourceUpdated; // Unsubscribe to our Spout2 controller script (to avoid memory leaks)

    private void Update()
    {
        if (!checkedState) // Check whenever the Spout2 signal is lost
        {
            if (webcamRunning) SetWebcamState(false);

            RefreshButtons();

            checkedState = true;
        }

        if (webcamRunning)
        {
            if (Time.time >= nextTimeToUpdateLighting) // Refreshing the full texture lighting every frame can be quite expensive so we only update it a few times per second
            {
                nextTimeToUpdateLighting = Time.time + 1f / lightingUpdateRate;
                UpdateLighting();
            }
        }
    }

    private void RefreshWebcamsList()
    {
        deviceDropdown.ClearOptions();

        webcams = WebCamTexture.devices;
        string[] devicesNames = new string[webcams.Length];
        int targetIndex = 0;

        for (int i = 0; i < webcams.Length; i++)
        {
            devicesNames[i] = webcams[i].name;

            if (devicesNames[i].Equals("OBS Virtual Camera")) targetIndex = i;
        }

        if (webcams.Length == 0)
        {
            devicesNames = new string[1] { translationsManager.GetTranslation("generic.no_devices_found") };
        }

        deviceDropdown.AddOptions(devicesNames.ToList());
        deviceDropdown.value = targetIndex;
    }

    private void SetWebcamState(bool state) // Controls if the lighting effect is running or not
    {
        if (webcams.Length == 0) return; // If we don't have any webcams available everything should be ignored

        webcamRunning = state;

        RefreshButtons(); // Refresh the UI elements

        if (state)
        {
            WebCamDevice currentCam = webcams[deviceDropdown.value];

            WebCamTexture webCamTexture = new(currentCam.name);

            webCamTexture.Play();
            currentWebcamTexture = webCamTexture;

            lightingTextureResized = new(lightingTextureSize.x, lightingTextureSize.y);

            // Assigning the lighting effect shader to the Vtuber Avatar source
            Material spoutMat = new(lightingMaterial);
            spoutDisplayImage.material = spoutMat;
            spoutMat.SetTexture("_MainTex", spoutDisplayImage.texture);
            spoutMat.SetTexture("_VirtualWebcamTex", lightingTextureResized);
            spoutMat.SetFloat("_LightIntensity", sliderLightIntensity.value);

            // Getting rid of the placeholder background and displaying the webcam source
            tempBackground.SetActive(false);
            lightBackground.gameObject.SetActive(true);
            lightBackground.texture = webCamTexture;
        }
        else
        {
            currentWebcamTexture?.Stop();

            // Removing the lighting effect shader from the Vtuber Avatar source
            spoutDisplayImage.material = null;

            // Free up some memory since we don't need that anymore
            lightingTextureResized = null;

            // Re-enabling the placeholder background
            tempBackground.SetActive(true);
            lightBackground.gameObject.SetActive(false);
            lightBackground.texture = null;
        }
    }

    private void UpdateLighting() // Refresh the lightingTexutre pixels to a lower resolution than webcamTexture
    {
        Color32[] pixels = currentWebcamTexture.GetPixels32();
        Color32[] newPixels = new Color32[lightingTextureSize.x * lightingTextureSize.y];

        for (int y = 0; y < lightingTextureSize.y; y++)
        {
            for (int x = 0; x < lightingTextureSize.x; x++)
            {
                float xFrac = x / (float)lightingTextureSize.x;
                float yFrac = y / (float)lightingTextureSize.y;

                int xOrig = (int)(xFrac * currentWebcamTexture.width);
                int yOrig = (int)(yFrac * currentWebcamTexture.height);

                newPixels[y * lightingTextureSize.x + x] = pixels[yOrig * currentWebcamTexture.width + xOrig];
            }
        }

        lightingTextureResized.SetPixels32(newPixels);
        lightingTextureResized.Apply();
    }

    private void RefreshButtons()
    {
        // If we don't have any webcams available everything should be ignored
        if (webcams.Length == 0 || !hasAvatarSource)
        {
            btnStartCapture.interactable = false;
            btnStopCapture.interactable = false;
            sliderLightIntensity.interactable = false;
            sliderLightIntensityField.interactable = false;
            sliderLightUpdate.interactable = false;
            sliderLightUpdateField.interactable = false;

            return;
        }

        btnStartCapture.interactable = !webcamRunning;
        btnStopCapture.interactable = webcamRunning;
        sliderLightIntensity.interactable = webcamRunning;
        sliderLightIntensityField.interactable = webcamRunning;
        sliderLightUpdate.interactable = webcamRunning;
        sliderLightUpdateField.interactable = webcamRunning;
    }

    // Delegate called from spoutInputManager whenever Spout2 signal is lost
    private void OnSourceUpdated(bool hasSource)
    {
        hasAvatarSource = hasSource;
        checkedState = false;
    }
}
