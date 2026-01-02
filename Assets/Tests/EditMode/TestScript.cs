using Mono.Data.Sqlite;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static UnityEngine.Networking.UnityWebRequest;

public class TestScript
{
    private static string connectionString = "URI=file:" + Application.dataPath + "/StreamingAssets/DataBase.sqlite";
    private string originalDbPath = Path.Combine(Application.streamingAssetsPath, "DataBase.sqlite"),
        originalDbMetaPath = Path.Combine(Application.streamingAssetsPath, "DataBase.sqlite.meta"),
        dbPath = Path.Combine(Application.streamingAssetsPath, "DataBase_Backup.sqlite"),
        dbMetaPath = Path.Combine(Application.streamingAssetsPath, "DataBase_Backup.sqlite.meta");

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        File.Copy(originalDbPath, dbPath, true);
        File.Copy(originalDbMetaPath, dbMetaPath, true);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        File.Copy(dbPath, originalDbPath, true);
        File.Copy(dbMetaPath, originalDbMetaPath, true);
        File.Delete(dbPath);
        File.Delete(dbMetaPath);
    }

    [Test, Order(1)]
    public void Test1()
    {
        DataBaseManager.CreateCharacter(0);

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT id, position_x, position_y, health, small FROM CHARACTER WHERE id = @id";
                sqlite_cmd.Parameters.AddWithValue("@id", 0);

                using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                {
                    if (sqlite_datareader.Read())
                    {
                        Assert.That(sqlite_datareader.GetInt32("id"), Is.EqualTo(0), "Значение id должно быть 0");
                        Assert.That(sqlite_datareader.GetFloat("position_x"), Is.EqualTo(0f), "Значение для position_x должно быть 0f");
                        Assert.That(sqlite_datareader.GetFloat("position_y"), Is.EqualTo(0f), "Значение для position_y должно быть 0f");
                        Assert.That(sqlite_datareader.GetInt32("health"), Is.EqualTo(3), "Значение для здоровья должно быть 3");
                        Assert.That(sqlite_datareader.GetInt32("small"), Is.EqualTo(0), "Значение для \"уменьшен ли персонаж\" должно быть false");
                    }
                    else
                    {
                        Assert.Fail("Игрок с id=0 не найден");
                    }
                }
            }

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT id, number, status, title, CHARACTER_id FROM LEVEL WHERE id = @id";
                sqlite_cmd.Parameters.AddWithValue("@id", 0);


                using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                {
                    if (sqlite_datareader.Read())
                    {
                        Assert.That(sqlite_datareader.GetInt32("id"), Is.EqualTo(0), "Значение id должно быть 0");
                        Assert.That(sqlite_datareader.GetInt32("number"), Is.EqualTo(1), "Значение для номера уровня должно быть 1");
                        Assert.That(sqlite_datareader.GetInt32("status"), Is.EqualTo(0), "Значение для статуса уровня должно быть 0");
                        Assert.That(sqlite_datareader.GetString("title"), Is.EqualTo("Level1"), "Значение для названия уровня должно быть \"Level1\"");
                        Assert.That(sqlite_datareader.GetInt32("CHARACTER_id"), Is.EqualTo(0), "Значение CHARACTER_id должно быть 0");
                    }
                    else
                    {
                        Assert.Fail("Первый уровень для игрока с id=0 не найден");
                    }
                }
            }
        }

        DataBaseManager.setCharacterId(0);
    }

    [Test, Order(2)]
    public void Test2()
    {
        var tmp = DataBaseManager.GetCharacterPosition();
        Assert.IsInstanceOf<ValueTuple<float, float>>(tmp, "Возвращаемый тип должен быть ValueTuple<float, float>");
        Assert.That(tmp.Item1, Is.EqualTo(0f), "Значение для первого элемента должно быть 0f");
        Assert.That(tmp.Item2, Is.EqualTo(0f), "Значение для второго элемента должно быть 0f");
    }

    [Test, Order(3)]
    public void Test3()
    {
        var tmp = DataBaseManager.GetCharacterHealthAndSmall();
        Assert.IsInstanceOf<ValueTuple<int, bool>>(tmp, "Возвращаемый тип должен быть ValueTuple<int, bool>");
        Assert.That(tmp.Item1, Is.EqualTo(3), "Значение для первого элемента должно быть 3");
        Assert.That(tmp.Item2, Is.EqualTo(false), "Значение для второго элемента должно быть false");
    }

    [Test, Order(4)]
    public void Test4()
    {
        var tmp = DataBaseManager.GetInfoLevel();
        Assert.IsInstanceOf<ValueTuple<string, string>>(tmp, "Возвращаемый тип должен быть ValueTuple<string, string>");
        Assert.That(tmp.Item1, Is.EqualTo("не пройден"), "Значение для первого элемента должно быть \"не пройден\"");
        Assert.That(tmp.Item2, Is.EqualTo("Level1"), "Значение для второго элемента должно быть \"Level1\"");
    }

    [Test, Order(5)]
    public void Test5()
    {
        var tmp = DataBaseManager.GetStatusLevel();
        Assert.IsInstanceOf<string>(tmp, "Возвращаемый тип должен быть <string>");
        Assert.That(tmp, Is.EqualTo("не пройден"), "Значение для статуса уровня должно быть \"не пройден\"");
    }

    [Test, Order(6)]
    public void Test6()
    {
        var tmp = DataBaseManager.GetNumberLevelForPlayers();
        Assert.IsInstanceOf<Dictionary<int, int>>(tmp, "Возвращаемый тип должен быть Dictionary<int, int>");
        Assert.That(tmp.ContainsKey(0), "Словарь должен содержать ключ 0");
        Assert.That(tmp[0], Is.EqualTo(1), "Значение для ключа 0 должно быть 1");
    }

    [Test, Order(7)]
    public void Test7()
    {
        var tmp = DataBaseManager.GetInventory();
        Assert.IsInstanceOf<Dictionary<string, int>>(tmp, "Возвращаемый тип должен быть Dictionary<string, int>");
        Assert.That(tmp.Count, Is.EqualTo(0), "Словарь должен быть пустым");
    }

    [Test, Order(8)]
    public void Test8()
    {
        DataBaseManager.AddItemInventory("key", 1);
        DataBaseManager.AddItemInventory("money", 1);

        var tmp = DataBaseManager.GetInventory();
        Assert.That(tmp.Count, Is.EqualTo(2), "Словарь должен содержать ровно 2 записи");
        Assert.That(tmp.ContainsKey("key"), "Словарь должен содержать ключ \"key\"");
        Assert.That(tmp["key"], Is.EqualTo(1), "Значение для ключа \"key\" должно быть 1");
        Assert.That(tmp.ContainsKey("money"), "Словарь должен содержать ключ \"money\"");
        Assert.That(tmp["key"], Is.EqualTo(1), "Значение для ключа \"money\" должно быть 1");
    }

    [Test, Order(9)]
    public void Test9()
    {
        DataBaseManager.RemoveItemInventory("keyt");

        var tmp = DataBaseManager.GetInventory();
        Assert.That(tmp.Count, Is.EqualTo(2), "Словарь должен содержать ровно 2 записи");
        Assert.That(tmp.ContainsKey("key"), "Словарь должен содержать ключ \"key\"");
        Assert.That(tmp["key"], Is.EqualTo(1), "Значение для ключа \"key\" должно быть 1");
        Assert.That(tmp.ContainsKey("money"), "Словарь должен содержать ключ \"money\"");
        Assert.That(tmp["key"], Is.EqualTo(1), "Значение для ключа \"money\" должно быть 1");
    }

    [Test, Order(10)]
    public void Test10()
    {
        DataBaseManager.RemoveItemInventory("key");

        var tmp = DataBaseManager.GetInventory();
        Assert.That(tmp.Count, Is.EqualTo(1), "Словарь должен содержать ровно 1 запись");
        Assert.That(tmp.ContainsKey("money"), "Словарь должен содержать ключ \"money\"");
        Assert.That(tmp["money"], Is.EqualTo(1), "Значение для ключа \"money\" должно быть 1");
    }

    [Test, Order(11)]
    public void Test11()
    {
        var tmp = DataBaseManager.GetInventoryByItemName("key");
        Assert.That(tmp, Is.EqualTo(0), "Метод должен возвращать 0 для ключа \"key\"");
    }

    [Test, Order(12)]
    public void Test12()
    {
        var tmp = DataBaseManager.GetInventoryByItemName("money");
        Assert.That(tmp, Is.EqualTo(1), "Метод должен возвращать 1 для ключа \"money\"");

        DataBaseManager.AddItemInventory("money", 1);

        tmp = DataBaseManager.GetInventoryByItemName("money");
        Assert.That(tmp, Is.EqualTo(2), "Метод должен возвращать 2 для ключа \"money\"");

        DataBaseManager.RemoveItemInventory("money");

        tmp = DataBaseManager.GetInventoryByItemName("money");
        Assert.That(tmp, Is.EqualTo(0), "Метод должен возвращать 0 для ключа \"money\"");
    }

    [Test, Order(13)]
    public void Test13()
    {
        DataBaseManager.SaveData(8, false, "Level8", (1, 0), 2);

        var tmp = DataBaseManager.GetCharacterPosition();
        Assert.That(tmp.Item1, Is.EqualTo(1f), "Значение для первого элемента должно быть 1f");
        Assert.That(tmp.Item2, Is.EqualTo(0f), "Значение для второго элемента должно быть 0f");

        var tmp2 = DataBaseManager.GetCharacterHealthAndSmall();
        Assert.That(tmp2.Item1, Is.EqualTo(2), "Значение для первого элемента должно быть 2");
        Assert.That(tmp2.Item2, Is.EqualTo(false), "Значение для второго элемента должно быть false");

        var tmp3 = DataBaseManager.GetInfoLevel();
        Assert.That(tmp3.Item1, Is.EqualTo("не пройден"), "Значение для первого элемента должно быть \"не пройден\"");
        Assert.That(tmp3.Item2, Is.EqualTo("Level8"), "Значение для второго элемента должно быть \"Level8\"");

        var tmp4 = DataBaseManager.GetStatusLevel();
        Assert.That(tmp4, Is.EqualTo("не пройден"), "Возвращаемое значение должно быть \"не пройден\"");

        var tmp5 = DataBaseManager.GetNumberLevelForPlayers();
        Assert.That(tmp5.ContainsKey(0), "Словарь должен содержать ключ 0");
        Assert.That(tmp5[0], Is.EqualTo(8), "Значение для ключа 0 должно быть 8");

        var tmp6 = DataBaseManager.GetInventory();
        Assert.That(tmp6.Count, Is.EqualTo(0), "Словарь должен быть пустым");

        var tmp7 = DataBaseManager.ReadGates();
        Assert.That(tmp7.Count, Is.EqualTo(0), "Список врат должен быть пустым");

        tmp7 = DataBaseManager.ReadGlass();
        Assert.That(tmp7.Count, Is.EqualTo(0), "Список стёкол должен быть пустым");

        tmp7 = DataBaseManager.ReadFallingObjects();
        Assert.That(tmp7.Count, Is.EqualTo(0), "Список падающих объектов должен быть пустым");

        tmp7 = DataBaseManager.ReadExtraHealth();
        Assert.That(tmp7.Count, Is.EqualTo(0), "Список дополнительных жизней должен быть пустым");

        var tmp8 = DataBaseManager.ReadFloods();
        Assert.That(tmp8.Count, Is.EqualTo(0), "Список затоплений должен быть пустым");
    }

    private static IEnumerable<Dictionary<string, List<bool>>> GetDataComb()
    {
        yield return null;
        yield return new Dictionary<string, List<bool>>
        {
            { "gate", new List<bool> { true, true, false } },
            { "glass", new List<bool> { true } },
            { "falling_object", new List<bool>() },
            { "extra_health", new List<bool>() }
        };
        yield return new Dictionary<string, List<bool>>
        {
            { "gate", new List<bool> { false, true, false } },
            { "glass", new List<bool> { false, true } },
            { "falling_object", new List<bool> { true } },
            { "extra_health", new List<bool> { false } }
        };
    }

    private static IEnumerable<List<float>> GetFloodComb()
    {
        yield return null;
        yield return new List<float>
        {
            1, 2, 3, 4
        };
    }

    private static IEnumerable<Dictionary<string, int>> GetInventoryComb()
    {
        yield return null;
        yield return new Dictionary<string, int>
        {
            { "key", 1 }
        };
        yield return new Dictionary<string, int>
        {
            { "money", 2 }
        };
        yield return new Dictionary<string, int>
        {
            { "SphereDash", 1 },
            { "SphereShrink", 1 },
            { "SphereAttack", 1 }

        };
    }

    [Test, Combinatorial, Order(14)]
    public void Test14(
        [Values(3)] int number,
        [Values(true)] bool status,
        [Values("Level3")] string title,
        [Values(2f)] float position_x,
        [Values(0f)] float position_y,
        [Values(2)] int health,
        [Values(false, true)] bool small,
        [ValueSource(nameof(GetDataComb))] Dictionary<string, List<bool>> data = null,
        [ValueSource(nameof(GetFloodComb))] List<float> flood = null,
        [ValueSource(nameof(GetInventoryComb))] Dictionary<string, int> inventory = null)
    {
        DataBaseManager.SaveData(number, status, title, (position_x, position_y), health, small, data, flood, inventory);

        var tmp = DataBaseManager.GetCharacterPosition();
        Assert.That(tmp.Item1, Is.EqualTo(position_x), $"Значение для первого элемента должно быть {position_x}");
        Assert.That(tmp.Item2, Is.EqualTo(position_y), $"Значение для второго элемента должно быть {position_y}");

        var tmp2 = DataBaseManager.GetCharacterHealthAndSmall();
        Assert.That(tmp2.Item1, Is.EqualTo(health), $"Значение для первого элемента должно быть {health}");
        Assert.That(tmp2.Item2, Is.EqualTo(small), $"Значение для второго элемента должно быть {small}");

        var tmp3 = DataBaseManager.GetInfoLevel();
        Assert.That(tmp3.Item1, Is.EqualTo(status ? "в процессе" : "не пройден"), $"Значение для первого элемента должно быть {(status ? "в процессе" : "не пройден")}");
        Assert.That(tmp3.Item2, Is.EqualTo(title), $"Значение для второго элемента должно быть {title}");

        var tmp4 = DataBaseManager.GetStatusLevel();
        Assert.IsInstanceOf<string>(tmp4);
        Assert.That(tmp4, Is.EqualTo(status ? "в процессе" : "не пройден"), $"Возвращаемое значение должно быть {(status ? "в процессе" : "не пройден")}");

        var tmp5 = DataBaseManager.GetNumberLevelForPlayers();
        Assert.That(tmp5.ContainsKey(0), "Словарь должен содержать ключ 0");
        Assert.That(tmp5[0], Is.EqualTo(number), "Значение для ключа 0 должно быть 3");

        var tmp6 = DataBaseManager.GetInventory();
        Assert.That(tmp6.Count, Is.EqualTo(inventory != null ? inventory.Count : 0), $"Словарь инвентаря должен содержать {(inventory != null ? inventory.Count : 0)} записей");

        if (inventory != null)
        {
            foreach (var item in inventory)
            {
                Assert.That(tmp6.ContainsKey(item.Key), $"Ключ {item.Key} отсутствует в словаре инвентаря, полученного из БД");
                Assert.That(tmp6[item.Key], Is.EqualTo(item.Value), $"Значение для ключа {item.Key} в словаре инвентаря не совпадает");
            }
        }

        var tmp7 = DataBaseManager.ReadGates();
        Assert.That(tmp7.Count, Is.EqualTo(data != null ? data["gate"].Count : 0), $"Список врат должен содержать {(data != null ? data["gate"].Count : 0)} элементов");

        for (int i = 0; i < tmp7.Count; i++)
        {
            Assert.That(tmp7[i], Is.EqualTo(data["gate"][i]), $"Элемент {i} в списке врат не совпадает: ожидалось {data["gate"][i]}, получено {tmp7[i]}");
        }

        tmp7 = DataBaseManager.ReadGlass();
        Assert.That(tmp7.Count, Is.EqualTo(data != null ? data["glass"].Count : 0), $"Список стёкол должен содержать {(data != null ? data["glass"].Count : 0)} элементов");

        for (int i = 0; i < tmp7.Count; i++)
        {
            Assert.That(tmp7[i], Is.EqualTo(data["glass"][i]), $"Элемент {i} в списке стёкол не совпадает: ожидалось {data["glass"][i]}, получено {tmp7[i]}");
        }

        tmp7 = DataBaseManager.ReadFallingObjects();
        Assert.That(tmp7.Count, Is.EqualTo(data != null ? data["falling_object"].Count : 0), $"Список падающих объектов должен содержать {(data != null ? data["falling_object"].Count : 0)} элементов");

        for (int i = 0; i < tmp7.Count; i++)
        {
            Assert.That(tmp7[i], Is.EqualTo(data["falling_object"][i]), $"Элемент {i} в списке падающих объектов не совпадает: ожидалось {data["falling_object"][i]}, получено {tmp7[i]}");
        }

        tmp7 = DataBaseManager.ReadExtraHealth();
        Assert.That(tmp7.Count, Is.EqualTo(data != null ? data["extra_health"].Count : 0), $"Список дополнительных жизней должен содержать {(data != null ? data["extra_health"].Count : 0)} элементов");

        for (int i = 0; i < tmp7.Count; i++)
        {
            Assert.That(tmp7[i], Is.EqualTo(data["extra_health"][i]), $"Элемент {i} в списке дополнительных жизней не совпадает: ожидалось {data["extra_health"][i]}, получено {tmp7[i]}");
        }

        var tmp8 = DataBaseManager.ReadFloods();
        Assert.That(tmp8.Count, Is.EqualTo(flood != null ? flood.Count : 0), $"Список затоплений должен содержать {(flood != null ? flood.Count : 0)} элементов");

        for (int i = 0; i < tmp8.Count; i++)
        {
            Assert.That(tmp8[i], Is.EqualTo(flood[i]), $"Элемент {i} в списке затоплений не совпадает: ожидалось {flood[i]}, получено {tmp8[i]}");
        }
    }

    [Test, Order(15)]
    public void Test15()
    {
        DataBaseManager.DeleteCharacter();
        
        var tmp = DataBaseManager.GetNumberLevelForPlayers();
        Assert.That(tmp.ContainsKey(0), Is.False, "Значение для ключа 0 не должно быть в словаре");

        var tmp2 = DataBaseManager.GetInventory();
        Assert.That(tmp2.Count, Is.EqualTo(0), "Словарь инвентаря должен быть пустым");

        var tmp3 = DataBaseManager.ReadGates();
        Assert.That(tmp3.Count, Is.EqualTo(0), "Список врат должен быть пустым");

        tmp3 = DataBaseManager.ReadGlass();
        Assert.That(tmp3.Count, Is.EqualTo(0), "Список стёкол должен быть пустым");

        tmp3 = DataBaseManager.ReadFallingObjects();
        Assert.That(tmp3.Count, Is.EqualTo(0), "Список падающих объектов должен быть пустым");

        tmp3 = DataBaseManager.ReadExtraHealth();
        Assert.That(tmp3.Count, Is.EqualTo(0), "Список дополнительных жизней должен быть пустым");

        var tmp4 = DataBaseManager.ReadFloods();
        Assert.That(tmp4.Count, Is.EqualTo(0), "Список затоплений должен быть пустым");
    }
}