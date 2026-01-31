using UnityEngine;

public class RPG : GunBase
{
   protected override void ShootGun(Vector3 origin, Vector3 dir)
   {
       // RPG-specific shooting logic (e.g., launching a rocket projectile)
       Debug.Log("RPG Fired!");

       // Call base shooting logic if needed
       base.Shoot();
   }
}
