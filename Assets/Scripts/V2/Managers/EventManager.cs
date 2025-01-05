using System;
using Prez.V2.Balls;
using Prez.V2.Data;
using Prez.V2.Enums;
using Prez.V2.Obstacles;
using Prez.V2.Ui;
using UnityEngine;

namespace Prez.V2.Managers
{
    public class EventManager : MonoBehaviour
    {
        #region State

        public event Action<EGameState, EGameState> OnGameStateChanged = delegate { };
        public void TriggerGameStateChanged(EGameState newState, EGameState oldState) => OnGameStateChanged?.Invoke(newState, oldState);

        #endregion

        #region Input

        public event Action<Vector2> OnPlayerInputMove = delegate { };
        public void TriggerPlayerInput(Vector2 playerInput) => OnPlayerInputMove?.Invoke(playerInput);

        public event Action OnPlayerInputBallAction1 = delegate { };
        public void TriggerPlayerBallAction1() => OnPlayerInputBallAction1?.Invoke();
        
        #endregion
        
        #region Balls

        public event Action<Ball, Brick> OnBallCollidedWithBrick = delegate { };
        public void TriggerBallCollidedWithBrick(Ball ball, Brick brick) => OnBallCollidedWithBrick?.Invoke(ball, brick);

        #endregion

        #region Bricks

        public event Action OnBricksLoaded = delegate { };
        public void TriggerBricksLoaded() => OnBricksLoaded?.Invoke();
        
        public event Action<DamageData> OnBrickDamaged = delegate { };
        public void TriggerBrickDamaged(DamageData data) => OnBrickDamaged.Invoke(data);

        #endregion

        #region Experience

        public event Action<double> OnExperienceGained = delegate { };
        public void TriggerExperienceGained(double experience) => OnExperienceGained?.Invoke(experience);

        public event Action<int> OnLeveledUp = delegate { };
        public void TriggerLeveledUp(int level) => OnLeveledUp.Invoke(level);

        #endregion

        #region Talents

        public event Action<TalentNodeUi> OnTalentNodeLeveledUp = delegate { };
        public void TriggerTalentPointLeveledUp(TalentNodeUi nodeUi) => OnTalentNodeLeveledUp?.Invoke(nodeUi);

        #endregion
    }
}