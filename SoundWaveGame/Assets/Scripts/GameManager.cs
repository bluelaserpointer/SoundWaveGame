using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //inspector
    //--
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public void GameOver()
    {
        GameOverUI.Instance.Show(true);
    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
