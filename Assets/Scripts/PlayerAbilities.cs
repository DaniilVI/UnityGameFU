using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public enum Ability { Dash = 0, Shrink = 1, Attack = 2 }

    [Header("State")]
    public bool hasKey = false;

    // Используем массив булевых флагов для лёгкого индексирования по типу сферы
    private bool[] abilities = new bool[3];

    // --- API ---
    public void GrantAbility(int abilityType)
    {
        if (abilityType < 0 || abilityType >= abilities.Length) return;
        abilities[abilityType] = true;
        Debug.Log($"Ability {abilityType} granted");
        // Здесь можно вызывать события, анимации и т.д.
    }

    public void RevokeAbility(int abilityType)
    {
        if (abilityType < 0 || abilityType >= abilities.Length) return;
        abilities[abilityType] = false;
        Debug.Log($"Ability {abilityType} revoked");
    }

    public bool HasAbility(int abilityType)
    {
        if (abilityType < 0 || abilityType >= abilities.Length) return false;
        return abilities[abilityType];
    }

    public void GiveKey()
    {
        hasKey = true;
        Debug.Log("Key picked up");
    }

    // Для отладки: показать все активные способности
    public string DebugAbilities()
    {
        return $"Key={hasKey}, Dash={abilities[(int)Ability.Dash]}, Shrink={abilities[(int)Ability.Shrink]}, Attack={abilities[(int)Ability.Attack]}";
    }
}

