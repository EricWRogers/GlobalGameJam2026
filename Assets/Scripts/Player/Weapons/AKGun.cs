using UnityEngine;

public class AKGun : GunBase
{
   [SerializeField] private float damage = 20f;

    [SerializeField] private LayerMask hitMask;

    protected override void ShotGun(Vector3 origin, Vector3 dir)
    {
        bool hit = Physics.Raycast(origin, dir, out RaycastHit hitInfo, range, hitMask);

        Vector3 endPoint = hit ? hitInfo.point : origin + dir * range;

        currentAmmo-= 1;

    }

}
