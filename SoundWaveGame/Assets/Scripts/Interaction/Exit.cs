using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Exit : MonoBehaviour
{
    [SerializeField]
    Transform _randomExitPositionCandidatesRoot;
    [SerializeField]
    PlayerEnterDetector area;

    private void Awake()
    {
        area.OnPlayerEnter.AddListener(OnPlayerEnter);
        gameObject.SetActive(false);
    }
    public void AppearInRandomPosition()
    {
        gameObject.SetActive(true);
        RandomizePosition();
    }
    public void RandomizePosition()
    {
        Transform randomAnchor = _randomExitPositionCandidatesRoot.GetChild(Random.Range(0, _randomExitPositionCandidatesRoot.childCount - 1));
        transform.SetPositionAndRotation(randomAnchor.position, randomAnchor.rotation);
    }
    public 
    void OnPlayerEnter()
    {
        GameManager.Instance.GameClear();
    }
}
