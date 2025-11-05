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
    float _hp;
    [SerializeField]
    Ability _ability;
    [SerializeField]
    List<Ability> _abilities;
    [SerializeField]
    Camera _playerCamera;
    [SerializeField]
    vThirdPersonCamera _cameraSystem;
    [SerializeField]
    vThirdPersonInput _movementInputSystem;
    [SerializeField]
    KnifeThrow _knifeThrow;

    public Ability Ability
    {
        get => _ability;
        set => _ability = value;
    }
    public List<Ability> Abilities => _abilities;
    public bool Controllable {
        get => _controllable;
        set
        {
            _controllable = value;
            if (_controllable)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _cameraSystem.enabled = true;
                _movementInputSystem.enabled = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _cameraSystem.enabled = false;
                _movementInputSystem.enabled = false;
            }
        }
    }
    public bool _controllable;
    //--
    public Camera Camera => _playerCamera;
    public Ability CurrentAbility => _ability;
    public bool IsDead { get; private set; }
    private void Awake()
    {
        Instance = this;
        GameManager.Instance.onKnifeAdd.AddListener(addition => _knifeThrow.KnifeCount += addition);
    }
    private void Start()
    {
        foreach (Ability ability in Abilities)
        {
            AbilitySelectorUI.Instance.AddSlot(ability, () => Ability = ability);
        }
    }
    private void Update()
    {
        if (Controllable)
        {
            CheckAbilitySwitch();
            CheckAbilityAcivation();
            CheckRaycast();
        }
        CheckDeath();
    }
    void CheckAbilitySwitch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            AbilitySelectorUI.Instance.Show(!AbilitySelectorUI.Instance.IsVisible);
        }
    }
    void CheckAbilityAcivation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentAbility.TryActivateAbility();
        }
    }
    void CheckRaycast()
    {
        RaycastHit raycastResult;
        Physics.Raycast(_playerCamera.ScreenPointToRay(Input.mousePosition), out raycastResult);
        //print(raycastResult.collider.gameObject.name);
        //看到就直接互动
        /*
        if (raycastResult.collider != null)
        {
            GameObject hitGO = raycastResult.collider.attachedRigidbody?.gameObject;
            if (hitGO != null)
            {
                hitGO.GetComponent<IInteractable>()?.Interact();
            }
        }
        */
    }
    public void Damage(float damage)
    {
        _hp -= damage;
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
        Controllable = false;
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
