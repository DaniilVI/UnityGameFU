using UnityEngine;

public class NoGrowZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterMove cm = other.GetComponent<CharacterMove>();
            cm?.SetCanGrow(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterMove cm = other.GetComponent<CharacterMove>();
            cm?.SetCanGrow(true);
        }
    }
}
