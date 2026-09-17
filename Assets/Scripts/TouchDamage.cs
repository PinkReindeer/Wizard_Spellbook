using UnityEngine;

public class TouchDamage : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                // Direction pointing from the monster to the player
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;

                player.TakeDamage(damage, knockbackDir, knockbackForce, knockbackDuration);
            }
        }
    }
}
