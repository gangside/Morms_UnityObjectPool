using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolName
{
    //사용할 풀종류의 이름을 정하세요.
    AObjectPool,
    BObjectPool
}

public class ObjectPool : MonoBehaviour
{
    public Pool[] pools;

    #region internal
    private void Awake()
    {
        CreatePoolsTr();
    }

    // CreatePoolsTr 풀이 될 부모객체를 생성
    private void CreatePoolsTr()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            GameObject poolTr = new GameObject(pools[i].poolName.ToString());
            pools[i].poolTr = poolTr.transform;
            poolTr.transform.SetParent(this.transform);
            PooiInitialize(pools[i], pools[i].quantity);
        }
    }
    //풀의 오브젝트 큐 리스트에 원하는 수량만큼 Init시킴
    private void PooiInitialize(Pool pool, int initCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            pool.poolingObjectQueue.Enqueue(CreateNewObject(pool));
        }
    }
    //풀에 들어갈 객체들을 생성하여 반환
    private GameObject CreateNewObject(Pool pool)
    {
        var newObj = Instantiate(pool.prefab);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(pool.poolTr);
        return newObj;
    }
    //풀타입에 따른 풀을 찾아옵니다
    private Pool FindPoolByPoolType(PoolName poolType)
    {
        for (int i = 0; i < pools.Length; i++)
        {
            if (pools[i].poolName == poolType)
            {
                return pools[i];
            }
        }
        return null;
    }
    #endregion

    /// <summary>
    /// 오브젝트를 풀로 반환합니다
    /// </summary>
    /// <param name="poolName">반환시키고 싶은 풀의 종류를 선택</param>
    /// <returns></returns>
    public GameObject GetObject(PoolName poolName)
    {
        Pool currentPool = FindPoolByPoolType(poolName);
        if (currentPool == null) Debug.LogError($"<color=red>[{poolName}]</color>의 풀을 찾지 못했습니다. 이름을 확인해주세요  </color>");
       
        if (currentPool.poolingObjectQueue.Count > 0)
        {
            var obj = currentPool.poolingObjectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = CreateNewObject(currentPool);
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }


    /// <summary>
    /// 오브젝트를 풀로 반환합니다
    /// </summary>
    /// <param name="poolName">반환시키고 싶은 풀</param>
    /// <param name="obj">반환할 오브젝트</param>
    public void ReturnObject(GameObject obj, PoolName poolName)
    {
        Pool currentPool = FindPoolByPoolType(poolName);
        if (currentPool == null) Debug.LogError($"<color=red>[{poolName}]</color>의 풀을 찾지 못했습니다. 이름을 확인해주세요.  </color>");
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(currentPool.poolTr.transform);
        currentPool.poolingObjectQueue.Enqueue(obj);
    }
    //딜레이 리턴용 메소드 오버라이딩
    public void ReturnObject(GameObject obj, float delay, PoolName poolType)
    {
        StartCoroutine(Co_ReturnObject(obj, delay, poolType));
    }
    private IEnumerator Co_ReturnObject(GameObject obj, float delay, PoolName poolType)
    {
        yield return new WaitForSeconds(delay);
        ReturnObject(obj, poolType);
    }


    [System.Serializable]
    public class Pool
    {
        public PoolName poolName;
        public int quantity;
        public GameObject prefab;

        [HideInInspector]
        public Transform poolTr;
        public Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();
    }
}




