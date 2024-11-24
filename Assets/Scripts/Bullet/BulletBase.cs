using System;
using Managers;
using Photon.Pun;
using UnityEngine;

namespace Bullet
{
    public abstract class BulletBase : MonoBehaviour
    {
        [Header("Photon")]
        [SerializeField] private PhotonView _photonView;

        #region Private Fields

        private readonly float _baseSpeed = 5f;
        private Vector3 _direction;

        #endregion

        #region Properties

        protected int Damage { get; set; }
        protected float SpeedMultiplier { get; set; }
        protected string BulletType { get; set; }

        #endregion

        private void OnEnable() => EventManager.AddHandler(GameEvent.OnGameOver, new Action(OnGameOver));

        private void OnDisable() => EventManager.RemoveHandler(GameEvent.OnGameOver, new Action(OnGameOver));

        //Set bullet's directon according to player's global direction
        public void SetDirection(Vector3 dir) => _direction = dir.normalized;

        private void FixedUpdate() => Move();

        //Bullet movement
        private void Move() => transform.Translate(_direction * (Time.fixedDeltaTime * SpeedMultiplier * _baseSpeed));

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Player":
                    HandlePlayerCollision(other);
                    break;
                case "Border":
                    Debug.Log("Missed the target!");
                    break;
                default:
                    return;
            }

            _photonView.RPC(nameof(ReturnBullet), RpcTarget.AllBuffered);
        }

        private void HandlePlayerCollision(Collider other)
        {
            var targetPhotonView = other.GetComponent<PhotonView>();
            if (targetPhotonView != null && !targetPhotonView.IsMine)
            {
                EventManager.Broadcast(GameEvent.OnHitTarget, other.gameObject, Damage);
                Debug.Log("Hit the enemy!");
            }
        }

        [PunRPC]
        public void ReturnBullet()
        {
            if (!_photonView.IsMine) return;

            PoolingManager.Instance.ReturnObject(BulletType, gameObject);
        }

        private void OnGameOver() => _photonView.RPC(nameof(ReturnBullet), RpcTarget.AllBuffered);
    }
}