using Assets.Scripts.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu {
    public class DifficultyChanger : MonoBehaviour {

        #region Exposed Variables
        public Button
            EasyButton,
            NormalButton,
            HardButton;

        public Toggle ImmortalToggle;
        #endregion

        #region Variables
        private Dictionary<Difficulty, Image> ButtonImages;
        #endregion

        #region Methods
        private void Start() {
            ButtonImages = new Dictionary<Difficulty, Image>();

            SetUpButton(Difficulty.Easy, ref EasyButton);
            SetUpButton(Difficulty.Normal, ref NormalButton);
            SetUpButton(Difficulty.Hard, ref HardButton);

            UpdateButtons(GameSettings.Difficulty);

            if (!ImmortalToggle) {
                ImmortalToggle = transform.Find(nameof(ImmortalToggle))?.GetComponent<Toggle>();
            }

            ImmortalToggle.isOn = GameSettings.MakePlayerImmortal;
        }

        private void SetUpButton(Difficulty diff, ref Button button) {
            if (!button) {
                var s = diff.ToString("G");
                var b = transform.Find(s)?.GetComponent<Button>();
                if (!b) {
                    Debug.LogError($"Could not find button for difficulty: {s}");
                    return;
                }
                button = b;
            }
            var image = button.GetComponent<Image>();

            if (image) {
                ButtonImages[diff] = image;
            } else {
                Debug.LogError($"Button for {diff} did not have an image!");
            }

        }

        private void UpdateButtons(Difficulty diff) {
            foreach (var b in ButtonImages) {
                var color = b.Value.color;
                color.a = (b.Key == diff) ? 1f : .5f;
                b.Value.color = color;
            }
        }


        private void OnDifficultyChange(Difficulty diff) {
            if (GameSettings.Difficulty == diff) return;

            GameSettings.UpdateDifficulty(diff);
            UpdateButtons(diff);
        }

        public void OnEasyClick() => OnDifficultyChange(Difficulty.Easy);
        public void OnNormalClick() => OnDifficultyChange(Difficulty.Normal);
        public void OnHardClick() => OnDifficultyChange(Difficulty.Hard);

        public void OnImmortalChecked() {
            if (!ImmortalToggle) return;
            GameSettings.MakePlayerImmortal = ImmortalToggle.isOn;
        }
        #endregion
    }
}
