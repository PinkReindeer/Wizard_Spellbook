using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement & Lifetime")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float fadeDuration = 0.6f;

    [Header("VFX & Collision")]
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject hitEffectPrefab;

    private Rigidbody2D rb;
    private float currentTimer;
    private Gradient initialGradient;
    private bool isInitialized = false;
    private bool isDestroyed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (trailRenderer == null)
        {
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        if (trailRenderer != null)
        {
            initialGradient = trailRenderer.colorGradient;
        }
    }

    public void Setup(Vector2 targetDirection)
    {
        if (targetDirection.sqrMagnitude <= 0f)
        {
            targetDirection = Vector2.down;
        }

        targetDirection.Normalize();

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (rb != null)
        {
            rb.linearVelocity = targetDirection * speed;
        }

        currentTimer = lifeTime;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || isDestroyed) return;

        currentTimer -= Time.deltaTime;

        if (currentTimer <= fadeDuration && trailRenderer != null)
        {
            float fadeProgress = Mathf.Clamp01(currentTimer / fadeDuration);
            UpdateTrailAlpha(fadeProgress);
        }

        if (currentTimer <= 0f)
        {
            DestroyProjectile();
        }
    }

    private void UpdateTrailAlpha(float alphaMultiplier)
    {
        GradientColorKey[] colorKeys = initialGradient.colorKeys;
        GradientAlphaKey[] baseAlphaKeys = initialGradient.alphaKeys;
        GradientAlphaKey[] newAlphaKeys = new GradientAlphaKey[baseAlphaKeys.Length];

        for (int i = 0; i < baseAlphaKeys.Length; i++)
        {
            newAlphaKeys[i].alpha = baseAlphaKeys[i].alpha * alphaMultiplier;
            newAlphaKeys[i].time = baseAlphaKeys[i].time;
        }

        Gradient updatedGradient = new Gradient();
        updatedGradient.SetKeys(colorKeys, newAlphaKeys);
        trailRenderer.colorGradient = updatedGradient;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed || collision.CompareTag("Player"))
        {
            return;
        }

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
