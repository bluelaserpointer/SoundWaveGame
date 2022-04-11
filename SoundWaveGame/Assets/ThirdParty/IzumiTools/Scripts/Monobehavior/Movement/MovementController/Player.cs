using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    //inspector
    [SerializeField]
    new Camera camera;
    [SerializeField]
    RBBasedHumanMovementController movementController;
    [SerializeField]
    Knife knifePrefab;
    [SerializeField]
    float knifeThrowingForce = 100;
    [SerializeField]
    Cooldown knifeThrowCD = new Cooldown(1);
    [SerializeField]
    AudioClip knifeThrowSE;
    //--
    public RBBasedHumanMovementController MovementController => movementController;
    int knifeAmount;
    public int KnifeAmount { get => knifeAmount;
        set
        {
            GameManager.KnifeAmountText.text = "Knife: " + (knifeAmount = value);
        }
    }
    private void Start()
    {
        KnifeAmount = 0;
    }
    private void Update()
    {
        RaycastHit raycastResult;
        Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out raycastResult);
        //print(raycastResult.collider.gameObject.name);
        if(raycastResult.collider != null)
        {
            GameObject hitGO = raycastResult.collider.attachedRigidbody?.gameObject;
            if (hitGO != null)
                hitGO.GetComponent<IInteractable>()?.Interact();
        }

        knifeThrowCD.Charge();
        if (Input.GetMouseButton(0))
        {
            if(KnifeAmount > 0 && knifeThrowCD.CheckReady())
            {
                --KnifeAmount;
                Knife knife = Instantiate(knifePrefab);
                knife.transform.SetPositionAndRotation(camera.transform.position + camera.transform.forward * 0.5f, camera.transform.rotation);
                knife.Rigidbody.AddForce(knifeThrowingForce * camera.transform.forward);
                AudioSource.PlayClipAtPoint(knifeThrowSE, camera.transform.position);
            }
        }
    }
    public void AddItem(Item item)
    {
        if(item.GetType().Equals(typeof(Knife)))
        {
            ++KnifeAmount;
            Destroy(item.gameObject);
        }
    }
}
