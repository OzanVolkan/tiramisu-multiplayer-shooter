using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Managers
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private string _lastRoomName;
        private readonly byte _maxPlayersPerRoom = 2;
        private readonly int _sendRate = 30;
        private readonly int _serializationRate = 15;

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.SendRate = _sendRate;
            PhotonNetwork.SerializationRate = _serializationRate;
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
                if (PhotonNetwork.CountOfRooms == 1)
                {
                    CreateNewRoom();
                }
                else if (PhotonNetwork.CountOfRooms > 1)
                {
                    PhotonNetwork.JoinRandomRoom();
                }
            }
            else
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinRandomFailed(short returncode, string message)
        {
            Debug.Log("Failed to join a room. Creating a new one.");
            CreateNewRoom();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
            EventManager.Broadcast(GameEvent.OnJoinedRoom);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (targetPlayer.IsLocal && changedProps.ContainsKey("Team"))
            {
                EventManager.Broadcast(GameEvent.OnPropertiesAssigned);
            }
        }

        public override void OnLeftRoom()
        {
            Debug.Log("Left the room. Attempting to join the lobby...");

            if (!PhotonNetwork.InLobby)
            {
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

            var userId = PhotonNetwork.LocalPlayer?.UserId ?? "Guest";
            var roomName = $"Room_{userId}_{System.DateTime.Now.Ticks}";

            RoomOptions roomOptions = new RoomOptions()
            {
                MaxPlayers = _maxPlayersPerRoom,
                EmptyRoomTtl = 0,
                CleanupCacheOnLeave = true
            };

            _lastRoomName = roomName;
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log($"Disconnected from Photon ({cause}). Reconnecting...");
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Created room");
        }
    }
}