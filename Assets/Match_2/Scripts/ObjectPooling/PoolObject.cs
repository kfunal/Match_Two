using DG.Tweening;
using Helpers;
using UnityEngine;

namespace ObjectPool
{
    public class PoolObject : MonoBehaviour
    {
        [SerializeField] private PoolType poolType;
        private ObjectPooling objectPooling;

        public PoolType PoolType => poolType;
        protected ObjectPooling ObjectPooling => objectPooling;

        protected virtual void Awake() { }
        protected virtual void Start() { }

        public void OnObjectInstantiated(ObjectPooling _objectPooling, bool _returnPool)
        {
            objectPooling = _objectPooling;

            if (_returnPool)
                OnReturnToPool();
        }

        public virtual void OnGetFromPool(Vector3 _position, Transform _parent)
        {
            transform.SetParent(_parent);
            transform.position = _position;
            gameObject.SetActive(true);

            ConsoleHelper.PrintLog($"OnGetFromPool -> {PoolType}");
        }

        public virtual void OnReturnToPool()
        {
            ConsoleHelper.PrintLog($"OnReturnToPool -> {PoolType}");

            transform.SetParent(ObjectPooling.transform);
            transform.position = Vector3.zero;
            transform.DOKill();
            gameObject.SetActive(false);
        }
    }
}
