using Assets.Scripts.Factions;
using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemy {
    public class Seeker : Enemy {
        #region Exposed Variables
        #endregion

        #region Variables
        private float m_ZSpeed = 0;
        #endregion

        #region Properties
        public override bool IsAttacking {
            get => base.IsAttacking;
            protected set {
                base.IsAttacking = value;
                Animator.SetBool("Attack", value);
            }
        }

        public float ZSpeed {
            get => m_ZSpeed;
            set {
                Animator.SetFloat("ZSpeed", m_ZSpeed = value);
                Agent.speed = value;
            }
        }
        #endregion

        #region Methods
        protected override void Start() {
            base.Start();

            var hitboxes = GetComponentsInChildren<HitBox>();
            foreach (var h in hitboxes) {
                h.OnCollide += OnHitBoxCollide;
            }
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
        protected override void OnSeekEnter() {
            if (Entity.HasTarget) {
                LookAndSetDestination(Entity.Target.Position);
                ZSpeed = 3f;
            } else {
                base.OnSeekEnter();
                ZSpeed = 3f;
            }
        }

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

        protected override void OnSeekExit() {
            base.OnSeekExit();
            ZSpeed = 0;
        }

        protected override void OnAttackEnter() {
            base.OnAttackEnter();
        }

        protected override void OnAttackStay() {
            base.OnAttackStay();
            if (!Entity.HasTarget || !At(Entity.Target.Position)) {
                StateMachine.TransitionTo("seek");
            }
        }

        protected override void OnAttackExit() {
            base.OnAttackExit();
        }
        #endregion

        private void OnHitBoxCollide(GameObject o) {
            if (IsAttacking && o.layer == FactionEntity.ENTITY_LAYER_INDEX) {
                var check = Faction.All ^ Entity.Faction;
                if (FactionManager.CheckCache(check, o.transform, out var entity, true)) {
                    entity.Health.CurrentHP -= Damage;
                }
            }
        }
        #endregion
    }
}
