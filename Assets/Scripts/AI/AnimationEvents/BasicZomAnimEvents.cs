using UnityEngine;

public class BasicZomAnimEvents : MonoBehaviour
{
    public BasicZombie basicZombie;

    void Start()
    {
        basicZombie = GetComponentInParent<BasicZombie>();
    }
    public void CallAttack()
    {
        basicZombie.Attack();
    }

    public void OnAttackAnimationEnd()
    {
        basicZombie.agent.isStopped = false;
    }
}
