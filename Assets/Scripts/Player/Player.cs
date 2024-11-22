using System;
using Interfaces;
using Managers;
using Photon.Pun;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI _healthText;
        private readonly Vector3 _canvasRotation = new Vector3(90f, 0f, 0f);
        private readonly float _canvasZ = 0.75f;

        #endregion

        private readonly int _maxHealth = 10;
        private int _health;

        private void Start()
        {
            if (!_photonView.IsMine)
                return;

            _photonView.RPC(nameof(SetPlayerCanvasTransformValues), RpcTarget.AllBuffered);
            _photonView.RPC(nameof(SetPlayerHealthValues), RpcTarget.AllBuffered, _maxHealth);
        }

        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnHitTarget, new Action<GameObject, int>(TakeDamage));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnHitTarget, new Action<GameObject, int>(TakeDamage));
        }

        private void TakeDamage(GameObject go, int damage)
        {
            if (go != gameObject) return;

            var health = Mathf.Clamp(_health - damage, 0, _maxHealth);

            _photonView.RPC(nameof(SetPlayerHealthValues), RpcTarget.AllBuffered, health);

            Debug.Log($"Enemy has {health} HP left!");

            if (_health <= 0)
            {
                _photonView.RPC(nameof(HandleDeath), RpcTarget.AllBuffered);
                Debug.Log("Enemy is dead!");
            }
        }

        #region RPCMethods

        [PunRPC]
        public void HandleDeath()
        {
            Debug.Log("Game Over!");
            EventManager.Broadcast(GameEvent.OnGameOver);
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

        [PunRPC]
        private void SetPlayerHealthValues(int newHealthValue)
        {
            _health = newHealthValue;
            _healthText.text = "HP: " + _health;
        }

        #endregion
    }
}