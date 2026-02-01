using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : NetworkBehaviour
{
    public int damage = 5;
    public float attackSpeed = 3f;
    public float attackRange = 2f;
    public float switchTargetRange = 10;
    public LayerMask losMask;
    public bool los;
    public NavMeshAgent agent;
    public GameObject curTarget;

    public bool m_serverIsReady = false;

    public override void OnNetworkSpawn()
    {
        m_serverIsReady = true;

    }

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(!IsOwner) return;
    }

    public void Update()
    {
        if(!m_serverIsReady && !IsOwner) return;
        if(curTarget == null)
        {
            curTarget = GetClosestPlayerInRange();
            if(curTarget == null)
            {
                Debug.Log("No players within range");
                return;
            }
        }
        if(agent.remainingDistance > switchTargetRange)
        {
            curTarget = GetClosestPlayerInRange();
        }
    }

    public GameObject GetClosestPlayerInRange()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.ConnectedClients.Count == 0)
            return null;

        GameObject closestPlayer = null;
        float compDistance = float.MaxValue;
        Vector3 myPos = transform.position;
        for (int x = 0; x < NetworkManager.Singleton.ConnectedClients.Count; x++)
        {
            ulong id = NetworkManager.Singleton.ConnectedClientsIds[x];
            var client = NetworkManager.Singleton.ConnectedClients[id];
            if (client == null || client.PlayerObject == null || client.PlayerObject.gameObject == null)
                continue;
            Vector3 playerPos = client.PlayerObject.transform.position;
            float distance = Vector3.Distance(myPos, playerPos);
            if (distance < compDistance && distance <= switchTargetRange)
            {
                compDistance = distance;
                closestPlayer = client.PlayerObject.gameObject;
            }
        }
        if (closestPlayer == null)
            return GetClosestPlayer();
        return closestPlayer;
    }
    public GameObject GetClosestPlayer()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.ConnectedClients.Count == 0)
            return null;

        GameObject closestPlayer = null;
        float compDistance = float.MaxValue;
        Vector3 myPos = transform.position;
        for (int x = 0; x < NetworkManager.Singleton.ConnectedClients.Count; x++)
        {
            ulong id = NetworkManager.Singleton.ConnectedClientsIds[x];
            var client = NetworkManager.Singleton.ConnectedClients[id];
            if (client == null || client.PlayerObject == null || client.PlayerObject.gameObject == null)
                continue;
            Vector3 playerPos = client.PlayerObject.transform.position;
            float distance = Vector3.Distance(myPos, playerPos);
            if (distance < compDistance)
            {
                compDistance = distance;
                closestPlayer = client.PlayerObject.gameObject;
            }
        }
        return closestPlayer;
    }

}
