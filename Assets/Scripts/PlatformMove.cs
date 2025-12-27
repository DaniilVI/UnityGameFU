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
    private bool justStopped = false;

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private CharacterMove cm;
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
            if (justStopped)
            {
                Vector3 v = playerRb.velocity;
                v.y = 0f;
                playerRb.velocity = v;
                justStopped = false;
            }
            else
            {
                Vector3 delta = transform.position - prevPos;
                playerRb.position += new Vector2(delta.x, 0f);
            }
        }

        prevPos = transform.position;
    }

    private void Move()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;;
            return;
        }

        elapsedTime += Time.fixedDeltaTime;;
        float t = Mathf.Clamp01(elapsedTime / transitTime);

        if (movingToFinish)
        {
            rb.MovePosition(Vector3.Lerp(startPoint, finishPoint, t));
        }
        else
        {
            rb.MovePosition(Vector3.Lerp(finishPoint, startPoint, t));
        }

        if (t >= 1f)
        {
            waitTimer = waitingTime;
            elapsedTime = 0f;
            movingToFinish = !movingToFinish;
            justStopped = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Vector2.Dot(contact.normal, Vector2.down) > 0.5f)
                {
                    playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    cm = collision.gameObject.GetComponent<CharacterMove>();
                    cm.isPlatform = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = null;

            if (cm)
            {
                cm.isPlatform = false;
            }
        }
    }
}
