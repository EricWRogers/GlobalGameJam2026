using Unity.Netcode;
using UnityEngine;

public abstract class GunBase : NetworkBehaviour
{
    [SerializeField] protected Transform muzzle;

    [Header("Stats")]
    [SerializeField] protected float fireRate = 10f; 
    [SerializeField] protected float range = 100f;

    [SerializeField] protected bool useAmmo = false;
    [SerializeField] protected int maxAmmo = 30;
    protected int currentAmmo;

    protected float nextFireLocal;
    protected float nextFireServer;

    public void TryFire()
    {
        if (!IsOwner) return;
        if (muzzle == null) return;

        if (Time.time < nextFireLocal) return;
        nextFireLocal = Time.time + (1f / fireRate);


        FireServerRpc(muzzle.position, muzzle.forward);
    }

    [ServerRpc]
    private void FireServerRpc(Vector3 origin, Vector3 dir, ServerRpcParams rpc = default)
    {

        if (Time.time < nextFireServer)
            return;

        nextFireServer = Time.time + (1f / fireRate);

        dir.Normalize();

        FireServer(origin, dir);
    }

    /// SERVER-ONLY
    protected abstract void FireServer(Vector3 origin, Vector3 dir);


}
