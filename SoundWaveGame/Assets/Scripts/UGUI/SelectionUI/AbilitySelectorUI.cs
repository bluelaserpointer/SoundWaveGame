using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class AbilitySelectorUI : MonoBehaviour
{
    public static AbilitySelectorUI Instance {  get; private set; }
    [SerializeField]
    GameObject _graphicRoot;
    [SerializeField]
    AbilitySlotUI _slotUITemplate;
    [SerializeField]
    Transform _slotParent;

    public readonly UnityEvent<int> onSelected = new();
    public bool IsVisible => _graphicRoot.activeSelf;

    private void Awake()
    {
        Instance = this;
        _graphicRoot.SetActive(false);
        _slotUITemplate.gameObject.SetActive(false);
    }
    public void Show(bool cond)
    {
        _graphicRoot.SetActive(cond);
    }
    public void Select(AbilitySlotUI slot)
    {
        foreach (Transform eachChild in _slotParent)
        {
            if (eachChild.TryGetComponent(out AbilitySlotUI eachSlot))
            {
                eachSlot.IsSelected = eachSlot.Equals(slot);
            }
        }
    }
    public void AddSlot(Ability ability, Action selectedAction)
    {
        AbilitySlotUI slot = Instantiate(_slotUITemplate, _slotParent);
        slot.gameObject.SetActive(true);
        slot.Init(ability);
        slot.OnSelected.AddListener(selectedAction.Invoke);
    }
}
