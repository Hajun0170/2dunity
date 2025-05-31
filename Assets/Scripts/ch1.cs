using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 10f;
    private bool canDoubleJump = false;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isDashing = false;
    private bool dashOnCooldown = false;
    private bool hasAirDashed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (!isDashing)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            if (moveInput != 0)
                transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }

        // Ground 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded)
        {
            // 착지 시 상태 초기화
            canDoubleJump = true;
            hasAirDashed = false;
        }

        // 점프 (C 키)
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = false;
            }
        }

        // 대시 (왼쪽 Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isGrounded && !isDashing && !dashOnCooldown)
            {
                StartCoroutine(Dash(true));
            }
            else if (!isGrounded && !isDashing && !hasAirDashed)
            {
                StartCoroutine(Dash(false));
            }
        }
    }

    IEnumerator Dash(bool isGroundDash)
    {
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        if (isGroundDash)
        {
            dashOnCooldown = true;
            yield return new WaitForSeconds(dashCooldown);
            dashOnCooldown = false;
        }
        else
        {
            hasAirDashed = true;
        }
    }

    // Scene에서 groundCheck 위치 확인용
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}
