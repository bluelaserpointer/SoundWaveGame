using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField]
    Transform _randomPositionCandidateRoot;
    [SerializeField]
    PlayerEnterDetector area;

    private void Awake()
    {
        area.OnPlayerEnter.AddListener(OnPlayerEnter);
    }
    public void Show()
    {
        gameObject.SetActive(true);
        RandomizePosition();
    }
    public void RandomizePosition()
    {
        Transform randomAnchor = _randomPositionCandidateRoot.GetChild(Random.Range(0, _randomPositionCandidateRoot.childCount - 1));
        transform.SetPositionAndRotation(randomAnchor.position, randomAnchor.rotation);
    }
    public 
    void OnPlayerEnter()
    {
        GameManager.Instance.GameClear();
    }
}
