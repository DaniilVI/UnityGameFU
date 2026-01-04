using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using UnityEngine;


public static class DataBaseManager
{
    private static Dictionary<int, string> statusLevel = new Dictionary<int, string>
    {
        { 0, "не пройден" },
        { 1, "в процессе" }
    };
    private static Dictionary<int, bool> translator = new Dictionary<int, bool>
    {
        { 0, false },
        { 1, true }
    };
    private static string connectionString = "URI=file:" + Application.dataPath + "/StreamingAssets/DataBase.sqlite";
    private static int selectedCharacterId;

    /// <summary>
    /// Установить ID текущего игрока/персонажа.
    /// </summary>
    /// <param name="characterId">ID персонажа.</param>
    public static void setCharacterId(int characterId)
    {
        selectedCharacterId = characterId;
    }

    /// <summary>
    /// Получить статус и название сцены уровня для текущего игрока/персонажа.
    /// </summary>
    /// <returns>
    /// Двумерный кортеж строк: первый элемент — статус уровня; второй элемент — название уровня.
    /// </returns>
    public static (string, string) GetInfoLevel()
    {
        int status = 0;
        string title = string.Empty;

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT status, title FROM LEVEL WHERE CHARACTER_id = @characterId";
                sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);

                try
                {
                    using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        if (sqlite_datareader.Read())
                        {
                            status = sqlite_datareader.GetInt32("status");
                            title = sqlite_datareader.GetString("title");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }

        return (statusLevel[status], title);
    }

    /// <summary>
    /// Получить статус уровня для текущего игрока/персонажа.
    /// </summary>
    /// <returns>
    /// Статус уровня в виде строки.
    /// </returns>
    public static string GetStatusLevel()
    {
        int status = 0;

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT status FROM LEVEL WHERE CHARACTER_id = @characterId";
                sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);

                try
                {
                    using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        if (sqlite_datareader.Read())
                        {
                            status = sqlite_datareader.GetInt32("status");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }

        return statusLevel[status];
    }

    /// <summary>
    /// Получить номера уровней для всех игроков.
    /// </summary>
    /// <returns>
    /// Словарь, где ключом является ID игрока/персонажа, а значением — номер уровня.
    /// </returns>
    public static Dictionary<int, int> GetNumberLevelForPlayers()
    {
        Dictionary<int, int> result = new Dictionary<int, int>();

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT number, CHARACTER_id FROM LEVEL";

                try
                {
                    using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        while (sqlite_datareader.Read())
                        {
                            int number = sqlite_datareader.GetInt32("number");
                            int characterId = sqlite_datareader.GetInt32("CHARACTER_id");

                            result[characterId] = number;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }

        return result;

    }

    /// <summary>
    /// Получить позицию текущего персонажа.
    /// </summary>
    /// <returns>
    /// Двумерный кортеж 32-разрядных числовых типов данных с плавающей точкой: первый элемент — координата x; второй элемент — координата y.
    /// </returns>
    public static (float, float) GetCharacterPosition()
    {
        float x = 0f;
        float y = 0f;

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT position_x, position_y FROM CHARACTER WHERE id = @characterId";
                sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);

                try
                {
                    using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        if (sqlite_datareader.Read())
                        {
                            x = sqlite_datareader.GetFloat("position_x");
                            y = sqlite_datareader.GetFloat("position_y");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }

        return (x, y);
    }

    /// <summary>
    /// Получить здоровье и размер текущего персонажа.
    /// </summary>
    /// <returns>
    /// Двумерный кортеж: первый элемент - здоровье, целое число в диапазоне от 0 до 3 включительно; второй элемент - булево значение, уменьшен ли персонаж.
    /// </returns>
    public static (int, bool) GetCharacterHealthAndSmall()
    {
        int health = 3;
        bool small = false;

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT health, small FROM CHARACTER WHERE id = @characterId";
                sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);

                try
                {
                    using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        if (sqlite_datareader.Read())
                        {
                            health = sqlite_datareader.GetInt32("health");
                            small = translator[sqlite_datareader.GetInt32("small")];
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }

        return (health, small);
    }

    /// <summary>
    /// Получить инвентарь текущего персонажа.
    /// </summary>
    /// <returns>
    /// Словарь, где ключом является название предмета, а значением — количество данного предмета.
    /// </returns>
    public static Dictionary<string, int> GetInventory()
    {
        Dictionary<string, int> result = new Dictionary<string, int>();

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT object_name, count FROM INVENTORY WHERE CHARACTER_id = @characterId";
                sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);

                try
                {
                    using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        while (sqlite_datareader.Read())
                        {
                            string item = sqlite_datareader.GetString("object_name");
                            int count = sqlite_datareader.GetInt32("count");

                            result[item] = count;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Добавить предмет в инвентарь текущего персонажа.
    /// </summary>
    /// <param name="item">Название предмета.</param>
    /// <param name="count">Количество предметов.</param>
    public static void AddItemInventory(string item, int count)
    {
        int lastId = selectedCharacterId * 1000;

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT MAX(id) AS id FROM INVENTORY WHERE CHARACTER_id = @characterId";
                sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);

                try
                {
                    object max = sqlite_cmd.ExecuteScalar();

                    if (max != DBNull.Value)
                    {
                        lastId = Convert.ToInt32(max) + 1;
                    }

                    sqlite_cmd.Parameters.Clear();

                    sqlite_cmd.CommandText = "UPDATE INVENTORY SET count = count + @count WHERE object_name = @objectName AND CHARACTER_id = @characterId";
                    sqlite_cmd.Parameters.AddWithValue("@objectName", item);
                    sqlite_cmd.Parameters.AddWithValue("@count", count);
                    sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);
                    var result = sqlite_cmd.ExecuteNonQuery();

                    if (result == 0)
                    {
                        sqlite_cmd.Parameters.Clear();

                        sqlite_cmd.CommandText = "INSERT INTO INVENTORY (id, object_name, count, CHARACTER_id) VALUES(@id, @objectName, @count, @characterId)";
                        sqlite_cmd.Parameters.AddWithValue("@id", lastId);
                        sqlite_cmd.Parameters.AddWithValue("@objectName", item);
                        sqlite_cmd.Parameters.AddWithValue("@count", count);
                        sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);
                        sqlite_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Удалить предмет из инвентаря текущего персонажа.
    /// </summary>
    /// <param name="item">Название предмета.</param>
    public static void RemoveItemInventory(string item)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                try
                {
                    sqlite_cmd.CommandText = "DELETE FROM INVENTORY WHERE CHARACTER_id = @characterId AND object_name = @objectName";
                    sqlite_cmd.Parameters.AddWithValue("@objectName", item);
                    sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Получить количество определённого предмета в инвентаре текущего персонажа по названию предмета.
    /// </summary>
    /// <param name="item">Название предмета.</param>
    public static int GetInventoryByItemName(string item)
    {
        int count = 0;

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT count FROM INVENTORY WHERE CHARACTER_id = @characterId AND object_name = @objectName";
                sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);
                sqlite_cmd.Parameters.AddWithValue("@objectName", item);

                try
                {
                    using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        if (sqlite_datareader.Read())
                        {
                            count = sqlite_datareader.GetInt32("count");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }

        return count;
    }

    /// <summary>
    /// Шаблон метода чтения из однотипных таблиц для текущего игрока/персонажа.
    /// </summary>
    /// <param name="table">Таблица в БД.</param>
    /// <param name="item">Столбец из указанной таблицы в БД.</param>
    /// <returns>
    /// Список булевых состояний объектов.
    /// </returns>
    private static List<bool> IRead(string table, string column)
    {
        List<bool> result = new List<bool>();

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = $"SELECT id, {column} FROM {table} WHERE CHARACTER_id = @characterId ORDER BY id";
                sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);

                try
                {
                    using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        while (sqlite_datareader.Read())
                        {
                            int raised = sqlite_datareader.GetInt32(column);
                            result.Add(translator[raised]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Чтение таблицы ворот.
    /// </summary>
    /// <returns>
    /// Список булевых состояний врат.
    /// </returns>
    public static List<bool> ReadGates()
    {
        return IRead("GATES", "raised");
    }

    /// <summary>
    /// Чтение таблицы стёкол.
    /// </summary>
    /// <returns>
    /// Список булевых состояний стёкол.
    /// </returns>
    public static List<bool> ReadGlass()
    {
        return IRead("GLASS", "broken");
    }

    /// <summary>
    /// Чтение таблицы затопления.
    /// </summary>
    /// <returns>
    /// Список высот затопления.
    /// </returns>
    public static List<float> ReadFloods()
    {
        List<float> result = new List<float>();

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                sqlite_cmd.CommandText = "SELECT id, height FROM FLOOD WHERE CHARACTER_id = @characterId ORDER BY id";
                sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);

                try
                {
                    using (var sqlite_datareader = sqlite_cmd.ExecuteReader())
                    {
                        while (sqlite_datareader.Read())
                        {
                            float raised = sqlite_datareader.GetFloat("height");
                            result.Add(raised);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Чтение таблицы падающих объектов.
    /// </summary>
    /// <returns>
    /// Список булевых состояний падающих объектов.
    /// </returns>
    public static List<bool> ReadFallingObjects()
    {
        return IRead("FALLING_OBJECT", "fell");
    }

    /// <summary>
    /// Чтение таблицы дополнительной жизни.
    /// </summary>
    /// <returns>
    /// Список булевых состояний дополнительных жизней.
    /// </returns>
    public static List<bool> ReadExtraHealth()
    {
        return IRead("EXTRA_HEALTH", "used");
    }

    /// <summary>
    /// Шаблон метода записи в однотипные таблицы для текущего игрока/персонажа.
    /// </summary>
    /// <param name="values">Список булевых состояний в строго упорядоченном виде.</param>
    /// <param name="table">Таблица в БД.</param>
    /// <param name="item">Столбец из указанной таблицы в БД.</param>
    private static void IWrite(List<bool> values, string table, string column)
    {
        int id = selectedCharacterId * 1000;

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                try
                {
                    foreach (var value in values)
                    {
                        sqlite_cmd.CommandText = $"INSERT INTO {table} (id, {column}, CHARACTER_id) VALUES(@id, @value, @characterId)";
                        sqlite_cmd.Parameters.AddWithValue("@id", id);
                        sqlite_cmd.Parameters.AddWithValue("@value", value ? 1 : 0);
                        sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);
                        sqlite_cmd.ExecuteNonQuery();
                        sqlite_cmd.Parameters.Clear();
                        id++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Запись в таблицу врат.
    /// </summary>
    /// <param name="values">Список булевых состояний в строго упорядоченном виде.</param>
    private static void WriteGates(List<bool> values)
    {
        IWrite(values, "GATES", "raised");
    }

    /// <summary>
    /// Запись в таблицу стёкол.
    /// </summary>
    /// <param name="values">Список булевых состояний в строго упорядоченном виде.</param>
    private static void WriteGlass(List<bool> values)
    {
        IWrite(values, "GLASS", "broken");
    }

    /// <summary>
    /// Запись в таблицу затопления.
    /// </summary>
    /// <param name="values">Список высот затопления в строго упорядоченном виде.</param>
    private static void WriteFloods(List<float> values)
    {
        int id = selectedCharacterId * 1000;

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {try
                {
                    foreach (var value in values)
                    {
                        sqlite_cmd.CommandText = "INSERT INTO FLOOD (id, height, CHARACTER_id) VALUES(@id, @value, @characterId)";
                        sqlite_cmd.Parameters.AddWithValue("@id", id);
                        sqlite_cmd.Parameters.AddWithValue("@value", value);
                        sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);
                        sqlite_cmd.ExecuteNonQuery();
                        sqlite_cmd.Parameters.Clear();
                        id++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Запись в таблицу падающих объектов.
    /// </summary>
    /// <param name="values">Список булевых состояний в строго упорядоченном виде.</param>
    private static void WriteFallingObjects(List<bool> values)
    {
        IWrite(values, "FALLING_OBJECT", "fell");
    }

    /// <summary>
    /// Запись в таблицу дополнительной жизни.
    /// </summary>
    /// <param name="values">Список булевых состояний в строго упорядоченном виде.</param>
    private static void WriteExtraHealth(List<bool> values)
    {
        IWrite(values, "EXTRA_HEALTH", "used");
    }

    /// <summary>
    /// Шаблон метода удаления из таблиц для текущего игрока/персонажа.
    /// </summary>
    /// <param name="table">Таблица в БД.</param>
    private static void IDelete(string table)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                try
                {
                    sqlite_cmd.CommandText = $"DELETE FROM {table} WHERE CHARACTER_id = @characterId";
                    sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Удаление инвентаря.
    /// </summary>
    private static void ClearInventory()
    {
        IDelete("INVENTORY");
    }

    /// <summary>
    /// Удаление врат.
    /// </summary>
    private static void ClearGates()
    {
        IDelete("GATES");
    }

    /// <summary>
    /// Удаление стёкол.
    /// </summary>
    private static void ClearGlass()
    {
        IDelete("GLASS");
    }

    /// <summary>
    /// Удаление затопления.
    /// </summary>
    private static void ClearFloods()
    {
        IDelete("FLOOD");
    }

    /// <summary>
    /// Удаление падающих объектов.
    /// </summary>
    private static void ClearFallingObjects()
    {
        IDelete("FALLING_OBJECT");
    }

    /// <summary>
    /// Удаление дополнительной жизни.
    /// </summary>
    private static void ClearExtraHealth()
    {
        IDelete("EXTRA_HEALTH");
    }

    /// <summary>
    /// Удалить текущего игрока/персонажа.
    /// </summary>
    public static void DeleteCharacter()
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                try
                {
                    sqlite_cmd.CommandText = "PRAGMA foreign_keys = ON;";
                    sqlite_cmd.ExecuteNonQuery();

                    sqlite_cmd.CommandText = "DELETE FROM CHARACTER WHERE id = @characterId";
                    sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Создать нового игрока/персонажа.
    /// </summary>
    /// <param name="characterId">ID игрока/персонажа.</param>
    public static void CreateCharacter(int characterId)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                try
                {
                    sqlite_cmd.CommandText = "INSERT INTO CHARACTER (id, position_x, position_y, health, small) VALUES(@characterId, 0, 0, 3, 0)";
                    sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);
                    sqlite_cmd.ExecuteNonQuery();
                    sqlite_cmd.Parameters.Clear();

                    sqlite_cmd.CommandText = "INSERT INTO LEVEL (id, number, status, title, CHARACTER_id) VALUES(@id, 1, 0, @title, @characterId)";
                    sqlite_cmd.Parameters.AddWithValue("@id", characterId);
                    sqlite_cmd.Parameters.AddWithValue("@title", "Level1");
                    sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Сохранить данные в БД.
    /// </summary>
    /// <param name="number">Номер уровня.</param>
    /// <param name="status">Статус уровня (булев).</param>
    /// <param name="title">Название сцены уровня.</param>
    /// <param name="position">Двумерный кортеж 32-разрядных числовых типов данных с плавающей точкой: первый элемент — координата Ox; второй элемент — координата Oy.</param>
    /// <param name="health">Здоровье в диапазоне от 0 до 3 включительно.</param>
    /// <param name="small">Уменьшен ли персонаж.</param>
    /// <param name="data">Словарь, где ключом является название объекта из перечисления: gate, glass, falling_object, extra_health, а значением — список булевых состояний объектов в строго упорядоченном виде.</param>
    /// <param name="flood">Список высот затоплений в строго упорядоченном виде.</param>
    /// <param name="inventory">Словарь, где ключом является название предмета в инвентаре, а значением — количество данного предмета в инвентаре.</param>
    public static void SaveData(int number, bool status, string title, (float, float) position, int health, bool small = false, Dictionary<string, List<bool>> data = null, List<float> flood = null, Dictionary<string, int> inventory = null)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var sqlite_cmd = conn.CreateCommand())
            {
                try
                {
                    sqlite_cmd.CommandText = $"UPDATE CHARACTER SET position_x = @positionX, position_y = @positionY, health = @health, small = @small WHERE id = @characterId";
                    sqlite_cmd.Parameters.AddWithValue("@positionX", position.Item1);
                    sqlite_cmd.Parameters.AddWithValue("@positionY", position.Item2);
                    sqlite_cmd.Parameters.AddWithValue("@health", health);
                    sqlite_cmd.Parameters.AddWithValue("@small", small ? 1 : 0);
                    sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);
                    sqlite_cmd.ExecuteNonQuery();
                    sqlite_cmd.Parameters.Clear();

                    sqlite_cmd.CommandText = "UPDATE LEVEL SET number = @number, status = @status, title = @title WHERE CHARACTER_id = @characterId";
                    sqlite_cmd.Parameters.AddWithValue("@number", number);
                    sqlite_cmd.Parameters.AddWithValue("@status", status ? 1 : 0);
                    sqlite_cmd.Parameters.AddWithValue("@title", title);
                    sqlite_cmd.Parameters.AddWithValue("@characterId", selectedCharacterId);
                    sqlite_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }

        ClearInventory();
        ClearGates();
        ClearGlass();
        ClearFloods();
        ClearFallingObjects();
        ClearExtraHealth();

        if (status)
        {
            if (data != null)
            {
                if (data.ContainsKey("gate"))
                {
                    WriteGates(data["gate"]);
                }
                if (data.ContainsKey("glass"))
                {
                    WriteGlass(data["glass"]);
                }
                if (data.ContainsKey("falling_object"))
                {
                    WriteFallingObjects(data["falling_object"]);
                }
                if (data.ContainsKey("extra_health"))
                {
                    WriteExtraHealth(data["extra_health"]);
                }
            }

            if (flood != null)
            {
                WriteFloods(flood);
            }

            if (inventory != null)
            {
                foreach (var item in inventory)
                {
                    AddItemInventory(item.Key, item.Value);
                }
            }
        }
    }
}