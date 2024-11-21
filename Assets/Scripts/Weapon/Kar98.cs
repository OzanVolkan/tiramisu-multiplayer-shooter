using Managers;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System;

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
        public void ShootRPC()
        {
            var bullet = PoolingManager.Instance.GetObject(BulletType);
            bullet.transform.position = _firePoint.position;
        }

        protected override void Shoot()
        {
            _photonView.RPC(nameof(ShootRPC), RpcTarget.AllBuffered);

            Debug.Log("Shot with Kar98!");
        }
    }
}