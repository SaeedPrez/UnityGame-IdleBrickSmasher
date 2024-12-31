using Prez.V1.Data;
using Prez.V1.Utilities;
using UnityEngine;

namespace Prez.V1.Core
{
    public class CombatTextManager : MonoBehaviour
    {
        [SerializeField] private ObjectPool _combatTextPool;

        private void OnEnable()
        {
            EventManager.I.OnBrickDamaged += OnBrickDamaged;
        }

        private void OnDisable()
        {
            EventManager.I.OnBrickDamaged -= OnBrickDamaged;
        }

        private void OnBrickDamaged(DamageData data)
        {
            var combatText = _combatTextPool.GetPooledObject().GetComponent<CombatText>();
            combatText.transform.position = data.Point;
            combatText.SetData(data);
            combatText.gameObject.SetActive(true);
        }
    }
}