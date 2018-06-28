using UnityEngine;
using UnityEngine.UI;
using RPGTALK.Localization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CrowShadowManager;

namespace CrowShadowMenu
{
    public class PreferencesManager : MonoBehaviour
    {

        void Start()
        {
            /* REINICIAR SAVE:
             *  1. Deletar todos os saves em C:\Users\Admin\AppData\LocalLow\DefaultCompany\AlGhaib
             *  2. Rodar o jogo uma vez, começando da cena do MainMenu
             *  3. Começar um novo jogo
             *  Observação: jogar pelo Continue (para salvar) ou Carregar (para não salvar)*/
            //PlayerPrefs.DeleteAll();

            if (PlayerPrefs.HasKey("Language"))
            {
                SetTextLanguage();
            }

            // Adaptar para mais de um save
            if (!GameManager.FilePatternExists(Application.persistentDataPath, "gamesave0_*.save"))
            {
                FindDeepChild(transform, "ContinueButton").gameObject.SetActive(false);
            }
            else
            {
                string[] files = GameManager.GetFilesByPattern(Application.persistentDataPath, "gamesave0_*.save");
                GameObject saveContent = FindDeepChild(transform, "ContinueSavesContent").gameObject;
                RectTransform rect = saveContent.gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, 30 * files.Length);
                foreach (string f in files)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(f, FileMode.Open);
                    Save save = (Save)bf.Deserialize(file);
                    file.Close();

                    string[] parts = f.Split('.');
                    string part = parts[parts.Length - 2];
                    string i = part.Substring(part.Length - 1);
                    AddButton("SaveButton", i + ": " + save.time, 0, i, saveContent);
                }
            }

            if (!GameManager.FilePatternExists(Application.persistentDataPath, "gamesave*.save"))
            {
                FindDeepChild(transform, "LoadGameButton").gameObject.SetActive(false);
            }

        }

        public void SetTextLanguage()
        {
            switch (PlayerPrefs.GetString("Language"))
            {
                case "EN_US":
                    LanguageSettings.actualLanguage = SupportedLanguages.EN_US;
                    ChangeTextMenu("New Game", "Continue", "Choose the save", "Load Game",
                        "Choose the mission", "*The game won't be saved", "Controls", "Options", "Language", "Exit", "Back");
                    break;
                case "PT_BR":
                    LanguageSettings.actualLanguage = SupportedLanguages.PT_BR;
                    ChangeTextMenu("Novo Jogo", "Continuar", "Escolha o jogo salvo", "Carregar Jogo",
                        "Escolha a missão", "*O jogo não será salvo", "Controles", "Opções", "Idioma", "Sair", "Voltar");
                    break;
            }
        }

        public void ChangeTextMenu(string newGame, string continueGame, string chooseSave, string loadGame, string missionLabel, string warning, string controls, string options, string optionsLabel, string exit, string back)
        {
            FindDeepChild(transform, "NewGameText").gameObject.GetComponent<Text>().text = newGame;
            FindDeepChild(transform, "ContinueText").gameObject.GetComponent<Text>().text = continueGame;
            FindDeepChild(transform, "ContinueLabel").gameObject.GetComponent<Text>().text = chooseSave;
            FindDeepChild(transform, "LoadGameText").gameObject.GetComponent<Text>().text = loadGame;
            FindDeepChild(transform, "MissionLabel").gameObject.GetComponent<Text>().text = missionLabel;
            FindDeepChild(transform, "LoadGameSubLabel").gameObject.GetComponent<Text>().text = chooseSave;
            FindDeepChild(transform, "WarningSaveLabel").gameObject.GetComponent<Text>().text = warning;
            FindDeepChild(transform, "ControlsText").gameObject.GetComponent<Text>().text = controls;
            FindDeepChild(transform, "OptionsText").gameObject.GetComponent<Text>().text = options;
            FindDeepChild(transform, "OptionsLabel").gameObject.GetComponent<Text>().text = optionsLabel;
            FindDeepChild(transform, "ExitText").gameObject.GetComponent<Text>().text = exit;
            FindDeepChild(transform, "BackContinueText").gameObject.GetComponent<Text>().text = back;
            FindDeepChild(transform, "BackLoadGameText").gameObject.GetComponent<Text>().text = back;
            FindDeepChild(transform, "BackLoadGameSubText").gameObject.GetComponent<Text>().text = back;
            FindDeepChild(transform, "BackControlsText").gameObject.GetComponent<Text>().text = back;
            FindDeepChild(transform, "BackOptionsText").gameObject.GetComponent<Text>().text = back;
            // Mudar imagem dos controles
        }

        public void SetSaveMenu(int m)
        {
            string[] files = GameManager.GetFilesByPattern(Application.persistentDataPath, "gamesave" + m + "_*.save");

            GameObject saveContent = FindDeepChild(transform, "LoadGameSavesContent").gameObject;
            foreach (Transform child in saveContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            RectTransform rect = saveContent.gameObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 30 * files.Length);

            foreach (string f in files)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(f, FileMode.Open);
                Save save = (Save)bf.Deserialize(file);
                file.Close();

                string[] parts = f.Split('.');
                string part = parts[parts.Length - 2];
                string i = part.Substring(part.Length - 1);
                AddButton("ContinueButton", i + ": " + save.time, m, i, saveContent);
            }
        }

        public static Transform FindDeepChild(Transform aParent, string aName)
        {
            var result = aParent.Find(aName);
            if (result != null) return result;

            foreach (Transform child in aParent)
            {
                result = FindDeepChild(child, aName);
                if (result != null) return result;
            }
            return null;
        }

        GameObject AddButton(string name, string text, int mission, string numSave, GameObject parent)
        {
            //print("UI: " + name + " >" + mission + "< [" + text + "]");
            GameObject instance = Instantiate(Resources.Load("Prefab/UI/" + name) as GameObject);

            instance.transform.SetParent(parent.transform);
            instance.GetComponent<RectTransform>().transform.localScale = new Vector3(1, 1, 1);
            if (text != "") instance.transform.Find("Text").GetComponent<Text>().text = text;

            if (name.Equals("SaveButton"))
            {
                instance.GetComponent<Button>().onClick.AddListener(delegate { instance.GetComponent<ContinueGame>().OnClick(int.Parse(numSave)); });
            }
            else if (name.Equals("ContinueButton"))
            {
                instance.GetComponent<Button>().onClick.AddListener(delegate { instance.GetComponent<LoadMission>().OnSaveClick(mission, int.Parse(numSave)); });
            }

            return instance;
        }

    }
}