using System.IO;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class SaveSystem : SystemBase
    {
        private string savePath => Application.persistentDataPath + "save.json";

        private SaveData _currentSave;

        public SaveData GetCurrentSave => _currentSave;

        protected override void OnInit()
        {
            _currentSave = LoadSave();
        }

        public void Save()
        {
            SaveData saveData = Data.GetSystem<GameBehaviorSystem>().GetSaveData;

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, json);
        }

        private SaveData LoadSave()
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                return JsonUtility.FromJson<SaveData>(json);
            }

            return null;
        }
    }
}