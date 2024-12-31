using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Prez.V2.Utilities
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
            SetupObjectPool();
        }

        #region Object Pool

        private void SetupObjectPool()
        {
            _pool = new ObjectPool<GameObject>(
                OnCreateObject,
                OnGetObject,
                OnReleaseObject,
                OnDestroyObject,
                true,
                _startQuantity,
                _maxQuantity
            );
        }
        
        private GameObject OnCreateObject()
        {
            var obj = Instantiate(_prefab, _container);
            obj.gameObject.SetActive(false);
            return obj;
        }

        private void OnGetObject(GameObject obj)
        {
        }

        private void OnReleaseObject(GameObject obj)
        {
            if (obj.activeInHierarchy)
                obj.SetActive(false);
        }

        private void OnDestroyObject(GameObject obj)
        {
            Destroy(obj);
        }

        #endregion

        #region Auto Release

        private IEnumerator AutoReturnPooledObject(GameObject obj)
        {
            yield return new WaitForSeconds(_autoReturnAfter);
            ReleasePooledObject(obj);
        }

        #endregion
        
        #region Get / Release

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

        #endregion
    }
}