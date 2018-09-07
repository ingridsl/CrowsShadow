using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using RPGTALK.Localization;
using CrowShadowManager;
using CrowShadowMenu;

namespace CrowShadowPlayer
{
    public class HUDPlayer : MonoBehaviour
    {

        public static HUDPlayer instance;
        public AudioClip sound;

        private AudioSource source { get { return GetComponent<AudioSource>(); } }

        private GameObject pauseMenu;

        public void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
                pauseMenu = transform.Find("PauseMenu").gameObject;
                pauseMenu.SetActive(false);
                SetTextLanguage();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            gameObject.AddComponent<AudioSource>();
            source.clip = sound;
            source.playOnAwake = false;
        }

        void Update()
        {
            if (CrossPlatformInputManager.GetButtonDown("keyPause") && !GameManager.instance.blocked &&
                !GameManager.instance.showMissionStart && !GameManager.instance.pausedObject && !Book.show)
            {
                if (!source.isPlaying)
                    source.PlayOneShot(sound);

                if (pauseMenu.activeSelf)
                {
                    GameManager.instance.PauseGame(false);
                    pauseMenu.SetActive(false);
                }
                else
                {
                    GameManager.instance.PauseGame(true);
                    pauseMenu.SetActive(true);
                }
            }
        }

        public void SetTextLanguage()
        {
            if (!PlayerPrefs.HasKey("Language"))
            {
                PlayerPrefs.SetString("Language", "EN_US");
            }

            switch (PlayerPrefs.GetString("Language"))
            {
                case "EN_US":
                    LanguageSettings.actualLanguage = SupportedLanguages.EN_US;
                    ChangeTextMenu("Pause", "Return", "Inventory", "Controls", "Exit game");
                    break;
                case "PT_BR":
                    LanguageSettings.actualLanguage = SupportedLanguages.PT_BR;
                    ChangeTextMenu("Pausa", "Retornar", "Inventário", "Controles", "Sair do jogo");
                    break;
                case "JP":
                    LanguageSettings.actualLanguage = SupportedLanguages.JP;
                    ChangeTextMenu("止む", "戻る", "発明", "コントロール", "ゲームを終了する");
                    break;
                case "FR":
                    LanguageSettings.actualLanguage = SupportedLanguages.FR;
                    ChangeTextMenu("Pause", "Revenir", "Inventaire", "Contrôles", "Quittez le jeu");
                    break;
                case "ES":
                    LanguageSettings.actualLanguage = SupportedLanguages.FR;
                    ChangeTextMenu("Pausa", "Retorno", "Inventario", "Controles", "Salir del juego");
                    break;
            }
        }

        public void ChangeTextMenu(string pause, string continueGame, string inventory, string controls, string exit)
        {
            PreferencesManager.FindDeepChild(transform, "PauseText").gameObject.GetComponent<Text>().text = pause;
            PreferencesManager.FindDeepChild(transform, "ContinueText").gameObject.GetComponent<Text>().text = continueGame;
            PreferencesManager.FindDeepChild(transform, "InventoryText").gameObject.GetComponent<Text>().text = inventory;
            PreferencesManager.FindDeepChild(transform, "ControlsText").gameObject.GetComponent<Text>().text = controls;
            PreferencesManager.FindDeepChild(transform, "ExitText").gameObject.GetComponent<Text>().text = exit;
            // Mudar imagem dos controles
        }
    }
}