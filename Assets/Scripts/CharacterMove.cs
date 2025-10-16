using System.Collections;
using System.Linq;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private GameObject spriteObject;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 13f;

    [Header("Рывок (Dash)")]
    [SerializeField] private float dashForce = 60f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Удар")]
    [SerializeField] private GameObject boxGloveObject;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    private bool canAttack = true;
    private bool isJump = false;
    private bool isSmall = false;
    private bool isFrozen = false;
    private bool direction;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private SpriteRenderer boxGloveSprite;
    private Vector3 inputDirection;
    private PolygonCollider2D polyCollider;
    private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        if (spriteObject == null)
        {
            Debug.LogError("Sprite object is not assigned!");
            return;
        }
        if (boxGloveObject == null)
        {
            Debug.LogError("Box Glove object is not assigned!");
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        polyCollider = GetComponent<PolygonCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = spriteObject.GetComponent<SpriteRenderer>();
        boxGloveSprite = boxGloveObject.GetComponentInChildren<SpriteRenderer>();
        boxGloveObject.SetActive(false);
        direction = sprite.flipX;
        boxCollider.enabled = false;
        polyCollider.enabled = true;
    }

    void FixedUpdate()
    {
        if (!isFrozen && !isDashing)
        {
            Run();
        }
        CheckJump();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (isDashing) return;
        if (isFrozen) return;

        CheckRun();
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isJump)
        {
            ToggleSize();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Attack());
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
            boxGloveSprite.flipX = inputDirection.x < 0;

            boxCollider.enabled = true;
            polyCollider.enabled = false;
            FlipBoxCollider();
            FlipPolygonCollider();

            Vector3 localPos = boxGloveObject.transform.localPosition;
            if (boxGloveSprite.flipX)
            {
                localPos.x = -Mathf.Abs(localPos.x);
            }
            else
            {
                localPos.x = Mathf.Abs(localPos.x);
            }
            boxGloveObject.transform.localPosition = localPos;
        }
        else
        {
            boxCollider.enabled = false;
            polyCollider.enabled = true;
        }
        direction = sprite.flipX;
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckJump()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.1f).Where(c => !c.isTrigger).ToArray();
        isJump = collider.Length <= 1;
    }

    private IEnumerator Dash()
    {
        if (!canDash) yield break;

        canDash = false;
        isDashing = true;

        float origGravity = rb.gravityScale;
        float origDrag = rb.drag;

        float direction = sprite.flipX ? -1f : 1f;;

        rb.gravityScale = 0f;
        rb.drag = 8f;
        rb.velocity = new Vector3(direction * dashForce, 0f, 0f);

        boxCollider.enabled = true;
        polyCollider.enabled = false;

        yield return new WaitForSeconds(dashDuration);

        boxCollider.enabled = false;
        polyCollider.enabled = true;
        
        rb.velocity = Vector3.zero;

        rb.drag = origDrag;
        rb.gravityScale = origGravity;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void ToggleSize()
    {
        if (isSmall)
        {
            transform.localScale *= 2f;
            isSmall = false;
        }
        else
        {
            transform.localScale *= 0.5f;
            isSmall = true;
        }
    }

    private IEnumerator Attack()
    {
        if (!canAttack)
            yield break;
        canAttack = false;

        isFrozen = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        boxCollider.enabled = true;
        polyCollider.enabled = false;
        boxGloveObject.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        boxCollider.enabled = false;
        polyCollider.enabled = true;
        boxGloveObject.SetActive(false);

        rb.isKinematic = false;
        isFrozen = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void FlipPolygonCollider()
    {
        if (direction != sprite.flipX)
        {
            Vector2[] points = polyCollider.points;
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector2(-points[i].x, points[i].y);
            }
            polyCollider.points = points;
        }
    }
    
    private void FlipBoxCollider()
    {
        Vector2 offset = boxCollider.offset;
        offset.x = sprite.flipX ? -Mathf.Abs(offset.x) : Mathf.Abs(offset.x);
        boxCollider.offset = offset;
    }
}
