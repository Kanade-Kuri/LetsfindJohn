using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//PointerEventDataはドラッグした時の情報を保持している。

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Variables
    public enum ItemAttribute
    {
        Null = 0,
        John = 1,
        Mirror = 2,
        Relocate = 3
    }

    public ItemAttribute itemAttribute; //⭐️ grassのタイプ 0=null, 1=ジョン, 2=ミラー, 3=リロケイト

    RectTransform rectTransform; // UI要素のサイズを変更するためのコンポーネント
    //private Vector2 prePosition; // 初期位置

    private GameObject targetBush; // 重なった芝生
    private Color targetObjectOriginalColor; //重なったオブジェクトの原色
    private bool changeFlg = false; // 色が変わったかどうかのフラグ

    private GameObject previousTargetBush;

    public GameObject dragPrefab; //⭐️ drag用Itemインスタンス化用の変数
    public GameObject dragItem; // dragするItem
    public Canvas targetCanvas; //Canvas情報

    private GrassManager grassmanager;
    public GameManager gameManager;

    private bool alreadyAssained; //属性配置時にすでにアサインされているかどうか

    #endregion

    #region Method

    //ドラッグ操作が開始された時に呼ばれる。
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Item.OnBeginDrag" + eventData);

        //prePosition = transform.position; //現在位置を格納
        CreateDragObject(); //インスタン生成メソッド呼び出し。

        dragItem.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f); //半透明にする。
        dragItem.GetComponent<Image>().raycastTarget = false; // RaycastTargetとして認識しない。(つまりドラッグなどで反応させない。)

    }

    //ドラッグされている最中
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Item.OnDrag" + eventData);

        dragItem.GetComponent<RectTransform>().Translate(eventData.delta); // ドラッグ位置を検知し、その場所へ移動させる。
                                                                           // eventData.deltaの戻り値はVector2で帰ってくる。その場所に変形させている。

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //現在のマウス位置をRayとして変数に格納。
        RaycastHit hit; // RaycastHit変数を用意。※ raycastがオブジェクトに当たった際の詳細情報を格納する構造体

        //もしもオブジェクトがrayに当たったら対象のオブジェクトの色を変化させる。
        if (Physics.Raycast(ray, out hit))
        {
            // 衝突したオブジェクトがなく、前に色が変化したobjectがあれば元に戻す。
            // 衝突したオブジェクトがあれば、そちらの色を変えて前に色が変化したオブジェクトがあれば元に戻す。
            if (changeFlg == true)
            {
                Debug.Log(hit.collider.gameObject);
                if (hit.collider.gameObject.CompareTag("PlayerBush") == false)
                {
                    targetBush.gameObject.GetComponent<Renderer>().material.color = targetObjectOriginalColor;
                    targetObjectOriginalColor = Color.black;
                    targetBush = null;
                    changeFlg = false;
                }
                else if (hit.collider.gameObject != targetBush && hit.collider.gameObject.CompareTag("PlayerBush"))
                {
                    targetBush.gameObject.GetComponent<Renderer>().material.color = targetObjectOriginalColor;
                    targetBush = hit.collider.gameObject;
                    targetObjectOriginalColor = targetBush.gameObject.GetComponent<Renderer>().material.color;
                    targetBush.GetComponent<Renderer>().material.color = Color.white;
                }
            }

            if (changeFlg == false && hit.collider.gameObject.CompareTag("PlayerBush"))
            {
                targetBush = hit.collider.gameObject;
                targetObjectOriginalColor = targetBush.gameObject.GetComponent<Renderer>().material.color;
                targetBush.GetComponent<Renderer>().material.color = Color.white;
                changeFlg = true;
            }
        }
    }

    // ドロップした時の処理
    public void OnEndDrag(PointerEventData eventData)
    {
        if (targetBush)
        {
            // Debug.Log(this.gameObject.name);
            gameManager.ManageAttribute((int)itemAttribute, targetBush);
            targetBush = null;
            changeFlg = false;
        }
        Destroy(dragItem);
    }

    // インスタンスを生成し、マウスが入力された位置に置く。
    void CreateDragObject()
    {
        dragItem = Instantiate(dragPrefab, targetCanvas.transform);
        dragItem.GetComponent<RectTransform>().position = Input.mousePosition;
    }

    public void savePreviousTargetBush()
    {
        previousTargetBush = targetBush; //前のBushを保持する。
    }
    #endregion

}
