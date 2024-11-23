using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private Button _rematchButton;

        private void Start()
        {
            print("BAŞLADIIII");
        }

        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnGameOver, new Action(OnGameOver));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnGameOver, new Action(OnGameOver));
        }

        private void OnGameOver()
        {
            _gameOverPanel.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                PhotonNetwork.RemovePlayerCustomProperties(new[] { "Team" });
                PhotonNetwork.LeaveRoom();


                // if (PhotonNetwork.IsMasterClient)
                // {
                //     print("Rye basıldı");
                //     GetComponent<PhotonView>().RPC(nameof(RematchBaby), RpcTarget.AllBuffered);
                // }
            }
        }

        [PunRPC]
        public void RematchBaby()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}