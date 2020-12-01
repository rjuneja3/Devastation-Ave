using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Factions;
using Assets.Scripts.Helpers;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Enemy {
    /**
     * @author Brenton Hauth
     * @date 11/18/20
     * <summary>
     * Handles Soldier Behaviour. Extends <c>Enemy</c> class.
     * </summary>
     * <see cref="Enemy"/>
     */
    [RequireComponent(typeof(EnemyWeaponHandler))]
    public class Soldier : Enemy {
        #region Exposed Variables
        public float Accuracy = 1f;
        public float RateOfAttack = 1.75f;
        #endregion

        #region Variables
        private EnemyWeaponHandler WeaponHandler;
        private float m_ZSpeed, m_XSpeed;
        private bool AbleToAttack = true;
        #endregion

        #region Properties
        private float ZSpeed {
            get => m_ZSpeed;
            set => Animator.SetFloat("ZSpeed", m_ZSpeed = value);
        }
        private float XSpeed {
            get => m_XSpeed;
            set => Animator.SetFloat("XSpeed", m_XSpeed = value);
        }
        public override bool IsAttacking {
            get => base.IsAttacking;
            protected set {
                base.IsAttacking = value;
            }
        }
        private bool HasCleanShot => Entity.HasTarget && Entity.DirectLineOfSight(Entity.Target);
        #endregion

        #region Methods
        /**
         * @author Brenton Hauth
         * @date 11/24/20
         * <summary>
         * Start method called by Unity
         * </summary>
         */
        protected override void Start() {
            base.Start();
            WeaponHandler = GetComponent<EnemyWeaponHandler>();
            WeaponHandler.ActivateLayer(AnimationLayer.Firearm);
            if (WeaponHandler.CurrentWeapon is Firearm f && f.CanReload) {
                f.Reload();
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/24/20
         * <summary>
         * Update method called by Unity
         * </summary>
         */
        protected override void Update() {
            base.Update();
        }

        /**
         * @author Brenton Hauth
         * @date 11/24/20
         * <summary>
         * </summary>
         */
        protected override void OnTarget(FactionEntity newTarget) {
            if (!newTarget) {
                StateMachine.TransitionTo("seek");
            }
        }

        #region State Methods
        /**
         * @author Brenton Hauth
         * @date 11/24/20
         * <summary>
         * </summary>
         */
        protected override void OnIdleEnter() {
            base.OnIdleEnter();
        }

        /**
         * @author Brenton Hauth
         * @date 11/24/20
         * <summary>
         * </summary>
         */
        protected override void OnIdleStay() {
            base.OnIdleStay();
        }

        /**
         * @author Brenton Hauth
         * @date 11/24/20
         * <summary>
         * </summary>
         */
        protected override void OnIdleExit() {
            base.OnIdleExit();
        }

        /**
         * @author Brenton Hauth
         * @date 11/22/20
         * <summary>
         * </summary>
         */
        protected override void OnSeekEnter() {
            if (Entity.HasTarget) {
                LookAndSetDestination(Entity.Target.Position);
                ZSpeed = 3f;
            } else {
                base.OnSeekEnter();
                ZSpeed = 1f;
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/22/20
         * <summary>
         * </summary>
         */
        protected override void OnSeekStay() {
            if (Entity.HasTarget) {
                if (HasCleanShot) {
                    StateMachine.TransitionTo("attack");
                } else {
                    LookAndSetDestination(Entity.Target.Position);
                }
            } else {
                base.OnSeekStay();
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/22/20
         * <summary>
         * </summary>
         */
        protected override void OnSeekExit() {
            base.OnSeekExit();
            ZSpeed = 0f;
        }

        /**
         * @author Brenton Hauth
         * @date 11/24/20
         * <summary>
         * </summary>
         */
        protected override void OnAttackEnter() {
            base.OnAttackEnter();
        }

        /**
         * @author Brenton Hauth
         * @date 11/24/20
         * <summary>
         * </summary>
         */
        protected override void OnAttackStay() {
            LookAt(Entity.Target.Position);
            if (!HasCleanShot) {
                StateMachine.TransitionTo("seek");
            }
            TryShooting();
        }
        
        /**
         * @author Brenton Hauth
         * @date 11/24/20
         * <summary>
         * </summary>
         */
        protected override void OnAttackExit() {
            base.OnAttackExit();
            if (WeaponHandler.CurrentWeapon is Firearm f && f.CanReload) {
                f.Reload();
            }
        }

        #endregion
        private void TryShooting() {
            if (AbleToAttack) {
                AbleToAttack = false;
                Invoke("SetAttackFlag", RateOfAttack);
            } else return;

            print("Try shooting");
            WeaponHandler.CurrentWeapon.TryAttacking();
        }

        private void SetAttackFlag() => AbleToAttack = true;
        #endregion
    }
}

