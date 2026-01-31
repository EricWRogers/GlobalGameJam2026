using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int damage = 5;
    public float attackSpeed = 3f;
    public float attackRange = 2f;
    public float switchTargetRange = 10;
    public NavMeshAgent agent;
    public GameObject curTarget;
    public GameObject[] allTargets;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        allTargets = GameObject.FindGameObjectsWithTag("Player");
        curTarget = GetClosestPlayerInRange();
    }

    public void Update()
    {
        if(Vector3.Distance(transform.position, curTarget.transform.position) > switchTargetRange)
        {
            curTarget = GetClosestPlayerInRange();
        }
    }

    public GameObject GetClosestPlayerInRange()
    {
        GameObject closestPlayer = null;
        float compDistance = 10000;
        foreach(GameObject go in allTargets)
        {
            float distance = Vector3.Distance(transform.position, go.transform.position);
            Debug.Log(go.name + " is " + distance + " units away");
            if(distance < compDistance && distance < switchTargetRange)
            {
                compDistance = distance;
                closestPlayer = go;
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
        foreach(GameObject go in allTargets)
        {
            float distance = Vector3.Distance(transform.position, go.transform.position);
            if(distance < compDistance)
            {
                compDistance = distance;
                closestPlayer = go;
            }
        }
        return closestPlayer;
    }

}
