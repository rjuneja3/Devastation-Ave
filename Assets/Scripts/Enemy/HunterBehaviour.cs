using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

// TODO: abstract out Enemy based behaviour
namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(NavMeshAgent))]
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
        private NavMeshAgent Agent;
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
            set {
                m_DetectedPlayer = value;
                // print($"Detected Player ${value}");
            }
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
            Agent = GetComponent<NavMeshAgent>();

            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) Player = p.transform;
            TransitionTo(HunterState.Idle);
        }

        private void LookAt(Vector3 pos) {
            if (pos == Vector3.zero) return;
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
                Stop();
                Invoke("StopIdle", 3);
                LookAt(Agent.destination);
                Animator.SetFloat("ZSpeed", 0);
            }
        }

        private void StopIdle() {
            if (State != HunterState.Seek) {
                TransitionTo(HunterState.Seek);
            }
        }

        private void Attack() {
            if (!At(Player.position)) {
                Animator.SetBool("Attack", false);
                TransitionTo(HunterState.Seek);
                return;
            }

            if (!IsAttacking) {
                Stop();
                Animator.SetBool("Attack", IsAttacking = true);
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
                Animator.SetFloat("ZSpeed", speed / 2f);
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
                    Point = VectorHelper.RandomVector3(-10, 10);
                    Point.y = transform.position.y;
                    p = Point;
                }


                LookAt(p);
                print($"POINT: {Point}");
            }
        }

        public bool At(Vector3 destination) {
            return VectorHelper.WithinRange(transform.position, destination, 1f);
        }

        public void Stop() {
            Agent.SetDestination(transform.position);
        }

        #endregion
    }
}
