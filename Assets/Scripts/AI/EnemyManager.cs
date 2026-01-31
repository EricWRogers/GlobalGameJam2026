using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;



public class EnemyManager : NetworkBehaviour
{
    public GameObject prefab;
    public int spawnAmount;
    public float spawnTimer;
    public Transform spawnPos;
    private int m_amountSpawned;
    private float m_timer;
    public bool m_serverIsReady = false;
    public override void OnNetworkSpawn()
    {
        m_serverIsReady = true;
        Debug.Log($"NM Exists: {NetworkManager.Singleton != null}");
    Debug.Log($"IsListening: {NetworkManager.Singleton?.IsListening}");
    Debug.Log($"IsServer: {NetworkManager.Singleton?.IsServer}");
    Debug.Log($"IsClient: {NetworkManager.Singleton?.IsClient}");

    }



    void Update()
    {
        
        if(!m_serverIsReady) return;
        if(!IsServer)return;
        m_timer -= Time.deltaTime;
        if(m_timer <= 0 && m_amountSpawned <= spawnAmount)
        {
            SpawnEnemy(spawnPos.position);
            m_amountSpawned++;
            m_timer = spawnTimer;
        }
    }
    void SpawnEnemy(Vector3 _pos)
    {
        GameObject enemy = Instantiate(prefab, _pos, transform.rotation);
        enemy.GetComponent<NetworkObject>().Spawn();
    }
}
