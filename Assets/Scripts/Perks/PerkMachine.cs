using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;


public class PerkMachine : NetworkBehaviour
{
    [Header("Perk Setup")]
    [SerializeField] 
    private PerkData[] availablePerks;

    [Header("Spawning")]
    [SerializeField] 
    private Transform spawnPoint;
    [SerializeField] 
    private PoolerProjectiles bottlePool;

    public void SpawnRandomPerk()
    {
        if (!IsServer) 
            return;

        if (availablePerks.Length == 0)
            return;

        PerkData perk = availablePerks[Random.Range(0, availablePerks.Length)];

        GameObject bottleObj = bottlePool.GetItem();
        bottleObj.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        PerkBottle bottle = bottleObj.GetComponent<PerkBottle>();
        bottle.Initialize(perk, bottlePool);

        NetworkObject netObj = bottleObj.GetComponent<NetworkObject>();

        if (!netObj.IsSpawned)
            netObj.Spawn();
    }
}
