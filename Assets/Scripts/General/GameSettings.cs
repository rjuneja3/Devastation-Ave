using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.General {
    #region Difficulty Enum
    /**
     * @author Brenton Hauth
     * @date 30/11/20
     * <summary>
     * An enum to hold the the different levels of difficulty
     * </summary>
     */
    public enum Difficulty {
        Easy = 1,
        Normal = 2,
        Hard = 3
    }
    #endregion

    /**
     * @author Brenton Hauth
     * @date 30/11/20
     * <summary>
     * Stores settings of the game
     * </summary>
     */
    public static class GameSettings {
        #region Variables
        private static Difficulty m_Difficulty = Difficulty.Normal;
        #endregion

        #region Properties
        public static Difficulty Difficulty {
            get => m_Difficulty;
            set => UpdateDifficulty(value);
        }
        public static float EnemyDamageMultiplier { get; private set; } = 1f;
        public static float DetectionMultiplier { get; private set; } = 1f;
        #endregion

        #region Methods
        /**
         * @author Brenton Hauth
         * @date 30/11/20
         * <summary>
         * Updates the difficulty of the game
         * </summary>
         * <param name="difficulty">Must be either Easy, Normal, or Hard</param>
         */
        public static void UpdateDifficulty(Difficulty difficulty) {
            if (difficulty < Difficulty.Easy || difficulty > Difficulty.Hard) {
                Debug.LogWarning("Unable to update difficulty.");
                return;
            }
            
            EnemyDamageMultiplier = 1 + (.5f * ((float)Difficulty - 2)); // Easy=0.5, Normal=1, Hard=1.5
            DetectionMultiplier = .5f + (.25f * (float)Difficulty); // Easy=0.75, Normal=1, Hard=1.25

            m_Difficulty = difficulty;
        }
        #endregion
    }
}

