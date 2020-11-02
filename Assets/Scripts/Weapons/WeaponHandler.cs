using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour {

    #region Exposed Variables
    public float BulletDelayAmount = 4f / 60f; // ~4 frames
    public bool DebugMode = true;
    #endregion

    #region Variables
    private bool Reloaded = true;
    private Queue<Ray> BulletQueue = new Queue<Ray>();
    #endregion

    #region Properties
    public GameObject CurrentWeapon { get; private set; }
    public WeaponType CurrentWeaponType { get; private set; }
    public float CurrentRateOfAttack { get; private set; } = /* for testing */ .5f;
    public bool HasWeapon => CurrentWeapon;
    public float MouseX => Input.GetAxis("Mouse X");
    public float MouseY => Input.GetAxis("Mouse Y");
    public bool Fire1 => Input.GetButton("Fire1");
    #endregion

    #region Methods
    void Start() {
    }

    void Update() {
        if (Fire1) {
            Attack();
        }
    }

    private void SetReloadedFlag() => Reloaded = true;

    private void Attack() {
        if (Reloaded) {
            Reloaded = false;
            Invoke("SetReloadedFlag", CurrentRateOfAttack);
        } else return;

        if (CurrentWeaponType == WeaponType.Melee)
            Melee();
        else if (CurrentWeaponType != WeaponType.None)
            Shoot();
    }

    private void Shoot() {
        Vector3 origin = Camera.main.transform.position,
            direction = Camera.main.transform.forward;

        BulletQueue.Enqueue(new Ray(origin, direction));

        Invoke("DelayBullet", BulletDelayAmount);
    }

    private void DelayBullet() {
        Ray ray = BulletQueue.Dequeue();
        
        if (Physics.Raycast(ray, out var hit)) {
            print(hit.collider.gameObject.name);

            if (DebugMode) {
                Debug.DrawLine(ray.origin, hit.point, Color.green, 5f);
            }
        } else if (DebugMode) {
            Debug.DrawLine(ray.origin,
                ray.origin + (ray.direction * 1000f),
                Color.yellow, 5f);
        }
    }

    private void Melee() {
        // Spawn hit box infront of player
    }
    #endregion
}
