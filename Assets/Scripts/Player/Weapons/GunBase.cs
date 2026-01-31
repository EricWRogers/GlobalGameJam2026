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

        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
    {
        Debug.Log("No network session running (no host/server).");
        return;
    }

    // Am I the host (client + server in one)?
    if (NetworkManager.Singleton.IsHost)
    {
        Debug.Log("I am the host (server + client).");
    }

    // Am I a dedicated server?
    if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
    {
        Debug.Log("I am a dedicated server.");
    }

    // Am I a pure client connected to some host?
    if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
    {
        Debug.Log("I am a client connected to a host.");
    }
    }

    public void TryFire()
    {
        Debug.Log("Before Is Owner Check");
        if (!IsOwner) return;
        Debug.Log("Trying to fire Is Owner");

        if (useAmmo && currentAmmo <= 0) return;

        nextFireLocal = Time.time + (1f / fireRate);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = GetOtherClientIds()
            }
        };


        FireClientRpc(muzzle.position, muzzle.forward, clientRpcParams);
        Debug.Log("Called Fire Client RPC");
    }


    //[ServerRpc(RequireOwnership = false)]
    //private void FireServerRpc(Vector3 origin, Vector3 dir, ServerRpcParams rpc = default)
    //{
    //    Debug.Log("Server RCP Fired");
    //    nextFireServer = Time.time + (1f / fireRate);

    //    dir.Normalize();

    //    FireClientRpc(origin, dir);
    //}


    [ClientRpc]
    private void FireClientRpc(Vector3 origin, Vector3 dir, ClientRpcParams clientRpcParams = default)
    {
        if(IsOwner)
        {
            Debug.Log("Owner firing gun");
            ShootGun(origin, dir);
            muzzleFlashVFX.Play();
            return;
        }
        
        MuzzleFlash();
        Debug.Log("Fired AK " );
    }




    protected void MuzzleFlash()
    {
    
        Debug.Log("Playing Muzzle Flash VFX");
        if(IsOwner) return;
        muzzleFlashVFX.Play();
    }
    protected abstract void ShootGun(Vector3 origin, Vector3 dir);




    private ulong[] GetOtherClientIds()
    {
        var clients = NetworkManager.Singleton.ConnectedClients;
        var otherClients = new System.Collections.Generic.List<ulong>();
        
        foreach (var client in clients)
        {
            if (client.Key != OwnerClientId) 
                otherClients.Add(client.Key);
        }
        return otherClients.ToArray();
    }


}
