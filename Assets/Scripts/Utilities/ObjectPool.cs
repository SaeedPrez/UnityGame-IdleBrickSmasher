using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Utilities
{
    public class ObjectPool : MonoBehaviour
    {
        
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Transform _container;
        [SerializeField] private int _startQuantity;
        [SerializeField] private int _maxQuantity;
        [SerializeField] private float _autoReturnAfter;
        
        private ObjectPool<GameObject> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<GameObject>(CreateEffect, 
                OnGetEffect, 
                OnReturnEffect, 
                OnDestroyEffect, 
                true, 
                _startQuantity, 
                _maxQuantity);
        }

        private GameObject CreateEffect()
        {
            var obj = Instantiate(_prefab, _container);
            obj.gameObject.SetActive(false);
            return obj;
        }

        private void OnGetEffect(GameObject obj)
        {
        }

        private void OnReturnEffect(GameObject obj)
        {
            if (obj.activeInHierarchy)
                obj.SetActive(false);
        }
        
        private void OnDestroyEffect(GameObject obj)
        {
            Destroy(obj);
        }

        public GameObject GetPooledObject()
        {
            var obj = _pool.Get();

            if (_autoReturnAfter > 0f)
                StartCoroutine(AutoReturnPooledObject(obj));

            return obj;
        }

        public void ReleasePooledObject(GameObject obj)
        {
            _pool.Release(obj);
        }

        private IEnumerator AutoReturnPooledObject(GameObject obj)
        {
            yield return new WaitForSeconds(_autoReturnAfter);
            ReleasePooledObject(obj);
        }
    }
}