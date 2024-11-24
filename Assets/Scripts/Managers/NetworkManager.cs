using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Managers
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private readonly byte _maxPlayersPerRoom = 2;
        private string _lastRoomName;
        private bool _isLeavingRoom;
        private bool _isTryingToCreateRoom;
        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();

            PhotonNetwork.SendRate = 30;
            PhotonNetwork.SerializationRate = 15;
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon Master Server.");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined Lobby");

            if (_lastRoomName != null)
            {
                Debug.Log("ROOM COUNT: " + PhotonNetwork.CountOfRooms);

                if (PhotonNetwork.CountOfRooms == 1)
                {
                    AttemptToCreateRoom();
                }
                else if (PhotonNetwork.CountOfRooms > 1)
                {
                    PhotonNetwork.JoinRandomRoom();
                }
            }

            if (_isTryingToCreateRoom)
            {
                _isTryingToCreateRoom = false;
                AttemptToCreateRoom();
            }
            else
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinRandomFailed(short returncode, string message)
        {
            Debug.Log("Failed to join a room. Creating a new one.");
            AttemptToCreateRoom();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
            _lastRoomName = PhotonNetwork.CurrentRoom.Name;
            EventManager.Broadcast(GameEvent.OnJoinedRoom);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (!targetPlayer.IsLocal)
                return;

            if (changedProps.ContainsKey("Team"))
            {
                EventManager.Broadcast(GameEvent.OnPropertiesAssigned);
            }
        }

        public override void OnLeftRoom()
        {
            Debug.Log("Left the room. Attempting to join the lobby...");
            _isTryingToCreateRoom = true;

            // Eğer lobide değilsek, lobiyi bekle
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public void AttemptToCreateRoom()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.Log("Waiting for connection to complete...");
                _isTryingToCreateRoom = true;
                return;
            }

            if (PhotonNetwork.InLobby)
            {
                CreateNewRoom();
            }
            else
            {
                Debug.Log("Joining lobby to create a new room...");
                _isTryingToCreateRoom = true;
                PhotonNetwork.JoinLobby();
            }
        }

        private void CreateNewRoom()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogError("Cannot create a room. Photon is not ready for operations.");
                return;
            }

            string userId = PhotonNetwork.LocalPlayer?.UserId ?? "Guest";
            string uniqueRoomName = $"Room_{userId}_{System.DateTime.Now.Ticks}";

            RoomOptions roomOptions = new RoomOptions()
            {
                MaxPlayers = _maxPlayersPerRoom,
                EmptyRoomTtl = 0,
                CleanupCacheOnLeave = true
            };

            _lastRoomName = uniqueRoomName;

            PhotonNetwork.CreateRoom(uniqueRoomName, roomOptions);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected from Photon. Attempting to reconnect...");
            PhotonNetwork.ConnectUsingSettings();
            _lastRoomName = null;

            // Bağlantıdan sonra yeni bir oda oluşturmayı dene
            AttemptToCreateRoom();
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Created room");
        }
    }
}