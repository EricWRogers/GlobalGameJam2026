using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class PerkBottle : NetworkBehaviour
{
    [Header("Perk Data")]
    public PerkData perkData;

    [Header("Visuals")]
    [SerializeField] private Renderer bottleRenderer;
    [SerializeField] private ParticleSystem drinkParticles;

    private bool consumed;
    private PoolerProjectiles pool;

    /// <summary>
    /// Called by the PerkMachine after pulling this object from the pool
    /// </summary>
    public void Initialize(PerkData data, PoolerProjectiles pooler)
    {
        perkData = data;
        pool = pooler;
        consumed = false;

        bottleRenderer.enabled = true;
        bottleRenderer.material = perkData.perkMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Server authoritative consumption
        if (!IsServer || consumed)
            return;

        // VR head-based interaction
        if (!other.CompareTag("PlayerHead"))
            return;

        NetworkPlayerPerksManager playerPerks =
            other.GetComponentInParent<NetworkPlayerPerksManager>();

        if (playerPerks == null)
            return;

        consumed = true;

        playerPerks.TryAddPerk(perkData.perkType);

        PlayDrinkEffectsClientRpc();
        DespawnBottle();
    }

    private void DespawnBottle()
    {
        // Despawn without destroying so pooling still works
        NetworkObject.Despawn(false);
        pool.ReturnItem(gameObject);
    }

    [ClientRpc]
    private void PlayDrinkEffectsClientRpc()
    {
        drinkParticles.Play();
    }
}