using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;


public class NetworkPlayerPerksManager : NetworkBehaviour
{
    public enum TypeOfPerks
    {
        Revive,
        Jug,
        DoubleTap
    }

    public int increaseHealth;

    private NetworkList<NetworkPerkType> activePerks;

    [SerializeField]
    private Health playerHealth;

    private void Awake()
    {
        activePerks = new NetworkList<NetworkPerkType>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            activePerks.Clear();
    }

    public bool TryAddPerk(TypeOfPerks perk)
    {
        if (!IsServer) return false;

        if (activePerks.Count >= 2)
        {
            RemoveOldestPerk();
        }

        if (activePerks.Contains(perk))
            return false;

        activePerks.Add(perk);
        ApplyPerk(perk);

        return true;
    }

    private void RemoveOldestPerk()
    {
        TypeOfPerks perk = activePerks[0];
        RemovePerk(perk);
        activePerks.RemoveAt(0);
    }

    private void RemovePerk(TypeOfPerks perk)
    {
        switch (perk)
        {
            case TypeOfPerks.Jug:
                playerHealth.totalHealth = playerHealth.totalHealth - increaseHealth;
                break;
            case TypeOfPerks.DoubleTap:
                // remove fire rate buff
                break;
            case TypeOfPerks.Revive:
                // revive logic
                break;
        }
    }

    private void ApplyPerk(TypeOfPerks perk)
    {
        switch (perk)
        {
            case TypeOfPerks.Jug:
                playerHealth.totalHealth = playerHealth.totalHealth + increaseHealth;
                break;
            case TypeOfPerks.DoubleTap:
                // increase fire rate
                break;
            case TypeOfPerks.Revive:
                // revive logic
                break;
        }
    }

}

