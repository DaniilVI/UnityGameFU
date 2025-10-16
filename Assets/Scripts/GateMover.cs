using UnityEngine;
using System.Collections;

public class GateMover : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Movement")]
    [SerializeField] private float moveTime = 1f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine moveRoutine;
    private bool atStart = true;
    private const float EPS = 0.01f;

    /// <summary>
    /// Переключает ворота между стартом и концом.
    /// </summary>
    public void ToggleMove()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogWarning($"GateMover '{name}': startPoint или endPoint не назначены.");
            return;
        }

        // Остановим текущее движение, если оно было
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        Vector3 target = atStart ? endPoint.position : startPoint.position;
        Debug.Log($"GateMover '{name}': ToggleMove. atStart={atStart}. Target={(atStart ? "End" : "Start")}");
        moveRoutine = StartCoroutine(MoveTo(target));
        atStart = !atStart;
    }

    /// <summary>
    /// Двигает ворота к указанной точке.
    /// </summary>
    private IEnumerator MoveTo(Vector3 target)
    {
        Vector3 origin = transform.position;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveTime);
            float curvedT = moveCurve.Evaluate(t);
            transform.position = Vector3.Lerp(origin, target, curvedT);
            yield return null;
        }

        transform.position = target;
        // atStart = Vector3.Distance(transform.position, startPoint.position) < EPS;
        moveRoutine = null;

        Debug.Log($"GateMover '{name}': Movement complete. atStart={atStart}");
    }

    /// <summary>
    /// Принудительно установить в начальное положение.
    /// </summary>
    public void SetToStart()
    {
        if (startPoint == null) return;
        transform.position = startPoint.position;
        atStart = true;

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }

    /// <summary>
    /// Принудительно установить в конечное положение.
    /// </summary>
    public void SetToEnd()
    {
        if (endPoint == null) return;
        transform.position = endPoint.position;
        atStart = false;

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }

    /// <summary>
    /// Проверка — ворота на старте?
    /// </summary>
    public bool IsAtStart() => atStart;

    /// <summary>
    /// Проверка — ворота в процессе движения?
    /// </summary>
    public bool IsMoving() => moveRoutine != null;
}
