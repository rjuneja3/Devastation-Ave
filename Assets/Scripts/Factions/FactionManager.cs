using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Factions {

    public enum NoiseType { Walking, GunShot }

    /**
     * @author Brenton Hauth
     * @date 11/13/20
     * <summary>
     * Stores, registers, and handles all the entities in the scene.
     * </summary>
     */
    public class FactionManager : MonoBehaviour {
        #region Exposed Variables
        #endregion

        #region Variables
        private Dictionary<Faction, FactionEntities> Entities;
        #endregion

        #region Properties
        public static FactionManager Self { get; private set; }
        #endregion

        #region Methods
        public static event Action<Faction, NoiseType, Vector3> ListenForNoise;

        /**
         * @author Brenton Hauth
         * @date 11/14/20
         * <summary>
         * Awake method called by Unity
         * </summary>
         */
        private void Awake() {
            Self = this; // Sets singleton
            Entities = new Dictionary<Faction, FactionEntities>() {
                { Faction.Player, new FactionEntities(Faction.Player) },
                { Faction.Monster, new FactionEntities(Faction.Monster) },
                { Faction.Gov, new FactionEntities(Faction.Gov) }
            };
            
        }

        // private void Start() { }
        // private void Update() { }

        /**
         * @author Brenton Hauth
         * @date 11/15/20
         * <summary>
         * Registers a entity with the FactionManager
         * </summary>
         * <param name="entity">The entity to register</param>
         */
        public static void Register(FactionEntity entity) {
            if (Self && !entity.IsRegistered) {
                Self.Entities[entity.Faction].Add(entity);
                if (entity.Sensitivity != SoundSensitivity.None) {
                    ListenForNoise += entity.ListenForNoise;
                }
                entity.IsRegistered = true;
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/15/20
         * <summary>
         * Registers a entity with the FactionManager
         * </summary>
         * <param name="entity">The entity to unregister</param>
         */
        public static void Unregister(FactionEntity entity) {
            Self.Entities[entity.Faction].Remove(entity.transform);
            if (entity.Sensitivity != SoundSensitivity.None) {
                ListenForNoise -= entity.ListenForNoise;
            }
            entity.IsRegistered = false;
        }
        
        /**
         * @author Brenton Hauth
         * @date 11/25/20
         * <summary>
         * Produces a noise and broadcasts it to all etities listening for noise
         * </summary>
         * <param name="faction">The faction that produced the noise</param>
         * <param name="noise">The type of noise that was produced</param>
         * <param name="position">The position of the noise</param>
         */
        public static void ProduceNoise(Faction faction, NoiseType noise, Vector3 position) {
            ListenForNoise?.Invoke(faction, noise, position);
        }

        /**
         * @author Brenton Hauth
         * @date 11/15/20
         * <summary>
         * Checks the stored transforms to see if there is an entity associated
         * </summary>
         * <param name="check">
         * The faction(s) to check. Can combine factions
         * to check multiple, E.g. (Player | Monster)
         * </param>
         * <param name="object">The transform to check.</param>
         * <param name="entity">The entity to (if it exists)</param>
         * <param name="add">
         * If an entity was not found,
         * choose to add it to the faction
         * (if GetComponent yields a FactionEntity)
         * </param>
         * <returns>If the transform was found in a checked faction</returns>
         */
        public static bool CheckCache(Faction check, Transform @object, out FactionEntity entity, bool add=false) {
            if (Self) {
                foreach (var f in Self.Entities) {
                    if ((f.Key & check) != 0) {
                        if (f.Value.CheckCache(@object, out entity, add)) {
                            return true;
                        }
                    }
                }
            }
            entity = null;
            return false;
        }

        /**
         * @author Brenton Hauth
         * @date 11/19/20
         * <summary>
         * Primarily used for Testing, gets the entity associated.
         * </summary>
         * <param name="faction">The faction to retrieve from. (must be single)</param>
         * <param name="key">The key to get</param>
         */
        public static FactionEntity RawGet(Faction faction, Transform key) {
            if (Self.Entities[faction].TryGetValue(key, out var value)) {
                print($"TRY GET WORKS: {value}");
                return value;
            }
            return null;
        }
        #endregion

        #region Faction Entities Class
        /**
         * @author Brenton Hauth
         * @date 11/19/20
         * <summary>
         * Internal class to store all entites for a faction.
         * </summary>
         */
        class FactionEntities : IEnumerable<KeyValuePair<Transform, FactionEntity>> {
            public readonly Faction Faction;
            private Dictionary<Transform, FactionEntity> Entities;

            /**
             * @author Brenton Hauth
             * @date 11/17/20
             * <summary>
             * Indexer to quickly get the value associated with the key
             * </summary>
             * <param name="key">The key to check</param>
             */
            public FactionEntity this[Transform key] {
                get => Entities[key];
                set => Entities[key] = value;
            }

            /**
             * @author Brenton Hauth
             * @date 11/17/20
             * <summary>Constructor</summary>
             * <param name="faction">The faction that stores the entities</param>
             */
            public FactionEntities(Faction faction) {
                Faction = faction;
                Entities = new Dictionary<Transform, FactionEntity>();
            }

            /**
             * @author Brenton Hauth
             * @date 11/17/20
             * <summary>
             * Adds an entity to the associated faction.
             * It's better to use Add(FactionEntity)
             * </summary>
             * <param name="key">The transform of the object</param>
             * <param name="entity">The entity object</param>
             * <see cref="Add(FactionEntity)"/>
             */
            public void Add(Transform key, FactionEntity entity) {
                if (entity.Faction != Faction) {
                    Debug.LogWarning(
                        $"Cannot add entity ({entity.name})," +
                        $"of faction {entity.Faction}, to {Faction}.");
                } else {
                    Debug.Log($"Added {entity.name} to {Faction}");
                    Entities.Add(key, entity);
                }

            }

            /**
             * @author Brenton Hauth
             * @date 11/17/20
             * <summary>
             * Adds an entity using its transform as its key 
             * </summary>
             * <param name="e">The entity to add</param>
             */
            public void Add(FactionEntity e) => Add(e.transform, e);

            /**
             * @author Brenton Hauth
             * @date 11/17/20
             * <summary>Removes an entity by it's key</summary>
             * <param name="key">The key to remove</param>
             */
            public void Remove(Transform key) {
                if (Entities.Remove(key)) {
                    Debug.Log($"Removed {key.name} from {Faction}");
                } else {
                    Debug.LogWarning(
                        $"Was NOT able to remove {key.name} from {Faction}." +
                        "It may have already been removed.");
                }
            }

            /**
             * @author Brenton Hauth
             * @date 11/17/20
             * <summary>
             * Checks the saved entites for a faction
             * </summary>
             * <param name="t">The transform to check.</param>
             * <param name="entity">The entity to get (if it exists)</param>
             * <param name="add">
             * If an entity was not found,
             * choose to add it to the faction
             * (if GetComponent yields a FactionEntity)
             * </param>
             */
            public bool CheckCache(Transform t, out FactionEntity entity, bool add=false) {
                if (Entities.TryGetValue(t, out entity)) {
                    // if found entity, but entity is hiden, return null
                    if (entity.Hidden) {
                        entity = null;
                        return false;
                    }
                    return true;
                } else if (add) {
                    // If you choose to add the Entity it will try to get the component
                    entity = t.GetComponent<FactionEntity>();
                    if (entity) {
                        if (entity.Hidden) {
                            entity = null;
                        } else if (entity.Faction == Faction) {
                            Add(t, entity);
                        }
                    }
                    return entity;
                }
                return false;
            }

            /**
             * @author Brenton Hauth
             * @date 11/17/20
             * <summary>
             * Tries to get the value associated with the key
             * </summary>
             * <param name="key">The key to check</param>
             * <param name="value">The value associated with the key</param>
             * <returns>If there is an entity paired with the key</returns>
             */
            public bool TryGetValue(Transform key, out FactionEntity value) => Entities.TryGetValue(key, out value);

            /**
             * @author Brenton Hauth
             * @date 11/17/20
             * <summary>Gets an enumerator for the entities.</summary>
             */
            public IEnumerator<KeyValuePair<Transform, FactionEntity>> GetEnumerator() => Entities.GetEnumerator();

            /**
             * @author Brenton Hauth
             * @date 11/17/20
             * <summary>Gets an enumerator for the entities.</summary>
             */
            IEnumerator IEnumerable.GetEnumerator() => Entities.GetEnumerator();
        }
        #endregion
    }
}
