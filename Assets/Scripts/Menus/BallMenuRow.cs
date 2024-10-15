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
        [SerializeField] private TMP_Text _ballNrUi;

        public Ball Ball { get; private set; }
        public BallData Data { get; private set; }
        public bool IsUnlocked { get; private set; }

        private void Awake()
        {
            if (!IsUnlocked)
                _lockedUi.SetActive(true);
        }

        public void SetBall(Ball ball)
        {
            Ball = ball;
        }
        
        public void SetData(BallData data)
        {
            Data = data;
            Ball.SetData(data);
            
            _ballNrUi.SetText($"Ball {Ball.Data.Id}");
            
            UpdateLockLevelUi();
        }

        public void Unlock()
        {
            _lockedUi.SetActive(false);
            IsUnlocked = true;
            EventManager.I.TriggerBallMenuRowUnlocked(this);
        }

        private void UpdateLockLevelUi()
        {
            _lockLevelUi.SetText($"Unlocks at level {Data.UnlockLevel}");
        }
    }
}