using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ButtonPress : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float releaseDelay = 0.1f; // задержка до "сброса"
    [SerializeField] private GameObject gates;
    [SerializeField] private AudioClip pressSound;

    private Transform buttonUp;
    private Transform buttonDown;

    private Coroutine releaseRoutine = null;
    private bool canClick = true;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

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

        Press();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (releaseRoutine != null) StopCoroutine(releaseRoutine);
        releaseRoutine = StartCoroutine(ReleaseAfterDelay());
    }

    private void Press()
    {
        if (releaseRoutine != null)
        {
            StopCoroutine(releaseRoutine);
            releaseRoutine = null;
        }
        
        if (!canClick) return;
        canClick = false;

        if (buttonUp != null && buttonDown != null)
        {
            buttonUp.gameObject.SetActive(false);
            buttonDown.gameObject.SetActive(true);
        }

        if (pressSound) AudioSource.PlayClipAtPoint(pressSound, transform.position);

        // Запускаем движение для всех привязанных ворот
        foreach (Transform gate in gates.transform)
        {
            GateMove script = gate.GetComponent<GateMove>();
            if (script != null)
            {
                script.Move();
            }  
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

        canClick = true;
        releaseRoutine = null;
    }
}
