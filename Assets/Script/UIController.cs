using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private GameObject itemUIJohn; //⭐️
    [SerializeField]
    private GameObject itemUIMirror; //⭐️
    [SerializeField]
    private GameObject itemUIRelocate; //⭐️


    //UIオブジェクト
    //Title scene
    public GameObject gameStartUI; //⭐️GameStart時のUI

    // Are You ready画面
    public GameObject dragAndDropItemsUI; //⭐️
    public GameObject areYouReadyUI; //⭐️
    public GameObject itemUI; //⭐️

    //GameSceneの scene
    public GameObject yourTurnUI; // ⭐️自分ターンの時のUI
    public GameObject opponentTurnUI; //⭐️敵のターンの時のUI

    //FindingPlayer scene
    public GameObject findingPlayerUI; // ⭐️Playerを探している時のUI

    public GameObject youWinUI; //⭐️勝った時のUI
    public GameObject youLostUI; //⭐️負けた時のUI

    public GameManager gameManager; //⭐️ GameManagerのオブジェクト




    #endregion

    #region Method
    private void Update()
    {
        if (gameManager.setAttributePhase)
        {
            if (gameManager.setAttribute)
            {
                dragAndDropItemsUI.SetActive(false);
                areYouReadyUI.SetActive(true);
            }
            else if (gameManager.setAttribute == false)
            {
                areYouReadyUI.SetActive(false);
                dragAndDropItemsUI.SetActive(true);
            }
        } 
    }

    public void CloseAreYouReady()
    {Debug.Log("UIController.CloseAreYouReady()");
        areYouReadyUI.SetActive(false);
        itemUI.SetActive(false);
    }

    public void YourTurnUIShow()
    {
        opponentTurnUI.SetActive(false);
        yourTurnUI.SetActive(true);
    }

    public void WaitingAttackUIShow()
    {
        yourTurnUI.SetActive(false); // 自分ターン非表示
    }

    public void YouWinUI()
    {
        Debug.Log("YouWinUI");

        opponentTurnUI.SetActive(false); //相手ターン非表示
        youWinUI.SetActive(true);
    }

    public void YouLostUI()
    {
        Debug.Log("YouLostUI");

        opponentTurnUI.SetActive(false); //相手ターン非表示
        youLostUI.SetActive(true);
    }

    public void DestroyItemUIAttribute(int bushAttribute)
    {
        switch (bushAttribute)
        {
            case 1:
                Destroy(itemUIJohn);
                break;

            case 2:
                Destroy(itemUIMirror);
                break;

            case 3:
                Destroy(itemUIRelocate);
                break;
        }
    }

    public void RelocateUI()
    {
        opponentTurnUI.SetActive(false);
        dragAndDropItemsUI.SetActive(true);
        itemUI.SetActive(true);
    }

    #endregion
}
