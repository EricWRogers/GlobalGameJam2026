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

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        if(curTarget == null)
        {
            curTarget = GetClosestPlayerInRange();
            //agent.SetDestination(curTarget.transform.position);
        }
        //if(agent.remainingDistance)
        if(agent.remainingDistance > switchTargetRange)
        {
            curTarget = GetClosestPlayerInRange();
        }
    }

    public GameObject GetClosestPlayerInRange()
    {
        GameObject closestPlayer = null;
        float compDistance = 10000;
        for(int x = 0; x < NetworkManager.Singleton.ConnectedClients.Count; x++)
        {
            ulong id = NetworkManager.Singleton.ConnectedClientsIds[x];
            agent.SetDestination(NetworkManager.Singleton.ConnectedClients[id].PlayerObject.transform.position);
            float distance = agent.remainingDistance;
            if(distance < compDistance && distance < switchTargetRange)
            {
                compDistance = distance;
                closestPlayer = NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject;
            }
        }
        if(closestPlayer == null)
            return GetClosestPlayer();
        else
            return closestPlayer;
    }
    public GameObject GetClosestPlayer()
    {
        GameObject closestPlayer = null;
        float compDistance = 10000;
        for(int x = 0; x < NetworkManager.Singleton.ConnectedClients.Count; x++)
        {
            ulong id = NetworkManager.Singleton.ConnectedClientsIds[x];
            agent.SetDestination(NetworkManager.Singleton.ConnectedClients[id].PlayerObject.transform.position);
            float distance = agent.remainingDistance;
            if(distance < compDistance)
            {
                compDistance = distance;
                closestPlayer = NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject;
            }
        }
        return closestPlayer;
    }

}
