using UnityEngine;

public class CharacterMove : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 13f;
    private bool isJump = false;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Vector3 inputDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        Run();
        CheckJump();
    }

    // Update is called once per frame
    void Update()
    {
        CheckRun();
        if (!isJump && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void Run()
    {
        Vector3 newVelocity = inputDirection * speed;
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;
    }

    private void CheckRun()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        inputDirection = new Vector3(moveHorizontal, 0.0f, 0.0f).normalized;

        if (inputDirection.x != 0)
        {
            sprite.flipX = inputDirection.x < 0;
        }
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckJump()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isJump = collider.Length <= 1;
    }
}
