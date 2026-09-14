using UnityEngine;
using System.Collections;

public class SlimeController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float restTime = 1.2f;
    [SerializeField] private Transform target;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(JumpRoutine());
    }

    private void Update()
    {
        anim.SetBool("isGrounded", isGrounded);
    }

    private IEnumerator JumpRoutine()
    {
        while (true)
        {
            // 1. Rest while on the ground
            yield return new WaitForSeconds(restTime);

            // 2. Face the target
            float direction = target != null ? Mathf.Sign(target.position.x - transform.position.x) : 1f;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);

            // 3. Calculate direction toward player
            Vector2 moveDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;

            // 4. Mark as airborne and trigger animation
            isGrounded = false;
            anim.SetTrigger("Jump");

            rb.linearVelocity = moveDirection * moveSpeed;

            // 5. Pause routine until the Animation Event calls OnJumpEnd()
            yield return new WaitUntil(() => isGrounded);
        }
    }

    public void OnJumpEnd()
    {
        rb.linearVelocity = Vector2.zero;
        isGrounded = true;
    }
}
