using System.Collections.Generic;
using System.Linq;
using Klak.Spout;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VtubeLighting.Core
{
    public class SpoutInputManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string preferredSourceName;

        [Header("UI References")]
        [SerializeField] private TMP_Dropdown sourceDropdown;
        [SerializeField] private Button refreshButton;

        [Header("Script References")]
        [SerializeField] private SpoutReceiver spoutReceiver;

        private List<string> sourcesAvailable;
        private bool hasSpoutSource;

        public delegate void OnSourceUpdated();
        public OnSourceUpdated onSourceUpdated;

        private void Start()
        {
            RefreshSpoutList();
            refreshButton.onClick.AddListener(RefreshSpoutList);

            sourceDropdown.onValueChanged.AddListener(value =>
            {
                if (hasSpoutSource)
                {
                    spoutReceiver.sourceName = sourcesAvailable[sourceDropdown.value];
                }
            });
        }

        private void RefreshSpoutList()
        {
            int targetIndex = 0;
            sourcesAvailable = SpoutManager.GetSourceNames().ToList();

            List<string> sourcesAvailableList = new();
            int iterations = 0;
            foreach (string source in sourcesAvailable)
            {
                if (source == "VtubeLighting") continue;
                sourcesAvailableList.Add(source);

                if (source == preferredSourceName) targetIndex = iterations;
                iterations++;
            }

            hasSpoutSource = sourcesAvailableList.Count > 0;
            onSourceUpdated?.Invoke();
            sourcesAvailable = sourcesAvailableList;

            if (!hasSpoutSource) spoutReceiver.sourceName = "";
            else spoutReceiver.sourceName = sourcesAvailable[targetIndex];

            sourceDropdown.ClearOptions();
            sourceDropdown.AddOptions(sourcesAvailable);
            sourceDropdown.value = targetIndex;
        }

        public bool HasSpoutSource => hasSpoutSource;
    }
}