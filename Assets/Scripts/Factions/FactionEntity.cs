using Assets.Scripts.General;
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

    /**
     * @author Brenton Hauth
     * @date 11/13/20
     * <summary>
     * Represents an entity in the scene. Provides numerous functions
     * to ensure the entity acts like a living creature.
     * </summary>
     */
    [RequireComponent(typeof(Health))]
    public class FactionEntity : MonoBehaviour {
        #region Layer Consts
        public const int ENTITY_LAYER_INDEX = 8;
        public const int ENTITY_LAYER = 1 << ENTITY_LAYER_INDEX; // The layer that all entites reside on
        public const int LANDSCAPE_LAYER = 1 << 9; // The layer that terrain & buildings reside on

        // Combines Landscape and entity layers
        public const int LANDSCAPE_AND_ENTITIES = ENTITY_LAYER | LANDSCAPE_LAYER;
        #endregion

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
        private Collider[] Results = new Collider[10]; // 10 may be too small, used for NonAloc
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
        /**
         * <summary>
         * Called when the entity aquires a new target
         * </summary>
         */
        public event Action<FactionEntity> OnTarget;
        public event Action<Vector3, float> OnNoise;
        public event Action OnRegistered;
        public event Action OnUnregistered;

        /**
         * @author Brenton Hauth
         * @date 11/13/20
         * <summary>Start method called by Unity.</summary>
         */
        private void Start() {
            Health = GetComponent<Health>();

            Health.OnDeath += OnDeath;

            FactionManager.Register(this);

            if (gameObject.layer != ENTITY_LAYER_INDEX) {
                Debug.LogWarning(
                    $"{this} is not on the 'Entity' layer. " +
                    "It will not be seen by other entities.");
            }
        }


        /**
         * @author Brenton Hauth
         * @date 11/24/20
         * <summary>Called when the entity hears a noise</summary>
         * <param name="faction">The faction that produced the noise</param>
         * <param name="noise">The type of noise produced</param>
         * <param name="position">The position of the noise</param>
         */
        public void ListenForNoise(Faction faction, NoiseType noise, Vector3 position) {
            if (faction == Faction) return;

            // TODO: tweek formula
            int s = 2 << (int)Sensitivity;
            s += ((int)noise * 20);

            if (VectorHelper.WithinRange(Position, position, s)) {
                OnNoise?.Invoke(position, 1f);
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/13/20
         * <summary>Update method called by Unity.</summary>
         */
        private void Update() {
            // Returns if this entity is controlled by
            // a player or if it already has a target
            if (IsControlled || HasTarget) return;
            Sweep(); // Sweeps the area
        }

        /**
         * @author Brenton Hauth
         * @date 11/13/20
         * <summary>
         * Called by Unity when Entity is Enabled
         * </summary>
         */
        private void OnEnable() {
            FactionManager.Register(this);
        }

        /**
         * @author Brenton Hauth
         * @date 11/14/20
         * @author Brenton Hauth
         * @date 11/13/20
         * <summary>
         * Called by Unity when Entity is Disabled
         * </summary>
         */
        private void OnDisable() {
            FactionManager.Unregister(this);
        }
        
        /**
         * @author Brenton Hauth
         * @date 11/14/20
         * <summary>
         * Checks if Entity is within a certain range of an object
         * </summary>
         * <param name="object">The object to check</param>
         * <returns>If the object is in range</returns>
         */
        public bool InRange(Transform @object) {
            return VectorHelper.WithinRange(
                transform.position, @object.position,
                DetectionRange * GameSettings.DetectionMultiplier);
        }

        /**
         * @author Brenton Hauth
         * @date 11/14/20
         * <summary>
         * Checks if an object is in the entities Field of View
         * </summary>
         * <param name="object">The object to check</param>
         * <returns>if the object is in the FOV</returns>
         */
        public bool InFov(Transform @object) {
            // Direction isn't normalized to save (small amount) resources
            Vector3 direction = @object.position - transform.position;
            // Checks if the calculated angle is smaller than it's FOV
            return Vector3.Angle(transform.forward, direction) <= FovAngle;
        }

        /**
         * @author Brenton Hauth
         * @date 11/14/20
         * <summary>
         * Checks if the entity has direct line of sight to the object
         * (not interrupted by an a wall)
         * </summary>
         * <param name="object">The object to check</param>
         * <returns>If the entity has a direct line of site</returns>
         */
        public bool DirectLineOfSight(Transform @object) {
            Vector3 to = @object.position + Vector3.up; // TODO: make sure ray hits y-center of object
            Ray ray = new Ray(Head.position, to - Head.position); // creates a ray from the entity's head
            float range = DetectionRange * GameSettings.DetectionMultiplier;

            // Casts a Sphere towards the entity and checks if it hit
            // May be better to use raycast
            return Physics.SphereCast(ray, .25f, out var hit, range, LANDSCAPE_AND_ENTITIES)
                ? hit.transform == @object // Ensures that hit transform is the same as object
                : false;
        }

        /**
         * @author Brenton Hauth
         * @date 11/15/20
         * <summary>
         * Checks if entity has direct line of sight to an entity
         * (not interrupted by an a wall)
         * </summary>
         * <param name="entity">The entity to check</param>
         * <returns>If the entity has a direct line of site</returns>
         */
        public bool DirectLineOfSight(FactionEntity entity) => DirectLineOfSight(entity.transform);

        /**
         * @author Brenton Hauth
         * @date 11/16/20
         * <summary>
         * Sweeps through entities and checks which ones are in range
         * TODO: Try optimizing method a bit more
         * </summary>
         */
        public void Sweep() {
            // Checks to see if there are any entities in the 'Detection Range'
            // Uses non-aloc with a result capacity of 10
            int n = Physics.OverlapSphereNonAlloc(
                position: Position,
                radius: DetectionRange * GameSettings.DetectionMultiplier,
                results: Results,
                layerMask: ENTITY_LAYER);

            for (int i = 0; i < n; i++) {
                // Gets associated entity with the collider
                var result = Results[i];
                if (result.transform == transform) continue;
                if (GetAssociatedEntity(result, out var entity)) {
                    // Checks if the entity is visible, ands updates the target
                    if (HasEyes) {
                        EntityIsVisible(result, entity);
                    } else {
                        // No need to check if entity is within range again
                        // just sets target to the entity.
                        SetTarget(entity);
                    }
                    // no need to continue; we have a target
                    if (HasTarget) break;
                }
            }
            
        }

        /**
         * @author Brenton Hauth
         * @date 11/15/20
         * <summary>
         * Gets an entity Attached to the collider. 'GetComponent' is very costly, so
         * it achieves this by checking the cached entities in the Faction Manager.
         * </summary>
         * <param name="collider">The collider to check</param>
         * <param name="entity">The entity found attached</param>
         * <returns>If there is an entity attached to the collider</returns>
         */
        private bool GetAssociatedEntity(Collider collider, out FactionEntity entity) {
            if (collider.gameObject.tag == "Player") {
                return FactionManager.CheckCache(Faction.Player, collider.transform, out entity, false);
            } else if (collider.gameObject.tag == "Enemy") {
                // Ensures it's not getting any enemies of the same faction by flipping the bit
                Faction faction = this.Faction ^ Faction.AllEnemies;
                // Gets entity from cache
                return FactionManager.CheckCache(faction, collider.transform, out entity, false);
            }
            entity = null; // no entity was found
            return false;
        }

        /**
         * @author Brenton Hauth
         * @date 11/15/20
         * <summary>
         * Checks if the entity is in the Field of View
         * and if we have direct line of sight to the entity
         * </summary>
         * <param name="c">Collider of a hit entity</param>
         * <param name="entity"></param>
         */
        private void EntityIsVisible(Collider c, FactionEntity entity) {
            if (InFov(c.transform) && DirectLineOfSight(c.transform)) {
                SetTarget(entity);
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/15/20
         * <summary>Sets the target for the entity.</summary>
         * <param name="entity">The target</param>
         */
        public void SetTarget(FactionEntity entity) {
            if (HasTarget) {
                // Removes OnTargetDeath from previous target
                Target.Health.OnDeath -= OnTargetDeath;
            }
            Target = entity;
            if (entity && entity.Health) entity.Health.OnDeath += OnTargetDeath;
            OnTarget?.Invoke(Target);
            Debug.Log($"{this} is now targeting {Target}");
        }

        /**
         * @author Brenton Hauth
         * @date 11/19/20
         * <summary>
         * Called when the entity's target health reaches 0
         * </summary>
         */
        private void OnTargetDeath() {
            if (!Target.Health.IsDead) return;
            print("Target is Dead");
            SetTarget(null);
        }

        /**
         * @author Brenton Hauth
         * @date 11/19/20
         * <summary>
         * Called when the entity's health reaches 0
         * </summary>
         */
        private void OnDeath() {
            if (HasTarget) {
                // Change to SetTarget(null) and test
                Target.Health.OnDeath -= OnTargetDeath;
            }
            FactionManager.Unregister(this);
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>Override of ToString</summary>
         */
        public override string ToString() {
            return $"[ENTITY: {name} ({Faction})]";
        }
        #endregion
    }
}
