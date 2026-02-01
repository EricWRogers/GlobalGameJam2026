using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class BasicZombie : Enemy
{
    private float m_curDist;
    private float m_attackTimer;
    public Animator anim;

    public bool m_isdead;
    public float decayDelay;
    public Vector3 deadOffset;
    private Vector3 m_deadPos;

    public SkinnedMeshRenderer modelRenderer;
    public Color hurtColor;
    public float flashTime;
    private float m_flashTimer;
    new void Start()
    {
        base.Start();
        if(!IsOwner) return;
    }
    new void Update()
    {
        if(!m_serverIsReady || !IsOwner) return;
        if (!m_isdead)
        {
            if(curTarget == null)
            {
                curTarget = GetClosestPlayerInRange();
            }
            base.Update();
            if (curTarget != null)
            {
                agent.SetDestination(curTarget.transform.position);
                m_curDist = agent.remainingDistance;
                Vector3 dir = curTarget.transform.position - transform.position;
                //los = Physics.Raycast(transform.position, dir, losMask) ? true : false;
                if(m_curDist <= attackRange)
                {
                    agent.isStopped = true;
                    anim.SetBool("Attacking", true);
                }
                else
                {
                    anim.SetBool("Attacking", false);
                }
            }
             m_flashTimer -= Time.deltaTime;
            if(m_flashTimer <= 0)
            {
                //modelRenderer.materials[1].color = Color.clear;
            }            
        }

        if (m_isdead)
        {
            agent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            decayDelay -= Time.deltaTime;
            float speed = 3 * Time.deltaTime;
            if(decayDelay <= 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_deadPos, speed);
            }
            if(transform.position == m_deadPos)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void Attack()
    {
        if(m_curDist <= attackRange && IsOwner)
        {
             Debug.Log("attack");
            if(curTarget == null)
            {
                Debug.Log("curTarget is null");
                return;
            }

            NetworkObject netObj = curTarget.GetComponent<NetworkObject>();
            if (netObj == null)
                netObj = curTarget.GetComponentInParent<NetworkObject>();
            if (netObj == null)
            {
                Debug.Log("Target has no NetworkObject; cannot determine owner id");
                return;
            }
            ulong id = netObj.OwnerClientId;

            DealDamageRpc(id);
        }
    }


    [Rpc(SendTo.Everyone)]
public void DealDamageRpc(ulong targetClientId)
{
    Debug.Log("[DealDamageRpc] Called for targetClientId: " + targetClientId);


    if(targetClientId != NetworkManager.Singleton.LocalClientId)
    {
        Debug.LogWarning("[DealDamageRpc] This is not meant for us. Aborting.");
        return;
    }

    if (targetClientId == 0)
    {
        Debug.LogWarning("[DealDamageRpc] targetClientId is 0. Aborting.");
        return;
    }

    if (NetworkManager.Singleton == null)
    {
        Debug.LogError("[DealDamageRpc] NetworkManager.Singleton is null. Cannot process RPC.");
        return;
    }

    if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(targetClientId, out var client))
    {
        Debug.LogWarning($"[DealDamageRpc] No connected client with ID {targetClientId}");
        return;
    }

    if (client.PlayerObject == null)
    {
        Debug.LogWarning($"[DealDamageRpc] Client {targetClientId} has no PlayerObject assigned.");
        return;
    }

    var health = client.PlayerObject.GetComponent<Health>();
    if (health == null)
    {
        Debug.LogWarning($"[DealDamageRpc] PlayerObject of client {targetClientId} has no Health component.");
        return;
    }

    Debug.Log($"[DealDamageRpc] Dealing {damage} damage to client {targetClientId}");
    health.TakeDamage(damage);
}

    public void Dead()
    {
        if(!IsOwner) return;    
        EnemyManager.Instance.amountKilled++;
        m_isdead = true;
        m_deadPos = transform.position - deadOffset;
        anim.SetBool("Dead", true);
        
    }
    public void FlashRed()
    {
        modelRenderer.materials[0].color = hurtColor;
        m_flashTimer = flashTime;
    }

     
}
