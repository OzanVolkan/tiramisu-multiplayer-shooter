using System;
using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour, IPunObservable
    {
        #region MovementParameters

        private readonly float _moveSpeed = 6f;
        private readonly float _maxMoveValue = 4f;

        #endregion

        private Vector3 _othersLatestPosition;
        private Vector3 _othersLatestVelocity;
        private float _lag;

        private PhotonView _photonView;

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
            {
                // Gecikmeyi dikkate alarak pozisyonu tahmin et
                Vector3 targetPosition = _othersLatestPosition + _othersLatestVelocity * _lag;
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * 10f);
                return;
            }

            var verticalInput = Input.GetAxis("Vertical");

            Vector3 tempPos = transform.position;

            tempPos.z = Mathf.Clamp(tempPos.z + verticalInput * _moveSpeed * Time.fixedDeltaTime, -_maxMoveValue, _maxMoveValue);

            transform.position = tempPos;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Pozisyon ve hız bilgisini gönder
                stream.SendNext(transform.position);
                stream.SendNext(GetComponent<Rigidbody>().velocity);
            }
            else
            {
                // Remote oyuncunun pozisyonunu ve hızını al
                _othersLatestPosition = (Vector3)stream.ReceiveNext();
                _othersLatestVelocity = (Vector3)stream.ReceiveNext();

                // Gecikmeyi hesapla
                _lag = Mathf.Abs((float)PhotonNetwork.Time - (float)info.SentServerTime);
            }
        }
    }
}
