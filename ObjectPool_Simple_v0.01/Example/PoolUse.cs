using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolUse : MonoBehaviour
{
    public ObjectPool objectPool;
    public int getQuantity;

    void Start()
    {
        for (int i = 0; i < getQuantity; i++)
        {
           GameObject AObj = objectPool.GetObject(PoolName.AObjectPool);
           objectPool.ReturnObject(AObj, 3, PoolName.AObjectPool);
        }
    }

}
