using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    [SerializeField] private Vector3 startPoint;
    [SerializeField] private Vector3 finishPoint;
    [SerializeField] private float transitTime;
    [SerializeField] private float waitingTime;

    private float elapsedTime = 0f;
    private bool movingToFinish = true;
    private float waitTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = startPoint;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / transitTime;

        if (movingToFinish)
        {
            transform.position = Vector3.Lerp(startPoint, finishPoint, t);
        }
        else
        {
            transform.position = Vector3.Lerp(finishPoint, startPoint, t);
        }

        if (elapsedTime >= transitTime)
        {
            waitTimer = waitingTime;
            elapsedTime = 0f;
            movingToFinish = !movingToFinish;
        }
    }
}
