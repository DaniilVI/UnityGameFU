using UnityEngine;
using System.Collections;

public class CharacterMove : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 13f;

    [Header("Рывок (Dash)")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Удар")]
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Transform attackSpawnPoint;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    private bool canAttack = true;
    private bool isJump = false;
    private bool isSmall = false;

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
        if (isDashing) return;
        CheckRun();
        if (!isJump && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleSize();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Attack());
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

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float direction = sprite.flipX ? -1f : 1f;

        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(direction * dashForce, 0f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
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
        if (!canAttack) yield break;
        canAttack = false;

        if (attackPrefab == null || attackSpawnPoint == null)
        {
            Debug.LogWarning("attackPrefab или attackSpawnPoint не назначены!");
            yield break;
        }

        bool lookingLeft = sprite.flipX;
        float dir = lookingLeft ? -1f : 1f;

        Vector3 spawnLocal = attackSpawnPoint.localPosition;
        spawnLocal.x = Mathf.Abs(spawnLocal.x) * dir;

        Vector3 spawnWorld = transform.position + new Vector3(
            spawnLocal.x * transform.lossyScale.x,
            spawnLocal.y * transform.lossyScale.y,
            0f
        );

        GameObject attack = Instantiate(attackPrefab, spawnWorld, Quaternion.identity, transform);

        Vector3 prefabBaseScale = attackPrefab.transform.localScale;
        attack.transform.localScale = Vector3.Scale(prefabBaseScale, transform.localScale);

        Vector3 s = attack.transform.localScale;
        s.x = Mathf.Abs(s.x) * dir;
        attack.transform.localScale = s;

        yield return new WaitForSeconds(attackDuration);
        Destroy(attack);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
