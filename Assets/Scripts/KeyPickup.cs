using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class KeyPickup : MonoBehaviour
{
    [Tooltip("Tag игрока, который может подбирать ключ")]
    [SerializeField] private string playerTag = "Player";
    private AudioSource audioSource;
    private Coroutine keyRoutine = null;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (keyRoutine != null) return;

        PlayerAbilities pa = other.GetComponent<PlayerAbilities>();
        if (pa != null)
        {
            pa.GiveKey();
            CharacterMove cm = other.GetComponent<CharacterMove>();
            cm.Position = cm.transform.position;
            keyRoutine = StartCoroutine(SetKey());
        }
    }

    private IEnumerator SetKey()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -50;
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
        gameObject.SetActive(false); // убираем ключ из сцены
        spriteRenderer.sortingOrder = 0;
        keyRoutine = null;
    }
}
