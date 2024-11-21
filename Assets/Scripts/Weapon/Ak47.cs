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

        [PunRPC]
        public void ShootRPC()
        {
            var bullet = PoolingManager.Instance.GetObject(BulletType);
            bullet.transform.position = _firePoint.position;
        }

        protected override void Shoot()
        {
            _photonView.RPC(nameof(ShootRPC), RpcTarget.AllBuffered);

            Debug.Log("Shot with AK-47!");
        }
    }
}