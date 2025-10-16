using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Если true — загрузит следующий уровень по индексу. Если нет — перезагрузит текущий.")]
    [SerializeField] private bool loadNextScene = false;
    [SerializeField] private AudioClip openSound;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        PlayerAbilities pa = other.GetComponent<PlayerAbilities>();
        if (pa == null) return;

        if (pa.hasKey)
        {
            if (openSound) AudioSource.PlayClipAtPoint(openSound, transform.position);
            if (loadNextScene)
            {
                int next = SceneManager.GetActiveScene().buildIndex + 1;
                if (next < SceneManager.sceneCountInBuildSettings)
                    SceneManager.LoadScene(next);
                else
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // fallback
            }
            else
            {
                // временно — перезагрузим текущую сцену (или можно использовать тот же индекс)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            // можно проиграть звук закрытой двери или показать подсказку
            Debug.Log("Door is locked. Pick up a key first.");
        }
    }
}
