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
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private Vector3 prevPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = startPoint;
        prevPos = startPoint;
    }

    void FixedUpdate()
    {
        Move();

        if (playerRb != null)
        {
            Vector3 delta = transform.position - prevPos;
            playerRb.position += new Vector2(delta.x, 0f);
        }

        prevPos = transform.position;
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
            rb.MovePosition(Vector3.Lerp(startPoint, finishPoint, t));
        }
        else
        {
            rb.MovePosition(Vector3.Lerp(finishPoint, startPoint, t));
        }

        if (elapsedTime >= transitTime)
        {
            waitTimer = waitingTime;
            elapsedTime = 0f;
            movingToFinish = !movingToFinish;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = null;
        }
    }
}
