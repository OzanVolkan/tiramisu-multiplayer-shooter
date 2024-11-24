using UnityEngine;

namespace Weapon
{
    public class Kar98 : WeaponBase
    {
        public Kar98()
        {
            FireRate = 1;
            BulletType = "Kar98Bullets";
        }

        protected override void Shoot()
        {
            ShootBullet(BulletType, _firePoint.position, _firePoint.right);
            Debug.Log("Shot with Kar98!");
        }
    }
}