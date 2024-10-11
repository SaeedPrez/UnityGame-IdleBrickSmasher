using Core;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class BallMenuRow : MonoBehaviour
    {
        [SerializeField] private GameObject _lockedUi;
        [SerializeField] private TMP_Text _lockLevelUi;
        [SerializeField] private Button _talentsButton;

        public Ball Ball { get; private set; }
        public BallData Data { get; private set; }
        public bool IsUnlocked { get; private set; }
        
        private EventManager _event;
        private GameManager _game;
        private GameData _gameData;

        private void Awake()
        {
            _event = EventManager.I;
            _game = GameManager.I;
            _gameData = _game.Data;
            
            _lockedUi.SetActive(true);
            Ball.gameObject.SetActive(false);
        }

        public void SetBall(Ball ball)
        {
            Ball = ball;
        }
        
        public void SetData(BallData data)
        {
            Data = data;
            Ball.SetData(data);
            
            UpdateLockLevelUi();
        }

        public void Unlock()
        {
            _lockedUi.SetActive(false);
            Ball.gameObject.SetActive(true);
            IsUnlocked = true;
            _event.TriggerBallMenuRowUnlocked(this);
        }

        private void UpdateLockLevelUi()
        {
            _lockLevelUi.SetText($"Unlocks at level {Data.UnlockLevel}");
        }
    }
}