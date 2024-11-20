using System;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Managers
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private byte _maxPlayersPerRoom = 2;

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon Master Server.");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined Lobby");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returncode, string message)
        {
            Debug.Log("Failed to join a room. Creating a new one.");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = _maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
            
            EventManager.Broadcast(GameEvent.OnJoinedRoom);
        }
        
        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey("Team"))
            {
                Debug.Log($"{targetPlayer.NickName} has joined team {changedProps["Team"]}");
                EventManager.Broadcast(GameEvent.OnPropertiesAssigned);
            }
        }
    }
}