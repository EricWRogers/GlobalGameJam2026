using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class Deagle : GunBase
{

readonly List<BloodScript> m_ProjectileQueue = new();
    

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
                //GameObject bloodEffect = bloodPool.GetItem();
                //bloodEffect.transform.SetParent(hitInfo.collider.transform);
                //bloodEffect.transform.position = hitInfo.point;
                //bloodEffect.GetComponent<BloodScript>().Setup(IsOwner, OnProjectileDestroy);
    //
                //bloodEffect.transform.rotation = Quaternion.LookRotation(hitInfo.normal);

           
        }
        GameObject hitEffect = sparkPool.GetItem();
        hitEffect.transform.position = hitInfo.point;
        hitEffect.GetComponent<SparkOBJ>().Setup(IsOwner, OnHitSparkDestroy);
        m_SparkQueue.Add(hitEffect.GetComponent<SparkOBJ>());
        currentAmmo-= 1;
    }

    void OnHitSparkDestroy(SparkOBJ projectile)
    {
        if (m_SparkQueue.Contains(projectile))
        {
            m_SparkQueue.Remove(projectile);
        }
        sparkPool.ReturnItem(projectile.gameObject);
    }

    private void OnProjectileDestroy(BloodScript projectile)
    {
        if (m_ProjectileQueue.Contains(projectile))
        {
            m_ProjectileQueue.Remove(projectile);
        }
        bloodPool.ReturnItem(projectile.gameObject);
    }

}
