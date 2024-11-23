using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameDataManager : MonoBehaviour
{
    // ONLINE戦かどうか？
    public bool isOnlineBattle { get; set; }

    // Player情報
    public bool yourTurn; // 自分のターンかどうか。

    public static GameDataManager Instance { get; private set; } // getは他から参照できるか、setは他から編集できるか

    // シーンを跨いでも破壊されない。
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでにInstanceが作成されていたら、自信を破壊する。
            Destroy(gameObject);
        }
    }
}
