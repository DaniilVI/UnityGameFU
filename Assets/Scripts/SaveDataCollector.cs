using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class SaveDataCollector : MonoBehaviour
{
    public static void SaveLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        Door door = GameObject.FindObjectOfType<Door>();
        string sceneName = door.SceneName;
        string numberPart = sceneName.Replace("Level", "");
        int levelNumber = int.Parse(numberPart);
        DataBaseManager.SaveData(levelNumber, false, sceneName, (0, 0), 3);
    }

    public static void SaveProgress()
    {
        int health = 3;
        Vector3 position = new Vector3(0, 0, 0);
        List<float> floodHeight = new List<float>();
        Dictionary<string, int> inventory = new Dictionary<string, int>();
        Dictionary<string, List<bool>> data = new Dictionary<string, List<bool>>();

        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        string sceneName = scene.name;
        string numberPart = scene.name.Replace("Level", "");
        int levelNumber = int.Parse(numberPart);

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == "Character")
            {
                position = obj.transform.position;

                CharacterHealth characterHealth = obj.GetComponent<CharacterHealth>();
                health = characterHealth.Health;
                
                PlayerAbilities playerAbilities = obj.GetComponent<PlayerAbilities>();

                inventory.Add("key", playerAbilities.Key ? 1 : 0);
                inventory.Add("money", playerAbilities.Money);
            } 
            else if (obj.name == "Flood")
            {
                floodHeight.Add(0);
            }
            else if (obj.name == "Gates")
            {
                List<bool> values = new List<bool>();

                foreach (Transform child in obj.transform)
                {
                    GateMove gateMove = child.gameObject.GetComponent<GateMove>();
                    values.Add(gateMove.Status);
                }

                data.Add("gate", values);
            }
            else if (obj.name == "ExtraHealth")
            {
                List<bool> values = new List<bool>
                {
                    false
                };

                //values.Add(obj.transform.gameObject.activeSelf);

                data.Add("extra_health", values);
            }
            else if (obj.name == "FallObjects")
            {
                List<bool> values = new List<bool>();

                foreach (Transform child in obj.transform)
                {
                    //values.Add(child.gameObject.activeSelf);
                    values.Add(false);
                }

                data.Add("falling_object", values);
            }
            else if (obj.name == "Glass")
            {
                List<bool> values = new List<bool>();

                foreach (Transform child in obj.transform)
                {
                    values.Add(child.gameObject.activeSelf);
                }

                data.Add("glass", values);
            }
        }

        DataBaseManager.SaveData(levelNumber, true, sceneName, (position.x, position.y), health, data, floodHeight, inventory);
    }
}
