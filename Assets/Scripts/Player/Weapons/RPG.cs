using Unity.Netcode;
using UnityEngine;

public class RPG : GunBase
{
    [SerializeField] private float projectileSpeed = 50f;
    public Transform rocketSpawnPoint;

    GameObject proj;
    private bool rocketLoaded = false;

    [SerializeField] private GameObject rocketPrefab;

    override public void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            ReloadRocketLauncher();
        }
    }
    protected override void ShootGun(Vector3 origin, Vector3 dir)
    {
    
        // Optional: Give it initial velocity
        proj.GetComponent<RocketProjectile>().StartRocket();
        
        rocketLoaded = false;
    }

    new void Update()
    {
        base.Update();
        if (!rocketLoaded && currentAmmo <=0 && IsOwner)
        {
            Debug.Log("Reloading Rocket Launcher");
            ReloadRocketLauncher();
        }


    }


    void ReloadRocketLauncher()
    {
        Debug.Log("ReloadRocketLauncher  INside");
        currentAmmo = maxAmmo;
        proj = Instantiate(rocketPrefab, rocketSpawnPoint);
        Debug.Log("Instantiated Rocket Prefab");
        proj.GetComponent<NetworkObject>().Spawn();
        Debug.Log("Spawned Rocket Network Object");
        rocketLoaded = true;
        Debug.Log("Rocket Loaded");
    }
}
