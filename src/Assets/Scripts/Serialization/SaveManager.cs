using System.IO;
using UnityEngine;

namespace VtubeLighting.Serialization
{
    public class SaveManager : MonoBehaviour
    {
        private const string fileName = "/app_settings.json";

        public SaveData Load()
        {
            string filePath = Application.persistentDataPath + fileName;
            SaveData saveData = new();

            if (File.Exists(filePath)) saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(filePath));
            else Save(new(), filePath);

            return saveData;
        }

        public void Save(SaveData saveFile, string filePath) => File.WriteAllText(filePath, JsonUtility.ToJson(saveFile));
        public void Save(SaveData saveFile)
        {
            string filePath = Application.persistentDataPath + fileName;
            File.WriteAllText(filePath, JsonUtility.ToJson(saveFile));
        }
    }
}