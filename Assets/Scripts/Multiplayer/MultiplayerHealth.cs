using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable CS0618 // Type or member is obsolete
namespace Assets.Scripts.Multiplayer {
    public class MultiplayerHealth : NetworkBehaviour {

        [SyncVar(hook ="OnHealthChange")]
        public float CurrentHP = 100f;

        public static MultiplayerHealth Local { get; private set; }

        public event Action OnDeath;

        private void Start() {
        }

        public override void OnStartLocalPlayer() {
            Local = this;
        }

        private void Update() { }

        public void OnHealthChange(float hp) {
            Debug.Log($"OnHealthChange() -> {hp}");
        }

        public void TakeDamage(float damage) {
            var take = CurrentHP - damage;
            CurrentHP = Mathf.Max(take, 0f);

            var value = GetComponent<NetworkIdentity>().netId.Value;
            Debug.Log($"{value} Take damage -> {CurrentHP}");
            if (CurrentHP == 0f) {
                OnDeath?.Invoke();
            }
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
