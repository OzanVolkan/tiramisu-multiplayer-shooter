using UnityEngine;

namespace Weapon
{
    public class Ak47 : WeaponBase
    {
        public Ak47()
        {
            FireRate = 0.15f;
            BulletType = "Ak47Bullets";
        }

        protected override void Shoot()
        {
            ShootBullet(BulletType, _firePoint.position, _firePoint.right);
            Debug.Log("Shot with AK-47!");
        }
    }
}