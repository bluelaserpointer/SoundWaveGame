using UnityEngine;

public class KeySwitchVisibility : MonoBehaviour
{
    public KeyCode key;
    public GameObject target;

    void Update()
    {
        if(Input.GetKeyDown(key))
        {
            target.SetActive(!target.activeSelf);
        }
    }
}
