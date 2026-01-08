using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Beam : MonoBehaviour
{
    [Tooltip("Тип способности, которую этот луч деактивирует")]
    [SerializeField] public int abilityType = 0;
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Ссылка на ту самую SpherePickup, которую нужно вернуть при потере способности")]
    [SerializeField] private SpherePickup targetSphereToRespawn;

    private AudioSource audioSource;
    private Coroutine BeamRoutine = null;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (BeamRoutine != null) return;

        PlayerAbilities pa = other.GetComponent<PlayerAbilities>();
        if (pa == null) return;
        if (!pa.HasAbility(abilityType)) return;

        CharacterMove cm = other.GetComponent<CharacterMove>();
        if (cm == null) return;

        // Снимаем способность
        pa.RevokeAbility(abilityType);
        if (abilityType == 1) cm.GrowToNormal();

        // Возрождаем сферу (если назначена)
        if (targetSphereToRespawn != null)
        {
            targetSphereToRespawn.Respawn();
        }

        BeamRoutine = StartCoroutine(UnsetSphere());
    }

    private IEnumerator UnsetSphere()
    {
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
        BeamRoutine = null;
    }
}
