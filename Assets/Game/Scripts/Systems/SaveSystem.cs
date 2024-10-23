using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class SaveSystem : SystemBase
    {
        private SaveData _currentSave;
        public SaveData GetCurrentSave => _currentSave;

        protected override void OnInit()
        {
            _currentSave = LoadSave();
        }

        public void Save()
        {
            SaveData saveData = Data.GetSystem<GameBehaviorSystem>().GetSaveData;
            
            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString("Save", json);
        }

        private SaveData LoadSave()
        {
            if (PlayerPrefs.HasKey("Save"))
            {
                string saveString = PlayerPrefs.GetString("Save");
                return JsonUtility.FromJson<SaveData>(saveString);
            }

            return null;
        }
    }
}