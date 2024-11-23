using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class OnlineManager : MonoBehaviourPunCallbacks
{
    bool enterRoom = false; //部屋に入ったかどうか
    bool isMatching = false; //マッチングしたかどうか

    //public int randomValue; // ランダムな値を生成する。 ①
    //public bool isMasterClient;



    public GameManager gameManager;

    void Start()
    {
            //randomValue = Random.Range(0, 100); // ランダムな値を生成する。 ② 
            PhotonNetwork.ConnectUsingSettings(); //マスターサーバーへ接続
    }

    // Onlineボタンが押されたら
    //public void OnMatchingButton()
    //{  
        //gameManager.gameStartUI.SetActive(false);
        //gameManager.findingPlayerUI.SetActive(true);

        //PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        
        //SceneManager.LoadScene("FindingPlayer"); // FindingPlayerSceneに移動する。
    //}

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();　//既存のゲームサーバーに入る。
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("ゲームサーバーに入った");
        enterRoom = true;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.NickName = "You";
        } else
        {
            PhotonNetwork.NickName = "Opponent";
        }
    }

    //ゲームサーバーに入れなかったら
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2}, TypedLobby.Default);
    }

    // 部屋が2人ならシーンを変える。
    private void Update()
    {
        if (isMatching)
            {
                return;
            }
            if(enterRoom)
            {
                if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
                {
                PhotonNetwork.CurrentRoom.IsOpen = false; //途中参加を禁止する。
                Debug.Log("2人部屋にいった。");
                    isMatching = true;
                SceneManager.LoadScene("GameScreen");
                }
            }
    }

    /*
    // Client側だったらHost側から値を受け取りどっちが大きいか決める。
    [PunRPC]
    void CompareValue(int value)
    {
        if(randomValue >= value)
        {
            // 先攻
            GameDataManager.Instance.yourTurn = true; // 先攻
            photonView.RPC("YourLater", RpcTarget.Others);
            GoToScene();
        }
        else
        {
            GameDataManager.Instance.yourTurn = false; // 後攻
            photonView.RPC("YourFirst", RpcTarget.Others, randomValue);
            GoToScene();
        }
    }

    // Host側、Client側の結果を受けて？
    // あなたは後攻に決まりました。
    [PunRPC]
    void YourLater()
    {
        GameDataManager.Instance.yourTurn = false;
        GoToScene();
    }

    // あなたは先攻に決まりました。
    [PunRPC]
    void YourFirst()
    {
        GameDataManager.Instance.yourTurn = true;
        GoToScene();
    }

    void GoToScene()
    {
        SceneManager.LoadScene("GameScreen");
    }
    */
}


   /*
        if (isMatching)
        {
            return;
        }
        if (enterRoom)
        {
            isMatching = true;
            // もしもルームが二人で、自身がHostだったら
            if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount && isMasterClient)
            {
                photonView.RPC("CompareValue", RpcTarget.Others, randomValue);
            }
            */