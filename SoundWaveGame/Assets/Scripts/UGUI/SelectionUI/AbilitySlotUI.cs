using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class AbilitySlotUI : MonoBehaviour
{
    public AbilitySelectorUI selector;
    [SerializeField]
    TextMeshProUGUI _itemNameText;
    [SerializeField]
    GameObject _litBackground;
    [SerializeField]
    GameObject _unlitBackground;

    [SerializeField]
    UnityEvent _onSelected = new();
    [SerializeField]
    UnityEvent _onDeselected = new();
    public Ability Ability { get; private set; }
    public UnityEvent OnSelected => _onSelected;
    public UnityEvent OnDeselected => _onDeselected;
    public TextMeshProUGUI ItemNaemText => _itemNameText;
    bool _isSelected = false;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                if (!_isSelected)
                {
                    Debug.Log("Slot select");
                    (value ? _onSelected : _onDeselected).Invoke();
                }
                _isSelected = value;
            }
            UpdateIfSelectedLit();
        }
    }
    public void Init(Ability ability)
    {
        Ability = ability;
        _itemNameText.text = ability.Name;
    }

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            Debug.Log("button click");
            selector.Select(this);
        });
        IsSelected = false;
    }
    public void UpdateIfSelectedLit()
    {
        if (_litBackground != null)
            _litBackground.SetActive(IsSelected);
        if (_unlitBackground != null)
            _unlitBackground.SetActive(!IsSelected);
    }
}
