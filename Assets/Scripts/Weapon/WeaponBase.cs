using Bullet;
using Managers;
using Photon.Pun;
using UnityEngine;

namespace Weapon
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Photon")]
        [SerializeField] protected PhotonView _photonView;
        
        [Header("GunFeatures")]
        [SerializeField] protected Transform _firePoint;

        #region Private Fields

        private float _lastFireTime;

        #endregion
        
        #region Properties
        protected float FireRate { get; set; }
        protected string BulletType { get; set; }
        #endregion

        public void Fire()
        {
            if (Time.time < _lastFireTime + FireRate)
            {
                Debug.Log("Weapon is on cooldown!");
                return;
            }

            _lastFireTime = Time.time;
            Shoot();
        }

        protected abstract void Shoot();
        
        //Shoot a bullet in the given direction
        protected void ShootBullet(string bulletType, Vector3 firePoint, Vector3 direction)
        {
            _photonView.RPC(nameof(ShootRPC), RpcTarget.AllBuffered, bulletType, firePoint, direction);
        }
        
        [PunRPC]
        public void ShootRPC(string bulletType, Vector3 firePoint, Vector3 direction)
        {
            var bullet = PoolingManager.Instance.GetObject(bulletType);
            bullet.transform.position = firePoint;

            var bulletBase = bullet.GetComponent<BulletBase>();
            bulletBase.SetDirection(direction);
        }
    }
}
