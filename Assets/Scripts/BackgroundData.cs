using UnityEngine;

public class BackgroundData : MonoBehaviour
{
    public static BackgroundData Instance;

    public int selectedBackgroundID = -1;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
