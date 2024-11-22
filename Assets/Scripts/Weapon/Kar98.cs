using Managers;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System;
using Bullet;

namespace Weapon
{
    public class Kar98 : WeaponBase
    {
        public Kar98()
        {
            FireRate = 1;
            BulletType = "Kar98Bullets";
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

            Debug.Log("Shot with Kar98!");
        }
    }
}