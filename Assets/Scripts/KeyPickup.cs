using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyPickup : MonoBehaviour
{
    [Tooltip("Tag игрока, который может подбирать ключ")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private AudioClip pickupSound;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        PlayerAbilities pa = other.GetComponent<PlayerAbilities>();
        if (pa != null)
        {
            pa.GiveKey();
            CharacterMove cm = other.GetComponent<CharacterMove>();
            cm.Position = cm.transform.position;
            if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            gameObject.SetActive(false); // убираем ключ из сцены
        }
    }
}
