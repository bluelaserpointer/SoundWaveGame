using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [SerializeField]
    float _hp;
    [SerializeField]
    Ability _ability;
    [SerializeField]
    List<Ability> _abilities;
    [SerializeField]
    vThirdPersonCamera _cameraSystem;
    [SerializeField]
    vThirdPersonInput _movementInputSystem;
    [SerializeField]
    KnifeThrow _knifeThrow;
    [SerializeField]
    SoundSourceGenerator _activelySoundSourceGenerator;

    public static Player Instance { get; private set; }
    public Camera Camera => _cameraSystem.Camera;
    public Ability Ability
    {
        get => _ability;
        set
        {
            _ability = value;
            PlayerStatsUI.Instance.UpdateAbilityUsageText();
        }
    }
    public List<Ability> Abilities => _abilities;
    public bool Controllable {
        get => _controllable;
        private set
        {
            _controllable = value;
            if (_controllable)
            {
                _cameraSystem.enabled = true;
                _movementInputSystem.enabled = true;
            }
            else
            {
                _cameraSystem.enabled = false;
                _movementInputSystem.enabled = false;
            }
        }
    }
    public bool _controllable;
    public Ability CurrentAbility => _ability;
    public bool IsDead { get; private set; }
    private void Awake()
    {
        Instance = this;
        _cameraSystem.transform.SetParent(null);
    }
    private void Start()
    {
        CursorModeService.Instance.onCursorShown.AddListener(cond => _movementInputSystem.readCameraRotationInput = !cond);
        GameManager.Instance.onGameEnd.AddListener(UpdateControllableState);
        PauseManager.Instance.onGamePaused.AddListener(_ => UpdateControllableState());
        AbilitySelectorUI.Instance.Clear();
        foreach (Ability ability in Abilities)
        {
            AbilitySelectorUI.Instance.AddSlot(ability, () => Ability = ability);
        }
        UpdateControllableState();
    }
    private void Update()
    {
        if (Controllable)
        {
            CheckAbilitySwitch();
            CheckAbilityAcivation();
            CheckRaycast();
            CheckActiveSoundGeneration();
        }
        CheckDeath();
    }
    void CheckActiveSoundGeneration()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _activelySoundSourceGenerator.GenerateSound();
        }
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
        if (CursorModeService.RequestCursor)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if (Ability.GetType() == typeof(KnifeThrow))
            {
                Ray cameraRay = Camera.ScreenPointToRay(Input.mousePosition);
                float minDistance = 100;
                Vector3 hitPosition = Camera.transform.position + Camera.transform.forward * 100;
                foreach (var hit in Physics.RaycastAll(cameraRay, minDistance))
                {
                    if (hit.collider.isTrigger || hit.distance > minDistance)
                        continue;
                    GameObject hitGo = hit.collider.gameObject;
                    if (hitGo.transform == transform || hitGo.transform.IsChildOf(transform))
                        continue;
                    minDistance = hit.distance;
                    hitPosition = hit.point;
                }
                ((KnifeThrow)Ability).targetPosition = hitPosition;
            }
            CurrentAbility.TryActivateAbility();
        }
    }
    void CheckRaycast()
    {
        RaycastHit raycastResult;
        Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out raycastResult);
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
    public void UpdateControllableState()
    {
        Controllable =
            !GameManager.Instance.IsGameEnd &&
            !PauseManager.Instance.IsPaused;
    }
    public void RestoreAbilityUsage(Ability.AbilityType abilityType, int count)
    {
        //TODO: 规范化每个技能的使用次数恢复
        //_abilities.Find(ability => ability.Type == abilityType);
        _knifeThrow.KnifeCount += count;
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
