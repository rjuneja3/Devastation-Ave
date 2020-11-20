using Assets.Scripts.Factions;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Enemy {

    public class TestEnemy : Enemy {
        #region Exposed Variables
        #endregion

        #region Variables
        #endregion

        #region Properties
        public override bool IsAttacking {
            get => base.IsAttacking;
            protected set {
                base.IsAttacking = value;
                // turn on or off attack animation
            }
        }
        #endregion

        #region Methods
        protected override void Start() {
            base.Start();

            var m = GetComponent<MeshRenderer>().material;
            if (Entity.Faction == Faction.Monster) {
                m.color = Color.red;
            } else if (Entity.Faction == Faction.Gov) {
                m.color = Color.blue;
            }
        }

        protected override void Update() {
            base.Update();
        }

        protected override void OnTarget(FactionEntity newTarget) {
            var m = Entity.Head.GetComponent<MeshRenderer>().material;
            if (newTarget.Faction == Faction.Monster) {
                m.color = Color.red;
            } else if (newTarget.Faction == Faction.Gov) {
                m.color = Color.blue;
            } else if (newTarget.Faction == Faction.Player) {
                m.color = Color.green;
            }
        }

        #region State Methods
        protected override void OnSeekEnter() {
            if (Entity.HasTarget) {
                LookAndSetDestination(Entity.Target.Position);
            } else {
                base.OnSeekEnter();
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

        #endregion
    }
}