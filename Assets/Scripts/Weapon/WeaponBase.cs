using UnityEngine;

namespace Weapon
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected Transform _firePoint;
        private float _lastFireTime;

        #region Properties

        protected float FireRate { get; set; }
        protected string BulletType { get; set; }

        #endregion

        public void Fire()
        {
            if (Time.time >= _lastFireTime + FireRate)
            {
                Shoot();
                _lastFireTime = Time.time;
            }
            else
            {
                Debug.Log("Weapon is on cooldown!");
            }
        }

        protected abstract void Shoot();
    }
}
