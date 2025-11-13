using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Level : MonoBehaviour
{
    [SerializeField]
    Player _playerPrefab;
    [SerializeField]
    Transform _playerStartAnchor;
    [SerializeField]
    ItemManager _itemManager;
    [SerializeField]
    GameProgress _mainGameProgress;

    public static Level Instance {
        get
        {
            if (_instance == null)
            {
                Debug.LogError($"[{nameof(Level)}] 当前场景缺少了Level，无法启动");
            }
            return _instance;
        }
        private set => _instance = value;
    }
    static Level _instance;

    public Transform PlayerStartAnchor => _playerStartAnchor;
    public GameProgress MainGameProgress => _mainGameProgress;

    private void Awake()
    {
        Instance = this;
        _itemManager.Init();
        Player player = Instantiate(_playerPrefab, _playerStartAnchor.position, _playerStartAnchor.rotation);
    }
}
