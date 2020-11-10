using System;
using Assets.Scripts.Player;
using UnityEngine;
using Assets.Scripts.Helpers;

namespace Assets.Scripts.Weapons {
    [RequireComponent(typeof(AudioSource))]
    public abstract class Weapon : MonoBehaviour {
        #region Exposed Variables
        public float RateOfAttack = .5f;
        public float Range = 100f;
        public float Damage = 10f;
        public bool DebugMode = true;
        #endregion

        #region Variables
        private const float pickupDistance = 1.5f;
        protected bool CanAttack = true;
        protected AudioSource AudioSource;
        protected Transform Player;
        private bool m_IsPrompting = false, m_IsPickedUp = false;
        private WeaponHandler PlayerWeaponHandler;
        #endregion

        #region Properties
        public bool IsPickedUp {
            get => m_IsPickedUp;
            set {
                m_IsPickedUp = value;
                m_IsPrompting = false;
            }
        }
        #endregion

        #region Methods
        public event Action<GameObject> OnHit;

        protected virtual void Start() {
            AudioSource = GetComponent<AudioSource>();
            Utils.FindPlayer(ref Player);
            PlayerWeaponHandler = Player.GetComponent<WeaponHandler>();
        }

        private void SetAttackFlag() => CanAttack = true;

        public void TryAttacking() {
            if (CanAttack) {
                CanAttack = false;
                Invoke("SetAttackFlag", RateOfAttack);
            } else return;

            AudioSource.Play();

            Attack();
        }

        public virtual void Hit(GameObject o) {
            OnHit?.Invoke(o);
            if (o.tag == "Enemy") {
                var health = o.GetComponent<Health>();
                health.CurrentAmount -= Math.Abs(Damage);
                print("Hit enemy!!!");
            }

            print($"Hit: {o.name} [{o.tag}]");
        }

        protected virtual void Update() {
            if (IsPickedUp) return;

            if (CheckPrompt() && Input.GetKeyDown(KeyCode.E)) {
                PlayerWeaponHandler.PickUp(gameObject);
            }
        }

        private bool CheckPrompt() {
            if (Player && VectorHelper.WithinRange(Player.position, transform.position, pickupDistance)) {
                if (!m_IsPrompting) {
                    m_IsPrompting = true;
                    HudHandler.Prompt(KeyCode.E, $"for {name}");
                }
            } else if (m_IsPrompting) {
                m_IsPrompting = false;
                HudHandler.ClearPrompt();
            }
            return m_IsPrompting;
        }

        public abstract void Attack();
        #endregion
    }
}