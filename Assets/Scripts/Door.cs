using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string sceneName = "LevelTest";
    private AudioSource audioSource;
    private Coroutine doorRoutine = null;

    public string SceneName
    {
        get { return sceneName; }
    }

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (doorRoutine != null) return;

        PlayerAbilities pa = other.GetComponent<PlayerAbilities>();
        if (pa == null) return;

        if (pa.Key)
        {
            LoadLevel.isLoad = false;
            if (!sceneName.Equals("EndVideo"))
                LevelDataManager.SaveLevel();
            
            other.GetComponent<CharacterMove>().enabled = false;
            doorRoutine = StartCoroutine(OpenDoor(other));
        }
        else
        {
            // можно проиграть звук закрытой двери или показать подсказку
            Debug.Log("Door is locked. Pick up a key first.");
        }
    }

    private IEnumerator OpenDoor(Collider2D other)
    {
        audioSource.Play();
        
        transform.Find("DoorClosed").gameObject.SetActive(false);
        transform.Find("DoorOpen").gameObject.SetActive(true);

        yield return new WaitWhile(() => audioSource.isPlaying);

        other.GetComponent<CharacterMove>().enabled = true;

        Debug.Log($"Загрузка уровня: {sceneName}");
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch
        {
            Debug.Log("Scene not found.");
        }
    }
}
