using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TeamBobFPS
{
    public class HighscoreUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text[] highscoreTexts;

        public void Activate(int levelIndex)
        {
            switch (levelIndex)
            {
                case 2:
                    highscoreTexts[0].text = "Pistol: " + GameInstance.Instance.GetGameProgressionManager().VillageEndlessModeHighScores["Pistol"];
                    highscoreTexts[1].text = "Shotgun: " + GameInstance.Instance.GetGameProgressionManager().VillageEndlessModeHighScores["Shotgun"];
                    highscoreTexts[2].text = "Minigun: " + GameInstance.Instance.GetGameProgressionManager().VillageEndlessModeHighScores["Minigun"];
                    highscoreTexts[3].text = "Railgun: " + GameInstance.Instance.GetGameProgressionManager().VillageEndlessModeHighScores["Railgun"];
                    break;
                case 3:
                    highscoreTexts[0].text = "Pistol: " + GameInstance.Instance.GetGameProgressionManager().TempleEndlessModeHighScores["Pistol"];
                    highscoreTexts[1].text = "Shotgun: " + GameInstance.Instance.GetGameProgressionManager().TempleEndlessModeHighScores["Shotgun"];
                    highscoreTexts[2].text = "Minigun: " + GameInstance.Instance.GetGameProgressionManager().TempleEndlessModeHighScores["Minigun"];
                    highscoreTexts[3].text = "Railgun: " + GameInstance.Instance.GetGameProgressionManager().TempleEndlessModeHighScores["Railgun"];
                    break;
            }
        }
    }
}
