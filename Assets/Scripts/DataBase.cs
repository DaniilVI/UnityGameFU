using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.TextCore.Text;


public static class DataBase
{
    static Dictionary<int, string> statusLevel = new Dictionary<int, string>
    {
        { 0, "не пройден" },
        { 1, "в процессе" }
    };
    static Dictionary<int, bool> translator = new Dictionary<int, bool>
    {
        { 0, false },
        { 1, true }
    };
    static string connectionString = "URI=file:" + Application.dataPath + "/StreamingAssets/DataBase.sqlite";
    static SqliteConnection sqliteConn;

    public static void CreateConnection()
    {
        SqliteConnection conn = new SqliteConnection(connectionString);

        try
        {
            conn.Open();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        sqliteConn = conn;
    }

    public static void CloseConnection()
    {
        sqliteConn.Close();
    }

    //TEST
    public static void ReadData()
    {
        SqliteDataReader sqlite_datareader;
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT * FROM CHARACTER";

        try
        {
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            Debug.Log("Содержимое:");
            while (sqlite_datareader.Read())
            {
                for (int i = 0; i < sqlite_datareader.FieldCount; i++)
                {
                    Debug.Log(sqlite_datareader.GetValue(i).ToString() + "\t");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    // Получить статус и сцену уровня по ID игрока
    public static (string, string) GetInfoLevel(int characterId)
    {
        int status = 0;
        string title = string.Empty;
        SqliteDataReader sqlite_datareader;
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT status, title FROM LEVEL WHERE CHARACTER_id = @characterId";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            if (sqlite_datareader.Read())
            {
                status = Convert.ToInt32(sqlite_datareader["status"]);
                title = sqlite_datareader["title"].ToString();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return (statusLevel[status], title);
    }

    // Получить статус уровня по ID игрока
    public static string GetStatusLevel(int characterId)
    {
        int status = 0;
        SqliteDataReader sqlite_datareader;
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT status FROM LEVEL WHERE CHARACTER_id = @characterId";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            if (sqlite_datareader.Read())
            {
                status = Convert.ToInt32(sqlite_datareader["status"]);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return statusLevel[status];
    }

    // Получить номера уровней для всех игроков
    public static Dictionary<int, int> GetNumberLevelForPlayers()
    {
        Dictionary<int, int> result = new Dictionary<int, int>();
        SqliteDataReader sqlite_datareader;
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT number, CHARACTER_id FROM LEVEL";

        try
        {
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                int number = Convert.ToInt32(sqlite_datareader["number"]);
                int characterId = Convert.ToInt32(sqlite_datareader["CHARACTER_id"]);

                result[characterId] = number;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return result;
    }

    // Получить позицию персонажа по ID
    public static (float, float) GetCharacterPosition(int characterId)
    {
        float x = 0f;
        float y = 0f;
        SqliteDataReader sqlite_datareader;
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT position_x, position_y FROM CHARACTER WHERE id = @characterId";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            if (sqlite_datareader.Read())
            {
                x = Convert.ToSingle(sqlite_datareader["position_x"]);
                y = Convert.ToSingle(sqlite_datareader["position_y"]);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return (x, y);
    }

    // Получить здоровье персонажа по ID
    public static int GetCharacterHealth(int characterId)
    {
        int health = 3;
        SqliteDataReader sqlite_datareader;
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT health FROM CHARACTER WHERE id = @characterId";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            if (sqlite_datareader.Read())
            {
                health = Convert.ToInt32(sqlite_datareader["health"]);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return health;
    }

    public static Dictionary<string, int> GetInventory(int characterId)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        SqliteDataReader sqlite_datareader;
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT * FROM INVENTORY WHERE CHARACTER_id = @characterId";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                string item = sqlite_datareader["object_name"].ToString();
                int count = Convert.ToInt32(sqlite_datareader["count"]);

                result[item] = count;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return result;
    }

    // Добавить предмет в инвентарь
    public static void AddItemInventory(int characterId, string item, int count)
    {
        int lastId = characterId * 1000;
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT MAX(id) AS id FROM INVENTORY WHERE CHARACTER_id = @characterId";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            object max = sqlite_cmd.ExecuteScalar();
            if (max != DBNull.Value)
            {
                lastId = Convert.ToInt32(max) + 1;
            }
            sqlite_cmd.Parameters.Clear();

            sqlite_cmd.CommandText = "INSERT INTO INVENTORY (id, object_name, count, CHARACTER_id) VALUES(@id, @objectName, @count, @characterId)";
            sqlite_cmd.Parameters.AddWithValue("@id", lastId);
            sqlite_cmd.Parameters.AddWithValue("@objectName", item);
            sqlite_cmd.Parameters.AddWithValue("@count", count);
            sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    // Удалить предмет из инвентаря
    public static void RemoveItemInventory(int characterId, string item)
    {
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "DELETE FROM INVENTORY WHERE CHARACTER_id = @characterId AND object_name = @objectName";
        sqlite_cmd.Parameters.AddWithValue("@objectName", item);
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    // Получить количество определённого предмета в инвентаре персонажа по ID и названию предмета
    public static int GetInventoryByItemName(int characterId, string item)
    {
        int count = 0;
        SqliteDataReader sqlite_datareader;
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT count FROM INVENTORY WHERE CHARACTER_id = @characterId AND object_name = @objectName";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);
        sqlite_cmd.Parameters.AddWithValue("@objectName", item);

        try
        {
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            if (sqlite_datareader.Read())
            {
                count = Convert.ToInt32(sqlite_datareader["count"]);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return count;
    }

    private static Dictionary<int, bool> IRead(int characterId, string table, string column)
    {
        Dictionary<int, bool> result = new Dictionary<int, bool>();
        SqliteDataReader sqlite_datareader;
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = $"SELECT * FROM {table} WHERE CHARACTER_id = @characterId ORDER BY id";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                int id = Convert.ToInt32(sqlite_datareader["id"]);
                int raised = Convert.ToInt32(sqlite_datareader[column]);

                result[id] = translator[raised];
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return result;
    }

    public static Dictionary<int, bool> ReadGates(int characterId)
    {
        return IRead(characterId, "GATES", "raised");
    }
    
    public static Dictionary<int, bool> ReadGlass(int characterId)
    {
        return IRead(characterId, "GLASS", "broken");
    }
    
    public static Dictionary<int, bool> ReadFloods(int characterId)
    {
        return IRead(characterId, "FLOOD", "height");
    }
    
    public static Dictionary<int, bool> ReadFallingObjects(int characterId)
    {
        return IRead(characterId, "FALLING_OBJECT", "fell");
    }
    
    public static Dictionary<int, bool> ReadExtraHealth(int characterId)
    {
        return IRead(characterId, "EXTRA_HEALTH", "used");
    }

    private static void IWrite(int characterId, List<bool> values, string table, string column)
    {
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = $"DELETE FROM {table} WHERE CHARACTER_id = @characterId";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Parameters.Clear();
            int id = characterId * 1000;

            foreach (var value in values)
            {
                sqlite_cmd.CommandText = $"INSERT INTO {table} (id, {column}, CHARACTER_id) VALUES(@id, @value, @characterId)";
                sqlite_cmd.Parameters.AddWithValue("@id", id);
                sqlite_cmd.Parameters.AddWithValue("@value", value ? 1 : 0);
                sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);
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

    private static void WriteGates(int characterId, List<bool> values)
    {
        IWrite(characterId, values, "GATES", "raised");
    }

    private static void WriteGlass(int characterId, List<bool> values)
    {
        IWrite(characterId, values, "GLASS", "broken");
    }

    private static void WriteFloods(int characterId, List<bool> values)
    {
        IWrite(characterId, values, "FLOOD", "height");
    }

    private static void WriteFallingObjects(int characterId, List<bool> values)
    {
        IWrite(characterId, values, "FALLING_OBJECT", "fell");
    }

    private static void WriteExtraHealth(int characterId, List<bool> values)
    {
        IWrite(characterId, values, "EXTRA_HEALTH", "used");
    }

    private static void IDelete(int characterId, string table)
    {
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = $"DELETE FROM {table} WHERE CHARACTER_id = @characterId";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private static void ClearInventory(int characterId)
    {
        IDelete(characterId, "INVENTORY");
    }

    private static void ClearGates(int characterId)
    {
        IDelete(characterId, "GATES");
    }

    private static void ClearGlass(int characterId)
    {
        IDelete(characterId, "GLASS");
    }

    private static void ClearFloods(int characterId)
    {
        IDelete(characterId, "FLOOD");
    }

    private static void ClearFallingObjects(int characterId)
    {
        IDelete(characterId, "FALLING_OBJECT");
    }

    private static void ClearExtraHealth(int characterId)
    {
        IDelete(characterId, "EXTRA_HEALTH");
    }

    public static void DeleteCharacter(int characterId)
    {
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "DELETE FROM CHARACTER WHERE id = @characterId";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public static void CreateCharacter(int characterId)
    {
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "INSERT INTO CHARACTER (id, position_x, position_y, health) VALUES(@characterId, 0, 0, 3)";
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Parameters.Clear();

            sqlite_cmd.CommandText = "INSERT INTO LEVEL (id, number, status, title, CHARACTER_id) VALUES(@id, 1, 0, Level1, @characterId)";
            sqlite_cmd.Parameters.AddWithValue("@id", characterId);
            sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);
            sqlite_cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public static void SaveData(int characterId, int number, bool status, string title, (float, float) position, int health, Dictionary<string, List<bool>> data = null, Dictionary<string, int> inventory = null)
    {
        SqliteCommand sqlite_cmd = sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = $"UPDATE CHARACTER SET position_x = @positionX, position_y = @positionY, health = @health WHERE id = @characterId";
        sqlite_cmd.Parameters.AddWithValue("@positionX", position.Item1);
        sqlite_cmd.Parameters.AddWithValue("@positionY", position.Item2);
        sqlite_cmd.Parameters.AddWithValue("@health", health);
        sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);

        try
        {
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.Parameters.Clear();

            sqlite_cmd.CommandText = "UPDATE LEVEL SET number = @number, status = @status, title = @title WHERE CHARACTER_id = @characterId";
            sqlite_cmd.Parameters.AddWithValue("@number", number);
            sqlite_cmd.Parameters.AddWithValue("@status", status);
            sqlite_cmd.Parameters.AddWithValue("@title", title);
            sqlite_cmd.Parameters.AddWithValue("@characterId", characterId);
            sqlite_cmd.ExecuteNonQuery();

            ClearInventory(characterId);

            if (status && data != null) // если уровень в процессе
            {
                WriteGates(characterId, data["gate"]);
                WriteGlass(characterId, data["glass"]);
                WriteFloods(characterId, data["flood"]);
                WriteFallingObjects(characterId, data["falling_object"]);
                WriteExtraHealth(characterId, data["extra_health"]);

                if (inventory != null)
                {
                    foreach (var item in inventory)
                    {
                        AddItemInventory(characterId, item.Key, item.Value);
                    }
                }
            }
            else
            {
                ClearGates(characterId);
                ClearGlass(characterId);
                ClearFloods(characterId);
                ClearFallingObjects(characterId);
                ClearExtraHealth(characterId);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
}