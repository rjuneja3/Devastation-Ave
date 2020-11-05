using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firearm : Weapon {
    public const float BulletDelay = 4f / 60f; // ~4 frames

    #region Exposed Variables
    public float HipFireRadius = 60f;
    public uint MagSize = 30;
    public uint Ammo = 0;
    public uint BulletCount = 1; // if it's a shotgun, change to 8
    #endregion

    #region Variables
    private Queue<Bullet> BulletQueue = new Queue<Bullet>();
    #endregion

    #region Properties
    public float CurrentAdsRatio { get; private set; }
    #endregion

    /// <param name="ratio">0.0 means they are aiming, 1.0 means they are not aiming</param>
    public void SetAdsRatio(float ratio) {
        CurrentAdsRatio = Mathf.Clamp01(ratio);
    }

    protected override void Start() {
        base.Start();
    }
    
    public override void Attack() { // Shoot
        // TODO: Append camera to Firearm script, get origin/direction from that camera
        Vector3 origin = Camera.main.transform.position,
            direction = Camera.main.transform.forward;

        var bullet = new Bullet {
            Origin = origin,
            Direction = direction,
            Accuracy = CurrentAdsRatio * HipFireRadius
        };

        BulletQueue.Enqueue(bullet);

        Invoke("DelayBullet", BulletDelay);
    }

    private void DelayBullet() {
        Bullet b = BulletQueue.Dequeue();

        if (b.Accuracy != 0f)
            RoughDirection(ref b.Direction, b.Accuracy);

        if (Physics.Raycast(b.Origin, b.Direction, out var hit, Range)) {
            Hit(hit.collider.gameObject);

            if (DebugMode)
                Debug.DrawLine(b.Origin, hit.point, Color.green, 5f);

        } else if (DebugMode) {
            var point = b.Origin + (b.Direction * Range);
            Debug.DrawLine(b.Origin, point, Color.yellow, 5f);
        }
    }

    public override void Hit(GameObject o) {
        base.Hit(o);
    }

    private static void RoughDirection(ref Vector3 direction, float hipfire) {
        // change calculation, not sure if correct
        direction.x += Random.Range(-hipfire, hipfire);
        direction.y += Random.Range(-hipfire, hipfire);
    }

    public struct Bullet {
        public Vector3 Origin;
        public Vector3 Direction;
        public float Accuracy;
    }
}
