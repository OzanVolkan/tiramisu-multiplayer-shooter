using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        Vector3 player1 = new Vector3(-7.5f, 1, 0);
        Vector3 player2 = new Vector3(7.5f, 1, 0);

        [SerializeField] private GameObject _playerPrefab;
        void Start()
        {
            if (_playerPrefab == null)
            {
                Debug.LogError("Player prefab is missing.");
                return;
            }
            
            // Vector3 spawnPoint = isLocalPlayer ? new Vector3(-5, 0, 0) : new Vector3(5, 0, 0);

        }

        void Update()
        {
        
        }
    }
}
