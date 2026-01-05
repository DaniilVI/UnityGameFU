using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GateMove;

public class GateMove : MonoBehaviour
{
    [SerializeField] private Vector3 startPoint;
    [SerializeField] private Vector3 finishPoint;
    [SerializeField] private float transitTime;
    private bool movingToFinish;

    void Awake()
    {
        movingToFinish = true;

    }

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = startPoint;
    }

    private Coroutine moveRoutine;

    public bool Status
    {
        get { return movingToFinish; }
        set { movingToFinish = value; }
    }

    public Vector3 Position
    {
        set { transform.position = value; }
    }

    public Vector3 StartPoint
    {
        get { return startPoint; }
    }
    
    public Vector3 FinishPoint
    {
        get { return finishPoint; }
    }

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