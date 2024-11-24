using System;
using System.Collections;
using JetBrains.Annotations;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : SingletonManager<UIManager>
    {
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

        private void OnGameOver()
        {
            _gameOverPanel.SetActive(true);
            _winnerText.text = WinnerTeam + " Player Wins!";
        }
        private void OnRematchButtonClicked()
        {
            EventManager.Broadcast(GameEvent.OnRematch);
            _gameOverPanel.SetActive(false);
            _waitingRematchPanel.SetActive(true);
        }

        private void OnGameStart()
        {
            _waitingRematchPanel.SetActive(false);
        }
    }
}