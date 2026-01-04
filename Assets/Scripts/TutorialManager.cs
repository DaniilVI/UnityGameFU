using UnityEngine;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private TutorialWindow tutorialWindowPrefab;
    [SerializeField] private TutorialData[] tutorials;

    private TutorialWindow window;
    private Dictionary<string, TutorialData> dataMap = new();
    private HashSet<string> shown = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var t in tutorials)
        {
            dataMap[t.id] = t;
        }
    }

    void Start()
    {
        window = Instantiate(tutorialWindowPrefab);
        DontDestroyOnLoad(window.gameObject);
        window.gameObject.SetActive(false);
    }

    public void Show(string id)
    {
        if (shown.Contains(id)) return;
        if (!dataMap.ContainsKey(id)) return;

        shown.Add(id);

        var data = dataMap[id];
        window.Open(data.texts, data.images);
    }

    public bool IsPaused()
    {
        if (Instance != null)
        {
            return window.IsPaused();
        }
        return false;
    }

    public void ResetTutorials()
    {
        window.Close();
        shown.Clear();
    }
}
