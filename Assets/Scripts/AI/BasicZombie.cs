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
    }
    new void Update()
    {
        if(!m_serverIsReady) return;
        if (!m_isdead)
        {
            if(curTarget == null)
            {
                curTarget = GetClosestPlayerInRange();
            }
            base.Update();
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
             m_flashTimer -= Time.deltaTime;
            if(m_flashTimer <= 0)
            {
                modelRenderer.materials[1].color = Color.clear;
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
        Debug.Log("attack");
        if(m_curDist <= attackRange)
        {
            ulong id = curTarget.GetComponent<NetworkObject>().OwnerClientId;

            ClientRpcParams clientRpcParams = new ClientRpcParams{
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { id },
                }
            };
            DealDamageClientRpc(id, clientRpcParams);
        }
    }


[ClientRpc]
    public void DealDamageClientRpc(ulong _id, ClientRpcParams rpcParams = default)
    {
        NetworkManager.Singleton.ConnectedClients[_id].PlayerObject.GetComponent<Health>().TakeDamage(damage);

    }

    public void Dead()
    {
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
