using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Player;

namespace Assets.Scripts.Enemy {

    // TODO: abstract out Enemy based behaviour
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class HunterBehaviour : MonoBehaviour {
        readonly Quaternion DontLookAtValue = Quaternion.Euler(Vector3.zero);
        const int HunterPointWorth = 10;

        public enum HunterState {
            Idle = 1,
            Seek = 2,
            Attack = 4,
        }
        
        public enum HunterPatrolType {
            None = 0,
            Random = 1,
            Ordered = 2,
        }

        #region Exposed Variables
        public float DetectionAngle = 60f;
        public float DetectionRange = 50f;
        public float Power = 40f;
        public float Speed = .5f;
        public Transform Head;
        public HunterPatrolType PatrolType = HunterPatrolType.Random;

        public Vector2 RandomRange = new Vector2(-3, 3);

        public Transform[] PatrolPoints = new Transform[2];
        #endregion

        #region Varibles
        private int LastPointIndex = -1;
        private Animator Animator;
        private NavMeshAgent Agent;
        private Transform Player;
        private Transform PlayerTorsoRef;
        private Health Health;
        private HunterState State = HunterState.Idle;
        private Vector3 Point, StartingPoint;
        private bool CalledStopIdle = false;
        private bool m_DetectedPlayer = false;
        private bool m_IsAttacking = false;
        private float m_ZSpeed = 0;
        #endregion

        #region Properties
        public bool IsAttacking {
            get => m_IsAttacking;
            set => Animator.SetBool("Attack", m_IsAttacking = value);
        }

        public float ZSpeed {
            get => m_ZSpeed;
            set => Animator.SetFloat("ZSpeed", m_ZSpeed = value);
        }

        private bool DetectedPlayer {
            get => m_DetectedPlayer;
            set {
                m_DetectedPlayer = value;
                if (value) print($"[{name}] Detected Player");
            }
        }

        public bool PlayerInRange => Player &&
            VectorHelper.WithinRange(Player.position, transform.position, DetectionRange);

        public bool HasDirectLineOfSight {
            get {
                var dir = PlayerTorsoRef.position - Head.position;
                Debug.DrawLine(Head.position, Head.position + (dir * 50), Color.green, 5f);
                return Physics.Raycast(Head.position, dir, out var h)
                    ? h.collider.gameObject.tag == "Player"
                    : false;
            }
        }

        // TODO: clean up logic
        public bool PlayerInFov {
            get {
                if (PlayerInRange) {
                    Vector3 direction = (Player.position - transform.position).normalized;
                    float angle = Vector3.Angle(transform.forward, direction);
                    if (angle <= DetectionAngle) {
                        return HasDirectLineOfSight;
                    }
                }
                return false;
            }
        }
        #endregion

        #region Methods
        void Start() {
            Animator = GetComponent<Animator>();
            Agent = GetComponent<NavMeshAgent>();

            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) {
                Player = p.transform;
                var c = p.GetComponent<PlayerController>();
                PlayerTorsoRef = c.Torso;
            }
            TransitionTo(HunterState.Idle);

            var hitBoxes = GetComponentsInChildren<HitBox>();

            StartingPoint = transform.position;

            foreach (var b in hitBoxes) {
                b.OnCollide += OnHitBoxCollide;
            }

            Health = GetComponent<Health>();

            Health.OnDeath += OnDeath;
        }

        private void OnDeath() {
            HudHandler.Points += HunterPointWorth;
        }

        private void LookAt(Vector3 pos) {
            if (pos == Vector3.zero) return;
            var dir = pos - transform.position;
            dir.y = 0;
            var r = Quaternion.LookRotation(dir);
            if (r != DontLookAtValue)
                transform.rotation = r;
        }

        void Update() {
            if (!DetectedPlayer && PlayerInFov) {
                DetectedPlayer = true;
                TransitionTo(HunterState.Seek);
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
                Stop();
                Invoke("StopIdle", 3);
                //LookAt(Agent.destination);
                ZSpeed = 0;
            }
        }

        private void StopIdle() {
            if (State != HunterState.Seek) {
                TransitionTo(HunterState.Seek);
            }
        }

        private void Attack() {
            if (!At(Player.position)) {
                IsAttacking = false;
                TransitionTo(HunterState.Seek);
                return;
            }

            if (!IsAttacking) {
                Stop();
                IsAttacking = true;
            }
        }

        private void Seek() {
            var p = DetectedPlayer ? Player.position : Point;

            Agent.SetDestination(p);

            if (At(p)) {
                TransitionTo(DetectedPlayer
                    ? HunterState.Attack
                    : HunterState.Idle);

            } else {
                float speed = DetectedPlayer ? 3f : 1f;
                ZSpeed = speed / 2f;
                Agent.speed = speed;
            }
        }

        private void SeekPlayer() {

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
                    Point = GetNextPoint();
                    Point.y = transform.position.y;
                    p = Point;
                }

                LookAt(p);
                //print($"POINT: {Point}");
            }
        }

        public bool At(Vector3 destination) {
            return VectorHelper.WithinRange(transform.position, destination, 1f);
        }

        public void Stop() {
            Agent.SetDestination(transform.position);
        }

        private void OnTriggerEnter(Collider o) {
            OnHitBoxCollide(o.gameObject);
        }

        private void OnHitBoxCollide(GameObject o) {
            if (IsAttacking && o.tag == "Player") {
                var health = o.GetComponent<Health>();
                if (health) health.CurrentHP -= Power;
                print($"Hit player!!! [deduction: {!!health}]");
            } else {
                print($"HIT: {o.gameObject.name} [{o.gameObject.tag}]");
            }
        }
        
        private Vector3 GetNextPoint() {
            if (PatrolType == HunterPatrolType.Random) {
                var vec = VectorHelper.RandomVector3(RandomRange.x, RandomRange.y);
                return StartingPoint + vec;
            } else if (PatrolType == HunterPatrolType.Ordered) {
                LastPointIndex++;
                if (PatrolPoints.Length > 0)
                    return PatrolPoints[LastPointIndex % PatrolPoints.Length].position;
                else
                    Debug.LogWarning("No patrol points");
            }

            return transform.position;
        }

        #endregion
    }
}