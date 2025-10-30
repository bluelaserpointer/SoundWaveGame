using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected vNpcAIController aiController;

    void Awake()
    {
        aiController = GetComponentInChildren<vNpcAIController>();
    }
}
