using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ButtonPress : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Sprite spritePressed;
    [SerializeField] private Sprite spriteUnpressed;
    [SerializeField] private float releaseDelay = 0.1f; // задержка до "сброса"
    [SerializeField] private GateMover[] gatesToToggle;
    [SerializeField] private AudioClip pressSound;

    private SpriteRenderer srFallback;
    private Transform buttonUp;
    private Transform buttonDown;

    private GameObject currentPlayer;
    private Coroutine releaseRoutine;

    private void Awake()
    {
        // заставляем коллайдер быть триггером (если он есть)
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // ищем дочерние объекты ButtonUp / ButtonDown (если такие есть)
        buttonUp = transform.Find("ButtonUp");
        buttonDown = transform.Find("ButtonDown");

        if (buttonUp != null && buttonDown != null)
        {
            // если оба дочерних найдены — считаем, что визуал реализован через включение/выключение
            // по умолчанию включим "up" и выключим "down"
            buttonUp.gameObject.SetActive(true);
            buttonDown.gameObject.SetActive(false);
            Debug.Log($"ButtonPress: found ButtonUp and ButtonDown children on '{name}'. Using active-state toggle.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (currentPlayer == null) // впервые вошёл
        {
            currentPlayer = other.gameObject;
            Press();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (other.gameObject == currentPlayer)
        {
            currentPlayer = null;
            if (releaseRoutine != null) StopCoroutine(releaseRoutine);
            releaseRoutine = StartCoroutine(ReleaseAfterDelay());
        }
    }

    private void Press()
    {
        if (releaseRoutine != null) { StopCoroutine(releaseRoutine); releaseRoutine = null; }

        // Вариант 1: переключаем дочерние объекты
        if (buttonUp != null && buttonDown != null)
        {
            buttonUp.gameObject.SetActive(false);
            buttonDown.gameObject.SetActive(true);
        }

        if (pressSound) AudioSource.PlayClipAtPoint(pressSound, transform.position);

        // Запускаем движение для всех привязанных ворот
        foreach (var g in gatesToToggle)
        {
            if (g != null)
                g.ToggleMove();
        }
    }

    private IEnumerator ReleaseAfterDelay()
    {
        yield return new WaitForSeconds(releaseDelay);

        // Сбрасываем в ненажатое состояние
        if (buttonUp != null && buttonDown != null)
        {
            buttonUp.gameObject.SetActive(true);
            buttonDown.gameObject.SetActive(false);
        }

        releaseRoutine = null;
    }
}
