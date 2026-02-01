using Unity.Netcode;
using UnityEngine;

public class RPG : GunBase
{
    [SerializeField] private float projectileSpeed = 50f;

    [SerializeField] private GameObject rocketPrefab;
    protected override void ShootGun(Vector3 origin, Vector3 dir)
    {
        // Client-authoritative spawn (distributed authority)
        GameObject proj = Instantiate(rocketPrefab, muzzle.position, muzzle.rotation);
        proj.GetComponent<NetworkObject>().Spawn();
    
        // Optional: Give it initial velocity
        var rb = proj.GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = dir * projectileSpeed;
    }
}
