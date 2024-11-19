using Managers;
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
            var bullet = PoolingManager.Instance.GetObject(BulletType);
            bullet.transform.position = _firePoint.position;
            
            Debug.Log("Shot with Kar98!");
        }
    }
}