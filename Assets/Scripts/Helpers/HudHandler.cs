using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour {
    #region Exposed Variables
    public Text PromptText;
    public Text DialogText;
    public Text PointsText;
    #endregion

    #region Variables
    private static int m_Points = 0;
    #endregion
    #region Properties
    public static HudHandler Self { get; private set; }
    public static int Points {
        get => m_Points;
        set {
            m_Points = value;
            if (Self && Self.PointsText) {
                Self.PointsText.text = $"{m_Points}";
            }
            PlayerPrefs.SetInt("Hiscore", m_Points);
        }
    }
    #endregion

    #region Methods

    private void Awake() {
        Self = this;
    }



    private void Start() {
        GetText("DialogText", ref DialogText);
        GetText("DialogText", ref DialogText);
        GetText("PointsText", ref PointsText);
    }

    private void GetText(string n, ref Text field) {
        if (!field) {
            var t = transform.Find(n);
            if (t) field = t.GetComponent<Text>();
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
