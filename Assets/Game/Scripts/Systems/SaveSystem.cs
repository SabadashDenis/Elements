using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class SaveSystem : SystemBase
    {
        public static SaveData GetCurrentSave => LoadSave();

        protected override void OnInit() { }
        
        public void Save()
        {
            SaveData saveData = Data.GetSystem<GameBehaviorSystem>().GetSaveData;

            if (saveData != null)
            {
                string json = JsonUtility.ToJson(saveData);
                PlayerPrefs.SetString("Save", json);
            }
        }

        private static SaveData LoadSave()
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