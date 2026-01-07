using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tutorial;
    [SerializeField] private bool triggerOnStart = false;

    private bool isTriggered = false;

    void Start()
    {
        if (triggerOnStart)
        {
            isTriggered = true;
            tutorial.GetComponent<TutorialWindow>().Open();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isTriggered) return;

        tutorial.GetComponent<TutorialWindow>().Open();
    }
}
