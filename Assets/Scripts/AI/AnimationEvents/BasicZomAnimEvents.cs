using Unity.Netcode;
using UnityEngine;

public class BasicZomAnimEvents : NetworkBehaviour
{
    public BasicZombie basicZombie;

    void Start()
    {
        basicZombie = GetComponentInParent<BasicZombie>();
        if(!IsOwner) return;
    }
    public void CallAttack()
    {
        if(!IsOwner) return;
        basicZombie.Attack();
    }

    public void OnAttackAnimationEnd()
    {
        if(!IsOwner) return;
        basicZombie.agent.isStopped = false;
    }
}
