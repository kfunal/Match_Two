using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    [System.Serializable]
    public class Pool
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int Size { get; private set; }
        [field: SerializeField] public PoolType type { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
        private Queue<PoolObject> objects;

        public bool IsThereObjectOnPool => objects.Count > 0;

        public void InitPoolQueue() => objects = new Queue<PoolObject>();
        public void ReturnToPool(PoolObject _poolObject) => objects.Enqueue(_poolObject);
        public PoolObject GetFromPool() => objects.Dequeue();
    }
}