using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Factions;
using Assets.Imported.Standard_Assets.Characters.FirstPersonCharacter.Scripts;
using ASP = Assets.Scripts.Player;

#pragma warning disable CS0618 // Type or member is obsolete
namespace Assets.Scripts.Multiplayer {
    [RequireComponent(typeof(MultiplayerHealth))]
    [RequireComponent(typeof(NetworkTransform), typeof(NetworkIdentity))]
    public class MultiplayerController : NetworkBehaviour {

        #region Exposed Variables
        public Transform Spine;
        public bool RotateSpine = false;
        #endregion

        #region Variables
        private Transform cameraPos;
        private Animator Animator;
        private static GameObject[] SpawnPoints = { };
        private float xAxis = 0f;
        #endregion

        #region Properties
        public float HorizontalAxis => Input.GetAxis("Horizontal");
        public float VerticalAxis => Input.GetAxis("Vertical");
        public MultiplayerHealth Health { get; private set; }
        public static MultiplayerController Local { get; private set; }
        #endregion

        #region Methods
        private void Start() {
            Health = GetComponent<MultiplayerHealth>();
            Animator = GetComponent<Animator>();
            //var NAnimator = GetComponent<NetworkAnimator>();
            //if (NAnimator) NAnimator.animator = Animator;
        }

        public override void OnStartLocalPlayer() {
            Local = this;
            Weapons.Weapon.SetPlayer(transform);
            cameraPos = transform.Find("CameraPosition");
            //Camera.main.transform.SetParent(cameraPos);
            //Camera.main.transform.SetParent(transform);
            //Camera.main.transform.localPosition = Vector3.zero;
            //Camera.main.transform.localRotation = cameraPos.localRotation;
            var camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.transform.SetParent(cameraPos);
            camera.transform.localPosition = Vector3.zero;
            //camera.transform.localRotation = Quaternion.identity;
            if (!Health) Health = GetComponent<MultiplayerHealth>();
            Health.OnDeath += OnDeath;

            SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            // Invoke("SpawnRandomly", 0);

            //SpawnRandomly();

            //var handler = GetComponent<MultiplayerWeaponHandler>();
            //if (handler.Weapon) handler.OutsideEquip(handler.Weapon);

            var fpc = GetComponent<FirstPersonController>();
            fpc.Start();
        }

        private void Update() {
            if (!isLocalPlayer) return;
            Move();
            //if (Input.GetKeyDown(KeyCode.T)) {
            //    SpawnRandomly();
            //}
            //RotateSpine();
        }

        private void Fire() {
            StartCoroutine("DelayFire");
        }

        private IEnumerator DelayFire() {
            Ray ray = new Ray(
                Camera.main.transform.position,
                Camera.main.transform.forward);

            yield return new WaitForSeconds(4f / 60f);
            if (Physics.Raycast(ray, out var hit, /*weapon range*/100f, FactionEntity.ENTITY_LAYER)) {
                var identity = hit.transform.GetComponent<NetworkIdentity>();
                CmdHit(identity.netId, 10f);
            }
        }

        [Command]
        public void CmdHit(NetworkInstanceId id, float damage) {
            var obj = NetworkServer.FindLocalObject(id);
            if (!obj) return;
            var health = obj.GetComponent<MultiplayerHealth>();
            health.TakeDamage(damage);
            // .BroadcastMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }

        [Command]
        public void CmdPickUp(NetworkInstanceId playerId, NetworkInstanceId weaponId) {
            if (playerId == netId) {
                Debug.Log($"Player ID matched. returning out.");
                return;
            }
            var player = NetworkServer.FindLocalObject(playerId);
            var handler = player.GetComponent<MultiplayerWeaponHandler>();
            
            if (handler) {
                var weapon = NetworkServer.FindLocalObject(weaponId);
                if (handler.OutsideEquip(weapon)) {
                    Debug.Log("Successfully equiped");
                } else Debug.LogWarning("Did not equip!");
            } else {
                Debug.LogError($"No Handler on {player.name}");
            }
        }

        [Command]
        public void CmdSendMessageToServer(NetworkInstanceId id, string message) {
            NetworkServer.FindLocalObject(id).BroadcastMessage(message);
        }
        
        private void OnDestroy() {
            if (isLocalPlayer) {
                Camera.main?.transform?.SetParent(null);
            }
        }

        private void UpdateSpineRotation() {
            if (!RotateSpine) return;
            //Spine.Rotate()
            const int min = -50, max = 400;
            //Spine.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Spine.up);
            //yAxis += Input.GetAxis("Mouse X");
            //yAxis = Mathf.Clamp(yAxis, -40, 40);
            xAxis -= Input.GetAxis("Mouse Y");// * 1f;
            //if (Input.Get)
            xAxis = Mathf.Clamp(xAxis, min, max);
            if (xAxis == min || xAxis == max) return;
            Spine.Rotate(0, 0, xAxis);
            //Spine.localEulerAngles = newSpineRotation;
            //Debug.Log($"Rotate Spine: {newSpineRotation}");
        }

        private void LateUpdate() {
            if (!isLocalPlayer) return;

            UpdateSpineRotation();
        }


        private void Move() {
            if (Input.GetKey(KeyCode.Space)) {
                Animator.SetTrigger("Jump");
            }

            const float speedLimit = 1;

            float h = HorizontalAxis,
                v = VerticalAxis;

            float xSpeed = h * speedLimit;
            float zSpeed = v * speedLimit;
            if (Input.GetKey(KeyCode.LeftShift)) {
                v *= 2f;
                h *= 2f;
            }

            Animator.SetFloat("XSpeed", h);//, .25f, Time.deltaTime);
            Animator.SetFloat("ZSpeed", v);//, .25f, Time.deltaTime);

            //Animator.SetFloat("Speed", 1);//, .25f, Time.deltaTime);

            if (v != 0 || h != 0) {
                FactionManager.ProduceNoise(Faction.Player, NoiseType.Walking, transform.position);
            }
        }

        private void OnDeath() {
            if (!isLocalPlayer) return;
            Debug.Log("ON Death " + netId.Value);
            Health.TakeDamage(-100); // Restores player
            Respawn(this);
        }

        private void SpawnRandomly() {
            if (SpawnPoints != null && SpawnPoints.Length > 0) {
                var old = transform.position;
                int p = Random.Range(0, SpawnPoints.Length - 1);
                Vector3 point = SpawnPoints[p].transform.position;
                point.y = transform.position.y + .5f;
                transform.position = point;

                Debug.Log($"Spawn: {old} ; {transform.position}");
            }
        }
        
        public static void Respawn(MultiplayerController mc) {
            var conn = mc.connectionToClient;
            var newPlayer = Instantiate(mc.gameObject);
            Destroy(mc.gameObject);
            NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);
        }

        #endregion
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
