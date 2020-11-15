using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Factions {
    public class FactionManager : MonoBehaviour {
        #region Exposed Variables
        #endregion

        #region Variables
        private Dictionary<Faction, FactionEntities> Entities;
        private List<FactionEntity> ScheduledForSearch;
        #endregion

        #region Properties
        public static FactionManager Self { get; private set; }
        #endregion

        #region Methods

        private void Awake() {
            Self = this;
            Entities = new Dictionary<Faction, FactionEntities>() {
                { Faction.Player, new FactionEntities() },
                { Faction.Monster, new FactionEntities() },
                { Faction.Gov, new FactionEntities() }
            };

            ScheduledForSearch = new List<FactionEntity>();
        }

        private void Start() { }

        private void Update() { }

        public static void Register(FactionEntity entity) {
            if (!Self) return;
            if (Self && !entity.IsRegistered) {
                entity.IsRegistered = true;
                Self.Entities[entity.Faction].Add(entity);
            }
        }

        public static void Unregister(FactionEntity entity) {
            if (!Self) return;
            entity.IsRegistered = false;
            Self.Entities[entity.Faction].Remove(entity.transform);
        }

        public static void ScheduleForSearch(FactionEntity entity) {

        }


        public static bool CheckCache(Faction check, Transform @object, out FactionEntity entity, bool add=true) {
            if (!Self) {
                entity = null;
                return false;
            }
            foreach (var f in Self.Entities) {
                if ((f.Key & check) != 0) {
                    return f.Value.CheckCache(@object, out entity, add);
                }
            }
            entity = null;
            return false;
        }


        #endregion

        #region Faction Entities Class
        class FactionEntities : IEnumerable<KeyValuePair<Transform, FactionEntity>> {
            private Dictionary<Transform, FactionEntity> Entities;

            public FactionEntity this[Transform key] {
                get => Entities[key];
                set => Entities[key] = value;
            }
            
            public FactionEntities() {
                Entities = new Dictionary<Transform, FactionEntity>();
            }

            public void Add(Transform t, FactionEntity e) => Entities.Add(t, e);
            public void Add(FactionEntity e) => Add(e.transform, e);

            public void Remove(Transform t) => Entities.Remove(t);

            public bool CheckCache(Transform @object, out FactionEntity entity, bool add=true) {
                if (Entities.TryGetValue(@object, out entity)) {
                    return true;
                } else if (add) {
                    entity = @object.GetComponent<FactionEntity>();
                    if (entity) Add(@object, entity);
                    return entity;
                }
                return false;
            }

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
