using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(StaticResources))]
public class StaticResources : ScriptableObject
{
    [Header("UI SFX")]
    [SerializeField]
    AudioClip _buttonClickSound;
    [SerializeField]
    AudioClip _panelSwitchSound;
    [SerializeField]
    AudioClip _notificationSound;

    public AudioClip ButtonClickSound => _buttonClickSound;
    public AudioClip PanelSwitchSound => _panelSwitchSound;
    public AudioClip NotificationSound => _notificationSound;
    public static StaticResources Instance {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<StaticResources>(nameof(StaticResources));
            }
            return _instance;
        }
    }
    public static StaticResources _instance;
}
