using Assets.Scripts.Factions;
using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy {
    #region Enemy Enums
    /**
     * @author Brenton Hauth
     * @date 11/16/20
     * <summary>
     * The different patrol patterns for enemy
     * </summary>
     */
    public enum PatrolPattern { None, Random, Ordered }
    #endregion

    /**
     * @author Brenton Hauth
     * @date 11/16/20
     * <summary>
     * Generic enemy class to be overriden by Specific enemies
     * </summary>
     */
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(FactionEntity))]
    [RequireComponent(typeof(Animator))]
    public abstract class Enemy : MonoBehaviour {
        #region Exposed Variables
        public float FovAngle = 60f;
        public float DetectionRange = 50f;
        public float AtRange = 1f;
        public float Damage = 10f;
        public PatrolPattern Pattern = PatrolPattern.Random;
        public Vector2 RandomRange;
        public Transform[] PatrolPoints;
        #endregion

        #region Variables
        protected NavMeshAgent Agent;
        protected Health Health;
        protected Animator Animator;
        protected FactionEntity Entity;
        protected StateMachine StateMachine;
        protected Vector3 Point;
        private Vector3 StartingPoint;
        private int LastPointIndex = -1;
        #endregion

        #region Properties
        public Vector3 Position1 {
            get => transform.position;
            set => transform.position = value;
        }
        public virtual bool IsAttacking { get; protected set; }
        #endregion

        #region Methods
        /**
         * @author Brenton Hauth
         * @date 11/16/20
         * <summary>
         * Awake method called by Unity
         * </summary>
         */
        protected virtual void Awake() {

            // Sets up StateMachine, and added default states
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

        /**
         * @author Brenton Hauth
         * @date 11/16/20
         * <summary>
         * Start method called by Unity
         * </summary>
         */
        protected virtual void Start() {
            // Get required components
            Agent = GetComponent<NavMeshAgent>();
            Health = GetComponent<Health>();
            Entity = GetComponent<FactionEntity>();
            Animator = GetComponent<Animator>();

            // Append listeners to components
            Entity.OnTarget += OnTarget;
            Health.OnDeath += OnDeath;

            StartingPoint = transform.position; // Grab entites starting position
            StateMachine.Start(); // start the State Machine
        }

        /**
         * @author Brenton Hauth
         * @date 11/16/20
         * <summary>
         * Update method called by Unity
         * </summary>
         */
        protected virtual void Update() {
            // Update current state
            StateMachine.Update();
        }

        /**
         * @author Brenton Hauth
         * @date 11/16/20
         * <summary>
         * Called when enemy's health reaches 0
         * </summary>
         */
        protected virtual void OnDeath() { }


        /**
         * @author Brenton Hauth
         * @date 11/16/20
         * <summary>
         * Called when FactionEntity spots a target
         * </summary>
         * <param name="newTarget"></param>
         */
        protected abstract void OnTarget(FactionEntity newTarget);

        #region State Methods

        // "idle" State methods
        protected virtual void OnIdleEnter() {
            if (Pattern != PatrolPattern.None) {
                Invoke("StopIdle", 2.5f);
            }
        }
        
        protected virtual void OnIdleStay() { }

        protected virtual void OnIdleExit() { }

        // "seek" State methods
        protected virtual void OnSeekEnter() {
            Point = NextPoint();
            LookAndSetDestination(Point);
        }

        protected virtual void OnSeekStay() {
            if (At(Agent.destination)) {
                // print($"At {Point}... Transitioning");
                StateMachine.TransitionTo("idle");
            }
        }

        protected virtual void OnSeekExit() {
            Stop();
        }

        // "attack" State methods
        protected virtual void OnAttackEnter() {
            IsAttacking = true;
        }

        protected virtual void OnAttackStay() { }

        protected virtual void OnAttackExit() {
            IsAttacking = false;
        }

        private void StopIdle() {
            if (StateMachine.CurrentState.Name == "idle") {
                StateMachine.TransitionTo("seek");
            }
        }
        #endregion

        protected void LookAt(Vector3 target) {
            if (!At(target)) {
                var dir = (target - transform.position).X0Z();
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }

        protected void LookAndSetDestination(Vector3 destination) {
            LookAt(destination);
            Agent.SetDestination(destination);
        }

        public bool At(Vector3 destination) {
            return VectorHelper.WithinRange(transform.position, destination, AtRange);
        }

        public void Stop() {
            // Fixes random null reference error
            if (this) {
                Agent.SetDestination(transform.position);
            }
        }

        public virtual bool InFov(Transform @object) {
            Vector3 direction = @object.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);
            return angle <= FovAngle;
        }

        public virtual bool InRange(Transform @object) {
            return VectorHelper.WithinRange(transform.position, @object.position, DetectionRange);
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
