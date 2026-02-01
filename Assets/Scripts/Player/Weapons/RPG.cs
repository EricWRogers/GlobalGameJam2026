using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class RPG : GunBase
{
    [SerializeField] private float projectileSpeed = 50f;
    public Transform rocketSpawnPoint;

    GameObject proj;
    private bool rocketLoaded = false;

    readonly List<RocketProjectile> m_ProjectileQueue = new();

    private Rocket_Pool m_rocketPool;

    [SerializeField] private GameObject rocketPrefab;

        void Awake()
    {
        m_rocketPool = FindFirstObjectByType<Rocket_Pool>();
    }

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
    
        GameObject newObject = m_rocketPool.GetItem();
        if (!newObject.TryGetComponent(out RocketProjectile projectile))
            {
                Debug.Log("RocketProjectile component not found on rocket object.");
                return;
            }
        projectile.transform.SetPositionAndRotation(rocketSpawnPoint.position, rocketSpawnPoint.rotation);
        projectile.Setup(IsOwner, OnProjectileDestroy, projectileSpeed);
        

         if (newObject.TryGetComponent(out Rigidbody rigidBody))
        {
            //rigidBody.isKinematic = true;
            //rigidBody.isKinematic = false;
            rigidBody.linearVelocity = Vector3.zero;
            //Vector3 force = dir.normalized * projectileSpeed;
            //rigidBody.AddForce(force );
        }
        Debug.Log("Fired RPG Rocket");
        m_ProjectileQueue.Add(projectile);
        
        rocketLoaded = false;
    }

    new void Update()
    {
        base.Update();


    }



    void ReloadRocketLauncher()
    {

    }


        void OnProjectileDestroy(RocketProjectile projectile)
    {
        projectile.explosionEffect.SetActive(false);
        if (m_ProjectileQueue.Contains(projectile))
        {
            m_ProjectileQueue.Remove(projectile);
        }
        m_rocketPool.ReturnItem(projectile.gameObject);
    }
}
