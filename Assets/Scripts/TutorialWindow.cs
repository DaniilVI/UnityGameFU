using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialWindow : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image contentImage;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;
    [SerializeField] private TMP_Text closeHint;

    private Sprite[] images;
    private string[] texts;
    private int page = 0;

    private CharacterMove playerMove;
    private bool isPaused = false;

    void Start()
    {
        playerMove = FindObjectOfType<CharacterMove>();
        leftArrow.onClick.AddListener(PrevPage);
        rightArrow.onClick.AddListener(NextPage);
    }

    void Update()
    {
        if (page == texts.Length - 1 && Input.GetKeyDown(KeyCode.Return))
        {
            Close();
        }
    }

    public void Open(string[] newTexts, Sprite[] newImages)
    {
        texts = newTexts;
        images = newImages;

        gameObject.SetActive(true);
        Time.timeScale = 0f; // Пауза
        if (playerMove != null) playerMove.enabled = false;
        isPaused = true;
        page = 0;
        UpdatePage();
    }

    public void Close()
    {
        Time.timeScale = 1f;
        if (playerMove != null) playerMove.enabled = true;
        isPaused = false;
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

    public bool IsPaused()
    {
        return isPaused;
    }
}
