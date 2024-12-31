using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prez.V1.Data;
using UnityEngine;

namespace Prez.V1.Core
{
    public class DpsManager : MonoBehaviour
    {
        [SerializeField] private float _dpsDuration;
        [SerializeField] private float _dpsUpdateTime;
        
        private readonly Dictionary<int, Dictionary<float, double>> _history = new();
        private readonly Dictionary<int, double> _dps = new();
        
        private void OnEnable()
        {
            EventManager.I.OnBrickDamaged += OnBrickDamaged;
            StartCoroutine(CalculateDps());
        }

        private void OnDisable()
        {
            EventManager.I.OnBrickDamaged -= OnBrickDamaged;
            StopAllCoroutines();
        }
        
        private void OnBrickDamaged(DamageData data)
        {
            AddDamageToHistory(data);
        }
        
        #region Dps

        /// <summary>
        /// Adds the damage to damage history.
        /// </summary>
        /// <param name="data"></param>
        private void AddDamageToHistory(DamageData data)
        {
            var key = data.Ball != null
                ? data.Ball.Data.Id
                : 0;

            if (!_history.ContainsKey(key))
                _history[key] = new Dictionary<float, double>();

            if (_history[key].ContainsKey(Time.time))
                _history[key][Time.time] += data.Damage;
            else
                _history[key][Time.time] = data.Damage;
        }

        public double GetDps(int id)
        {
            if (_dps.ContainsKey(id))
                return _dps[id];

            return 0;
        }

        /// <summary>
        ///     Calculates the DPS based on damage done in the last 30 seconds.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CalculateDps()
        {
            while (true)
            {
                var duration = Mathf.Min(_dpsDuration, Time.time);

                yield return new WaitForSeconds(_dpsUpdateTime);
                
                if (_history.Keys.Count < 1)
                    continue;

                var historyKeys = _history.Keys.ToArray();

                foreach (var historyKey in historyKeys)
                {
                    _history[historyKey] = _history[historyKey]
                        .Where(x => x.Key >= Time.time - duration)
                        .ToDictionary(x => x.Key, x => x.Value);
                    
                    _dps[historyKey] = _history[historyKey].Sum(x => x.Value) / duration;
                }

                EventManager.I.TriggerDpsUpdated(_dps);
            }
        }

        #endregion
    }
}