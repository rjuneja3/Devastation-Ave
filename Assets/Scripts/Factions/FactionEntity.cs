using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using System;
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
        public const int ENTITY_LAYER = 1 << 8; // The layer that all entites reside on
        public const int LANDSCAPE_LAYER = 1 << 9; // The layer that terrain & buildings reside on
        
        #region Exposed Variables
        public Faction Faction = Faction.Monster;
        public FactionEntityTarget TargetType = FactionEntityTarget.Nearest;
        public bool IsControlled = false;
        public float FovAngle = 60f;
        public float DetectionRange = 50f;
        public Transform Head;
        #endregion

        #region Variables
        private Collider[] Results = new Collider[10]; // 10 may be too small
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
        /// <summary>
        /// Called when the entity aquires a new target
        /// </summary>
        public event Action<FactionEntity> OnTarget;

        private void Start() {
            Health = GetComponent<Health>();
            FactionManager.Register(this);
            Health.OnDeath += OnDeath;
        }

        private void Update() {
            // Returns if this entity is controlled by a player or if it already has a target
            if (IsControlled || HasTarget) return;
            Sweep();
        }

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

        /// <summary>
        /// Checks if Entity is within a certain range of an object
        /// </summary>
        /// <param name="object">The object to check</param>
        /// <returns>If the object is in range</returns>
        public bool InRange(Transform @object) {
            return VectorHelper.WithinRange(
                transform.position,
                @object.position,
                DetectionRange);
        }

        /// <summary>
        /// Checks if an object is in the entities Field of View
        /// </summary>
        /// <param name="object">The object to check</param>
        /// <returns>if the object is in the FOV</returns>
        public bool InFov(Transform @object) {
            Vector3 direction = @object.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);
            return angle <= FovAngle;
        }

        /// <summary>
        /// Checks if the entity has direct line of sight to the object
        /// (not interrupted by an a wall)
        /// </summary>
        /// <param name="object">The object to check</param>
        /// <returns>If the entity has a direct line of site</returns>
        public bool DirectLineOfSight(Transform @object) {
            const int layer = LANDSCAPE_LAYER | ENTITY_LAYER; // Combines Landscape and entity layers

            Vector3 to = @object.position + Vector3.up; // TODO: make sure ray hits y-center of object
            Ray ray = new Ray(Head.position, to - Head.position); // creates a ray from the entity's head

            // Casts a Sphere towards the entity and checks if it hit
            return Physics.SphereCast(ray, .25f, out var hit, DetectionRange, layer)
                ? hit.transform == @object
                : false;
        }

        /// <summary>
        /// Sweeps through entities and 
        /// TODO: Try optimizing method a bit more
        /// </summary>
        public void Sweep() {
            // Checks to see if there are any entities in the 'Detection Range'
            int n = Physics.OverlapSphereNonAlloc(
                position: Position,
                radius: DetectionRange,
                results: Results,
                layerMask: ENTITY_LAYER);

            for (int i = 0; i < n; i++) {
                // Gets associated entity with the collider
                if (GetAssociatedEntity(Results[i], out var entity)) {
                    // Checks if the entity is visible, ands updates the target
                    EntityIsVisible(Results[i], entity);

                    // no need to continue; we have a target
                    if (HasTarget) break;
                }
            }
        }

        private bool GetAssociatedEntity(Collider c, out FactionEntity entity) {
            if (c.gameObject.tag == "Player") {
                return FactionManager.CheckCache(Faction, c.transform, out entity);
            } else if (c.gameObject.tag == "Enemy") {
                // Ensures it's not getting any enemies of the same faction by flipping the bit
                Faction faction = this.Faction ^ Faction.AllEnemies;
                // Gets entity from cache
                return FactionManager.CheckCache(faction, c.transform, out entity, false);
            }
            entity = null; // no entity was found
            return false;
        }

        /// <summary>
        /// Checks if the entity is in the Field of View
        /// and if we have direct line of sight to the entity
        /// </summary>
        /// <param name="c">Collider of a hit entity</param>
        /// <param name="entity"></param>
        private void EntityIsVisible(Collider c, FactionEntity entity) {
            if (InFov(c.transform) && DirectLineOfSight(c.transform)) {
                Target = entity;
                OnTarget?.Invoke(Target);
            }
        }

        private void OnDeath() {
            FactionManager.Unregister(this);
        }
        #endregion
    }
}
