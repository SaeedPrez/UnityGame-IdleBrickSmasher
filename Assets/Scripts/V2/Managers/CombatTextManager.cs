using Prez.V2.Data;
using Prez.V2.Obstacles;
using Prez.V2.Utilities;
using UnityEngine;

namespace Prez.V2.Managers
{
    public class CombatTextManager : MonoBehaviour
    {
        [SerializeField] private ObjectPool _combatTextPool;

        private EventManager _event;

        #region Bootstrap

        private void Awake()
        {
            _event = RefManager.Event;
        }

        private void OnEnable()
        {
            _event.OnBrickDamaged += OnBrickDamaged;
        }

        private void OnDisable()
        {
            _event.OnBrickDamaged -= OnBrickDamaged;
        }

        #endregion

        #region Combat Text

        private void OnBrickDamaged(DamageData data)
        {
            var combatText = _combatTextPool.GetPooledObject().GetComponent<CombatText>();
            combatText.transform.position = data.CollisionPoint;
            combatText.SetData(data);
            combatText.gameObject.SetActive(true);
        }

        #endregion
    }
}