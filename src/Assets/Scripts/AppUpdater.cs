using System.Net.Http;
using UnityEngine;

namespace VtubeLighting.Core
{
    public class AppUpdater : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string appVersion = "1.0.0";
        [SerializeField] private string versionUrl = "https://raw.githubusercontent.com/DayMoniakk/VtubeLighting/main/VERSION";

        [Header("References")]
        [SerializeField] private GameObject updatePanel;

        private void Awake()
        {
            CheckForUpdate();
        }

        private void CheckForUpdate()
        {
            if (string.IsNullOrEmpty(versionUrl)) return;

            string result = "";

            try
            {
                using HttpClient client = new();
                result = client.GetStringAsync(versionUrl).Result;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Updater cannot read app version from URL. Stacktrace: " + e.StackTrace);
            }

            if (result != "")
            {
                if (!result.Contains(appVersion))
                {
                    updatePanel.SetActive(true);
                    Debug.Log("A newest version is available, current=" + appVersion + " newest=" + result);
                }
                else Debug.Log("The application version is up to date");
            }

            Destroy(this);
        }
    }
}