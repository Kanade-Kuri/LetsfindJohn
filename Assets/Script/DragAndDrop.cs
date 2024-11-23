using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Color hoverColor; // ホバー時の色
    private Color originalColor;


    private void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Renderer>().material.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Renderer>().material.color = originalColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("当たった");
        if (eventData.pointerDrag != null)
        {
            Debug.Log("当たった");
            /*
            // ドロップされたオブジェクトを取得
            GameObject droppedObject = eventData.pointerDrag;

            // ドロップされたオブジェクトのRectTransformを取得
            RectTransform droppedRectTransform = droppedObject.GetComponent<RectTransform>();

            // ドロップ先のRectTransformを取得
            RectTransform dropAreaRectTransform = GetComponent<RectTransform>();

            // ドロップされたオブジェクトの位置がドロップ先の範囲内にあるか確認
            if (RectTransformUtility.RectangleContainsScreenPoint(dropAreaRectTransform, eventData.position, eventData.pressEventCamera))
            {
                // ドロップが成功した場合の処理
                droppedRectTransform.anchoredPosition = dropAreaRectTransform.anchoredPosition;
                Debug.Log("ドロップが成功しました！");
            }
            else
            {
                // ドロップが失敗した場合の処理
                Debug.Log("ドロップが範囲外です。");
            }
            */
        }
    }
}
