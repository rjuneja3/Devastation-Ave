using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu {
    public class MenuHandler : MonoBehaviour {

        public void OnMultiplayerClick() {
            SceneManager.LoadScene("MultiplayerLevel");
        }
    }
}
