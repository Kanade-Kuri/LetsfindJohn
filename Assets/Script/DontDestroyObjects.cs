using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObjects : MonoBehaviour
{

    #region Method
    // シーンを跨いでも破壊されない。
    private void Awake()
    {
        // 他のインスタンスが存在する場合は破棄する
        if (FindObjectsOfType<DontDestroyObjects>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion
}
