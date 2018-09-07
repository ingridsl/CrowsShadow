using UnityEngine;
using CrowShadowManager;

namespace CrowShadowMenu
{
    public class PauseMenu : MonoBehaviour
    {
        public PauseMenuOptions option;
        public enum PauseMenuOptions { BackToGame, BackToMainMenu };

        public void OnClick()
        {
            if (option == PauseMenuOptions.BackToGame)
            {
                GameManager.instance.PauseGame(false);
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                GameManager.LoadScene("MainMenu", true);
            }

        }

    }
}