using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class PerkBottle : NetworkBehaviour
{
    public PerkData perkData;

    [Header("Visuals")]
    [SerializeField] private Renderer bottleRenderer;
    [Header("Particles")]
    [SerializeField] private GameObject jug;
    [SerializeField] private GameObject revive;
    [SerializeField] private GameObject doubleTap;

    private Dictionary<NetworkPlayerPerksManager.TypeOfPerks, ParticleSystem[]> perkParticles;

    private bool isEquipped;
    private PlayerPerkSocket currentSocket;
    private PoolerProjectiles pool;

    private void Awake()
    {
        perkParticles = new Dictionary<NetworkPlayerPerksManager.TypeOfPerks, ParticleSystem[]>
        {
            {
                NetworkPlayerPerksManager.TypeOfPerks.Jug,
                jug.GetComponentsInChildren<ParticleSystem>(true)
            },
            {
                NetworkPlayerPerksManager.TypeOfPerks.Revive,
                revive.GetComponentsInChildren<ParticleSystem>(true)
            },
            {
                NetworkPlayerPerksManager.TypeOfPerks.DoubleTap,
                doubleTap.GetComponentsInChildren<ParticleSystem>(true)
            }
        };

        DisableAllParticleGroups();
    }

    public void Initialize(PerkData data, PoolerProjectiles pooler)
    {
        perkData = data;
        pool = pooler;
        bottleRenderer.material = perkData.perkMaterial;

        isEquipped = false;
        currentSocket = null;

        DisableAllParticleGroups();

        GameObject activeGroup = GetParticleGroupForPerk();
        if (activeGroup != null)
            activeGroup.SetActive(true);
        }

    public void Equip(PlayerPerkSocket socket)
    {
        isEquipped = true;
        currentSocket = socket;

        transform.SetParent(socket.SnapPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        PlayParticles();
    }

    public void Unequip()
    {
        isEquipped = false;
        currentSocket = null;

        transform.SetParent(null);
        StopParticles();
    }

    private void DisableAllParticleGroups()
    {
        jug.SetActive(false);
        revive.SetActive(false);
        doubleTap.SetActive(false);
    }

    private GameObject GetParticleGroupForPerk()
    {
        return perkData.perkType switch
        {
            NetworkPlayerPerksManager.TypeOfPerks.Jug => jug,
            NetworkPlayerPerksManager.TypeOfPerks.Revive => revive,
            NetworkPlayerPerksManager.TypeOfPerks.DoubleTap => doubleTap,
            _ => null
        };
    }

    private void PlayParticles()
    {
        if (!perkParticles.TryGetValue(perkData.perkType, out var systems))
            return;

        foreach (var ps in systems)
            ps.Play(true);
    }

    private void StopParticles()
    {
        if (!perkParticles.TryGetValue(perkData.perkType, out var systems))
            return;

        foreach (var ps in systems)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public bool IsEquipped => isEquipped;
}