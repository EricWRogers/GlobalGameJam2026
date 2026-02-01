using Unity.Netcode;
using UnityEngine;

public class RPG : GunBase
{
    [SerializeField] private float projectileSpeed = 50f;
    public Transform rocketSpawnPoint;

    GameObject proj;
    private bool rocketLoaded = false;

    [SerializeField] private GameObject rocketPrefab;
    protected override void ShootGun(Vector3 origin, Vector3 dir)
    {
    
        // Optional: Give it initial velocity
        var rb = proj.GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = dir * projectileSpeed;
        rocketLoaded = false;
    }

    new void Update()
    {
        base.Update();
        if (!rocketLoaded)
        {
            ReloadRocketLauncher();
        }


    }


    void ReloadRocketLauncher()
    {
        currentAmmo = maxAmmo;
        proj = Instantiate(rocketPrefab, rocketSpawnPoint);
        proj.GetComponent<NetworkObject>().Spawn();
        rocketLoaded = true;
    }
}
