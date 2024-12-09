using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    private PlayerControls playerControls;
    private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;

    [Header("Movement Mechanics")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    private float sideMoveInput;
    [Header("")]
    [Header("Jump Mechanics")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayCastLong;
    [SerializeField] private bool isGrounded;

    [Header("Combat Mechanics")]
    [SerializeField]private Transform AttackOrigin;
    [SerializeField]private float attackRange;
    [SerializeField]private LayerMask enemyLayer;
    [SerializeField]private float attackDamage;
    private bool player_Jump;
    private bool player_Run;
    private bool player_Down;
    private bool player_Attack;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Update()
    {
        PlayerInput();
        Move();
        HandleAnimation();
        FlipSprite();
    }

    public void PlayerInput()
    {
        sideMoveInput = playerControls.Movement.SideMove.ReadValue<float>();
        player_Run = playerControls.Movement.Run.IsPressed(); // Menggunakan ReadValue<bool>()
        player_Attack= playerControls.Combat.BasicAttack.triggered;
        player_Jump = playerControls.Movement.Jump.triggered;
        player_Down = playerControls.Movement.Down.triggered;
    }

    void Move()
    {
        float moveSpeedToUse = player_Run ? runSpeed : moveSpeed; // Memilih kecepatan berdasarkan status Run

        CheckGroundStatus();
        rb.velocity = new Vector2(sideMoveInput * moveSpeedToUse, rb.velocity.y);

        if (player_Jump && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (player_Down)
        {
            rb.velocity = new Vector2(rb.velocity.x, -jumpForce / 2);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayCastLong);
    }

    void CheckGroundStatus()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, rayCastLong, groundLayer);
    }

    void HandleAnimation()
    {
        if (sideMoveInput != 0)
        {

            animator.SetBool("isRunning", true); // Animasi 'isRunning' tergantung status Run
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        animator.SetBool("isJumping", !isGrounded);
    }

    void FlipSprite()
    {
        if (sideMoveInput > 0)
        {
            sprite.flipX = false;
        }
        else if (sideMoveInput < 0)
        {
            sprite.flipX = true;
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackOrigin.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<IDamageable>().TakeDamage(attackDamage);
        }
    }
}
