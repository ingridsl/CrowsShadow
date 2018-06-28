using UnityEngine;
using CrowShadowManager;

namespace CrowShadowMenu
{
    public class NewGame : MonoBehaviour
    {

        public void OnClick()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("Mission", -1);
            GameManager.LoadScene("QuartoKid", true);
        }

    }
}