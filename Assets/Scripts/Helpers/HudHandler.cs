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
    public const string SCORE_PREF = "player_score";
    private static int m_Points = 0;
    #endregion

    #region Properties
    public static HudHandler Self { get; private set; }
    public static int Points {
        get => m_Points;
        set {
            m_Points = value;
            if (m_Points != value) {
                PlayerPrefs.SetInt(SCORE_PREF, m_Points);
            }
            if (Self && Self.PointsText) {
                Self.PointsText.text = m_Points.ToString();
            }
        }
    }
    #endregion

    #region Methods

    private void Awake() {
        Self = this;
    }

    private void Start() {
        GetText("DialogText", ref DialogText);
        GetText("PromptText", ref PromptText);
        GetText("PointsText", ref PointsText);

        Points = PlayerPrefs.GetInt(SCORE_PREF, 0);
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
