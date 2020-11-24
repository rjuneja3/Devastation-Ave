using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Factions;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Enemy {
    [RequireComponent(typeof(EnemyWeaponHandler))]
    public class Soldier : Enemy {
        #region Exposed Variables
        public float Accuracy = 1f;
        #endregion

        #region Variables
        private EnemyWeaponHandler WeaponHandler;
        private float m_ZSpeed, m_XSpeed;
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
        protected override void Start() {
            base.Start();
            WeaponHandler = GetComponent<EnemyWeaponHandler>();

            // Resets layer positions
            int @base = Animator.GetLayerIndex("Base"),
                firearm = Animator.GetLayerIndex("Firearm");
            Animator.SetLayerWeight(@base, 0f);
            Animator.SetLayerWeight(firearm, 1f);
        }

        protected override void Update() {
            base.Update();
        }

        protected override void OnTarget(FactionEntity newTarget) {
            if (!newTarget) {
                StateMachine.TransitionTo("seek");
            }
        }

        #region State Methods
        // Idle State
        protected override void OnIdleEnter() {
            base.OnIdleEnter();
        }

        protected override void OnIdleStay() {
            base.OnIdleStay();
        }

        protected override void OnIdleExit() {
            base.OnIdleExit();
        }

        // Seek State
        protected override void OnSeekEnter() {
            if (Entity.HasTarget) {
                LookAndSetDestination(Entity.Target.Position);
                ZSpeed = 3f;
            } else {
                base.OnSeekEnter();
                ZSpeed = 1f;
            }
        }

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

        protected override void OnSeekExit() {
            base.OnSeekExit();
            ZSpeed = 0f;
        }

        // Attack State
        protected override void OnAttackEnter() {
            base.OnAttackEnter();
        }

        protected override void OnAttackStay() {
            base.OnAttackStay();
            LookAt(Entity.Target.Position);
            if (!HasCleanShot) {
                StateMachine.TransitionTo("seek");
            }
        }

        protected override void OnAttackExit() {
            base.OnAttackExit();
        }
        #endregion
        #endregion
    }
}

