//using System.Collections;
//using UnityEngine;

//public class Gun : MonoBehaviour
//{
//    [Header("Bullet Settings")]
//    public GameObject bulletPrefab;
//    public Transform spawn;
//    public float bulletSpeed = 50f;
//    public float bulletLifetime = 1f;
//    public bool useRaycast = false;
//    public float raycastRange = 100f;
//    public LayerMask hitLayers;
//    public ParticleSystem shotVfx;

//    [Header("Sound")]
//    public AudioSource gunAudioSource;  // Assign this in Inspector

//    [Header("Bullet Casing Ejection")]
//    public GameObject casingPrefab;
//    public Transform casingEjectPoint;
//    public float casingEjectForce = 1.5f;


//    public void Shoot()
//    {
//        if (useRaycast)
//        {
//            RaycastShoot();
//        }
//        else
//        {
//            PhysicsShoot();
//        }

//        // Play gun sound
//        if (gunAudioSource != null)
//        {
//            gunAudioSource.Play();
//        }

//        // Play muzzle flash or smoke
//        if (shotVfx != null)
//        {
//            shotVfx.Play();
//        }

//        // Eject bullet casing
//        if (casingPrefab != null && casingEjectPoint != null)
//        {
//            GameObject casing = Instantiate(casingPrefab, casingEjectPoint.position, casingEjectPoint.rotation);
//            Rigidbody casingRb = casing.GetComponent<Rigidbody>();
//            if (casingRb != null)
//            {
//                // Random direction with force
//                Vector3 ejectDirection = casingEjectPoint.right + casingEjectPoint.up * 0.5f;
//                casingRb.AddForce(ejectDirection.normalized * casingEjectForce, ForceMode.Impulse);
//                casingRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
//            }

//            Destroy(casing, 3f); // Optional cleanup
//        }

//    }

//    private void PhysicsShoot()
//    {
//        GameObject bullet = Instantiate(bulletPrefab, spawn.position, spawn.rotation);
//        Rigidbody rb = bullet.GetComponent<Rigidbody>();

//        if (rb != null)
//        {
//            rb.velocity = Vector3.zero;
//            rb.angularVelocity = Vector3.zero;
//            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
//            rb.interpolation = RigidbodyInterpolation.Interpolate;
//            rb.useGravity = false;

//            rb.AddForce(spawn.forward * bulletSpeed, ForceMode.Impulse);
//        }

//        Destroy(bullet, bulletLifetime);
//    }

//    private void RaycastShoot()
//    {
//        RaycastHit hit;
//        if (Physics.Raycast(spawn.position, spawn.forward, out hit, raycastRange, hitLayers))
//        {
//            Debug.Log("Raycast Hit: " + hit.collider.name);

//            Rigidbody rb = hit.collider.attachedRigidbody;
//            if (rb != null)
//            {
//                rb.AddForce(spawn.forward * bulletSpeed, ForceMode.Impulse);
//            }

//            // Optional: impact effect or decal here
//        }

//        // Fire dummy bullet for visual effect
//        PhysicsShoot();
//    }
//}
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform spawn;
    public float bulletSpeed = 50f;
    public float bulletLifetime = 1f;
    public bool useRaycast = false;
    public float raycastRange = 100f;
    public LayerMask hitLayers;
    public ParticleSystem shotVfx;

    [Header("Bullet Type")]
    public BulletType bulletType = BulletType.Normal; // Dropdown selector for bullet type

    [Header("Sound")]
    public AudioSource gunAudioSource;

    [Header("Bullet Casing Ejection")]
    public GameObject casingPrefab;
    public Transform casingEjectPoint;
    public float casingEjectForce = 1.5f;

    public void Shoot()
    {
        if (useRaycast)
        {
            RaycastShoot();
        }
        else
        {
            PhysicsShoot();
        }

        if (gunAudioSource != null)
            gunAudioSource.Play();

        if (shotVfx != null)
            shotVfx.Play();

        EjectCasing();
    }

    private void PhysicsShoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, spawn.position, spawn.rotation);

        // Assign bullet type to bullet if it has Bullet.cs
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.bulletType = bulletType;
        }

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.useGravity = false;
            rb.AddForce(spawn.forward * bulletSpeed, ForceMode.Impulse);
        }

        Destroy(bullet, bulletLifetime);
    }

    private void RaycastShoot()
    {
        if (Physics.Raycast(spawn.position, spawn.forward, out RaycastHit hit, raycastRange, hitLayers))
        {
            Debug.Log("Raycast Hit: " + hit.collider.name);

            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
                rb.AddForce(spawn.forward * bulletSpeed, ForceMode.Impulse);

            // You can optionally spawn a hit effect or apply damage here
        }

        // Spawn visual bullet
        PhysicsShoot();
    }

    private void EjectCasing()
    {
        if (casingPrefab != null && casingEjectPoint != null)
        {
            GameObject casing = Instantiate(casingPrefab, casingEjectPoint.position, casingEjectPoint.rotation);
            Rigidbody casingRb = casing.GetComponent<Rigidbody>();
            if (casingRb != null)
            {
                Vector3 ejectDirection = casingEjectPoint.right + casingEjectPoint.up * 0.5f;
                casingRb.AddForce(ejectDirection.normalized * casingEjectForce, ForceMode.Impulse);
                casingRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }

            Destroy(casing, 3f);
        }
    }
}
