using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private Button _rematchButton;
        
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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Rematch(0));
            }
        }

        private void OnRematchButtonClicked()
        {
            //BİRİ ODADAN AYRILINCA MASTER CLIENT OLMA HAKKI DİĞERİNE GEÇİYOR!! İSTEDİĞİMİ SONUCA ULAŞAMIYORUZ
            //TAKIMA GÖRE KONTROL ŞARTI KOYABİLİRİZ ÖRN. RED TEAM İSE 5 SN DELAY?
            if (PhotonNetwork.IsMasterClient)
            {
                _gameOverPanel.SetActive(false);
                StartCoroutine(Rematch(0f));

            }
            else
            {
                _gameOverPanel.SetActive(false);
                StartCoroutine(Rematch(5f));
            }
        }

        //bunu network managera taşı ve eventi tetikle
        
        private IEnumerator Rematch(float delay)
        {
            yield return new WaitForSeconds(delay);
            PhotonNetwork.RemovePlayerCustomProperties(new[] { "Team" });
            PhotonNetwork.LeaveRoom();
        }
        
    }
}