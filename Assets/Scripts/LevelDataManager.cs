using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDataManager : MonoBehaviour
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
        bool small = false;
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
            switch (obj.name)
            {
                case "Character":
                    {
                        position = obj.GetComponent<CharacterMove>().Position;
                        small = obj.GetComponent <CharacterMove>().Small;
                        health = obj.GetComponent<CharacterHealth>().Health;

                        PlayerAbilities playerAbilities = obj.GetComponent<PlayerAbilities>();

                        inventory.Add("Key", playerAbilities.Key ? 1 : 0);
                        inventory.Add("Money", playerAbilities.Money ? 1 : 0);
                        break;
                    }
                case "SphereDash":
                    {
                        inventory.Add("SphereDash", obj.activeSelf ? 1 : 0);
                        break;
                    }
                case "SphereShrink":
                    {
                        inventory.Add("SphereShrink", obj.activeSelf ? 1 : 0);
                        break;
                    }
                case "SphereAttack":
                    {
                        inventory.Add("SphereAttack", obj.activeSelf ? 1 : 0);
                        break;
                    }
                case "Flood":
                    {
                        floodHeight.Add(0);
                        break;
                    }
                case "Gates":
                    {
                        List<bool> values = new List<bool>();

                        foreach (Transform child in obj.transform)
                        {
                            GateMove gateMove = child.gameObject.GetComponent<GateMove>();
                            values.Add(gateMove.Status);
                        }

                        data.Add("gate", values);
                        break;
                    }
                case "ExtraHealth":
                    {
                        List<bool> values = new List<bool>
                        {
                            false
                        };

                        //values.Add(obj.transform.gameObject.activeSelf);

                        data.Add("extra_health", values);
                        break;
                    }
                case "FallObjects":
                    {
                        List<bool> values = new List<bool>();

                        foreach (Transform child in obj.transform)
                        {
                            //values.Add(child.gameObject.activeSelf);
                            values.Add(false);
                        }

                        data.Add("falling_object", values);
                        break;
                    }
                case "Glass":
                    {
                        List<bool> values = new List<bool>();

                        foreach (Transform child in obj.transform)
                        {
                            values.Add(child.gameObject.activeSelf);
                        }

                        data.Add("glass", values);
                        break;
                    }
                default: break;
            }
        }

        DataBaseManager.SaveData(levelNumber, true, sceneName, (position.x, position.y), health, small, data, floodHeight, inventory);
    }

    public static void LoadProgress()
    {
        Dictionary<string, int> inventory = DataBaseManager.GetInventory();

        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            switch (obj.name)
            {
                case "Character":
                    {
                        var characterInfo = DataBaseManager.GetCharacterHealthAndSmall();
                        var position = DataBaseManager.GetCharacterPosition();

                        obj.GetComponent<CharacterHealth>().Health = characterInfo.Item1;
                        obj.GetComponent<CharacterMove>().Position = new Vector3(position.Item1, position.Item2, 0);
                        obj.GetComponent<PlayerAbilities>().Key = inventory["Key"] == 1 ? true : false;
                        obj.GetComponent<PlayerAbilities>().Money = inventory["Money"] == 1 ? true : false;

                        if (characterInfo.Item2)
                        {
                            obj.GetComponent<CharacterMove>().ToggleSize();
                        }

                        if (inventory["SphereDash"] == 1)
                        {
                            obj.GetComponent<PlayerAbilities>().GrantAbility(0);
                        }

                        if (inventory["SphereShrink"] == 1)
                        {
                            obj.GetComponent<PlayerAbilities>().GrantAbility(1);
                        }

                        if (inventory["SphereAttack"] == 1)
                        {
                            obj.GetComponent<PlayerAbilities>().GrantAbility(2);
                        }

                        break;
                    }
                case "SphereDash":
                    {
                        obj.SetActive(inventory["SphereDash"] == 1 ? true : false);
                        break;
                    }
                case "SphereShrink":
                    {
                        obj.SetActive(inventory["SphereShrink"] == 1 ? true : false);
                        break;
                    }
                case "SphereAttack":
                    {
                        obj.SetActive(inventory["SphereAttack"] == 1 ? true : false);
                        break;
                    }
                case "Flood":
                    {
                        break;
                    }
                case "Gates":
                    {
                        List<bool> values = DataBaseManager.ReadGates();
                        int i = 0;

                        foreach (Transform child in obj.transform)
                        {
                            GateMove gateMove = child.gameObject.GetComponent<GateMove>();

                            if (i > values.Count - 1)
                            {
                                Debug.LogError("Несовпадение фактического количества ворот на сцене и записи в БД!");
                            }
                            else
                            {
                                gateMove.Status = values[i];
                                gateMove.Position = values[i] ? gateMove.StartPoint : gateMove.FinishPoint;
                            }
                            i++;
                        }

                        break;
                    }
                case "ExtraHealth":
                    {
                        break;
                    }
                case "FallObjects":
                    {
                        break;
                    }
                case "Glass":
                    {
                        List<bool> values = DataBaseManager.ReadGlass();
                        int i = 0;

                        foreach (Transform child in obj.transform)
                        {
                            if (i > values.Count - 1)
                            {
                                Debug.LogError("Несовпадение фактического количества стёкол на сцене и записи в БД!");
                            }
                            else
                            {
                                child.gameObject.SetActive(values[i]);
                            }
                            i++;
                        }

                        break;
                    }
                default: break;
            }
        }
    }
}
