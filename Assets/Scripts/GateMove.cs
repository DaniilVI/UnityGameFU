using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateMove : MonoBehaviour
{
    [SerializeField] private Vector3 startPoint;
    [SerializeField] private Vector3 finishPoint;
    [SerializeField] private float transitTime;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPoint;
    }

    private bool movingToFinish = true;
    private Coroutine moveRoutine;

    public void Move()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        Vector3 target = movingToFinish ? finishPoint : startPoint;
        moveRoutine = StartCoroutine(MoveTo(target));
        movingToFinish = !movingToFinish;
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        Vector3 origin = transform.position;
        float elapsed = 0f;

        while (elapsed < transitTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitTime);
            transform.position = Vector3.Lerp(origin, target, t);
            yield return null;
        }

        transform.position = target;
        moveRoutine = null;
    }
}
