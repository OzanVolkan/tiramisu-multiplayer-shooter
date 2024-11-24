using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Managers
{
    public class GameRestartManager : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;
        
        private int _readyPlayerCount = 0;
        
        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnRematch, new Action(PlayerReadyToRematch));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnRematch, new Action(PlayerReadyToRematch));
        }

        private void PlayerReadyToRematch()
        {
            _photonView.RPC(nameof(IncrementReadyCount), RpcTarget.All);
        }

        [PunRPC]
        private void IncrementReadyCount()
        {
            _readyPlayerCount++;

            if (_readyPlayerCount >= 2)
            {
                StartCoroutine(RestartGame());
                _readyPlayerCount = 0;
            }
        }

        private IEnumerator RestartGame()
        {
            var team = (string)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            var delay = team == "Blue" ? 0f : 3f;
            yield return new WaitForSeconds(delay);

            PhotonNetwork.RemovePlayerCustomProperties(new[] { "Team" });
            PhotonNetwork.LeaveRoom();
        }
    }
}
