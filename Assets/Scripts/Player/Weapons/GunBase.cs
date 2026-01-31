using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;
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
    
    // Owner-only rate limiting
    protected float nextFireTime;

    public XRGrabInteractable grabInteractable;
    public bool triggerHeld = false;

    public void SetTriggerHeld(bool held)
    {
        triggerHeld = held;
    }

    public override void OnNetworkSpawn()
    {
        currentAmmo = maxAmmo;

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnDropped);

        grabInteractable.activated.AddListener(OnActivated);
        grabInteractable.deactivated.AddListener(OnDeactivated);
        
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        
    }

    private void OnDropped(SelectExitEventArgs args)
    {
        triggerHeld = false;
    }

    private void OnActivated(ActivateEventArgs args)
    {
        triggerHeld = true;   // trigger pressed
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        triggerHeld = false;  // trigger released
    }

    void Update()
    {
        // Owner-only firing logic
        if (!IsOwner || !triggerHeld) return;
        TryFire();
    }

    public void TryFire()
    {
        // Rate limiting FIRST
        if (Time.time < nextFireTime) return;
        
        if (useAmmo && currentAmmo <= 0) return;

        // Update authoritative state
        nextFireTime = Time.time + (1f / fireRate);
        if (useAmmo) currentAmmo--;

        Vector3 origin = muzzle.position;
        Vector3 dir = muzzle.forward;

        // Owner does FULL authoritative firing (damage, raycast, etc.)
        ShootGun(origin, dir);
        
        // Owner plays effects locally
        MuzzleFlash();

        // Broadcast effects to ALL other clients
        FireEffectsClientRpc(origin, dir);
    }

    [ClientRpc]
    private void FireEffectsClientRpc(Vector3 origin, Vector3 dir)
    {
        Debug.Log("Firing Client RPC");
        if (IsOwner) return;
        MuzzleFlash();
    }

    protected void MuzzleFlash()
    {
        Debug.Log("Playing Muzzle Flash VFX");  
        if (muzzleFlashVFX != null)
            muzzleFlashVFX.Play();
    }
    
    protected abstract void ShootGun(Vector3 origin, Vector3 dir);

    public override void OnNetworkDespawn()
    {
        if (IsOwner && grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnDropped);
        }
    }
}