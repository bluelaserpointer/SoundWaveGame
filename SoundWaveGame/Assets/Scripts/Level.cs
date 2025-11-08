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

    public static Level Instance { get; private set; }

    public Transform PlayerStartAnchor => _playerStartAnchor;
    public GameProgress MainGameProgress => _mainGameProgress;

    private void Awake()
    {
        Instance = this;
        _itemManager.Init();
        Player player = Instantiate(_playerPrefab);
        player.transform.SetPositionAndRotation(_playerStartAnchor.position, _playerStartAnchor.rotation);
        player.Controllable = true;
    }
}
