using Assets.Scripts.Factions;
using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy {
    #region Enemy Enums
    public enum PatrolPattern { None, Random, Ordered }
    public enum EnemyState { Idle, Seek, Attack }
    #endregion

    // [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(NavMeshAgent))]
    //[RequireComponent(typeof(FactionEntity))]
    public class Enemy : MonoBehaviour {
        #region Exposed Variables
        public PatrolPattern Pattern = PatrolPattern.Random;
        public Vector2 RandomRange;
        public float FovAngle = 60f;
        public float DetectionRange = 50f;
        public Transform[] PatrolPoints;
        #endregion

        #region Variables
        protected NavMeshAgent Agent;
        protected Health Health;
        protected FactionEntity FactionEntity;
        protected StateMachine StateMachine;
        private Vector3 Point, StartingPoint;
        private int LastPointIndex = -1;
        #endregion

        #region Properties
        public Vector3 Position {
            get => transform.position;
            set => transform.position = value;
        }
        #endregion

        #region Methods
        protected virtual void Awake() {
            StateMachine = new StateMachine();

            StateMachine.AddState(new StateMachine.State {
                Name = "idle",
                OnEnter = OnIdleEnter,
                OnStay = OnIdleStay,
                OnExit = OnIdleExit
            });

            StateMachine.AddState(new StateMachine.State {
                Name = "seek",
                OnEnter = OnSeekEnter,
                OnStay = OnSeekStay,
                OnExit = OnSeekExit
            });

            StateMachine.AddState(new StateMachine.State {
                Name = "attack",
                OnEnter = OnAttackEnter,
                OnStay = OnAttackStay,
                OnExit = OnAttackExit,
            });
        }

        protected virtual void Start() {
            // Get required components
            Agent = GetComponent<NavMeshAgent>();
            Health = GetComponent<Health>();
            FactionEntity = GetComponent<FactionEntity>();

            // Append listeners to components
            FactionEntity.OnTarget += OnTarget;
            Health.OnDeath += OnDeath;

            StartingPoint = transform.position; // Grab entites starting position
            StateMachine.Start(); // start the State Machine
        }

        protected virtual void Update() {
            // Update current state
            StateMachine.Update();
        }

        /// <summary>
        /// Called when enemy's health reaches 0
        /// </summary>
        protected virtual void OnDeath() { }

        /// <summary>
        /// Called when FactionEntity spots a target
        /// </summary>
        /// <param name="newTarget"></param>
        protected virtual void OnTarget(FactionEntity newTarget) {
            GetComponent<MeshRenderer>().material.color = Color.green;
        }

        #region State Methods

        // "idle" State methods
        protected virtual void OnIdleEnter() {
            Invoke("StopIdle", 2.5f);
        }
        
        protected virtual void OnIdleStay() { }

        protected virtual void OnIdleExit() { }

        // "seek" State methods
        protected virtual void OnSeekEnter() {
            Point = NextPoint();
            LookAt(Point);
            Agent.SetDestination(Point);
        }

        protected virtual void OnSeekStay() {
            if (At(Agent.destination)) {
                // print($"At {Point}... Transitioning");
                StateMachine.TransitionTo("idle");
            }
        }

        protected virtual void OnSeekExit() { }

        // "attack" State methods
        protected virtual void OnAttackEnter() { }

        protected virtual void OnAttackStay() { }

        protected virtual void OnAttackExit() { }

        private void StopIdle() {
            if (StateMachine.CurrentState.Name == "idle") {
                StateMachine.TransitionTo("seek");
            }
        }
        #endregion

        protected void LookAt(Vector3 target) {
            if (target != Vector3.zero) {
                var dir = target - transform.position;
                dir.y = 0; // Either set to 0 or position.y
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }

        public bool At(Vector3 destination) {
            return VectorHelper.WithinRange(Position, destination, 1f);
        }

        public virtual bool InFov(Transform @object) {
            Vector3 direction = @object.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);
            return angle <= FovAngle;
        }

        public virtual bool InRange(Transform @object) {
            return VectorHelper.WithinRange(Position, @object.position, DetectionRange);
        }

        private Vector3 NextPoint() {
            if (Pattern == PatrolPattern.Ordered) {
                if (PatrolPoints.Length > 0) {
                    return PatrolPoints[++LastPointIndex % PatrolPoints.Length].position;
                }
            } else if (Pattern == PatrolPattern.Random) {
                var v = VectorHelper.RandomVector2(
                    RandomRange.x,
                    RandomRange.y);

                return StartingPoint + new Vector3(v.x, 0, v.y);
            }
            return transform.position;
        }
        #endregion
    }
}
