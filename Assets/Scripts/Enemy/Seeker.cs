using Assets.Scripts.Factions;
using Assets.Scripts.General;
using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemy {
    /**
     * @author Brenton Hauth
     * @date 11/20/20
     * <summary>
     * Specific class for Seeker behaviour
     * </summary>
     */
    public class Seeker : Enemy {
        #region Exposed Variables
        #endregion

        #region Variables
        private float m_ZSpeed = 0;
        #endregion

        #region Properties
        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Flag to indicate if the enemy is attacking 
         * </summary>
         */
        public override bool IsAttacking {
            get => base.IsAttacking;
            protected set {
                base.IsAttacking = value;
                Animator.SetBool("Attack", value);
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Flag to indicate speed of enemy
         * </summary>
         */
        public float ZSpeed {
            get => m_ZSpeed;
            set {
                Animator.SetFloat("ZSpeed", m_ZSpeed = value);
                Agent.speed = value;
            }
        }
        #endregion

        #region Methods
        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Start method called by Unity
         * </summary>
         */
        protected override void Start() {
            base.Start();

            var hitboxes = GetComponentsInChildren<HitBox>();
            foreach (var h in hitboxes) {
                h.OnCollide += OnHitBoxCollide;
            }

            Entity.OnNoise += OnNoise;
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Called when the enemy finds a target
         * </summary>
         */
        protected override void OnTarget(FactionEntity newTarget) {
            if (!newTarget) {
                ZSpeed = 2f;
                StateMachine.TransitionTo("seek");
            }
        }

        #region State Methods
        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Called when the Idle state starts
         * </summary>
         */
        protected override void OnIdleEnter() {
            ZSpeed = 0;
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Called when the Idle state ends
         * </summary>
         */
        protected override void OnIdleExit() {
            base.OnIdleExit();
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Called when the Seek state starts
         * </summary>
         */
        protected override void OnSeekEnter() {
            if (Entity.HasTarget) {
                LookAndSetDestination(Entity.Target.Position);
                ZSpeed = 2f;
            } else {
                LookAndSetDestination(Point);
                ZSpeed = 1f;
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Handles the Seek state for the Seeker
         * </summary>
         */
        protected override void OnSeekStay() {
            if (Entity.HasTarget) {
                LookAndSetDestination(Entity.Target.Position);
                if (At(Agent.destination)) {
                    StateMachine.TransitionTo("attack");
                }
            } else {
                base.OnSeekStay();
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Called when the Seek state ends
         * </summary>
         */
        protected override void OnSeekExit() {
            base.OnSeekExit();
            ZSpeed = 0;
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Called when the Attack state starts
         * </summary>
         */
        protected override void OnAttackEnter() {
            base.OnAttackEnter();
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Handles the attack state for the seeker
         * </summary>
         */
        protected override void OnAttackStay() {
            base.OnAttackStay();
            if (!Entity.HasTarget || !At(Entity.Target.Position)) {
                StateMachine.TransitionTo("seek");
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Called when the Attack state ends
         * </summary>
         */
        protected override void OnAttackExit() {
            base.OnAttackExit();
        }
        #endregion

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Called when an enemy hit box hits an object
         * </summary>
         * <param name="o">The object hit by the hit box</param>
         */
        private void OnHitBoxCollide(GameObject o) {
            if (IsAttacking && o.layer == FactionEntity.ENTITY_LAYER_INDEX) {
                var check = Faction.All ^ Entity.Faction;
                if (FactionManager.CheckCache(check, o.transform, out var entity, false)) {
                    entity.Health.CurrentHP -= Damage * GameSettings.EnemyDamageMultiplier;
                }
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Called when a noise is produced by the FactionManager
         * </summary>
         */
        private void OnNoise(Vector3 origin, float strenth) {
            if (StateMachine.CurrentState.Name == "idle") {
                Point = origin;
                StateMachine.TransitionTo("seek");
            }
        }
        #endregion
    }
}
