using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Beam : MonoBehaviour
{
    [Tooltip("Тип способности, которую этот луч деактивирует")]
    [SerializeField] public int abilityType = 0;
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Ссылка на ту самую SpherePickup, которую нужно вернуть при потере способности")]
    [SerializeField] private SpherePickup targetSphereToRespawn;

    [SerializeField] private AudioClip deactivateSound;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        PlayerAbilities pa = other.GetComponent<PlayerAbilities>();
        if (pa == null) return;
        if (!pa.HasAbility(abilityType)) return;

        CharacterMove cm = other.GetComponent<CharacterMove>();
        if (cm == null) return;

        // Снимаем способность
        pa.RevokeAbility(abilityType);
        if (abilityType == 1) cm.GrowToNormal();
        if (deactivateSound) AudioSource.PlayClipAtPoint(deactivateSound, transform.position);

        // Возрождаем сферу (если назначена)
        if (targetSphereToRespawn != null)
        {
            targetSphereToRespawn.Respawn();
        }
    }
}
