using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private readonly float _moveSpeed = 6f;
        private readonly float _maxMoveValue = 4f;

        private void Update()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 tempPos = transform.position;

            tempPos.z = Mathf.Clamp(tempPos.z + verticalInput * _moveSpeed * Time.deltaTime, -_maxMoveValue, _maxMoveValue);

            transform.position = tempPos;
        }
    }
}
