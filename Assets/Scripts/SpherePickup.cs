using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpherePickup : MonoBehaviour
{
    [Tooltip("Тип способности: 0 - Dash, 1 - Shrink, 2 - Attack")]
    [SerializeField] public int abilityType = 0;
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
        if (pa == null) return;

        pa.GrantAbility(abilityType);

        if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // Деактивируем сферу (вместо Destroy) — потом Beam может вызвать Respawn
        gameObject.SetActive(false);
    }

    // Вызывать извне (Beam) чтобы снова сделать сферу доступной
    public void Respawn()
    {
        gameObject.SetActive(true);
    }
}
