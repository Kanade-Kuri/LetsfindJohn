using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class SceneController : MonoBehaviour
{
    #region variables
    private enum SceneType
    {
        Title = 0, // 0
        SetAttribute = 1,
        GameScreen = 2,
        FindingPlayer = 3
    }

    private SceneType scenetype; // どのSceneかを格納
    #endregion

    #region Method
    // Titleへ遷移
    public void GoToTitle()
    {
        scenetype = (SceneType)0;
        SceneManager.LoadScene(scenetype.ToString());
    }

    public void GoToSetAttribute()
    {
        scenetype = (SceneType)1;
        SceneManager.LoadScene(scenetype.ToString());
    }

    // GameSceneへ遷移
    public void GoToGameScene()
    {
        scenetype = (SceneType)2;
        SceneManager.LoadScene(scenetype.ToString());
    }

    // GameSceneへ遷移
    public void GoToFindingPlayer()
    {
        scenetype = (SceneType)3;
        SceneManager.LoadScene(scenetype.ToString());
    }

    #endregion
}
