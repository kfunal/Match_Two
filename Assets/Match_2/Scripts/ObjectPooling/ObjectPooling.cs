using Helpers;
using UnityEngine;

namespace ObjectPool
{
    public class ObjectPooling : Singleton<ObjectPooling>
    {
        [SerializeField] private Pool[] pools;

        private GameObject tempObject;
        private PoolObject tempPoolObject;
        private PoolObject createdPoolObject;
        private Pool tempPool;
        private PoolObject[] poolObjectsOnScene;

        protected override void Awake()
        {
            base.Awake();
            CreatePools();
        }

        private void CreatePools()
        {
            if (pools.Length == 0)
            {
                ConsoleHelper.PrintError("There is no pools defined");
                return;
            }

            for (int i = 0; i < pools.Length; i++)
            {
                pools[i].InitPoolQueue();
                CreatePool(pools[i]);
            }
        }

        private void CreatePool(Pool _pool)
        {
            for (int i = 0; i < _pool.Size; i++)
            {
                tempObject = Instantiate(_pool.Prefab, transform);
                tempPoolObject = tempObject.GetComponent<PoolObject>();
                tempPoolObject.OnObjectInstantiated(this, true);
                _pool.ReturnToPool(tempPoolObject);
            }
        }

        private Pool GetPool(PoolType _poolType)
        {
            if (pools == null || pools.Length == 0)
            {
                ConsoleHelper.PrintError("There is no pool");
                return null;
            }

            for (int i = 0; i < pools.Length; i++)
            {
                if (pools[i].type == _poolType)
                    return pools[i];
            }

            return null;
        }

        public void ResetPool()
        {
            poolObjectsOnScene = FindObjectsOfType<PoolObject>();

            if (poolObjectsOnScene == null || poolObjectsOnScene.Length == 0)
                return;

            for (int i = 0; i < poolObjectsOnScene.Length; i++)
            {
                tempPool = GetPool(poolObjectsOnScene[i].PoolType);

                if (tempPool == null)
                    continue;

                ReturnPool(poolObjectsOnScene[i]);
            }
        }

        public void ResetPool(PoolType _type)
        {
            poolObjectsOnScene = FindObjectsOfType<PoolObject>();

            if (poolObjectsOnScene == null || poolObjectsOnScene.Length == 0)
                return;

            for (int i = 0; i < poolObjectsOnScene.Length; i++)
            {
                if (poolObjectsOnScene[i].PoolType != _type)
                    continue;

                tempPool = GetPool(poolObjectsOnScene[i].PoolType);

                if (tempPool == null)
                    continue;

                ReturnPool(poolObjectsOnScene[i]);
            }
        }

        public GameObject SpawnObject(PoolType _poolType, Vector3 _position, Transform _parent)
        {
            return GetObjectFromPool(_poolType, _position, _parent).gameObject;
        }

        public T SpawnObject<T>(PoolType _poolType, Vector3 _position, Transform _parent)
        {
            return GetObjectFromPool(_poolType, _position, _parent).GetComponent<T>();
        }

        public void ReturnPool(PoolObject _object)
        {
            tempPool = GetPool(_object.PoolType);

            if (tempPool == null)
            {
                ConsoleHelper.PrintError($"There is no pool with given type -> {_object.PoolType}");
                return;
            }

            tempPool.ReturnToPool(_object);
            _object.OnReturnToPool();
        }

        private PoolObject GetObjectFromPool(PoolType _poolType, Vector3 _position, Transform _parent)
        {
            tempPool = GetPool(_poolType);

            if (tempPool == null)
            {
                ConsoleHelper.PrintError($"There is no pool with given type -> {_poolType}");
                return default;
            }

            tempPoolObject = tempPool.IsThereObjectOnPool ? tempPool.GetFromPool() : CreateObject(tempPool);
            tempPoolObject.OnGetFromPool(_position, _parent);
            return tempPoolObject;
        }

        private PoolObject CreateObject(Pool _pool)
        {
            tempObject = Instantiate(_pool.Prefab, transform);
            createdPoolObject = tempObject.GetComponent<PoolObject>();
            createdPoolObject.OnObjectInstantiated(this, false);
            return createdPoolObject;
        }

        // #if UNITY_EDITOR
        //         private void OnApplicationQuit()
        //         {
        //             ResetPool();
        //         }
        // #endif
    }
}
