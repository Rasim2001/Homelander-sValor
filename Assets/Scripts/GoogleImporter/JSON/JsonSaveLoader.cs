using UnityEngine;

namespace GoogleImporter.JSON
{
    public class JsonSaveLoader
    {
        public string ConvertToJson(GameJsonData staticDataScene) =>
            JsonUtility.ToJson(staticDataScene, true);

        public GameJsonData Load(string json)
        {
            GameJsonData gameJsonData = JsonUtility.FromJson<GameJsonData>(json);

            ConvertToJson(gameJsonData); //Update after loading from internet

            Debug.Log("Progress loaded successfully");
            return gameJsonData;
        }
    }
}