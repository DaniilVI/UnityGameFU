using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string sceneName = "LevelTest";
    [SerializeField] private AudioClip openSound;

    public string SceneName
    {
        get { return sceneName; }
    }

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        PlayerAbilities pa = other.GetComponent<PlayerAbilities>();
        if (pa == null) return;

        if (pa.Key)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (openSound) AudioSource.PlayClipAtPoint(openSound, transform.position);
            if (scene.IsValid())
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.Log("Scene not found.");
            }
        }
        else
        {
            // можно проиграть звук закрытой двери или показать подсказку
            Debug.Log("Door is locked. Pick up a key first.");
        }
    }
}
