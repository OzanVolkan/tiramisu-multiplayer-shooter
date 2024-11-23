using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Managers
{
    public class GameManager : SingletonManager<GameManager>
    {
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private GameObject _playerPrefab;
        private bool _isGameOver;

        #region PlayerValues

        private readonly Vector3 _blueTeamPos = new Vector3(-7.5f, 1f, 0f);
        private readonly Vector3 _redTeamPos = new Vector3(7.5f, 1f, 0f);

        private readonly Vector3 _blueTeamRotation = new Vector3(0f, 0f, 0f);
        private readonly Vector3 _redTeamRotation = new Vector3(0f, 180f, 0f);

        #endregion

        #region Teams

        private readonly string _teamBlue = "Blue";
        private readonly string _teamRed = "Red";

        public string TeamBlue => _teamBlue;
        public string TeamRed => _teamRed;

        #endregion

        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnJoinedRoom, new Action(AssignTeam));
            EventManager.AddHandler(GameEvent.OnPropertiesAssigned, new Action(CreatePlayer));
            EventManager.AddHandler(GameEvent.OnGameOver, new Action(OnGameOver));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnJoinedRoom, new Action(AssignTeam));
            EventManager.RemoveHandler(GameEvent.OnPropertiesAssigned, new Action(CreatePlayer));
            EventManager.RemoveHandler(GameEvent.OnGameOver, new Action(OnGameOver));
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
                print("Create player!");
                
                GameObject newPlayer = PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);
                PhotonView photonView = newPlayer.GetComponent<PhotonView>();

                Vector3 spawnPos = Vector3.zero;
                Vector3 spawnRotation = Vector3.zero;

                var team = (string)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

                if (team == _teamBlue)
                {
                    spawnPos = _blueTeamPos;
                    spawnRotation = _blueTeamRotation;
                }
                else if (team == _teamRed)
                {
                    spawnPos = _redTeamPos;
                    spawnRotation = _redTeamRotation;
                }

                newPlayer.transform.position = spawnPos;
                newPlayer.transform.rotation = Quaternion.Euler(spawnRotation);

                photonView.RPC("SetPlayerMat", RpcTarget.AllBuffered, team);
            }
        }

        private void AssignTeam()
        {
            print("Assign1");
            
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
                return;

            print("Assign2");

            var assignedTeam = PhotonNetwork.CurrentRoom.PlayerCount - 1 % 2 == 0 ? _teamBlue : _teamRed;

            Hashtable playerProperties = new Hashtable
            {
                { "Team", assignedTeam },
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }

        private void OnGameOver()
        {
            if (_isGameOver) return;

            _isGameOver = true;

            _photonView.RPC(nameof(GameOverRPC), RpcTarget.AllBuffered);
        }

        //timeScale'i 0 yapmak istemedim, controlleri manuel olarak devredışı bıraktım. ***İngilizceye çevrilecek!!!
        [PunRPC]
        private void GameOverRPC()
        {
            ShowGameOverScreen();
        }

        private void ShowGameOverScreen()
        {
            // Game Over ekranını göster
        }
    }
}