using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCastSkill : MonoBehaviour
{
    [Header("Projectile Setup")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float firePointOffset = 0.5f;
    [SerializeField] private Camera mainCamera;

    private bool isCasting;

    private void Awake()
    {
        if (!mainCamera)
        {
            mainCamera = Camera.main;
        }
    }

    public bool BeginCast()
    {
        if (isCasting || !projectilePrefab || !firePoint) return false;

        isCasting = true;
        return true;
    }

    public void SpawnProjectile()
    {
        if (!isCasting || !projectilePrefab || !firePoint) return;

        Vector2 shootDirection = GetMouseDirection();

        if (shootDirection == Vector2.zero)
        {
            shootDirection = Vector2.down;
        }

        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (!projectile)
        {
            Debug.LogError("The projectile prefab must contain a Projectile component.", projectileObject);

            Destroy(projectileObject);
            isCasting = false;
            return;
        }

        projectile.Setup(shootDirection);
        isCasting = false;
    }

    public void UpdateFirePoint(Vector2 direction)
    {
        if (!firePoint) return;

        firePoint.localPosition = direction * firePointOffset;
    }

    private Vector2 GetMouseDirection()
    {
        if (!mainCamera || Mouse.current == null)
        {
            return Vector2.zero;
        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        mouseWorldPosition.z = firePoint.position.z;

        return ((Vector2)mouseWorldPosition - (Vector2)firePoint.position).normalized;
    }

    private void OnDisable()
    {
        isCasting = false;
    }
}