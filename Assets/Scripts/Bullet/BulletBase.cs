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
        private Vector3 _direction;

        #region Properties

        protected int Damage { get; set; }
        protected float SpeedMultiplier { get; set; }
        protected string BulletType { get; set; }

        #endregion

        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnGameOver, new Action(OnGameOver));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnGameOver, new Action(OnGameOver));
        }

        public void SetDirection(Vector3 dir)
        {
            _direction = dir.normalized;
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            transform.Translate(_direction * (Time.fixedDeltaTime * SpeedMultiplier * _baseSpeed));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var targetPhotonView = other.GetComponent<PhotonView>();
                if (targetPhotonView != null && !targetPhotonView.IsMine)
                {
                    EventManager.Broadcast(GameEvent.OnHitTarget, other.gameObject, Damage);
                    Debug.Log("Hit the enemy!");
                }
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

        private void OnGameOver()
        {
            _photonView.RPC(nameof(ReturnBullet), RpcTarget.AllBuffered);
        }
    }
}