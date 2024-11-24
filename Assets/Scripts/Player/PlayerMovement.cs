using System;
using Managers;
using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Photon")] 
        [SerializeField] private PhotonView _photonView;

        #region MovementParameters

        private readonly float _moveSpeed = 6f;
        private readonly float _maxMoveValue = 4f;

        #endregion


        private void OnEnable() => EventManager.AddHandler(GameEvent.OnGameOver, new Action(OnGameOver));

        private void OnDisable() => EventManager.RemoveHandler(GameEvent.OnGameOver, new Action(OnGameOver));

        private void FixedUpdate()
        {
            if (!_photonView.IsMine)
                return;

            MovePlayer();
        }

        private void MovePlayer()
        {
            var verticalInput = Input.GetAxis("Vertical");
            var tempPos = transform.position;
            tempPos.z = Mathf.Clamp(tempPos.z + verticalInput * _moveSpeed * Time.fixedDeltaTime, -_maxMoveValue,
                _maxMoveValue);
            transform.position = tempPos;
        }

        private void OnGameOver() => enabled = false;
    }
}