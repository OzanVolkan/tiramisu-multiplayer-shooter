using System;
using Managers;
using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        #region MovementParameters

        private readonly float _moveSpeed = 6f;
        private readonly float _maxMoveValue = 4f;

        #endregion

        private PhotonView _photonView;

        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnGameOver, new Action(OnGameOver));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnGameOver, new Action(OnGameOver));
        }
        
        private void Awake()
        {
            PhotonNetwork.SendRate = 30;
            PhotonNetwork.SerializationRate = 15;
        }

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            if (!_photonView.IsMine)
                return;

            var verticalInput = Input.GetAxis("Vertical");
            Vector3 tempPos = transform.position;
            tempPos.z = Mathf.Clamp(tempPos.z + verticalInput * _moveSpeed * Time.fixedDeltaTime, -_maxMoveValue,
                _maxMoveValue);
            transform.position = tempPos;
        }

        private void OnGameOver()
        {
            enabled = false;
        }
    }
}