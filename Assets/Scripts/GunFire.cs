
using UnityEngine;

public class GunFire : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireProjectile();
        }
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = firePoint.forward * projectileSpeed;
        }
    }
}