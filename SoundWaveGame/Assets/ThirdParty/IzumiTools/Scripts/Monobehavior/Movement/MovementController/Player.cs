using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    //inspector
    [SerializeField]
    int _hp;
    [SerializeField]
    Camera _playerCamera;
    [SerializeField]
    vThirdPersonCamera _cameraSystem;
    [SerializeField]
    vThirdPersonInput _movementInputSystem;
    [SerializeField]
    KnifeThrow _knifeThrow;
    public bool controllable;
    //--
    public Camera Camera => _playerCamera;
    public int KnifeCount => _knifeThrow.KnifeCount;
    public bool IsDead { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (controllable)
        {
            CheckRaycast();
        }
        CheckDeath();
    }
    void CheckRaycast()
    {
        RaycastHit raycastResult;
        Physics.Raycast(_playerCamera.ScreenPointToRay(Input.mousePosition), out raycastResult);
        //print(raycastResult.collider.gameObject.name);
        //看到就直接互动
        if (raycastResult.collider != null)
        {
            GameObject hitGO = raycastResult.collider.attachedRigidbody?.gameObject;
            if (hitGO != null)
            {
                hitGO.GetComponent<IInteractable>()?.Interact();
            }
        }
    }
    void CheckDeath()
    {
        if (IsDead)
        {
            return;
        }
        if (_hp > 0)
        {
            return;
        }
        Dead();
    }
    public void Dead()
    {
        GameManager.Instance.GameOver();
        IsDead = true;
        _cameraSystem.enabled = false;
        _movementInputSystem.enabled = false;
        _knifeThrow.enabled = false;
    }
    public void AddItem(Item item)
    {
        if(item.GetType().Equals(typeof(Knife)))
        {
            ++_knifeThrow.KnifeCount;
            Destroy(item.gameObject);
        }
    }
}
