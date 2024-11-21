using System;
using Managers;
using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;

        #region PlayerMaterialValues

        [SerializeField] private Renderer _renderer;
        [SerializeField] private Material _blueMat;
        [SerializeField] private Material _redMat;

        #endregion

        #region PlayerCanvasValues

        [SerializeField] private Transform _playerCanvasTrans;
        private readonly Vector3 _canvasRotation = new Vector3(90f, 0f, 0f);
        private readonly float _canvasZ = 0.75f;

        #endregion

        private readonly int _maxHealth = 10;
        private int _health;

        private void Start()
        {
            _health = _maxHealth;

            if (!_photonView.IsMine)
                return;

            _photonView.RPC(nameof(SetPlayerCanvasTransformValues), RpcTarget.AllBuffered);
        }

        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnHitTarget, new Action<int>(TakeDamage));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnHitTarget, new Action<int>(TakeDamage));
        }

        private void TakeDamage(int damage)
        {
            _health = Mathf.Clamp(_health - damage, 0, _maxHealth);

            if (_health <= 0)
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            Debug.Log("Game Over!");
        }

        [PunRPC]
        public void SetPlayerMat(string team)
        {
            Material[] mats = new Material[1];

            if (team == GameManager.Instance.TeamBlue)
            {
                mats[0] = _blueMat;
            }
            else if (team == GameManager.Instance.TeamRed)
            {
                mats[0] = _redMat;
            }

            _renderer.materials = mats;
        }

        [PunRPC]
        private void SetPlayerCanvasTransformValues()
        {
            _playerCanvasTrans.rotation = Quaternion.Euler(_canvasRotation);
            _playerCanvasTrans.position =
                new Vector3(_playerCanvasTrans.position.x, _playerCanvasTrans.position.y, _canvasZ);
        }
    }
}