using System;
using System.Collections;
using Managers;
using Photon.Pun;
using UnityEngine;

namespace Bullet
{
    public abstract class BulletBase : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;

        private readonly float _baseSpeed = 5f;

        #region Properties

        protected int Damage { get; set; }
        protected float SpeedMultiplier { get; set; }
        protected string BulletType { get; set; }

        #endregion
        
        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            transform.Translate(transform.right * (Time.fixedDeltaTime * SpeedMultiplier * _baseSpeed));
            Debug.Log($"Ping: {PhotonNetwork.GetPing()}ms");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EventManager.Broadcast(GameEvent.OnHitTarget, Damage);
                Debug.Log("Hit the enemy!");
            }
            else if (other.CompareTag("Border"))
            {
                Debug.Log("Missed the target!");
            }
            else
            {
                return;
            }

            _photonView.RPC(nameof(ReturnBullet), RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void ReturnBullet()
        {
            if (!_photonView.IsMine) return;

            PoolingManager.Instance.ReturnObject(BulletType, gameObject);
        }
    }
}