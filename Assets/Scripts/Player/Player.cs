using System;
using Managers;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [Header("Photon")] 
        [SerializeField] private PhotonView _photonView;

        #region Player Material Values

        [Header("Material Values")] [SerializeField]
        private Renderer _renderer;

        [SerializeField] private Material _blueMat;
        [SerializeField] private Material _redMat;

        #endregion

        #region Player Canvas Values

        [Header("Canvas Values")] [SerializeField]
        private Transform _playerCanvasTrans;

        [SerializeField] private TextMeshProUGUI _healthText;

        private readonly Vector3 _canvasRotation = new Vector3(90f, 0f, 0f);
        private readonly float _canvasZ = 0.75f;

        #endregion

        #region Private Fields

        private readonly int _maxHealth = 10;
        private int _health;

        #endregion

        private void Start()
        {
            if (!_photonView.IsMine)
                return;

            InitializePlayer();
        }

        private void OnEnable() =>
            EventManager.AddHandler(GameEvent.OnHitTarget, new Action<GameObject, int>(TakeDamage));

        private void OnDisable() =>
            EventManager.RemoveHandler(GameEvent.OnHitTarget, new Action<GameObject, int>(TakeDamage));

        private void InitializePlayer()
        {
            _photonView.RPC(nameof(SetPlayerCanvasTransformValues), RpcTarget.AllBuffered);
            _photonView.RPC(nameof(SetPlayerHealthValues), RpcTarget.AllBuffered, _maxHealth);
        }

        //Update the health value after taking damage
        private void TakeDamage(GameObject target, int damage)
        {
            if (target != gameObject) return;

            var health = Mathf.Clamp(_health - damage, 0, _maxHealth);

            _photonView.RPC(nameof(SetPlayerHealthValues), RpcTarget.AllBuffered, health);

            Debug.Log($"Enemy has {health} HP left!");

            if (_health <= 0)
            {
                var winnerTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
                _photonView.RPC(nameof(HandleDeath), RpcTarget.AllBuffered, winnerTeam);
                Debug.Log("Enemy is dead!");
            }
        }

        #region RPC Methods

        [PunRPC]
        public void HandleDeath(string winnerTeam)
        {
            Debug.Log("Game Over!");
            UIManager.Instance.WinnerTeam = winnerTeam;
            EventManager.Broadcast(GameEvent.OnGameOver);
        }

        //Set the player's material according to their team
        [PunRPC]
        public void SetPlayerMat(string team)
        {
            Material[] mats = new Material[1];

            Material material = team == GameManager.Instance.TeamBlue ? _blueMat : _redMat;

            mats[0] = material;

            _renderer.materials = mats;
        }

        //Fix the canvas values that are affected by the rotation
        [PunRPC]
        private void SetPlayerCanvasTransformValues()
        {
            _playerCanvasTrans.rotation = Quaternion.Euler(_canvasRotation);
            _playerCanvasTrans.position =
                new Vector3(_playerCanvasTrans.position.x, _playerCanvasTrans.position.y, _canvasZ);
        }

        //Update the player's health value
        [PunRPC]
        private void SetPlayerHealthValues(int newHealthValue)
        {
            _health = newHealthValue;
            _healthText.text = "HP: " + _health;
        }

        #endregion
    }
}