using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class SpherePickup : MonoBehaviour
{
    [Tooltip("Тип способности: 0 - Dash, 1 - Shrink, 2 - Attack")]
    [SerializeField] public int abilityType = 0;
    [SerializeField] private string playerTag = "Player";
    private AudioSource audioSource;
    private Coroutine sphereRoutine = null;


    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (sphereRoutine != null) return;

        PlayerAbilities pa = other.GetComponent<PlayerAbilities>();
        if (pa == null) return;

        pa.GrantAbility(abilityType);

        sphereRoutine = StartCoroutine(SetSphere());
    }

    // Вызывать извне (Beam) чтобы снова сделать сферу доступной
    public void Respawn()
    {
        gameObject.transform.parent.gameObject.SetActive(true);
    }

    private IEnumerator SetSphere()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -50;
        
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
        
        // Деактивируем сферу (вместо Destroy) — потом Beam может вызвать Respawn
        gameObject.transform.parent.gameObject.SetActive(false);
        spriteRenderer.sortingOrder = 0;
        sphereRoutine = null;
    }
}
