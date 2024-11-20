using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;

        #region PlayerTransformValues

        private readonly Vector3 _blueTeamPos = new Vector3(-7.5f, 1f, 0f);
        private readonly Vector3 _redTeamPos = new Vector3(7.5f, 1f, 0f);

        private readonly Vector3 _blueTeamRotation = new Vector3(0f, 0f, 0f);
        private readonly Vector3 _redTeamRotation = new Vector3(0f, 180f, 0f);

        #endregion

        #region Teams

        private readonly string _teamBlue = "Blue";
        private readonly string _teamRed = "Red";

        #endregion

        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnJoinedRoom, new Action(AssignTeam));
            EventManager.AddHandler(GameEvent.OnPropertiesAssigned, new Action(CreatePlayer));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnJoinedRoom, new Action(AssignTeam));
            EventManager.RemoveHandler(GameEvent.OnPropertiesAssigned, new Action(CreatePlayer));
        }

        private void CreatePlayer()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogError("Not connected to Photon. Please join a room first.");
                return;
            }

            if (PhotonNetwork.InRoom)
            {
                GameObject newPlayer = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);

                PhotonView photonView = newPlayer.GetComponent<PhotonView>();

                Vector3 spawnPos = Vector3.zero;
                Vector3 spawnRotation = Vector3.zero;
                
                print("teammmm: " + (string)PhotonNetwork.LocalPlayer.CustomProperties["Team"]);

                if ((string)PhotonNetwork.LocalPlayer.CustomProperties["Team"] == _teamBlue)
                {
                    spawnPos = _blueTeamPos;
                    spawnRotation = _blueTeamRotation;
                }
                else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["Team"] == _teamRed)
                {
                    spawnPos = _redTeamPos;
                    spawnRotation = _redTeamRotation;
                }

                // var spawnPos = photonView.IsMine  ? _player1Pos : _player2Pos;

                newPlayer.transform.position = spawnPos;
                newPlayer.transform.rotation = Quaternion.Euler(spawnRotation);
            }
        }

        private void AssignTeam()
        {
            var assignedTeam = PhotonNetwork.CurrentRoom.PlayerCount - 1 % 2 == 0 ? _teamBlue : _teamRed;
            Hashtable playerProperties = new Hashtable { { "Team", assignedTeam } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }
    }
}