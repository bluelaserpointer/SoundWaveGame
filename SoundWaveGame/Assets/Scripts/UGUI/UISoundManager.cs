using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance;
    
    [Header("UI音效")]
    public AudioClip buttonClickSound;
    public AudioClip panelSwitchSound;
    public AudioClip notificationSound;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlayButtonSound()
    {
        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    public void PlayPanelSwitchSound()
    {
        if (panelSwitchSound != null)
        {
            audioSource.PlayOneShot(panelSwitchSound);
        }
    }
    
    public void PlayNotificationSound()
    {
        if (notificationSound != null)
        {
            audioSource.PlayOneShot(notificationSound);
        }
    }
}