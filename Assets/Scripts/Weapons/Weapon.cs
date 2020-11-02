using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon {

    public readonly GameObject WeaponPrefab;
    public readonly string Name;
    // TODO: store audio clip, and play for later use

    public WeaponType Type { get; private set; }
    public float RateOfAttack { get; private set; } = 60f;

    public Weapon(GameObject prefab, string name, WeaponType type, float rateOfAttack) {
        WeaponPrefab = prefab;
        Name = name;
        Type = type;
        RateOfAttack = rateOfAttack;
    }

    public GameObject Instantiate(Transform parent) {
        return Object.Instantiate(WeaponPrefab, parent);
    }
}

public enum WeaponType {
    None = 0,
    FullAuto = 1,
    SemiAuto = 2,
    PumpAction = 3,
    Melee = 4
}
