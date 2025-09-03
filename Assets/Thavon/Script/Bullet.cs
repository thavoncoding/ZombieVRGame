//using UnityEngine;

//public class Bullet : MonoBehaviour
//{
//    [Header("Destroy On Hit Layers")]
//    public LayerMask collisionLayers;  // Layers that destroy the bullet (e.g., Wall, Ground)
//    public LayerMask zombies;          // Layers that destroy the enemy

//    [Header("Effects")]
//    public GameObject bloodEffectPrefab;  // Assign your blood particle prefab here

//    private void OnCollisionEnter(Collision collision)
//    {
//        GameObject hitObject = collision.gameObject;

//        // Destroy bullet on any collision in collisionLayers
//        if (((1 << hitObject.layer) & collisionLayers) != 0)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        // Destroy enemy if it belongs to the zombies layer
//        if (((1 << hitObject.layer) & zombies) != 0)
//        {
//            // Play blood effect at collision point
//            if (bloodEffectPrefab != null)
//            {
//                ContactPoint contact = collision.contacts[0];
//                GameObject blood = Instantiate(bloodEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
//                Destroy(blood, 2f); // Destroy particle after 2 seconds
//            }

//            var zombie = hitObject.GetComponent<EnemyWander>();
//            if (zombie != null)
//            {
//                zombie.Die();
//            }

//            Destroy(gameObject);
//        }
//    }
//}
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public BulletType bulletType;

    [Header("Collision Layers")]
    public LayerMask collisionLayers;
    public LayerMask zombies;

    [Header("Effects")]
    public GameObject bloodEffectPrefab;
    public GameObject explosionEffectPrefab;

    [Header("RPG Settings")]
    public float explosionRadius = 5f;
    public float explosionForce = 700f;

    private void OnCollisionEnter(Collision collision)
    {
        switch (bulletType)
        {
            case BulletType.Normal:
                HandleNormalBullet(collision);
                break;
            case BulletType.RPG:
                HandleRpgBullet(collision);
                break;
            // Future case:
            // case BulletType.Fire:
            //     HandleFireBullet(collision);
            //     break;
            default:
                Destroy(gameObject);
                break;
        }
    }

    private void HandleNormalBullet(Collision collision)
    {
        GameObject hitObject = collision.gameObject;

        if (((1 << hitObject.layer) & collisionLayers) != 0)
        {
            Destroy(gameObject);
            return;
        }

        if (((1 << hitObject.layer) & zombies) != 0)
        {
            ContactPoint contact = collision.contacts[0];

            if (bloodEffectPrefab != null)
            {
                GameObject blood = Instantiate(bloodEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
                Destroy(blood, 2f);
            }

            var zombie = hitObject.GetComponent<EnemyWander>();
            if (zombie != null)
            {
                zombie.Die();
            }

            Destroy(gameObject);
        }
    }

    private void HandleRpgBullet(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Vector3 explosionPosition = contact.point;

        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, explosionPosition, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        Collider[] hitColliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (var hit in hitColliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }

            if (((1 << hit.gameObject.layer) & zombies) != 0)
            {
                var zombie = hit.GetComponent<EnemyWander>();
                if (zombie != null)
                {
                    zombie.Die();
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (bulletType == BulletType.RPG)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
public enum BulletType
{
    Normal,
    RPG,
    Fire,    // Easy to extend more types
    Ice
}
