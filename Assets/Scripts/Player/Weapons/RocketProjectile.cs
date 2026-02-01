using Unity.Netcode;
using UnityEngine;

public class RocketProjectile : NetworkBehaviour    
{
    [SerializeField] float lifetime = 5f;
    [SerializeField] float speed = 50f;
    [SerializeField] LayerMask enemyMask;



    void Start() 
    {
        Destroy(gameObject, lifetime);
    }

    public void StartRocket()
    {
        if (IsOwner)
        {
            // Owner moves it forward (client-side prediction)
            var rb = GetComponent<Rigidbody>();
            rb.linearVelocity = transform.forward * speed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Hit effects, damage logic here
        if (IsServer || IsHost) // Authoritative damage
        {
           HitTarget(collision.gameObject);
        }
        NetworkObject.Despawn(); // Server can despawn
    }


    void HitTarget(GameObject target)
    {
        // Example damage logic

    
    }

    [ClientRpc]
    private void RocketExplosionClientRpc(Vector3 explosionPos, float explosionRadius, int damage)
    {
        DamageInExplosionSphere(explosionPos, explosionRadius, damage);
    }


    private void DamageInExplosionSphere(Vector3 center, float radius, int damage)
{
    // Sphere cast from explosion center
    RaycastHit[] hits = Physics.SphereCastAll(
        center, 
        radius, 
        transform.forward,  
        0f,                 
        enemyMask,         
        QueryTriggerInteraction.Collide
    );

    foreach (RaycastHit hit in hits)
    {
        // Get the health component
        var health = hit.collider.GetComponent<Health>();
        if (health != null)
        {
            // Falloff damage based on distance from center
            float distance = Vector3.Distance(center, hit.point);

            
            // Apply damage (only authoritative if you have server validation)
            health.TakeDamage(damage);
        }
    }

}
}

