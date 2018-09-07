using UnityEngine;
using UnityEngine.UI;

namespace CrowShadowMenu
{
    public class ChangeLanguage : MonoBehaviour
    {
        public int language;
        private Button button { get { return GetComponent<Button>(); } }
        PreferencesManager script;

        private void Awake()
        {
            GameObject menuCanvas = GameObject.Find("Canvas").gameObject;
            script = menuCanvas.GetComponent<PreferencesManager>();
        }

        void Start()
        {
            button.onClick.AddListener(() => ChangeTextLanguage());
        }

        void ChangeTextLanguage()
        {
            switch (language)
            {
                case 1:
                    PlayerPrefs.SetString("Language", "EN_US");
                    script.SetTextLanguage();
                    break;
                case 2:
                    PlayerPrefs.SetString("Language", "PT_BR");
                    script.SetTextLanguage();
                    break;
                case 3:
                    PlayerPrefs.SetString("Language", "JP");
                    script.SetTextLanguage();
                    break;
                case 4:
                    PlayerPrefs.SetString("Language", "FR");
                    script.SetTextLanguage();
                    break;
            }
        }

    }
}
