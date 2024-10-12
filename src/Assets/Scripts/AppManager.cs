using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VtubeLighting.Serialization;

namespace VtubeLighting.Core
{
    public class AppManager : MonoBehaviour
    {
        public enum AppState 
        {
            Idle,
            SavingPreset,
            DeletingPreset
        }

        [Header("UI References")]
        [SerializeField] private GameObject sidebar;
        [SerializeField] private GameObject tinyPreview;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown maxFpsDropdown;

        [Header("Script References")]
        [SerializeField] private SaveManager saveManager;

        private bool sidebarVisible = true;
        private SaveData saveData;
        private AppState state = AppState.Idle;

        private void Reset()
        {
            saveManager = FindObjectOfType<SaveManager>();
        }

        private void Awake()
        {
            saveData = saveManager.Load();
        }

        private void Start()
        {
            List<Resolution> screenResolutions = GetResolutions();

            resolutionDropdown.ClearOptions();

            List<string> screenResolutionsName = new();
            foreach (Resolution resolution in screenResolutions) screenResolutionsName.Add(resolution.width + "x" + resolution.height);
            resolutionDropdown.AddOptions(screenResolutionsName);

            resolutionDropdown.onValueChanged.AddListener(index =>
            {
                Screen.SetResolution(screenResolutions[index].width, screenResolutions[index].height, Screen.fullScreenMode);
                saveData.resolutionWidth = screenResolutions[index].width;
                saveData.resolutionHeight = screenResolutions[index].height;
                saveManager.Save(saveData);
            });

            maxFpsDropdown.onValueChanged.AddListener(index =>
            {
                Application.targetFrameRate = GetMaxFps(index);
                saveData.maxFps = Application.targetFrameRate;
                saveManager.Save(saveData);
            });

            int resolutionIndex = screenResolutionsName.IndexOf(saveData.resolutionWidth + "x" + saveData.resolutionHeight); // Try to get the index of the saved resolution
            if (resolutionIndex == -1)
            {
                resolutionIndex = screenResolutionsName.IndexOf("1280x720"); // If Failed try to get the 720p resolution

                if (resolutionIndex == -1)  // If still fails then choose the middle resolution index and save it to the settings
                {
                    resolutionIndex = screenResolutionsName.Count / 2;
                    saveData.resolutionWidth = screenResolutions[resolutionIndex].width;
                    saveData.resolutionHeight = screenResolutions[resolutionIndex].height;
                    saveManager.Save(saveData);
                }
                else // If 720p is available then save it to the settings
                {
                    saveData.resolutionWidth = 1280;
                    saveData.resolutionHeight = 720;
                    saveManager.Save(saveData);
                }
            }

            resolutionDropdown.value = resolutionIndex; // Then assign it to the dropdown
            Screen.SetResolution(screenResolutions[resolutionDropdown.value].width, screenResolutions[resolutionDropdown.value].height, Screen.fullScreenMode);

            maxFpsDropdown.value = GetMaxFpsIndex(saveData.maxFps);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = GetMaxFps(maxFpsDropdown.value);

            sidebar.SetActive(true);
            tinyPreview.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && state == AppState.Idle)
            {
                sidebarVisible = !sidebarVisible;
                sidebar.SetActive(sidebarVisible);
                tinyPreview.SetActive(sidebarVisible);
            }
        }

        public SaveData GetSavedData() => saveData;
        public void SaveData() => saveManager.Save(saveData);
        public void SaveData(SaveData saveData) => saveManager.Save(saveData);

        private int GetMaxFps(int index)
        {
            return index switch
            {
                0 => 30,
                1 => 45,
                2 => 50,
                3 => 60,
                4 => 70,
                5 => 90,
                6 => 120,
                _ => 60,
            };
        }

        private int GetMaxFpsIndex(int index)
        {
            return index switch
            {
                30 => 0,
                45 => 1,
                50 => 2,
                60 => 3,
                70 => 4,
                90 => 5,
                120 => 6,
                _ => 0,
            };
        }

        private List<Resolution> GetResolutions()
        {
            Resolution[] resolutions = Screen.resolutions;
            List<Resolution> finalResolutions = new();

            foreach (Resolution resolution in resolutions)
            {
                if (!ContainsResolution(finalResolutions, resolution) && IsResolution16by9(resolution.width, resolution.height)) finalResolutions.Add(resolution);
            }

            return finalResolutions;
        }

        private bool IsResolution16by9(int width, int height) => Math.Abs((double)width / height - 16.0 / 9.0) < 0.01;

        private bool ContainsResolution(List<Resolution> resolutionsList, Resolution resolution)
        {
            foreach (Resolution r in resolutionsList)
            {
                if (r.width == resolution.width && r.height == resolution.height) return true;
            }

            return false;
        }

        public void SetAppState(AppState state) => this.state = state;
    }
}