using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
// [RequireComponent(typeof(Health))]
public class HunterBehaviour : MonoBehaviour {
    
    public enum HunterState {
        Idle = 1,
        Seek = 2,
        Attack = 4,
    }

    #region Exposed Variables
    public float DetectionAngle = 60f;
    public float DetectionRange = 50f;
    public float Power = 40f;
    public float Speed = .5f;
    public Transform LeftHand, RightHand;
    #endregion

    #region Varibles
    private Animator Animator;
    private Transform Player;
    private HunterState State = HunterState.Idle;
    private Vector3 Point;
    private bool CalledStopIdle = false;
    private bool m_DetectedPlayer = false;
    private bool IsAttacking = false;
    #endregion

    #region Properties
    private bool DetectedPlayer {
        get => m_DetectedPlayer;
        set => print($"Detected Player ${m_DetectedPlayer = value}");
    }

    public bool PlayerInRange => Player &&
        VectorHelper.WithinRange(Player.position, transform.position, DetectionRange);

    public bool PlayerInFov {
        get {
            if (PlayerInRange) {
                Vector3 direction = (Player.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, direction);
                if (angle <= DetectionAngle) {
                    return true;
                }
            }
            return false;
        }
    }
    #endregion

    #region Methods
    void Start() {
        Animator = GetComponent<Animator>();

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) Player = p.transform;
        TransitionTo(HunterState.Idle);
    }

    private void LookAt(Vector3 pos) {
        var dir = pos - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }
    
    void Update() {
        if (PlayerInFov) {
            if (!DetectedPlayer) {
                DetectedPlayer = true;
                TransitionTo(HunterState.Seek);
            }
        } else if (!PlayerInRange) {
            DetectedPlayer = false;
        }

        FSM();
    }

    private void FSM() {
        switch (State) {
            case HunterState.Idle:
                Idle(); break;
            case HunterState.Attack:
                Attack(); break;
            case HunterState.Seek:
                Seek(); break;
        }
    }

    private void Idle() {
        if (!CalledStopIdle) {
            CalledStopIdle = true;
            Invoke("StopIdle", 3);
            Animator.SetFloat("ZSpeed", 0);
        }
    }

    private void StopIdle() {
        TransitionTo(HunterState.Seek);
    }

    private void Attack() {
        var p = Player?.position ?? Vector3.zero;
        if (!VectorHelper.WithinRange(transform.position, p, 2)) {
            Animator.SetBool("Attack", false);
            TransitionTo(HunterState.Seek);
            return;
        }

        if (!IsAttacking) {
            Animator.SetBool("Attack", IsAttacking = true);
            Animator.SetFloat("ZSpeed", 0f);
        }
    }
    private void Seek() {
        var p = DetectedPlayer ? (Player?.position ?? Vector3.zero) : Point;
        
        if (VectorHelper.WithinRange(transform.position, p, 2)) {
            var state = DetectedPlayer ? HunterState.Attack : HunterState.Idle;
            TransitionTo(state);
        } else {
            var speed = DetectedPlayer ? 2f : .5f;
            Animator.SetFloat("ZSpeed", speed);
        }
    }

    private void TransitionTo(HunterState state) {
        State = state;
        CalledStopIdle = false;
        IsAttacking = false;
        if (state == HunterState.Seek) {
            Vector3 p = new Vector3();
            if (DetectedPlayer) {
                p.x = Player.position.x;
                p.y = transform.position.y;
                p.z = Player.position.z;
            } else {
                Point = VectorHelper.RandomVector3(-10, 10);
                Point.y = transform.position.y;
                p = Point;
            }


            LookAt(p);
            print($"POINT: {Point}");
        }
    } 

    #endregion
}
