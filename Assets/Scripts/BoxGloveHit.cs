using UnityEngine;

public class BoxGloveHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Glass"))
        {
            Debug.Log("Удар по стеклу: " + other.name);
            other.gameObject.SetActive(false);
        }
    }
}
