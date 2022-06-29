using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RedStudio.Battle10
{
    public class EndGameUI : MonoBehaviour
    {
        [SerializeField, BoxGroup("External")] ObservableSO _winEvent;
        [SerializeField, BoxGroup("External")] ObservableSO _loseEvent;
        [SerializeField, BoxGroup("External")] ObservableSO _gameEnd;
        [SerializeField, BoxGroup("External")] ObservableSO _closeGame;

        [SerializeField, BoxGroup("InternalRef")] Transform _uiRoot;
        [SerializeField, BoxGroup("InternalRef")] Button _quitGameButton;

        [SerializeField, BoxGroup("Conf")] float _waitDuration = 1f;
        [SerializeField, BoxGroup("Conf")] List<SerializableEvent> _onGameOverAfterWait;

        [SerializeField, Foldout("Event")] UnityEvent _onGameOver;
        [SerializeField, Foldout("Event")] UnityEvent _onWin;
        [SerializeField, Foldout("Event")] UnityEvent _onGameEnd;

        void Start()
        {
            _uiRoot.gameObject.SetActive(false);
            // Event binding
            _loseEvent.OnInvoke += LaunchGameOver;
            _winEvent.OnInvoke += LaunchWinScreen;
            _gameEnd.OnInvoke += ShowEndGame;
        }

        void LaunchGameOver()
        {
            _uiRoot.gameObject.SetActive(true);
            _loseEvent.OnInvoke -= LaunchGameOver;
            StartCoroutine(GameOverRoutine());
            IEnumerator GameOverRoutine()
            {
                _onGameOver?.Invoke();
                yield return new WaitForSeconds(_waitDuration);
                _onGameOverAfterWait?.Invoke();
            }
        }

        void LaunchWinScreen()
        {
            _uiRoot.gameObject.SetActive(true);
            _winEvent.OnInvoke -= LaunchWinScreen;
            StartCoroutine(WinRoutine());
            IEnumerator WinRoutine()
            {
                _onWin?.Invoke();
                yield return new WaitForSeconds(_waitDuration);
                _onGameOverAfterWait?.Invoke();
            }
        }
    
        void ShowEndGame()
        {
            _onGameEnd?.Invoke();
            _quitGameButton.onClick.AddListener(QuitGame);
        }
        void QuitGame()
        { 
            _quitGameButton.onClick.RemoveListener(QuitGame);
            (_closeGame as IObservableFire).Invoke();
        }
    }
}
