using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicZombie : Enemy
{
    new void Update()
    {
        base.Update();
        agent.SetDestination(curTarget.transform.position);
    }
}
