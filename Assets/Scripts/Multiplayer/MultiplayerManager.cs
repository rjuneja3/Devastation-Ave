using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


#pragma warning disable CS0618 // Type or member is obsolete
namespace Assets.Scripts.Multiplayer {
    public class MultiplayerManager : NetworkManager {
        void Start() { }
        void Update() { }

        public override void OnClientConnect(NetworkConnection conn) {
            base.OnClientConnect(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn) {
            base.OnClientDisconnect(conn);
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
