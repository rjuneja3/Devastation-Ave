using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Factions {
    #region Faction Enums
    public enum Faction {
        Player = 1,
        Monster = 2,
        Gov = 4,

        // Don't use as an actual faction
        AllEnemies = Monster | Gov,
        AllFaction = Player | AllEnemies,
    }

    public enum FactionEntityTarget { AlwaysPlayer, Strongest, Nearest }
    #endregion

    [RequireComponent(typeof(Health))]
    public class FactionEntity : MonoBehaviour {
        public const int ENTITY_LAYER = 1 << 8;
        public const int LANDSCAPE_LAYER = 1 << 9;
        
        #region Exposed Variables
        public Faction Faction = Faction.Monster;
        public FactionEntityTarget TargetType = FactionEntityTarget.Nearest;
        public bool IsControlled = false;
        public float FovAngle = 60f;
        public float DetectionRange = 50f;
        public Transform Head;
        #endregion

        #region Variables
        private Collider[] Results = new Collider[10];
        #endregion

        #region Properties
        public bool IsRegistered { get; set; }
        public bool IsSearching { get; set; }
        public FactionEntity Target { get; private set; }
        public Health Health { get; private set; }
        public Vector3 Position => transform.position;
        public bool HasTarget => Target;
        #endregion

        #region Methods
        private void Start() {
            Health = GetComponent<Health>();
            FactionManager.Register(this);
            Health.OnDeath += OnDeath;
        }

        /*private void Update() {
            if (IsControlled || HasTarget) return;
            Sweep();
        }*/

        private void OnEnable() {
            FactionManager.Register(this);
        }

        private void OnDisable() {
            FactionManager.Unregister(this);
        }

        public void Search(FactionEntity entity) {
            if (InRange(entity.transform)) {
                if (InFov(entity.transform)) {
                    
                }
            }
        }

        public bool InRange(Transform @object) {
            return VectorHelper.WithinRange(
                transform.position,
                @object.position,
                DetectionRange);
        }

        public bool InFov(Transform @object) {
            Vector3 direction = @object.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);
            return angle <= FovAngle;
        }

        public bool DirectLineOfSight(Transform @object) {
            const int layer = LANDSCAPE_LAYER | ENTITY_LAYER;

            Vector3 to = @object.position + Vector3.up;
            Ray ray = new Ray(Head.position, to - Head.position);

            return Physics.SphereCast(ray, .25f, out var hit, DetectionRange, layer)
                ? hit.transform == @object
                : false;
        }

        public void Sweep() {
            int count = Physics.OverlapSphereNonAlloc(Position, DetectionRange, Results, ENTITY_LAYER);

            for (int i = 0; i < count; i++) {
                if (CheckObject(Results[i], out var entity)) {
                    CheckEntity(Results[i], entity);
                }
            }
        }

        private bool CheckObject(Collider c, out FactionEntity entity) {
            if (c.gameObject.tag == "Player") {
                return FactionManager.CheckCache(Faction, c.transform, out entity);
            } else if (c.gameObject.tag == "Enemy") {
                Faction faction = this.Faction ^ Faction.AllEnemies;
                return FactionManager.CheckCache(faction, c.transform, out entity, false);
            }
            entity = null;
            return false;
        }

        private void CheckEntity(Collider c, FactionEntity entity) {
            if (InFov(c.transform) && DirectLineOfSight(c.transform)) {
                Target = entity;
            }
        }

        private void OnDeath() {
            FactionManager.Unregister(this);
        }
        #endregion
    }
}
