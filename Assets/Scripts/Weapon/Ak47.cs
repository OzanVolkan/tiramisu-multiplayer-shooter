using System;
using Managers;
using Photon.Pun;
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
            var bullet = PoolingManager.Instance.GetObject(BulletType);
            bullet.transform.position = _firePoint.position;
            
            Debug.Log("Shot with AK-47!");
        }
    }
}