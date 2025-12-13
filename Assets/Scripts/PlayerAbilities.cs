using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    public enum Ability { Dash = 0, Shrink = 1, Attack = 2 }

    [Header("State")]
    [SerializeField] private bool hasKey = false;

    [Header("UI")]
    [SerializeField] private Image[] abilityIcons;
    [SerializeField] private Sprite[] inactiveSprites;
    [SerializeField] private Sprite[] activeSprites;
    [SerializeField] private GameObject keyIcon;

    private bool hasMoney = false;

    public bool Key
    {
        get { return hasKey; }
        set { hasKey = value; }
    }

    public bool Money
    { 
        get { return hasMoney; }
        set { hasMoney = value; }
    }

    // Используем массив булевых флагов для лёгкого индексирования по типу сферы
    private bool[] abilities = new bool[3];

    private void Awake()
    {
        UpdateUI();
    }

    // --- API ---
    public void GrantAbility(int abilityType)
    {
        if (abilityType < 0 || abilityType >= abilities.Length) return;
        abilities[abilityType] = true;
        Debug.Log($"Ability {abilityType} granted");
        // Здесь можно вызывать события, анимации и т.д.
        UpdateUI();
    }

    public void RevokeAbility(int abilityType)
    {
        if (abilityType < 0 || abilityType >= abilities.Length) return;
        abilities[abilityType] = false;
        Debug.Log($"Ability {abilityType} revoked");
        UpdateUI();
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
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            if (i >= abilityIcons.Length) break;

            abilityIcons[i].sprite = abilities[i]
                ? activeSprites[i]
                : inactiveSprites[i];
        }

        keyIcon.SetActive(hasKey);
    }

    // Для отладки: показать все активные способности
    public string DebugAbilities()
    {
        return $"Key={hasKey}, Dash={abilities[(int)Ability.Dash]}, Shrink={abilities[(int)Ability.Shrink]}, Attack={abilities[(int)Ability.Attack]}";
    }
}

