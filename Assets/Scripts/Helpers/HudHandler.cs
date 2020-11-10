using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour {
    #region Exposed Variables
    public Text PromptText;
    public Text DialogText;
    #endregion

    #region Properties
    public static HudHandler Self { get; private set; }
    #endregion

    #region Methods

    private void Awake() {
        Self = this;
    }

    private void Start() {
        if (!PromptText) {
            var p = transform.Find("PromptText");
            if (p) PromptText = p.GetComponent<Text>();
        }

        if (!DialogText) {
            var d = transform.Find("DialogText");
            if (d) DialogText = d.GetComponent<Text>();
        }
    }
    
    // private void Update() { }

    public static void Prompt(KeyCode key, string text) {
        if (Self) {
            Self.PromptText.text = $"Press [{key}] {text}";
        }
    }

    public static void ClearPrompt() {
        if (Self) {
            Self.PromptText.text = string.Empty;
        }
    }
    
    #endregion
}
