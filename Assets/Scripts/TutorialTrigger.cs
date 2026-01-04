using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private string tutorialID;
    [SerializeField] private bool triggerOnStart = false;
    [SerializeField] private bool destroyAfterTrigger = true;

    void Start()
    {
        if (triggerOnStart)
        {
            TutorialManager.Instance.Show(tutorialID);
            if (destroyAfterTrigger)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        TutorialManager.Instance.Show(tutorialID);

        if (destroyAfterTrigger)
            Destroy(gameObject);
    }
}
