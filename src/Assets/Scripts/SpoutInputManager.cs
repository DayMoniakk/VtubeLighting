using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Klak.Spout;
using System.Linq;

// This class handles the rendering of the Vtuber Avatar on the screen from a Spout2 source

public class SpoutInputManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown sourceDropdown; // The dropdown used to display the available Spout2 sources
    [SerializeField] private GameObject noSignalText; // The warning text displayed when no Spout2 source is available
    [SerializeField] private Button refreshButton; // The button used to refresh the Spout2 sources list

    [Header("Script References")]
    [SerializeField] private SpoutReceiver spoutReceiver; // The library class that manages all the Spout2 receiving stuff
    [SerializeField] private TranslationsManager translationsManager; // Used to localize strings

    // Used whenever the Spout2 signal is updated
    // (Since this is an external library I don't have any controls on its behaviour, this will only be called when the user refresh the sourceDropdown 
    // because there's not methods to check if a Spout2 source is currently received)
    public delegate void OnSourceUpdated(bool hasSource);
    public OnSourceUpdated onSourceUpdated;

    private List<string> spoutSources; // The list of all current Spout2 sources available
    private bool hasSpoutSource; // Returns true if the Spout2 source is rendering

    private void Reset() // Used when the script is added to the gameobject (utility only)
    {
        spoutReceiver = FindObjectOfType<SpoutReceiver>(); 
        translationsManager = FindObjectOfType<TranslationsManager>();
    }

    private void Start()
    {
        RefreshSpoutList();

        // Here i'm just setting up all the UI callbacks so I don't have to drag and drop every events by hand
        refreshButton.onClick.AddListener(RefreshSpoutList);

        sourceDropdown.onValueChanged.AddListener(value =>
        {
            noSignalText.SetActive(!hasSpoutSource);

            if (hasSpoutSource)
                spoutReceiver.sourceName = spoutSources[sourceDropdown.value];
        });
    }

    private void RefreshSpoutList()
    {
        spoutSources = SpoutManager.GetSourceNames().ToList();
        hasSpoutSource = spoutSources.Count > 0 && spoutSources[0] != "VtubeLighting";
        onSourceUpdated?.Invoke(hasSpoutSource);

        if (!hasSpoutSource) 
        {
            spoutSources = new() { translationsManager.GetTranslation("generic.none") };
            spoutReceiver.sourceName = "";
        }
        else spoutReceiver.sourceName = spoutSources[0];

        sourceDropdown.ClearOptions();
        sourceDropdown.AddOptions(spoutSources);

        noSignalText.SetActive(!hasSpoutSource);
    }
}
