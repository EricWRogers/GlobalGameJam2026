using Unity.Netcode;
using UnityEngine;

public class AKGun : GunBase
{
    

    protected override void ShootGun(Vector3 origin, Vector3 dir)
    {
       
       ShootGunRpc(origin, dir); 

    }

    [Rpc(SendTo.Everyone)]
    private void ShootGunRpc(Vector3 origin, Vector3 dir)
    {
          bool hit = Physics.Raycast(origin, dir, out RaycastHit hitInfo, range, hitMask);

        Vector3 endPoint = hit ? hitInfo.point : origin + dir * range;

        if(hitInfo.collider.tag == "Enemy")
        {
            
             Health enemyHealth = hitInfo.collider.gameObject.GetComponent<Health>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage((int)damage);
            }
           
        }
        currentAmmo-= 1;
    }

}
