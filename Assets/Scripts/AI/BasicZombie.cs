using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class BasicZombie : Enemy
{
    private float m_curDist;
    private float m_attackTimer;
    new void Update()
    {

        base.Update();
        agent.SetDestination(curTarget.transform.position);
        m_curDist = agent.remainingDistance;
        Vector3 dir = curTarget.transform.position - transform.position;
        los = Physics.Raycast(transform.position, dir, losMask) ? true : false;
        if(m_curDist <= attackRange && los)
        {
            m_attackTimer -= Time.deltaTime;
            agent.isStopped = true;
            if(m_attackTimer <= 0)
            {
                Attack();
                m_attackTimer = attackSpeed;
            }
            //play attack animation and call attack when needed in animation
        }
        else
        {
            agent.isStopped = false;
        }
    }

    public void Attack()
    {
        Debug.Log("attack");
        if(m_curDist <= attackRange && los)
        {
            ulong id = curTarget.GetComponent<NetworkObject>().OwnerClientId;

            ClientRpcParams clientRpcParams = new ClientRpcParams{
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { id },
                }
            };
            DealDamageClientRPC(id, clientRpcParams);
        }
    }


[ClientRpc]
    public void DealDamageClientRPC(ulong _id, ClientRpcParams rpcParams = default)
    {
        NetworkManager.Singleton.ConnectedClients[_id].PlayerObject.GetComponent<Health>().TakeDamage(damage);

    }
}
