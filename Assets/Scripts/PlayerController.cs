using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Player References")]
    [SerializeField] private PlayerCastSkill castSkill;

    [Header("Attack Timings")]
    [SerializeField] private float attackDuration = 0.4f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private Vector2 lastMoveDirection = new(0f, -1f); // Down

    private bool isAttacking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (!castSkill)
        {
            castSkill = GetComponent<PlayerCastSkill>();
        }
    }

    void Update()
    {
        HandleAction();

        if (isAttacking)
        {
            movement = Vector2.zero;
            animator.SetBool("isMoving", false);
            return;
        }

        ReadMovementInput();
        UpdateAnimation();
    }

    private void HandleAction()
    {
        if (Keyboard.current == null) return;

        // Space = Stationary Basic Attack
        if (Mouse.current.leftButton.wasPressedThisFrame && !isAttacking)
        {
            PerformSkill();
        }
    }

    private void PerformSkill()
    {
        if (castSkill && !castSkill.BeginCast()) return;

        isAttacking = true;
        animator.SetTrigger("attack");

        CancelInvoke(nameof(ResetAttack));
        Invoke(nameof(ResetAttack), attackDuration);
    }

    private void ReadMovementInput()
    {
        movement = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                movement.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                movement.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                movement.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                movement.x += 1f;
        }

        if (movement.sqrMagnitude > 0f)
        {
            movement.Normalize();
            lastMoveDirection = movement;
        }
    }

    private void UpdateAnimation()
    {
        bool isMoving = movement.sqrMagnitude > 0f;

        animator.SetFloat("moveX", lastMoveDirection.x);
        animator.SetFloat("moveY", lastMoveDirection.y);
        animator.SetBool("isMoving", isMoving);

        UpdateFirePoint();
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void UpdateFirePoint()
    {
        if (!castSkill) return;

        castSkill.UpdateFirePoint(lastMoveDirection);
    }

    void FixedUpdate()
    {
        if (!rb) return;

        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ResetAttack));
    }
}
