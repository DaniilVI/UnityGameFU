using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialWindow : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private Image contentImage;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private Sprite[] images;
    [SerializeField] private string[] texts;
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;
    [SerializeField] private TMP_Text closeHint;

    public static bool enableTutorial = true;
    private PauseMenu pauseMenu = null;
    private int page = 0;

    void Start()
    {
        leftArrow.onClick.AddListener(PrevPage);
        rightArrow.onClick.AddListener(NextPage);
    }

    void Update()
    {
        if (page == texts.Length - 1 && Input.GetKeyDown(KeyCode.Return) && !pauseMenu.IsPaused)
        {
            Close();
        }
    }

    public void Open()
    {
        if (!enableTutorial) return;

        enableTutorial = false;
        
        pauseMenu = GameObject.Find("PauseManager").GetComponent<PauseMenu>();
        if (pauseMenu == null)
        {
            Debug.Log("PauseMenu not found");
            return;
        }
        pauseMenu.IsTutorial = true;
        gameObject.SetActive(true);
        page = 0;
        UpdatePage();
    }

    public void Close()
    {
        pauseMenu.IsTutorial = false;
        gameObject.SetActive(false);
    }

    private void NextPage()
    {
        page++;
        UpdatePage();
    }

    private void PrevPage()
    {
        page--;
        UpdatePage();
    }

    private void UpdatePage()
    {
        contentText.text = texts[page];
        contentImage.sprite = images[page];

        leftArrow.gameObject.SetActive(page > 0);
        rightArrow.gameObject.SetActive(page < texts.Length - 1);

        closeHint.gameObject.SetActive(page == texts.Length - 1);
    }

}
