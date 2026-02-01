using UnityEngine;

public class AKGun : GunBase
{


    protected override void ShootGun(Vector3 origin, Vector3 dir)
    {
        bool hit = Physics.Raycast(origin, dir, out RaycastHit hitInfo, range, hitMask);

        Vector3 endPoint = hit ? hitInfo.point : origin + dir * range;

        if(hitInfo.collider.tag == "Enemy")
        {
            Debug.Log("Hit Enemy with AK");
            Health enemyHealth = hitInfo.collider.GetComponent<Health>();
            if(enemyHealth != null)
            {
                Debug.Log("Applying Damage to Enemy");
                enemyHealth.TakeDamage((int)damage);
            }
        }
        currentAmmo-= 1;
        

    }

}
