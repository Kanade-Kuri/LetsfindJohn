using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using static Bush;

public class Player : MonoBehaviour
{
    public string scriptOwner { get; } //⭐️

    // 現在残っている特殊属性
    public bool johnFlg { get; private set; } = true;
    public bool mirrorFlg { get; private set; } = true;
    public bool relocateFlg { get; private set; } = true;

    // 現在残っている自分の茂みの数
    public int bushCount { get; private set; } = 5;

    public bool relocatePrivilege { get; set; } = false; // Relocateできるかどうか。

    public void ManagingAttributeFlg(int bushAttribute)
    {
        switch (bushAttribute)
        {
            case 0:
                break;

            case 1:
                johnFlg = false;
                break;

            case 2:
                mirrorFlg = false;
                break;

            case 3:
                relocateFlg = false;
                relocatePrivilege = true; //Relocateの権利を付与
                break;
        }

        bushCount -= 1;
    }
}
