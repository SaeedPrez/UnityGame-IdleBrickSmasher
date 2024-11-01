using Prez.Data;
using Prez.Utilities;
using UnityEngine;

namespace Prez.Core
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
            combatText.SetText(Helper.GetNumberAsString(data.DamageRaw));
            combatText.gameObject.SetActive(true);
        }
    }
}