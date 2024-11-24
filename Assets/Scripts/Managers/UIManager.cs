using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : SingletonManager<UIManager>
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject _waitingRematchPanel;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _winnerText;
        [SerializeField] private Button _rematchButton;

        public string WinnerTeam { get; set; }

        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnGameStart, new Action(OnGameStart));
            EventManager.AddHandler(GameEvent.OnGameOver, new Action(OnGameOver));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnGameStart, new Action(OnGameStart));
            EventManager.RemoveHandler(GameEvent.OnGameOver, new Action(OnGameOver));
        }

        private void Start()
        {
            _rematchButton.onClick.AddListener(OnRematchButtonClicked);
        }

        // Display the game over panel and show the winner
        private void OnGameOver()
        {
            _gameOverPanel.SetActive(true);
            _winnerText.text = WinnerTeam + " Player Wins!";
        }
        
        // Broadcast the rematch event and update UI visibility
        private void OnRematchButtonClicked()
        {
            EventManager.Broadcast(GameEvent.OnRematch);
            _gameOverPanel.SetActive(false);
            _waitingRematchPanel.SetActive(true);
        }

        // Hide the rematch waiting panel once the game starts
        private void OnGameStart()
        {
            _waitingRematchPanel.SetActive(false);
        }
    }
}