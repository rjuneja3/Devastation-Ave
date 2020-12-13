using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Factions;
using Assets.Scripts.General;
using Assets.Scripts.Player;
using UnityEngine;


namespace Assets.Scripts.Enemy {
    public class Hunter : Enemy {
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
                Animator?.SetBool("Attack", value);
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

            Entity.OnNoise += OnNoise;
        }

        protected override void Update() {
            base.Update();
        }

        protected override void OnTarget(FactionEntity newTarget) {
            StateMachine.TransitionTo("seek");
        }

        #region State Methods
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

        // TODO: fix hits on other enemies
        private void OnHitBoxCollide(GameObject o) {
            if (IsAttacking && o.layer == FactionEntity.ENTITY_LAYER_INDEX) {
                if (o.tag == "Player" && GameSettings.MakePlayerImmortal) {
                    Debug.Log($"Player {o.name} is immortal.");
                    return;
                }
                var check = Faction.All ^ Entity.Faction;
                print($"{name} is attacking {o.name}");
                if (FactionManager.CheckCache(check, o.transform, out var entity, false)) {
                    entity.Health.CurrentHP -= Damage * GameSettings.EnemyDamageMultiplier;
                }
            }
        }

        private void OnNoise(Vector3 origin, float strength) {
            if (StateMachine.CurrentState.Name == "idle") {
                LookAt(origin);
            }
        }
        #endregion
    }
}
