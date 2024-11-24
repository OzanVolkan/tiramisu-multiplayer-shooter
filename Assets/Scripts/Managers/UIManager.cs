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
            EventManager.AddHandler(GameEvent.OnGameOver, new Action(OnGameOver));
        }

        private void OnDisable()
        {
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Rematch());
            }
        }

        private void OnRematchButtonClicked()
        {
            //RED PLAYER WINS TEXTINI KIMIN KAZANDIGINA GÖRE GUNCELLE!

            StartCoroutine(Rematch());
        }

        //bunu network managera taşı ve eventi tetikle

        private IEnumerator Rematch()
        {
            _gameOverPanel.SetActive(false);
            _waitingRematchPanel.SetActive(true);
            var team = (string)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

            var delay = team == "Blue" ? 0f : 3f;

            yield return new WaitForSeconds(delay);
            _waitingRematchPanel.SetActive(false);
            PhotonNetwork.RemovePlayerCustomProperties(new[] { "Team" });
            PhotonNetwork.LeaveRoom();
        }
    }
}