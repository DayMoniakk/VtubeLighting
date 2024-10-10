using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VtubeLighting.Core;
using VtubeLighting.Serialization;

public class LightingPresetsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button saveNewPresetBtn;
    [SerializeField] private Button deletePresetBtn;
    [SerializeField] private TMP_Dropdown presetsDropdown;
    [SerializeField] private GameObject windowNewPreset;
    [SerializeField] private TMP_InputField inputFieldPresetName;
    [SerializeField] private Button btnSaveNewPreset;
    [SerializeField] private Button btnCancelNewPreset;
    [SerializeField] private GameObject windowDeletePresetConfirm;
    [SerializeField] private Button btnDeletePreset;
    [SerializeField] private Button btnCancelDeletePreset;

    [Header("Script References")]
    [SerializeField] private AppManager appManager;
    [SerializeField] private LightingManager lightingManager;

    private List<LightingPreset> presets = new();

    private void Start()
    {
        lightingManager.onLightingStateChanged += OnLightingStateChanged;

        saveNewPresetBtn.onClick.AddListener(() =>
        {
            windowNewPreset.SetActive(true);
            inputFieldPresetName.text = $"Preset {presets.Count + 1}";
            appManager.SetAppState(AppManager.AppState.SavingPreset);
        });

        btnCancelNewPreset.onClick.AddListener(() =>
        {
            windowNewPreset.SetActive(false);
            appManager.SetAppState(AppManager.AppState.Idle);
        });

        deletePresetBtn.onClick.AddListener(() =>
        {
            windowDeletePresetConfirm.SetActive(true);
            appManager.SetAppState(AppManager.AppState.DeletingPreset);
        });

        btnCancelDeletePreset.onClick.AddListener(() =>
        {
            windowDeletePresetConfirm.SetActive(false);
            appManager.SetAppState(AppManager.AppState.Idle);
        });

        btnDeletePreset.onClick.AddListener(() =>
        {
            Debug.Log($"Trying to delete {presets[presetsDropdown.value].name} with index {presetsDropdown.value}");

            DeletePreset(presets[presetsDropdown.value].name);
            windowDeletePresetConfirm.SetActive(false);
            RefreshPresets();
            SelectPreset(0);
            appManager.SetAppState(AppManager.AppState.Idle);
        });

        btnSaveNewPreset.onClick.AddListener(() =>
        {
            CreatePreset(inputFieldPresetName.text, lightingManager.LightingOpacity, lightingManager.LightingIntensity, lightingManager.LightingUpdateRate);
            windowNewPreset.SetActive(false);
            RefreshPresets();
            presetsDropdown.value = presets.Count - 1;
            appManager.SetAppState(AppManager.AppState.Idle);
        });

        presetsDropdown.onValueChanged.AddListener(SelectPreset);

        LoadPresets();
        RefreshPresets();
    }

    private void OnDestroy()
    {
        lightingManager.onLightingStateChanged -= OnLightingStateChanged;
    }

    private void OnLightingStateChanged(bool state)
    {
        saveNewPresetBtn.interactable = state;
        deletePresetBtn.interactable = state && presets.Count > 1;
        btnSaveNewPreset.interactable = state;
        presetsDropdown.interactable = state;
    }

    private void LoadPresets()
    {
        SaveData savedData = appManager.GetSavedData();
        presets = savedData.lightingPresets;

        if (presets.Count == 0)
        {
            CreateDefaultPreset();
        }
        else
        {
            Debug.Log($"Loaded {presets.Count} lighting presets");
        }
    }

    private void CreateDefaultPreset()
    {
        SaveData savedData = appManager.GetSavedData();
        presets.Add(LightingPreset.GetDefault());
        savedData.lightingPresets = presets;
        appManager.SaveData();
        Debug.Log("Created default lighting preset");
    }

    private void RefreshPresets()
    {
        presetsDropdown.ClearOptions();
        presetsDropdown.AddOptions(presets.ConvertAll(p => p.name));

        deletePresetBtn.interactable = presets.Count > 1 && lightingManager.IsLightingEnabled;
    }

    public void CreatePreset(string name, float opacity, float intensity, float updateRate)
    {
        presets.Add(new LightingPreset(name, opacity, intensity, updateRate));
        SaveData saveData = appManager.GetSavedData();
        saveData.lightingPresets = presets;
        appManager.SaveData();
        Debug.Log($"Created lighting preset '{name}'");
    }

    public List<LightingPreset> GetPresets() => presets;

    public LightingPreset GetPreset(string name) => presets.Find(p => p.name == name);

    public void DeletePreset(string name)
    {
        presets.Remove(GetPreset(name));
        SaveData saveData = appManager.GetSavedData();
        saveData.lightingPresets = presets;
        appManager.SaveData();
        Debug.Log($"Deleted lighting preset '{name}'");
    }

    public void SelectPreset(int index)
    {
        if (presets.Count == 0)
        {
            CreateDefaultPreset();
            RefreshPresets();
        }

        lightingManager.SetLightingOpacity(presets[index].opacity);
        lightingManager.SetLightingIntensity(presets[index].intensity);
        lightingManager.SetLightingUpdateRate(presets[index].updateRate);
    }
}
