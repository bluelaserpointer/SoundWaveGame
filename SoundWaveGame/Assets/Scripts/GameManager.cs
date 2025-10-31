using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //inspector
    [SerializeField]
    ItemManager _itemManager;
    //--

    public readonly Dictionary<string, List<Item>> stageItems = new();
    public readonly Dictionary<string, int> collectedItemCounts = new();

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        _itemManager.Init();
    }
    private void Start()
    {
        Player.Instance.Controllable = true;
    }
    public void GameOver()
    {
        GameOverUI.Instance.Show(true);
        Player.Instance.Controllable = false;
    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
