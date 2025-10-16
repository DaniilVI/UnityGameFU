using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpherePickup : MonoBehaviour
{
    [Tooltip("Тип способности: 0 - Dash, 1 - Shrink, 2 - Attack")]
    [SerializeField] public int abilityType = 0;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private AudioClip pickupSound;

    // Сохраняем начальное состояние, чтобы можно было восстановить объект
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
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
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
        gameObject.SetActive(true);
    }
}
