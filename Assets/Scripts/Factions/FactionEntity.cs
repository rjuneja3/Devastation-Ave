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
        All = Player | AllEnemies,
    }

    public enum FactionEntityTarget { AlwaysPlayer, Strongest, Nearest }

    public enum SoundSensitivity {
        None = 0,
        Low = 1,
        Normal = 2,
        High = 3,
    }
    #endregion

    [RequireComponent(typeof(Health))]
    public class FactionEntity : MonoBehaviour {
        public const int ENTITY_LAYER_INDEX = 8;
        public const int ENTITY_LAYER = 1 << ENTITY_LAYER_INDEX; // The layer that all entites reside on
        public const int LANDSCAPE_LAYER = 1 << 9; // The layer that terrain & buildings reside on

        // Combines Landscape and entity layers
        public const int LANDSCAPE_AND_ENTITIES = ENTITY_LAYER | LANDSCAPE_LAYER;
        
        #region Exposed Variables
        public Faction Faction = Faction.Monster;
        public FactionEntityTarget TargetType = FactionEntityTarget.Nearest;
        public SoundSensitivity Sensitivity = SoundSensitivity.Normal;
        public bool IsControlled = false;
        public float FovAngle = 60f;
        public float DetectionRange = 50f;
        public bool HasEyes = true;
        public bool Hidden = false;
        public Transform Head;
        #endregion

        #region Variables
        private Collider[] Results = new Collider[10]; // 10 may be too small
        private bool m_IsRegistered = false;
        #endregion

        #region Properties
        public bool IsRegistered {
            get => m_IsRegistered;
            set {
                m_IsRegistered = value;
                if (value) OnRegistered?.Invoke();
                else OnUnregistered?.Invoke();
            }
        }
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
        public event Action<Vector3, float> OnNoise;
        public event Action OnRegistered;
        public event Action OnUnregistered;
        
        private void Start() {
            Health = GetComponent<Health>();

            Health.OnDeath += OnDeath;

            FactionManager.Register(this);

            if (gameObject.layer != ENTITY_LAYER_INDEX) {
                Debug.LogWarning($"{this} is not on the 'Entity' layer.");
            }
        }

        public void ListenForNoise(Faction faction, NoiseType noise, Vector3 position) {
            if (faction == Faction) return;

            // TODO: tweek formula
            int s = 2 << (int)Sensitivity;
            s += ((int)noise * 20);

            if (VectorHelper.WithinRange(Position, position, s)) {
                OnNoise?.Invoke(position, 1f);
            }
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
            Vector3 to = @object.position + Vector3.up; // TODO: make sure ray hits y-center of object
            Ray ray = new Ray(Head.position, to - Head.position); // creates a ray from the entity's head

            // Casts a Sphere towards the entity and checks if it hit
            return Physics.SphereCast(ray, .25f, out var hit, DetectionRange, LANDSCAPE_AND_ENTITIES)
                ? hit.transform == @object
                : false;
        }

        public bool DirectLineOfSight(FactionEntity entity) => DirectLineOfSight(entity.transform);

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
                    if (HasEyes) {
                        EntityIsVisible(Results[i], entity);
                    } else {
                        SetTarget(entity);
                    }

                    // no need to continue; we have a target
                    if (HasTarget) break;
                }
            }
            
        }

        private bool GetAssociatedEntity(Collider c, out FactionEntity entity) {
            if (c.gameObject.tag == "Player") {
                return FactionManager.CheckCache(Faction.Player, c.transform, out entity, false);
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
                SetTarget(entity);
            }
        }

        public void SetTarget(FactionEntity entity) {
            if (HasTarget) {
                Target.Health.OnDeath -= OnTargetDeath;
            }
            Target = entity;
            if (entity && entity.Health) entity.Health.OnDeath += OnTargetDeath;
            OnTarget?.Invoke(Target);
            print($"{this} set target {Target}");
        }

        private void OnTargetDeath() {
            if (!Target.Health.IsDead) return;
            print("Target is Dead");
            SetTarget(null);
        }

        private void OnDeath() {
            if (HasTarget) {
                Target.Health.OnDeath -= OnTargetDeath;
            }
            FactionManager.Unregister(this);
        }

        public override string ToString() {
            return $"[FACTION ENTITY: {name} ({Faction})]";
        }
        #endregion
    }
}
