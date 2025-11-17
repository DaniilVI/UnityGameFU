using UnityEngine;

public class SpecialPlatformMove : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Vector3 startPoint;
    [SerializeField] private Vector3 finishPoint;
    [SerializeField] private float transitTime = 1f;

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;

    private float elapsed = 0f;
    private bool moving = false;
    private bool finished = false;
    private bool justStopped = false;

    private Vector3 prevPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        transform.position = startPoint;
        prevPos = startPoint;
    }

    void FixedUpdate()
    {
        if (moving && !finished)
            MovePlatform();

        if (playerRb != null)
        {
            Vector3 delta = transform.position - prevPos;
            playerRb.position += new Vector2(delta.x, 0f);

            if (justStopped)
            {
                Vector3 v = playerRb.velocity;
                v.y = 0f;
                playerRb.velocity = v;
                justStopped = false;
            }
        }

        prevPos = transform.position;
    }

    private void MovePlatform()
    {
        elapsed += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(elapsed / transitTime);

        Vector3 newPos = Vector3.Lerp(startPoint, finishPoint, t);
        rb.MovePosition(newPos);

        if (t >= 1f)
        {
            finished = true;
            moving = false;
            justStopped = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(playerTag))
        {
            playerRb = col.gameObject.GetComponent<Rigidbody2D>();
            moving = true;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(playerTag))
            playerRb = null;
    }

    [ContextMenu("Reset Platform")]
    public void ResetPlatform()
    {
        rb.position = startPoint;
        elapsed = 0f;
        moving = false;
        finished = false;
        justStopped = false;
        prevPos = startPoint;
    }
}
