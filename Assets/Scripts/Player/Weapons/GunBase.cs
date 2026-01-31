using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public abstract class GunBase : NetworkBehaviour
{
    [SerializeField] protected Transform muzzle;

    [Header("Stats")]
    public float damage = 10f;
    public float fireRate = 10f; 
    [SerializeField] protected float range = 100f;

    [SerializeField] protected bool useAmmo = false;
    [SerializeField] protected int maxAmmo = 30;
    protected int currentAmmo;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected LayerMask hitMask;
    [SerializeField] private VisualEffect muzzleFlashVFX;
    protected float nextFireLocal;
    protected float nextFireServer = 0f;

    public XRGrabInteractable grabInteractable;
    public bool triggerHeld = false;

    public void SetTriggerHeld(bool held)
    {
        triggerHeld = held;
    }

    void Awake()
    {
        currentAmmo = maxAmmo;
    }
    //public override void OnNetworkSpawn()
    //{
    //    if(!IsOwner)
    //    {
    //        grabInteractable.enabled = false;
    //    }
    //    else
    //    {
    //        grabInteractable.enabled = true;
    //        grabInteractable.selectExited.AddListener(_ => triggerHeld = false);
    //        grabInteractable.selectEntered.AddListener(_ => triggerHeld = true);
    //    }
    //}
//
    void Update()
    {
        if(triggerHeld)
        {
            TryFire();
        }
    }

    public void TryFire()
    {
        Debug.Log("Before Is Owner Check");
        if (!IsOwner) return;
        Debug.Log("Trying to fire Is Owner");

        if (useAmmo && currentAmmo <= 0) return;

        nextFireLocal = Time.time + (1f / fireRate);


        FireClientRpc(muzzle.position, muzzle.forward);
    }


    [ServerRpc(RequireOwnership = false)]
    private void FireServerRpc(Vector3 origin, Vector3 dir, ServerRpcParams rpc = default)
    {

        if (Time.time < nextFireServer)
            return;

        nextFireServer = Time.time + (1f / fireRate);

        dir.Normalize();

        FireClientRpc(origin, dir);
    }


    [ClientRpc]
    private void FireClientRpc(Vector3 origin, Vector3 dir)
    {
        ShootGun(origin, dir);
        MuzzleFlash();
        Debug.Log("Fired AK " );
    }




    protected void MuzzleFlash()
    {
        Debug.Log("Playing Muzzle Flash VFX");
        muzzleFlashVFX.Play();
    }
    protected abstract void ShootGun(Vector3 origin, Vector3 dir);


}
