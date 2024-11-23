using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.Asteroids;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems; //1
using static Item;

public class Bush : MonoBehaviour, IPointerClickHandler //2
{
    [SerializeField] GameObject explosionPrefab;

    #region variables
    public enum BushPosition
    {
        First = 0,
        Second = 1,
        Third = 2,
        Forth = 3,
        Fifth = 4
    }

    public enum BushAttribute
    {
        Null = 0,
        John = 1,
        Mirror = 2,
        Relocate = 3
    }

    public BushAttribute bushAttribute;　// 属性値

    [SerializeField]
    private string bushOwner; //⭐️

    public BushPosition bushPosition; // ⭐️芝生の位置

    public GameObject bushOnTheOppositeSite; //⭐️反対側にあるBush
    private bool reflectFlg = true; 

    // grassのタイプ 1=ジョン, 2=ミラー, 3=リロケイト, 
    //public int bushAttribute;

    //bool alreadySet = false;

    public Material originalMaterial;
    public GameObject playerCannon; //⭐️

    public bool deleteFlg = false; // そのオブジェクトが消されたかどうか
    private bool isJohnThere = false; //Johnがいるかどうか
    public GameManager gameManager; //⭐️GameManager参照用

    public GameObject UIController; //⭐️

    public GameObject[] playerBushes; // 配列にPlayerのBushScriptを格納 

    public bool playerRelocateFlg = false; // PlayerがRelocateできるかどうか？
    public bool OpponentRelocateFLg = false;　// OpponentがRelocateできるかどうか？

    #endregion

    #region method
    void Start()
    {
        // 元のMaterialを格納。
        originalMaterial = this.gameObject.GetComponent<Renderer>().sharedMaterial;
        bushAttribute = BushAttribute.Null;
    }

    // Playerクリック用
    public void OnPointerClick(PointerEventData eventData)
    {
        /*
        // Player Grassで最初に選択する用
        if (this.gameObject.CompareTag("Player") && !gameManager.setAttribute)
        {
            //AddJohn();

            gameManager.setAttribute = true;
            gameManager.pleaseSelectBushUI.SetActive(false);

            // CPU戦
            if (!GameDataManager.Instance.isOnlineBattle)
            {
                gameManager.DecideWhichFirst();
            }
            // オンライン戦
            else
            {
                // ゲームオブジェクトの情報を相手に与える。
                gameManager.SetJohn(gameObject.name);
                gameManager.WaitingOponent();
            }
        }
        */

        if (gameManager.targetBush)
        {
            gameManager.selectingPhase = false;
            gameManager.InitiateTargetBushAndChangeFlg();


            UIController.GetComponent<UIController>().WaitingAttackUIShow();

            Cannon cannonScript = playerCannon.GetComponent<Cannon>();

            // 大砲を動かす。
            playerCannon.GetComponent<Cannon>().MoveToTarget(transform.position.x);
        }

        /*
        if (this.gameObject.CompareTag("Opponent") && gameManager.yourTurnPhase)
        {
            gameManager.yourTurnPhase = false;
            gameManager.selectOpponentBushUI.SetActive(false);

            //cannon = GameObject.FindGameObjectWithTag("Player");
            //Cannon cannonScript = cannon.GetComponent<Cannon>();

            // 大砲を動かす。
            cannon.GetComponent<Cannon>().MoveToTarget(transform.position.x, bulletZ, cannon.GetComponent<Cannon>().speed);

            if (GameDataManager.Instance.isOnlineBattle)
            {
                //クリックしたオブジェクトの座標を相手に送る。
                gameManager.Attack(gameObject.name);
            }
        }
        */
    }

    /*
    // 属性値を追加する。
    // ドロップされたら呼ばれる関数
    public void ChangeAttribute(int bushAttribute)
    {
        if (alreadySet == false)
        {
            //beforeGrassAttribute = grassType;
        }
        grassAttribute = grassType; //その属性値に変える。
        //gameManager.ManageAttribute(beforeGrassAttribute, grassAttribute, this.gameObject);
        alreadySet = true;
    }

    // オブジェクトの色を元に戻す。 + 属性を外す。
    public void RevertMaterial()
    {

        Debug.Log(originalMaterial.color);
        this.gameObject.GetComponent<Renderer>().material = originalMaterial;
        this.grassAttribute = 0;
        //this.beforeGrassAttribute = 0;
        alreadySet = false;

       
    }
    */

    // このオブジェクトが何かに当たったら消す。
    private void OnCollisionEnter(Collision collision)
    {Debug.Log("Bush.OnCollisionEnter(" + collision + ")");

        
        if(bushAttribute != BushAttribute.Mirror)
        {
            Destroy(collision.gameObject); // 球を破壊する。
            Instantiate(explosionPrefab, transform.position, Quaternion.identity); // 爆破モーション
        }

        deleteFlg = true; //deleteFlgを立てる。

        // 破壊されたBushと反対側のreflectFlgを立てる。(すでに破壊されている場合は処理しない)
        if (bushOnTheOppositeSite != null)
        {
            bushOnTheOppositeSite.GetComponent<Bush>().reflectFlg = false;
        }

        Destroy(this.gameObject); // オブジェクトを消す。

       
            gameManager.ManagingPlayerAttributeSetting((int)bushPosition, (int)bushAttribute, bushOwner); //bushの属性をint型で渡す。
            //gameManager.ManagingPlayerAttributeSetting(this.bushAttribute.ToString(), bushOwner); // Playerオブジェクトで設定を変更する。
        

        switch (bushAttribute)
        {
            case BushAttribute.John:
                // 勝利者に勝利者UIを表示
                // 敗者に敗者UIを表示
                if (gameObject.CompareTag("OpponentBush"))
                {         
                    // 勝利者画面を表示
                    UIController.GetComponent<UIController>().YouWinUI();
                    gameManager.mirrorPhase = false;
                }
                else
                {
                    // 敗者画面を表示
                    UIController.GetComponent<UIController>().YouLostUI();
                    gameManager.mirrorPhase = false;
                }

                break;

            case BushAttribute.Mirror:
                // 弾丸を打った方に跳ね返す。

                // もしもミラー ⇨ ミラーだったら爆破だけにする。
                // ターンを相手にする。
                Debug.Log(reflectFlg);
                if (this.reflectFlg)
                {
                    Debug.Log("if(this.reflectFlg");
                    gameManager.mirrorPhase = true;

                    Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                    rb.AddForce(-transform.forward * 25.0f, ForceMode.Impulse);

                    //  Destroy(this.gameObject); // オブジェクトを消す。
                } else {
                    Destroy(collision.gameObject); // 球を破壊する。
                    if (gameManager.yourTurn)
                    {
                        gameManager.OpponentTurn();
                    }
                    else
                    {
                        gameManager.YourTurn();
                    }
                }

                /*
                if(gameManager.mirrorFlg != true) {
                    gameManager.mirrorFlg = true;

                    Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                    rb.AddForce(-transform.forward * 25.0f, ForceMode.Impulse);

                  //  Destroy(this.gameObject); // オブジェクトを消す。
                }
                */

                /*
                else
                {
                    gameManager.mirrorFlg = false;

                    Instantiate(explosionPrefab, transform.position, Quaternion.identity); // 爆破モーション
                    Destroy(collision.gameObject); // 球を破壊する。

                    if (gameManager.yourTurn)
                    {
                        gameManager.OpponentTurn();
                    }
                    else
                    {
                        gameManager.YourTurn();
                    }
                }
                */

                break;

            case BushAttribute.Relocate:
                // 当たった方のプレイヤーがbushを入れ替えることができる。
                // Relocateできるかの処理は前で処理をする。
                if (gameManager.yourTurn)
                {
                    gameManager.OpponentTurn();
                }
                else
                {
                    gameManager.YourTurn();
                }

                gameManager.mirrorPhase = false;

                break;

            case BushAttribute.Null:
                // ターンを相手にする。
                if(gameManager.yourTurn)
                {
                    gameManager.OpponentTurn();
                } else
                {
                    gameManager.YourTurn();
                }

                gameManager.mirrorPhase = false;
                break;
        }

        /*
        if (this.isJohnThere && this.gameObject.CompareTag("Opponent"))
        {
            gameManager.Win();
        } //Johnだったら
        else if (this.gameObject.CompareTag("Opponent"))
        {
            gameManager.OpponentTurn();
        }
        else if (this.isJohnThere && this.gameObject.CompareTag("Player"))
        {
            gameManager.Lose();
        }
        else if (this.gameObject.CompareTag("Player"))
        {
            gameManager.YourTurn();
        }
        */
    }

    // bushを破壊する。
    public void DestroyBush()
    {
        Destroy(this.gameObject);
    }
    #endregion

    /*
    public void OnPointerClick(PointerEventData eventData) //3
    {
        //Debug.Log("クリック");
        //Debug.Log(content.ToString());
        if(isJohnThere)
        {
            Debug.Log("You found John!!");
            Destroy(this.gameObject);
        }
    }
    */



    
}

// EventSystemというObjectを入れる。//4
// カメラのコンポーネントを追加する。//5
