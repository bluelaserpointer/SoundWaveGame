using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class BeaconSpawner : Ability
{
    [SerializeField]
    Beacon _beaconPrefab;
    [SerializeField]
    Transform _spawnAnchor;
    [SerializeField]
    int _beaconStock;

    public override string Name => "Beacon";

    public override bool IsUssable => _beaconStock > 0;

    public int BeaconStock
    {
        get => _beaconStock;
        set
        {
            _beaconStock = value;
        }
    }
    public override void ActivateAbility()
    {
        --BeaconStock;
        SpawnBeacon();
    }
    public void SpawnBeacon()
    {
        Transform spawnAnchor;
        if (_spawnAnchor == null)
            spawnAnchor = transform;
        else
            spawnAnchor = _spawnAnchor;
        Beacon beacon = Instantiate(_beaconPrefab);
        beacon.transform.SetPositionAndRotation(spawnAnchor.position, spawnAnchor.rotation);
    }
}
