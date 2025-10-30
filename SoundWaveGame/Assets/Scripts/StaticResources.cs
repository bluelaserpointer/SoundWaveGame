using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(StaticResources))]
public class StaticResources : ScriptableObject
{

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
