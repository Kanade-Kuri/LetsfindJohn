using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   
   // [SerializeField] GameObject explosionPrefab;

    public string bulletOwner;
    /*
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); //GameManagerを格納
    }

    private void OnCollisionEnter(Collision collision) {

        if(collision.gameObject.GetComponent<Bush>().bushAttribute != Bush.BushAttribute.Mirror)
        {
            Destroy(this.gameObject); // 球を破壊する。
            Instantiate(explosionPrefab, transform.position, Quaternion.identity); // 爆破モーション
        } else if(collision.gameObject.GetComponent<Bush>().reflectFlg != true)
        {
            Destroy(this.gameObject); // 球を破壊する。
        }
    

    }
    */
}
