using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using static Bush;
using Photon.Pun.Demo.PunBasics;
using System;
//using System;


public class GameManager : MonoBehaviourPunCallbacks
{
    #region variables
    private enum MaterialList
    {
        Null = 0,
        John = 1,
        Mirror = 2,
        Relocate = 3
    }

    // Bushの管理
    private MaterialList materialList; // 属性値のリスト

    public GameObject[] bushAttributeManager = { null, null, null, null }; //属性番号に対してどの茂みが対象か格納する。 (1 = John, 2 = Mirror, 3 = Relocate)

    //Oponent Bush
    public GameObject[] opponentBushes = new GameObject[5]; // ⭐️敵のBushオブジェクトを格納①
    //public List<GameObject> testOpponentBushes = new List<GameObject>(); // ‼️ テスト

    public GameObject[] opponentBushAttributeManager = { null, null, null, null }; //敵の属性番号に対してどの茂みが対象か格納 (1 = John, 2 = Mirror, 3 = Relocate)
    public int[] opponentBushesAttributes = new int[5] { 1, 2, 3, 0, 0 }; // 敵用の属性の位置を格納 1=John, 2=Mirror, 3=Relocate, 9=削除
    //int arrayCount = 0; //ループする際に使用


    //Player Bush
    public GameObject[] playerBushes; // 配列にPlayerのBushScriptを格納 ①

    //Cannon
    public GameObject cannon;

    [SerializeField]
    private GameObject dammyObjectPrefab; //dammyObjectのPrefabを置いておく

    [SerializeField]
    private GameObject you; //⭐️
    [SerializeField]
    private GameObject opponent; //⭐️

    // フェーズ管理
    public bool setAttributePhase = true;　//自分の芝生設置フェーズかどうか
    public bool selectingPhase = false; //相手の芝生を選択可能かどうか
    public bool relocatingPhase = false; // Relocateのフェーズかどうか

    public bool yourTurn = false; //今が自分のターンかどうか
    public bool oponentTurn = false; //今が相手のターンかどうか

    //判定
    public bool setAttribute = false; // 属性値を設定したかどうか
    public bool OponentalreadySelectJohn = false; //相手がJohnを選択したかどうか。

    public GameObject targetBush; // 重なった茂み
    private Color targetObjectOriginalColor; //重なったオブジェクトの原色
    private bool changeFlg = false; // 色が変わったかどうかのフラグ

    public bool SelectRetry = false; // Retryを押したかどうか。
    public bool OponentSelectRetry = false; // 相手がRetryしたかどうか。

    private bool alreadySetTurn = false;

    public bool bushclickMoreThan1 = false; //芝生が1回以上ボタンが押されたかどうか

    public bool mirrorPhase = false; // ミラーの処理中かどうか


    //UI
    public GameObject gameStartUI; //GameStart時のUI
    public GameObject findingPlayerUI; // Onlineを選択した時のUI
    public GameObject pleaseSelectBushUI; //芝生を選ばせるUI
    public GameObject selectOpponentBushUI; //相手の芝生を選ばせるUI
    public GameObject youWinUI; //勝った時のUI
    public GameObject youLostUI; //負けた時のUI
    public GameObject OpponentTurnUI; //敵のターンの時のUI

    public GameObject UIController; //⭐️ UIController
    public GameObject sceneController; //⭐️

    // 乱数
    System.Random rand = new System.Random(); //インスタンス生成

    // 先攻後攻に使う変数
    int randomValue;

    //
    float bulletZ = 14.7f; //Opponent用の球の角度
    #endregion

    #region method

    #region Updateの処理
    void Update()
    {
        // 両方Retryしたらシーンを再読み込み。
        if (SelectRetry && OponentSelectRetry)
        {
            SceneManager.LoadScene("GameScreen");
        }

        if (alreadySetTurn)
        {
            return;
        }
        // 両方ともselectしたら
        if (setAttribute && OponentalreadySelectJohn)
        {
            alreadySetTurn = true;
            DecideWhichFirst();
        }

        #region 相手の芝生を選択する処理
        if (selectingPhase)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //現在のマウス位置をRayとして変数に格納。
            RaycastHit hit; // RaycastHit変数を用意。※ raycastがオブジェクトに当たった際の詳細情報を格納する構造体

            if (Physics.Raycast(ray, out hit))
            {
                // 衝突したオブジェクトがなく、前に色が変化したobjectがあれば元に戻す。
                if (changeFlg == true)
                {
                   
                    if (hit.collider.gameObject.CompareTag("OpponentBush") == false)
                    {
                        targetBush.gameObject.GetComponent<Renderer>().material.color = targetObjectOriginalColor;
                        targetObjectOriginalColor = Color.black;
                        targetBush = null;
                        changeFlg = false;
                    }

                // 衝突したオブジェクトがあれば、そちらの色を変えて前に色が変化したオブジェクトがあれば元に戻す。
                    else if (hit.collider.gameObject != targetBush && hit.collider.gameObject.CompareTag("OpponentBush"))
                    {
                        targetBush.gameObject.GetComponent<Renderer>().material.color = targetObjectOriginalColor;
                        targetBush = hit.collider.gameObject;
                        targetObjectOriginalColor = targetBush.gameObject.GetComponent<Renderer>().material.color;
                        targetBush.GetComponent<Renderer>().material.color = Color.white;
                    }
                }

                if (changeFlg == false && hit.collider.gameObject.CompareTag("OpponentBush"))
                {
                    targetBush = hit.collider.gameObject;
                    targetObjectOriginalColor = targetBush.gameObject.GetComponent<Renderer>().material.color;
                    targetBush.GetComponent<Renderer>().material.color = Color.white;
                    changeFlg = true;
                }
            }

        }
        #endregion

    }
    #endregion

    # region 相手の芝生を選択したらセットしていたtargetBushとchangeFlgを消す。
    public void InitiateTargetBushAndChangeFlg()
    {
        targetBush = null;
        changeFlg = false;
    }
    #endregion

    #region 芝生の属性を管理
    // 属性を管理する
    // 引数(芝生のタイプ, Grassのゲームオブジェクト)
    public void ManageAttribute(int itemAttribute, GameObject targetBush) // itemの属性、対象のゲームオブジェクト
    {
        Debug.Log("GameManager.ManageAttribute" + "(" + itemAttribute + ", " + targetBush + ")");

        Bush targetBushScript = targetBush.GetComponent<Bush>(); // targetBushのScriptを格納

        //ドロップされた芝生の属性が代入したItemと異なっているか(属性が0を除く)
        //yesなら対象Bushの設定値を戻して、対象番号のObjectを消す。
        if ((int)targetBushScript.bushAttribute != 0 && (int)targetBushScript.bushAttribute != itemAttribute) //属性が0ではない,かつ属性が芝生と異なる時。
        {
            int beforeAttribute = (int)targetBushScript.bushAttribute; // 変更する芝生の属性値を格納しとく。

            RevertBushAttribute((int)targetBushScript.bushAttribute, bushAttributeManager[(int)targetBushScript.bushAttribute], targetBushScript);
            bushAttributeManager[beforeAttribute] = null; // ‼️ テスト // bushAttributeManagerの元の属性値の要素番号のゲームオブジェクトを削除
        }

        //ドロップされるItemが他で使われていたか。(前と同じところにドロップしようとした場合を除く)
        //yesなら対象番号の対応表に紐づけられているObjectの属性とMaterialをデフォルトに戻す。
        if (bushAttributeManager[itemAttribute] != null && bushAttributeManager[itemAttribute] != targetBush)
        {
            Bush targetBushScript2 = bushAttributeManager[itemAttribute].GetComponent<Bush>();
            RevertBushAttribute(itemAttribute, bushAttributeManager[itemAttribute], targetBushScript2);
            bushAttributeManager[itemAttribute] = null; // ‼️ テスト
        }

        bushAttributeManager[itemAttribute] = targetBush;

        ChangeBushAttribute(itemAttribute, targetBush, targetBushScript);

        if ((bushAttributeManager[1] != null || (bushAttributeManager[1] != null && bushAttributeManager[1].name == "DammyObject"))
            && (bushAttributeManager[2] != null || (bushAttributeManager[1] != null && bushAttributeManager[1].name == "DammyObject"))
            && (bushAttributeManager[3] != null || (bushAttributeManager[1] != null && bushAttributeManager[1].name == "DammyObject")))
        {
            setAttribute = true;
        }
        else if (setAttribute = true && bushAttributeManager[1] != null && bushAttributeManager[2] != null && bushAttributeManager[3] != null)
        {
            setAttribute = false;
        }
    }

    // Grassの属性を管理。
    // 変更する点↓
    // 1, 対象のbushScriptの属性値を変更。
    // 2, 対象のbushの色を変更する。
    // ドロップされたBush
    private void ChangeBushAttribute(int bushAttribute, GameObject targetBush, Bush bushScript) // bushAttribute
    {
        Debug.Log("GameManager.ChangeBushAttribute(" + bushAttribute + ", " + targetBush + ", " + bushScript);

        materialList = (MaterialList)bushAttribute; // bushAttributeのMaterialを取得。
        Debug.Log("GameManager.ChangeBushAttribute: materialList =" + materialList);

        bushScript.bushAttribute = (Bush.BushAttribute)bushAttribute;　//targetBushの属性値を変更。
        Debug.Log((Bush.BushAttribute)bushAttribute);

        if (setAttributePhase)
        {
            targetBush.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>($"Material/{materialList.ToString()}"); //TargetBushのMaterialを変更
        }

        /*
        if (bushAttribute != 0) // Opponentの属性値を設定する時のためにbushAttributeが0かどうかの判定を入れております。
        {
            targetBush.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>($"Material/{materialList.ToString()}"); //TargetBushのMaterialを変更
        } else
        {
            targetBush.GetComponent<Renderer>().material = bushScript.originalMaterial;
        }
        */
    }

    // Materialと属性を戻す。
    // 変更する点↓
    // 1, 対象のbushScriptの属性値を0に変更。
    // 2, 対象のbushの色を元に戻す。
    private void RevertBushAttribute(int revertBushAttribute, GameObject revertBush, Bush bushScript)
    {
        Debug.Log("RevertBushAttribute(" + revertBushAttribute + ", " + revertBush + ", " + bushScript + ")");

        bushScript.bushAttribute = (Bush.BushAttribute)0;
        revertBush.GetComponent<Renderer>().material = bushScript.originalMaterial;

        //bushAttributeManager[revertBushAttribute] = null; ‼️テストで消してます。
        //    grassTypeManager[originalGrassAttribute].gameObject.GetComponent<Grass>().RevertMaterial(); //元々の芝生の色を戻す。
    }
    #endregion

    #region 対戦すすむ
    public void GoToBattle()
    {
        Debug.Log("GameManager.GoToBattle()");
        if (relocatingPhase != true)
        {
            setAttributePhase = false; // 属性セットフェーズの終了
  
            UIController.GetComponent<UIController>().CloseAreYouReady(); // AreYouReadyのClose

            //CPUの属性をセットする。
            // オンラインじゃなかったら下記を実行
            if (!GameDataManager.Instance.isOnlineBattle)
            {
                Shuffle(opponentBushesAttributes); // 相手の当たり芝生をシャッフル
                SetOpponentAttributes(); //OpponentのAttributeを設定する。
            }

            //順番を決める。
            DecideWhichFirst();
        } else
        {Debug.Log("GoToBattle if(relocatingPhase != true) else");
            setAttributePhase = false;
            relocatingPhase = false;

            yourTurn = true;
            selectingPhase = true;
            UIController.GetComponent<UIController>().CloseAreYouReady();
            UIController.GetComponent<UIController>().YourTurnUIShow();
        }
    }
    #endregion

    #region CPUの属性をランダムで設定
    // 相手のAttributesの値をランダムで設定。
    public int[] Shuffle(int[] array) //　配列を引数として受け取る。
    {
        Debug.Log("GameManager.Shuffle(" + string.Join(", ", array) + ")");

        // arrayの要素数-1ぶん繰り返す。
        for (int i = array.Length - 1; i >= 1; i--) // iの初期値は4; iが1以上だったらやる, iを1ずつ減らす。
        {
            int j = rand.Next(i + 1);　//0~4のどれかを生成
            int temp = array[j]; // arrayのj番目に入れる。+ tempにその値を代入。

            //下記で値を入れ替えている。
            array[j] = array[i]; // 最後尾をarrayのj番目に入れる。
            array[i] = temp; // 最後尾に入れていく。
        }

        Debug.Log("GameManager.Shuffle 戻り値:" + string.Join(", ", array));
        return array;
    }


    // 敵のAttributesを設定する。

    /*
    public void SetOpponentAttributes()
    {
        Debug.Log("GameManager.SetOpponentAttributes()");

        int arrayCount = 0; // arrayCountは0にセット

        // ランダムにセットされた属性を茂みにセットする。
        foreach (GameObject g in opponentBushes.Where(r => r.GetComponent<Bush>().deleteFlg != true)) //opponentBushesの要素を順番に回す。
        {
            Debug.Log("GameManager.SetOpponentAttributes 対象オブジェクト:" + g);
            if (opponentBushesAttributes[arrayCount] != 0 && opponentBushesAttributes[arrayCount] != 9)　//opponentBushAttributeManagerのうち、属性を持った値だけを取り出す。
            {
                if(opponentBushAttributeManager[opponentBushesAttributes[arrayCount]] != null && opponentBushAttributeManager[opponentBushesAttributes[arrayCount]] != g)
                {
                    RevertBushAttribute(opponentBushesAttributes[arrayCount], opponentBushAttributeManager[opponentBushesAttributes[arrayCount]], opponentBushAttributeManager[opponentBushesAttributes[arrayCount]].GetComponent<Bush>());
                    opponentBushAttributeManager[opponentBushesAttributes[arrayCount]] = null; //‼️ テスト
                }
                opponentBushAttributeManager[opponentBushesAttributes[arrayCount]] = g; //opponentBushAttributeManagerの対象の属性番号のオブジェクトを対象のものにする。
                ChangeBushAttribute(opponentBushesAttributes[arrayCount], g, g.GetComponent<Bush>()); // bushAttribute
            }
            arrayCount += 1;
        }

        Debug.Log(opponentBushAttributeManager);

    }
    */

    
    public void SetOpponentAttributes()
    {
        Debug.Log("GameManager.SetOpponentAttributes()");

        int arrayCount = 0;

        for (int i = 0; i <= opponentBushesAttributes.Length - 1; i++)
        {
            Debug.Log("GameManager.SetOpponentAttributes 対象オブジェクト:" + opponentBushes[i]);
            if (opponentBushesAttributes[i] != 9 /*&& opponentBushesAttributes[i] != 0*/)
            {
                /*
                if( opponentBushAttributeManager[opponentBushesAttributes[i]] != null && opponentBushAttributeManager[opponentBushesAttributes[i]] != opponentBushes[i])
                {
                    RevertBushAttribute(opponentBushesAttributes[i], opponentBushAttributeManager[opponentBushesAttributes[i]], opponentBushAttributeManager[opponentBushesAttributes[i]].GetComponent<Bush>());
                    opponentBushAttributeManager[opponentBushesAttributes[i]] = null; //‼️ テスト
                }
                */

                // オブジェクト変更
                opponentBushAttributeManager[opponentBushesAttributes[i]] = opponentBushes[i]; //opponentBushAttributeManagerの対象の属性番号のオブジェクトを対象のものにする。
                ChangeBushAttribute(opponentBushesAttributes[i], opponentBushes[i], opponentBushes[i].GetComponent<Bush>()); // bushAttribute
            }
            arrayCount += 1;
        }
    }
    

    #endregion

    #region 順番を決める
    // 順番を決める。
    public void DecideWhichFirst()
    {
        // 0〜100までの数字
        randomValue = UnityEngine.Random.Range(0, 100);

        // CPU戦
        if (!GameDataManager.Instance.isOnlineBattle)
        {
            if (randomValue >= 50)
            {
                YourTurn();
            }
            else
            {
                OpponentTurn();
            }
        }

        // オンラインだったら↓
        else
        {
            // HostかClientか
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("CompareValue", RpcTarget.Others, randomValue);
            }
        }
    }
    #endregion

    #region 自分のターン
    // 自分のターンになったら呼ばれるメソッド
    [PunRPC]
    public void YourTurn()
    {
        oponentTurn = false;

        // RelocatePrivilegeがtrueで残りの茂みが2以上ならRelocatePhaseに行く。
        // それ以外なら攻撃ターンに移行
        if (you.GetComponent<Player>().relocatePrivilege && you.GetComponent<Player>().bushCount >= 2)
        {
            you.GetComponent<Player>().relocatePrivilege = false;
            relocatingPhase = true;
            setAttributePhase = true;
            UIController.GetComponent<UIController>().RelocateUI();
        } else {
        Debug.Log("YourTurn");
            yourTurn = true;
            selectingPhase = true;
            UIController.GetComponent<UIController>().YourTurnUIShow();
        }
    }
    #endregion

    #region 攻撃する (消していいかも？)

    public void Attack(GameObject targetBush)
    {


        /*
        public void Attack(GameObject BushToBeAttacked)
        {




            if (GameDataManager.Instance.isOnlineBattle)
                {
                    //クリックしたオブジェクトの座標を相手に送る。
                 //   gameManager.Attack(gameObject.name);
                }

        }
        */
    }
    #endregion

    #region 茂みが破壊された時にPlayerの属性管理
    public void ManagingPlayerAttributeSetting(int bushPosition, int bushAttribute, string bushOwner)
    {
        Debug.Log("GameManager.ManagingPlayerAttributeSetting(" + bushPosition + ", " + bushAttribute + ", " + bushOwner + ")");

        if (bushOwner == "You")
        {
            if(bushAttribute != 0) { 
            //引数が0以外の場合、削除された属性の配列にdammyObjectを代わりに入れる。
            GameObject dammyObject = Instantiate(dammyObjectPrefab);
            bushAttributeManager[bushAttribute] = dammyObject;

            UIController.GetComponent<UIController>().DestroyItemUIAttribute(bushAttribute);
            }
            you.GetComponent<Player>().ManagingAttributeFlg(bushAttribute);

        } else
        {
            if (bushAttribute != 0)
            {
                GameObject dammyObject = Instantiate(dammyObjectPrefab);
                opponentBushAttributeManager[bushAttribute] = dammyObject;
            }
            Debug.Log(bushPosition);
            opponentBushesAttributes[bushPosition] = 9; //削除された芝生は9を挿入
                opponent.GetComponent<Player>().ManagingAttributeFlg(bushAttribute);
        }
    }
    #endregion



    // Johnを選択したら敵側に送ったことを報告する。
    public void WaitingOponent()
    {
        photonView.RPC("SetOponentValue", RpcTarget.Others, setAttribute);
    }

    [PunRPC]
    void SetOponentValue(bool value)
    {
        OponentalreadySelectJohn = value;
    }

    public void SetJohn(string name)
    {
        photonView.RPC("SetOpponentJohn", RpcTarget.Others, name);
    }

    [PunRPC]
    void SetOpponentJohn(string name)
    {
        GameObject playerObject = GameObject.FindObjectsOfType<GameObject>()
        .FirstOrDefault(obj => obj.name == name && obj.CompareTag("Opponent"));

        //playerObject.GetComponent<Bush>().AddJohn();
    }

    // Cliet側で呼ばれるメソッド
    [PunRPC]
    void CompareValue(int value)
    {
        randomValue = 100; //2
        if (randomValue >= value)
        {
            // Client側が先攻
            YourTurn();
            photonView.RPC("OpponentTurn", RpcTarget.Others); // Hostが待機

        }
        else
        {
            // Clientが後攻
            photonView.RPC("YourTurn", RpcTarget.Others);
            OpponentTurn(); // Clientは待機
        }
    }




    // 相手のターンになったら呼ばれるメソッド (Onlineじゃない)
    [PunRPC]
    public void OpponentTurn()
    {
        yourTurn = false;
        OpponentTurnUI.SetActive(true);
        oponentTurn = true;

        if (!GameDataManager.Instance.isOnlineBattle)
        {

            //RelocatePhaseだった時の処理
            if (opponent.GetComponent<Player>().relocatePrivilege && you.GetComponent<Player>().bushCount >= 2)
            {
                opponent.GetComponent<Player>().relocatePrivilege = false;
                // 茂みのランダムの処理をする。
                int[] tempAttrPosBushes = opponentBushesAttributes.Where(r => r != 9).ToArray();
                Shuffle(tempAttrPosBushes); // 相手の当たり芝生をシャッフル

                int ti = 0; // tempAttrPosBushesの要素番号格納用変数
                
                for (int ia = 0; ia <= opponentBushesAttributes.Length -1; ia++) 
                {
                    if (opponentBushesAttributes[ia] != 9)
                    {
                        opponentBushesAttributes[ia] = tempAttrPosBushes[ti];
                        ti += 1;
                    }
                }
                Debug.Log("最終結果:" + string.Join(", ", opponentBushesAttributes));
                SetOpponentAttributes();
            }

            //攻撃Phase
            InitializePlayerGrass();
            foreach (GameObject obj in playerBushes)
            {
                Debug.Log(obj.name);
            }
            Debug.Log(playerBushes.Length);

            int i = rand.Next(playerBushes.Length);

             //大砲を動かす。
            cannon = GameObject.FindGameObjectWithTag("Opponent");
            Cannon cannonScript = cannon.GetComponent<Cannon>();
            cannon.GetComponent<Cannon>().MoveToTarget(playerBushes[i].gameObject.transform.position.x);         
        }
    }

    //
    public void Attack(string name)
    {
        photonView.RPC("OpponentAttack", RpcTarget.Others, name);
    }

    [PunRPC]
    void OpponentAttack(string name)
    {
        GameObject playerObject = GameObject.FindObjectsOfType<GameObject>()
        .FirstOrDefault(obj => obj.name == name && obj.CompareTag("PlayerBush"));

//        cannon.GetComponent<Cannon>().MoveToTarget(playerObject.transform.position.x, bulletZ, cannon.GetComponent<Cannon>().speed);
    }




    // CPUBattle or OnlineでMatchingが成功したら
    public void GameStart()
    {
        //SceneManager.LoadScene("GameScreen"); //Game画面へ移行

        /*
        Shuffle(randomJohn); // 相手の当たり芝生をシャッフル
        AddJohn();
        */

        //Start画面を消して、芝生選択画面を表示し、芝生を選べるようにする。
        // gameStartUI.SetActive(false);
        //pleaseSelectBushUI.SetActive(true);

        //selectTime = true;
    }






    //勝ちの画面
    public void Win()
    {
        youWinUI.SetActive(true);
    }

    //負けの画面
    public void Lose()
    {
        OpponentTurnUI.SetActive(false);
        youLostUI.SetActive(true);
    }

    // Retry画面が押されたらSceneを再読み込み
    public void SceneLoad()
    {
        if (!GameDataManager.Instance.isOnlineBattle)
        {
            SceneManager.LoadScene("GameScreen");
        }
        else
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("FindingPlayer");
            //youWinUI.SetActive(false);
            //youLostUI.SetActive(false);
            //OpponentTurnUI.SetActive(true);

            //SelectRetry = true;
            //photonView.RPC("InformRetry", RpcTarget.Others, SelectRetry);
        }

    }

    // CPUボタンが押されたら
    public void CPUBattleStart()
    {
        GameDataManager.Instance.isOnlineBattle = false; // オンラインじゃないですよ。
        SceneManager.LoadScene("GameScreen");
    }

    // Onlineボタンが押されたら
    public void OnMatchingButton()
    {
        GameDataManager.Instance.isOnlineBattle = true; // オンラインですよ。
        SceneManager.LoadScene("FindingPlayer"); //FindingPlayerシーンに移動
    }

    // Menuボタンが押されたら、メニューに戻る。
    public void BackToMenu()
    {
        PhotonNetwork.Disconnect(); //Photonから切断する。
        SceneManager.LoadScene("Title");

        // もしもOnlineだったら、Photonからの接続を切断する
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }


    [PunRPC]
    void InformRetry(bool value)
    {
        OponentSelectRetry = value;
    }

    void InitializePlayerGrass()
    {
        playerBushes = null;

        // 配列にPlayerオブジェクトを格納 ②
        playerBushes = GameObject.FindGameObjectsWithTag("PlayerBush");
        playerBushes = playerBushes.Where(bush => bush.GetComponent<Bush>().deleteFlg == false).ToArray();
        Debug.Log(playerBushes.Length);
        /*
        .Select(go2 => go2.GetComponent<Bush>())　// Grass scriptを選択
.Where(grass2 => grass2 != null) // Grass scriptがついてないものは除外
.ToArray(); //配列に加える。
          */
    }

    #endregion
}
