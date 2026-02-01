using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;



public class EnemyManager : NetworkBehaviour
{
    public static EnemyManager Instance {get; private set; }
    public GameObject prefab;
    public int amountToSpawn;
    public float spawnTimer;
    public Transform[] spawnPos;
    public int amountKilled;
    public int startingHealth = 50;
    private int m_amountSpawned;
    private float m_timer;
    public bool m_serverIsReady = false;
    public int waveCounter;
    public float waveDelay;
    public float zombieCountIncreasePercentage = 15;
    public float zombieHealthIncreasePercentage = 30;
    private float m_waveTimer;
    private bool m_waveIsDone;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnNetworkSpawn()
    {
        m_serverIsReady = true;

    }
    void Update()
    {
        if(!m_serverIsReady || !IsOwner) return;
        if(m_waveTimer > 0)
        {
            m_waveTimer -= Time.deltaTime;
        }
        if(m_amountSpawned <= amountToSpawn && m_waveTimer <= 0)//start of wave
        {
            m_timer -= Time.deltaTime;
            if(m_timer <= 0)
            {
                int spawner = Random.Range(0, spawnPos.Length-1);
                SpawnEnemy(spawnPos[spawner].position);
                m_amountSpawned++;
                m_timer = spawnTimer;
            }
        }
        if(amountKilled >= amountToSpawn)//end of wave / reset wave
        {
            m_waveIsDone = true;
        }
        if (m_waveIsDone)
        {
            m_waveTimer = waveDelay;
            waveCounter++;
            startingHealth += (int)(startingHealth * (zombieHealthIncreasePercentage/100));
            amountToSpawn += (int)(amountToSpawn * (zombieCountIncreasePercentage/100));
            m_amountSpawned = 0;
            amountKilled = 0;

            m_waveIsDone = false;
        }
        
    }
    void SpawnEnemy(Vector3 _pos)
    {
        GameObject enemy = Instantiate(prefab, _pos, transform.rotation);
        enemy.GetComponent<Health>().AddMaxHealth(startingHealth, true);
        float randomScale = Random.Range(.85f, 1.15f);
        enemy.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        enemy.GetComponent<NetworkObject>().Spawn();
    }
}
