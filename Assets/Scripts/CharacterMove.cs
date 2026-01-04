using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterMove : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private GameObject spriteObject;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 13f;
    private bool isMoving = false;

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
    private bool canGrow = true;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Sprite inputSprite;
    private SpriteRenderer boxGloveSprite;
    private Vector3 inputDirection;
    private PolygonCollider2D polyCollider;
    private BoxCollider2D boxCollider;
    private PlayerAbilities playerAbilities;
    private Animator anim;

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private Vector3 position;
    public bool isPlatform;
    private float delayTime = 0.5f;
    private float lastSaveTime;

    public Vector3 Position
    {
        get { return position; }
        set { 
            position = value;
            transform.position = position;
        }
    }

    public bool Small
    {
        get { return isSmall; }
        set { isSmall = value; }
    }

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
        anim = GetComponent<Animator>();
        polyCollider = GetComponent<PolygonCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = spriteObject.GetComponent<SpriteRenderer>();
        inputSprite = sprite.sprite;
        boxGloveSprite = boxGloveObject.GetComponentInChildren<SpriteRenderer>();
        boxGloveObject.SetActive(false);
        boxCollider.enabled = false;
        polyCollider.enabled = true;

        playerAbilities = GetComponent<PlayerAbilities>();
        if (playerAbilities == null)
        {
            Debug.LogWarning("PlayerAbilities component not found. All abilities will be disabled.");
        }

        if (LoadLevel.isLoad && DataBaseManager.GetStatusLevel().Equals("в процессе"))
        {
            LevelDataManager.LoadProgress();
        }
    }

    void FixedUpdate()
    {
        if (isFrozen || isDashing)
        {
            return;
        }
        Run();
        CheckJump();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing) return;
        if (isFrozen) return;
        if (!isJump && !isFrozen) State = States.idle;

        CheckRun();
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && isMoving && HasAbility(PlayerAbilities.Ability.Dash))
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isJump && HasAbility(PlayerAbilities.Ability.Shrink))
        {
            ToggleSize();
        }

        if (Input.GetKeyDown(KeyCode.E) && isMoving && HasAbility(PlayerAbilities.Ability.Attack))
        {
            StartCoroutine(Attack());
        }

        if (isJump && !isMoving)
        {
            if (anim.enabled)
            {
                anim.enabled = false;
                anim.Rebind();
                anim.Update(0f);
                sprite.sprite = inputSprite;
            }
        }
        else
        {
            if (!anim.enabled)
            {
                anim.enabled = true;
            }
        }

        if (isPlatform)
        {
            lastSaveTime = Time.time;
        }
        else if (!isJump && !isPlatform && Time.time >= lastSaveTime + delayTime)
        {
            position = transform.position;
            //Debug.Log(position);
            lastSaveTime = Time.time;
        }
    }

    private bool HasAbility(PlayerAbilities.Ability ability)
    {
        if (playerAbilities == null) return false;
        return playerAbilities.HasAbility((int)ability);
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
        isMoving = Mathf.Abs(inputDirection.x) > 0.01f;

        if (isMoving)
        {
            if (!isJump) State = States.run;

            boxCollider.enabled = true;
            polyCollider.enabled = false;
            Flip(inputDirection.x < 0);

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
            polyCollider.enabled = true;
            boxCollider.enabled = false;
        }
    }

    private void Jump()
    {
        Vector3 v = rb.velocity;
        v.y = 0f;
        rb.velocity = v;
        
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckJump()
    {
        if (boxCollider.enabled)
        {
            Vector3 scale = transform.localScale;
            float absScaleX = Mathf.Abs(scale.x);
            float absScaleY = Mathf.Abs(scale.y);
            
            Vector2 realSize = boxCollider.size * new Vector2(absScaleX, absScaleY);
            Vector2 realOffset = boxCollider.offset * new Vector2(scale.x, scale.y);
            
            Vector2 bottomLeft = (Vector2)transform.position + realOffset + new Vector2(-realSize.x/2, -realSize.y/2);
            Vector2 bottomRight = (Vector2)transform.position + realOffset + new Vector2(realSize.x/2, -realSize.y/2);
            
            int rayCount = 4;
            isJump = true;
            for (int i = 0; i <= rayCount; i++)
            {
                float t = (float)i / rayCount;
                Vector2 rayOrigin = Vector2.Lerp(bottomLeft, bottomRight, t);
                
                RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, 0.1f)
                    .Where(h => !h.collider.isTrigger && h.collider.gameObject != gameObject && !h.collider.CompareTag("Danger")).ToArray();
                
                if (hits.Length > 0) 
                {
                    isJump = false;
                    break;
                }
            }
        }
        else
        {
            Vector2[] points = polyCollider.points;
            
            Vector2 localPoint1 = points[3] + polyCollider.offset;;
            Vector2 localPoint2 = points[4] + polyCollider.offset;;
            
            Vector2 worldPoint1 = (Vector2)transform.TransformPoint(localPoint1);
            Vector2 worldPoint2 = (Vector2)transform.TransformPoint(localPoint2);
            
            int rayCount = 4;
            isJump = true;
            
            for (int i = 0; i <= rayCount; i++)
            {
                float t = (float)i / rayCount;
                Vector2 rayOrigin = Vector2.Lerp(worldPoint1, worldPoint2, t);
                
                RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, 0.1f)
                    .Where(h => !h.collider.isTrigger && h.collider.gameObject != gameObject && !h.collider.CompareTag("Danger")).ToArray();
                
                if (hits.Length > 0)
                {
                    isJump = false;
                    break;
                }
            }
        }

        if (isJump && !isFrozen) State = States.jump;
    }

    private IEnumerator Dash()
    {
        if (!canDash) yield break;

        canDash = false;
        isDashing = true;

        float origGravity = rb.gravityScale;
        float origDrag = rb.drag;

        float direction = (transform.localScale.x < 0) ? -1f : 1f;

        rb.gravityScale = 0f;
        rb.drag = 8f;
        rb.velocity = new Vector3(direction * dashForce, 0f, 0f);

        boxCollider.enabled = true;
        polyCollider.enabled = false;

        yield return new WaitForSeconds(dashDuration);

        polyCollider.enabled = true;
        boxCollider.enabled = false;

        rb.velocity = Vector3.zero;

        rb.drag = origDrag;
        rb.gravityScale = origGravity;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void ToggleSize()
    {
        if (isSmall)
        {
            if (!canGrow)
            {
                return;
            }

            transform.localScale *= 2f;
            isSmall = false;
        }
        else
        {
            transform.localScale *= 0.5f;
            isSmall = true;
        }
    }

    public void SetCanGrow(bool value)
    {
        canGrow = value;
    }

    public void GrowToNormal()
    {
        if (isSmall)
        {
            transform.localScale *= 2f;
            isSmall = false;
        }
    }

    private IEnumerator Attack()
    {
        if (!canAttack)
            yield break;
        canAttack = false;
        State = States.punch;

        isFrozen = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        boxCollider.enabled = true;
        polyCollider.enabled = false;
        boxGloveObject.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        polyCollider.enabled = true;
        boxCollider.enabled = false;
        boxGloveObject.SetActive(false);

        rb.isKinematic = false;
        isFrozen = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void Flip(bool faceRight)
    {
        Vector3 scale = transform.localScale;
        if (faceRight)
        {
            if (scale.x > 0)
            {
                scale.x*= -1f;
            }
        }
        else
        {
            if (scale.x < 0)
            {
                scale.x*= -1f;
            }
        }
        transform.localScale = scale;
    }
}

public enum States
{
    idle,
    run,
    jump,
    punch
}
