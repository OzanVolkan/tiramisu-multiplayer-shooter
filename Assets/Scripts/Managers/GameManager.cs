using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Managers
{
    public class GameManager : SingletonManager<GameManager>
    {
        [Header("Photon")] 
        [SerializeField] private PhotonView _photonView;

        #region Player Values

        [Header("Player")] [SerializeField] private GameObject _playerPrefab;

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

        private bool _isGameOver;

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

        //Create players and assign their position, rotations and materials based to their teams
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

                var team = (string)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

                var spawnPos = team == _teamBlue ? _blueTeamPos : _redTeamPos;
                var spawnRotation = team == _teamBlue ? _blueTeamRotation : _redTeamRotation;

                newPlayer.transform.position = spawnPos;
                newPlayer.transform.rotation = Quaternion.Euler(spawnRotation);

                photonView.RPC("SetPlayerMat", RpcTarget.AllBuffered, team);
                EventManager.Broadcast(GameEvent.OnGameStart);
            }
        }

        //Assign the player to a team as a custom property
        private void AssignTeam()
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
                return;

            var assignedTeam = _teamBlue;

            var blueTeamExists = false;
            var redTeamExists = false;

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("Team"))
                {
                    string team = (string)player.CustomProperties["Team"];
                    if (team == _teamBlue) blueTeamExists = true;
                    if (team == _teamRed) redTeamExists = true;
                }
            }

            if (blueTeamExists)
            {
                assignedTeam = _teamRed;  // If Blue team exists, assign Red team
            }
            else if (redTeamExists)
            {
                assignedTeam = _teamBlue;  // If Red team exists, assign Blue team
            }

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
        }
    }
}