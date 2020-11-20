using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Factions {

    public enum NoiseType { Walking, GunShot }

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

        private void Awake() {
            Self = this;
            Entities = new Dictionary<Faction, FactionEntities>() {
                { Faction.Player, new FactionEntities(Faction.Player) },
                { Faction.Monster, new FactionEntities(Faction.Monster) },
                { Faction.Gov, new FactionEntities(Faction.Gov) }
            };
            
        }

        // private void Start() { }
        // private void Update() { }

        public static void Register(FactionEntity entity) {
            if (Self && !entity.IsRegistered) {
                Self.Entities[entity.Faction].Add(entity);
                if (entity.Sensitivity != SoundSensitivity.None) {
                    ListenForNoise += entity.ListenForNoise;
                }
                entity.IsRegistered = true;
            }
        }

        public static void Unregister(FactionEntity entity) {
            Self.Entities[entity.Faction].Remove(entity.transform);
            if (entity.Sensitivity != SoundSensitivity.None) {
                ListenForNoise -= entity.ListenForNoise;
            }
            entity.IsRegistered = false;
        }
        
        public static void ProduceNoise(Faction faction, NoiseType noise, Vector3 position) {
            ListenForNoise?.Invoke(faction, noise, position);
        }

        public static bool CheckCache(Faction check, Transform @object, out FactionEntity entity, bool add = true) {
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

        public static FactionEntity RawGet(Faction f, Transform key) {
            if (Self.Entities[f].TryGetValue(key, out var value)) {
                print($"TRY GET WORKS: {value}");
                return value;
            }
            return null;
        }
        #endregion

        #region Faction Entities Class
        class FactionEntities : IEnumerable<KeyValuePair<Transform, FactionEntity>> {
            public readonly Faction Faction;
            private Dictionary<Transform, FactionEntity> Entities;

            public FactionEntity this[Transform key] {
                get => Entities[key];
                set => Entities[key] = value;
            }

            public FactionEntities(Faction faction) {
                Faction = faction;
                Entities = new Dictionary<Transform, FactionEntity>();
            }

            public void Add(Transform t, FactionEntity e) {
                print($"Added {t.name} to {Faction} ({t}, {e})");
                Entities.Add(t, e);

            }
            public void Add(FactionEntity e) => Add(e.transform, e);

            public void Remove(Transform t) {
                if (Entities.Remove(t)) {
                    print($"Removed {t.name} ({t}) from {Faction}");
                } else {
                    print($"Was NOT able to remove {t.name} ({t}) from {Faction}");
                }
            }

            public bool CheckCache(Transform t, out FactionEntity entity, bool add = true) {
                if (Entities.TryGetValue(t, out entity)) {
                    if (entity.Hidden) {
                        entity = null;
                        return false;
                    }
                    return true;
                } else if (add) {
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

            public bool TryGetValue(Transform key, out FactionEntity value) => Entities.TryGetValue(key, out value);

            public IEnumerator<KeyValuePair<Transform, FactionEntity>> GetEnumerator() {
                return Entities.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return Entities.GetEnumerator();
            }
        }
        #endregion
    }
}
