using System;
using Assets.Scripts.Player;
using UnityEngine;
using Assets.Scripts.Helpers;

namespace Assets.Scripts.Weapons {
    /**
     * @author Brenton Hauth
     * @date 10/28/20
     * <summary>
     * Generic Weapon class to be extended by <c>Melee</c> & <c>Firearm</c>
     * </summary>
     * <see cref="Firearm"/>
     * <see cref="Melee"/>
     */
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

        protected AudioSource AudioSource;
        protected static Transform Player;
        private bool m_IsPrompting = false, m_IsPickedUp = false;
        #endregion

        #region Properties
        public static WeaponHandler PlayerWeaponHandler { get; set; }
        protected virtual bool CanAttack { get; set; } = true;

        public WeaponHandler CurrentHandler { get; private set; }
        public bool IsPickedUp {
            get => CurrentHandler;
            set {
                m_IsPickedUp = value;
                m_IsPrompting = false;
            }
        }
        #endregion

        #region Methods
        public event Action<GameObject> OnHit; // Most likely will remove
        
        /**
         * @author Brenton Hauth
         * @date 10/20/20
         * <summary>Start method called by Unity</summary>
         */
        protected virtual void Start() {
            AudioSource = GetComponent<AudioSource>();
            if (Utils.FindPlayer(ref Player)) {
                PlayerWeaponHandler = Player.GetComponent<WeaponHandler>();
            }
        }

        public static void SetPlayer(Transform player) {
            if (!player) return;
            Player = player;
            PlayerWeaponHandler = Player.GetComponent<WeaponHandler>();
            Debug.Log($"SET PLAYER {Player}, {PlayerWeaponHandler}");
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Attaches the weapon to the handler's right hand. If the handler
         * is null (or doesn't have a right hand) then it sets the parent to null.
         * </summary>
         * <param name="handler">The handler to attach the weapon. Pass 'null' to unequip</param>
         */
        public virtual void AttachToHandler(WeaponHandler handler) {
            transform.SetParent(handler?.RightHand);
            if (transform.parent) CurrentHandler = handler;
            SetAttackFlag();
        }

        /**
         * @author Brenton Hauth
         * @date 10/23/20
         * <summary>
         * Sets the <c>CanAttack</c> flag to true.
         * Is called by the <c>Invoke</c> method.
         * </summary>
         */
        private void SetAttackFlag() => CanAttack = true;

        /**
         * @author Brenton Hauth
         * @date 11/08/20
         * <summary>Attempts to attack with the weapon</summary>
         */
        public void TryAttacking() {
            if (CanAttack) {
                CanAttack = false;
                Invoke("SetAttackFlag", RateOfAttack);
            } else return;


            PlaySound();

            Attack();
        }

        /**
         * @author Brenton Hauth
         * @date 11/18/20
         * <summary>Called when the weapon (or bullet) hit something</summary>
         * <param name="o">The object hit by the weapon/bullet</param>
         */
        public virtual void Hit(GameObject o) {
            CurrentHandler?.OnHit(o);
            OnHit?.Invoke(o);
        }

        public void PlaySound() {
            AudioSource?.Play();
        }

        /**
         * @author Brenton Hauth
         * @date 10/20/20
         * <summary>Update method called by Unity</summary>
         */
        protected virtual void Update() {
            if (IsPickedUp) return;

            if (CheckPrompt() && Input.GetKeyDown(KeyCode.E)) {
                PlayerWeaponHandler.Equip(gameObject);
            }
        }

        /**
         * @author Brenton Hauth
         * @date 11/07/20
         * <summary>
         * Checks if the prompt is active when the Player is close
         * </summary>
         */
        private bool CheckPrompt() {
            // TODO: needs to be rewritten or removed
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

        /**
         * @author Brenton Hauth
         * @date 10/31/20
         * <summary>Used to handle the weapon "attacking" or "shooting"</summary>
         */
        public abstract void Attack();
        #endregion
    }
}