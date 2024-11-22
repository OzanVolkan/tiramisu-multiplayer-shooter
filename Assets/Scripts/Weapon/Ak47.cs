using System;
using Bullet;
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

        [PunRPC]
        public void ShootRPC(Vector3 firePoint, Vector3 direction)
        {
            var bullet = PoolingManager.Instance.GetObject(BulletType);
            bullet.transform.position = firePoint;
            
            var bulletBase = bullet.GetComponent<BulletBase>();
            bulletBase.SetDirection(direction);
        }

        protected override void Shoot()
        {
            _photonView.RPC(nameof(ShootRPC), RpcTarget.AllBuffered, _firePoint.position, _firePoint.right);

            Debug.Log("Shot with AK-47!");
        }
    }
}