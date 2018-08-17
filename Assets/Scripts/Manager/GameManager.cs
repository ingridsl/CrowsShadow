using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

namespace CrowShadowManager
{

    public class GameManager : MonoBehaviour
    {
        // PUBLIC

        public static GameManager instance;

        // MISSÕES
        public Mission mission;
        public SideQuest sideQuest;

        [Header("Mission")]
        public float timer;
        [Range(0,12)]
        public int currentMission = 0;
        public static bool initMission = false;
        public static float initX = 0, initY = 0;
        public static int initDir = 0;
        
        // CENAS
        public static string previousSceneName = "", currentSceneName = "";
        public static bool forceReload = false;

        // EXTRAS
        [Range(0, 5)]
        public int sideQuests = 0;

        // ESCOLHAS
        public float pathBird = 0, pathCat = 0;

        [Header("Choices")]
        // ESCOLHAS ESPECÍFICAS
        public bool mission1AssustaGato = false;
        public bool mission2ContestaMae = false;
        public bool mission4QuebraSozinho = false;
        public bool mission10BurnCorredor = false;

        [Header("Game Conditions")]
        // CONIDÇÕES DO JOGO
        public bool paused = false;
        public bool pausedObject = false;
        public bool blocked = false;
        public bool invertWorld = false;
        public bool invertWorldBlocked = true;
        public bool playerProtected = false;

        // HUD - INÍCIO MISSÃO
        public bool showMissionStart = true;
        public float startMissionDelay = 3f;

        [Header("Components")]
        // COMPONENTS
        public Player player;
        public GameObject tampa;
        public GameObject escudo;

        [Header("External Sources")]

        // RPG TALK
        public RPGTalk rpgTalk;

        // SONS
        public ScenerySounds scenerySounds;
        public ScenerySounds2 scenerySounds2;

        // PRIVATE

        // LOCALIZAÇÃO ALEATÓRIA DE OBJETOS
        private struct RandomPlace
        {
            public Inventory.InventoryItems objectToPick;
            public string placeChoosen;
            public string sceneObjectPickUp;

            public RandomPlace(Inventory.InventoryItems newObjectToPick, string newPlaceChoosen, string newSceneObjectPickUp)
            {
                objectToPick = newObjectToPick;
                placeChoosen = newPlaceChoosen;
                sceneObjectPickUp = newSceneObjectPickUp;
            }
        }
        private List<RandomPlace> randomPlaces = new List<RandomPlace>();

        // SAVE
        private Save currentSave;

        // COMPONENTES AUXILIARES
        private GameObject hud, levelImage;
        private Text levelText;
        private ProtectionObject tampaProtectionObject, escudoProtectionObject;
        private Renderer rendererPlayer;
        private UnityStandardAssets.ImageEffects.ColorCorrectionLookup colorCamera;


        // GAMEOVER
        private bool gameOver = false;

        // INVOKE
        private bool wasInvoked = false;
        private string invokeFunction = "";
        private float invokeDelay = 0f;

        // CRIAÇÃO JOGO
        public void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;

                currentSceneName = SceneManager.GetActiveScene().name;
                previousSceneName = currentSceneName;

                hud = GameObject.Find("HUDCanvas").gameObject;

                levelImage = hud.transform.Find("LevelImage").gameObject;
                levelText = levelImage.transform.Find("LevelText").GetComponent<Text>();

                tampaProtectionObject = tampa.GetComponent<ProtectionObject>();
                escudoProtectionObject = escudo.GetComponent<ProtectionObject>();

                player = GetComponent<Player>();
                rendererPlayer = GetComponent<Renderer>();
                colorCamera = GameObject.Find("MainCamera").GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionLookup>();

                currentMission = PlayerPrefs.GetInt("Mission");
                // Salva o jogo somente quando jogar através do New Game ou Continue
                if (currentMission == -1 || currentMission == 0)
                {
                    PlayerPrefs.SetInt("SaveGame", 0);
                }
                else
                {
                    PlayerPrefs.SetInt("SaveGame", 1);
                }

                if (currentMission == -1)
                {
                    currentMission = 1;

                    Inventory.SetInventory(null);
                    tampaProtectionObject.life = 80;
                    escudoProtectionObject.life = 120;

                    Book.pageQuantity = 0;
                    sideQuests = 0;

                    pathBird = 0;
                    pathCat = 0;

                    mission1AssustaGato = false;
                    mission2ContestaMae = false;
                    mission4QuebraSozinho = false;
                    mission10BurnCorredor = false;

                    SetMission(currentMission);
                    SaveGame(1, -1);
                }
                else
                {
                    LoadGame(currentMission);
                }

                rpgTalk.OnChoiceMade += OnChoiceMade;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        // ATUALIZAÇÕES DO JOGO
        public void Update()
        {
            timer += Time.deltaTime;
            if (!showMissionStart)
            {

                if (mission != null)
                {
                    mission.UpdateMission();
                }

                if (sideQuest != null)
                {
                    sideQuest.UpdateSideQuest();
                }

                if (CrossPlatformInputManager.GetButtonDown("EndText"))
                {
                    rpgTalk.EndTalk();
                }

                if (!blocked && (!paused || Book.show) && CrossPlatformInputManager.GetButtonDown("keyInvert") && !invertWorldBlocked)
                {
                    scenerySounds.PlayDemon(6);
                    InvertWorld(!invertWorld);
                }

            }
            else
            {
                if (CrossPlatformInputManager.GetButtonDown("JumpText"))
                {
                    HideLevelImage();
                }
            }

            // CONDIÇÃO PARA SAIR - MENU
            if (CrossPlatformInputManager.GetButtonDown("Exit"))
            {
                LoadScene("MainMenu");
            }

            // CHEATS
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeMission(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeMission(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeMission(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ChangeMission(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ChangeMission(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ChangeMission(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ChangeMission(7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                ChangeMission(8);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                ChangeMission(9);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                ChangeMission(10);
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                ChangeMission(11);
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                ChangeMission(12);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                LoadGame(0);
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                GameOver();
            }

        }

        /************ FUNÇÕES DE CENA ************/

        // FUNÇÕES DE MUDANÇA DE CENA
        public static void LoadScene(string name, bool menu = false)
        {
            print("OLDSCENE: " + previousSceneName + " " + Time.time * 1000);
            print("NEWSCENE: " + currentSceneName + " " + Time.time * 1000);
            if (!(name.Equals(currentSceneName) && !forceReload))
            {
                forceReload = false;
                if (!menu)
                {
                    SaveObjectsVariables();
                }
                print("ACTIVE " + Time.time * 1000);

                if (FadingScene.instance != null)
                {
                    FadingScene.instance.LoadSceneAsync(name, 0);
                }
                else
                {
                    SceneManager.LoadSceneAsync(name, 0);
                }
            }
        }

        // FUNÇÕES PARA CONTAR MUDANÇA DE CENA
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // FUNÇÃO APÓS MUDANÇA DE CENA
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            previousSceneName = currentSceneName;
            currentSceneName = scene.name;

            print("OLDSCENE: " + previousSceneName + " " + Time.time * 1000);
            print("NEWSCENE: " + currentSceneName + " " + Time.time * 1000);

            print("CLOSED " + Time.time * 1000);

            SetSceneWhenLoaded();
        }

        // ESPECIFICA DETALHES DA CENA QUANDO ESTÁ É CARREGADA
        private void SetSceneWhenLoaded()
        {
            if (currentSceneName.Equals("GameOver"))
            {
                player.enabled = false;
                rendererPlayer.enabled = false;
                if (rpgTalk.isPlaying)
                {
                    rpgTalk.EndTalk();
                }
            }
            else if (currentSceneName.Equals("MainMenu"))
            {
                Destroy(gameObject);
                Destroy(hud);
                if (Cat.instance != null) Cat.instance.DestroyCat();
                if (Crow.instance != null) Crow.instance.DestroyRaven();
                DeleteAllPlayerPrefs();
            }

            SetPlayerOnScene();
            InvertWorld(invertWorld);

            if (mission != null) mission.LoadMissionScene();
            ExtrasManager.SideQuestsManager();
            ExtrasManager.PagesManager();
            SetObjectsVariables();
            SetPickUps();

            if (previousSceneName.Equals("GameOver"))
            {
                gameOver = false;
                blocked = false;
                hud.SetActive(true);
                player.enabled = true;
                rendererPlayer.enabled = true;

                if (wasInvoked)
                {
                    Invoke(invokeFunction, invokeDelay);
                }
            }
        }

        // DETERMINA POSIÇÃO DO PLAYER AO CARREGAR A CENA
        private void SetPlayerOnScene()
        {
            if (!initMission && !currentSceneName.Equals("SideQuest") && !previousSceneName.Equals("SideQuest"))
            {
                player.ChangePosition();
            }
            else if (currentSceneName.Equals("SideQuest") || previousSceneName.Equals("SideQuest"))
            {
                if (currentSceneName.Equals("SideQuest") && sideQuest != null)
                {
                    sideQuest.InitSideQuest();
                }
                else if (previousSceneName.Equals("SideQuest") && !currentSceneName.Equals("GameOver") && sideQuest != null)
                {
                    sideQuest.EndSideQuest();
                }
                player.ChangePositionDefault(initX, initY, initDir);
            }
            else
            {
                player.ChangePositionDefault(initX, initY, initDir);
                if (initMission && currentSceneName.Equals(mission.sceneInit))
                {
                    initMission = false;
                    initX = initY = 0;
                }
            }
        }

        /************ FUNÇÕES DE OBJETO ************/

        // ADICIONAR OBJETO NA CENA
        public GameObject AddObject(GameObject newGameObject, string sprite, Vector3 position, Vector3 scale)
        {
            print("ADD OBJECT ");
            GameObject instance =
                Instantiate(newGameObject,
                position, Quaternion.identity) as GameObject;
            if (sprite != "") instance.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sprite);
            instance.transform.localScale = scale;
            return instance;
        }

        // ADICIONAR OBJETO NA CENA
        public GameObject AddObject(string name, string sprite, Vector3 position, Vector3 scale)
        {
            print("ADD OBJECT: " + name + " [" + sprite + "]");
            GameObject instance =
                Instantiate(Resources.Load("Prefab/" + name),
                position, Quaternion.identity) as GameObject;
            if (sprite != "") instance.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sprite);
            instance.transform.localScale = scale;
            return instance;
        }

        // ADICIONAR OBJETO NA CENA, COM DADOS REDUZIDOS
        public GameObject AddObject(string name, Vector3 position)
        {
            print("ADD OBJECT: " + name);
            GameObject instance =
                Instantiate(Resources.Load("Prefab/" + name), position, Quaternion.identity) as GameObject;
            return instance;
        }

        // ADICIONAR OBJETO NA CENA, COM DADOS REDUZIDOS
        public GameObject AddObject(string name)
        {
            print("ADD OBJECT: " + name);
            GameObject instance = Instantiate(Resources.Load("Prefab/" + name)) as GameObject;
            return instance;
        }

        // ADICIONAR OBJETO NA CENA COM PAI ASSOCIADO
        public GameObject AddObjectWithParent(GameObject newGameObject, string sprite, Vector3 position, Vector3 scale, Transform parent)
        {
            GameObject instance = AddObject(newGameObject, sprite, position, scale);
            instance.transform.parent = parent;
            return instance;
        }

        // ADICIONAR OBJETO NA CENA COM PAI ASSOCIADO
        public GameObject AddObjectWithParent(string name, string sprite, Vector3 position, Vector3 scale, Transform parent)
        {
            GameObject instance = AddObject(name, sprite, position, scale);
            instance.transform.parent = parent;
            return instance;
        }

        // ADICIONAR OBJETO NA CENA COM PAI ASSOCIADO, COM DADOS REDUZIDOS
        public GameObject AddObjectWithParent(string name, Transform parent)
        {
            GameObject instance = AddObject(name);
            instance.transform.SetParent(parent);
            return instance;
        }



        // SALVA AS VARIÁVEIS DE OBJETOS
        private static void SaveObjectsVariables()
        {
            SaveMovingObjectsPosition();
            SaveRotateObjectsPosition();
            SaveSceneObjectsState();
            SaveLampState();
        }

        // SALVAR POSIÇÃO DE OBJETOS MÓVEIS
        private static void SaveMovingObjectsPosition()
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("MovingObject");
            foreach (GameObject i in list)
            {
                if (i.GetComponent<MovingObject>() != null && !i.GetComponent<MovingObject>().prefName.Equals(""))
                {
                    PlayerPrefs.SetFloat(i.GetComponent<MovingObject>().prefName + "X", i.GetComponent<Rigidbody2D>().position.x);
                    PlayerPrefs.SetFloat(i.GetComponent<MovingObject>().prefName + "Y", i.GetComponent<Rigidbody2D>().position.y);
                }
            }
        }

        // SALVAR POSIÇÃO DE OBJETOS ROTACIONÁVEIS
        private static void SaveRotateObjectsPosition()
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("RotateObject");
            foreach (GameObject i in list)
            {
                if (i.GetComponent<RotateObject>() != null && !i.GetComponent<RotateObject>().prefName.Equals(""))
                {
                    PlayerPrefs.SetFloat(i.GetComponent<RotateObject>().prefName + "X", i.GetComponent<Rigidbody2D>().position.x);
                    PlayerPrefs.SetFloat(i.GetComponent<RotateObject>().prefName + "Y", i.GetComponent<Rigidbody2D>().position.y);
                }
            }
        }

        // SALVAR ESTADO DOS OBJETOS DE CENA
        private static void SaveSceneObjectsState()
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("SceneObject");
            foreach (GameObject i in list)
            {
                if (i.GetComponent<SceneObject>() != null && !i.GetComponent<SceneObject>().prefName.Equals(""))
                {
                    int active = 0;
                    if (i.GetComponent<SceneObject>().IsActive()) active = 1;
                    PlayerPrefs.SetInt(i.GetComponent<SceneObject>().prefName, active);
                }
            }
        }

        // SALVAR ESTADO DAS LÂMPADAS
        private static void SaveLampState()
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("Lamp");
            foreach (GameObject i in list)
            {
                if (i.GetComponent<Lamp>() != null && !i.GetComponent<Lamp>().prefName.Equals(""))
                {
                    int enabled = 0;
                    if (i.GetComponent<Light>().enabled) enabled = 1;
                    PlayerPrefs.SetInt(i.GetComponent<Lamp>().prefName, enabled);
                }
            }
        }

        // ESPECIFICA AS VARIÁVEIS DE OBJETOS
        private void SetObjectsVariables()
        {
            SetMovingObjectsPosition();
            SetRotateObjectsPosition();
            SetSceneObjectsState();
            SetLampState();
        }

        // POSICIONA OS OBJETOS MÓVEIS
        private void SetMovingObjectsPosition()
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("MovingObject");
            foreach (GameObject i in list)
            {
                if (i.GetComponent<MovingObject>() != null)
                {
                    string prefName = i.GetComponent<MovingObject>().prefName;
                    if (!prefName.Equals("") && PlayerPrefs.HasKey(prefName + "X") && PlayerPrefs.HasKey(prefName + "Y"))
                    {
                        //print("MOVING: " + prefName);
                        i.GetComponent<MovingObject>().ChangePosition(
                            PlayerPrefs.GetFloat(prefName + "X"),
                            PlayerPrefs.GetFloat(prefName + "Y"));
                    }
                }
            }
        }

        // POSICIONA OS OBJETOS ROTACIONÁVEIS
        private void SetRotateObjectsPosition()
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("RotateObject");
            foreach (GameObject i in list)
            {
                if (i.GetComponent<RotateObject>() != null)
                {
                    string prefName = i.GetComponent<RotateObject>().prefName;
                    if (!prefName.Equals("") && PlayerPrefs.HasKey(prefName + "X") && PlayerPrefs.HasKey(prefName + "Y"))
                    {
                        //print("ROTATE: " + prefName);
                        i.GetComponent<RotateObject>().ChangePosition(
                            PlayerPrefs.GetFloat(prefName + "X"),
                            PlayerPrefs.GetFloat(prefName + "Y"));
                    }
                }
            }
        }

        // DETERMINA O ESTADO DOS OBJETOS DE CENA
        private void SetSceneObjectsState()
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("SceneObject");
            foreach (GameObject i in list)
            {
                if (i.GetComponent<SceneObject>())
                {
                    string prefName = i.GetComponent<SceneObject>().prefName;
                    if (!prefName.Equals("") && PlayerPrefs.HasKey(prefName))
                    {
                        //print("SCENEOBJ: " + prefName);
                        bool active = false;
                        if (PlayerPrefs.GetInt(prefName) == 1) active = true;
                        i.GetComponent<SceneObject>().ChangeSpriteActive(active);
                    }
                }
            }
        }

        // DETERMINA O ESTADO DAS LÂMAPDAS
        private void SetLampState()
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("Lamp");
            foreach (GameObject i in list)
            {
                if (i.GetComponent<Lamp>() != null)
                {
                    string prefName = i.GetComponent<Lamp>().prefName;
                    if (!prefName.Equals("") && PlayerPrefs.HasKey(prefName))
                    {
                        //print("LAMP: " + i.GetComponent<Lamp>().prefName);
                        bool enabled = false;
                        if (PlayerPrefs.GetInt(prefName) == 1) enabled = true;
                        i.GetComponent<Light>().enabled = enabled;
                    }
                }
            }
        }

        // DELETAR TODOS OS ESTADOS DOS OBJETOS
        public void DeleteAllPlayerPrefs()
        {
            string language = "";
            int currentSave = 0, maxSave = 0;
            if (PlayerPrefs.HasKey("Language"))
            {
                language = PlayerPrefs.GetString("Language");
            }
            if (PlayerPrefs.HasKey("CurrentSaveGame"))
            {
                currentSave = PlayerPrefs.GetInt("CurrentSaveGame");
            }
            if (PlayerPrefs.HasKey("MaxSaveGame"))
            {
                maxSave = PlayerPrefs.GetInt("MaxSaveGame");
            }
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("Language", language);
            PlayerPrefs.SetInt("CurrentSaveGame", currentSave);
            PlayerPrefs.SetInt("MaxSaveGame", maxSave);
        }

        /************ FUNÇÕES DE SAVE ************/

        // CRIAR SAVE
        private Save CreateSaveGameObject()
        {
            Save save = new Save();

            save.mission = currentMission;

            save.inventory = Inventory.GetInventoryItems();
            save.currentItem = Inventory.GetCurrentItem();
            if (Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
            {
                save.lifeTampa = tampaProtectionObject.life;
            }
            else
            {
                save.lifeTampa = 0;
            }
            if (Inventory.HasItemType(Inventory.InventoryItems.ESCUDO))
            {
                save.lifeEscudo = escudoProtectionObject.life;
            }
            else
            {
                save.lifeEscudo = 0;
            }
            save.pedraCount = Inventory.pedraCount;
            save.papelCount = Inventory.papelCount;


            save.numberPages = Book.pageQuantity;
            save.sideQuests = sideQuests;

            save.pathBird = pathBird;
            save.pathCat = pathCat;

            save.mission1AssustaGato = mission1AssustaGato;
            save.mission2ContestaMae = mission2ContestaMae;
            save.mission4QuebraSozinho = mission4QuebraSozinho;
            save.mission10BurnCorredor = mission10BurnCorredor;

            save.time = DateTime.UtcNow.ToString();

            return save;
        }

        // SALVAR JOGO - COMPLETO
        public void SaveGame(int m, int opt)
        {
            Save save = CreateSaveGameObject();
            SaveGame(save, m, opt);
        }

        // SALVAR JOGO - ATUALIZAÇÃO
        public void UpdateSave()
        {
            if (currentSave != null)
            {
                currentSave.sideQuests = sideQuests;
                currentSave.numberPages = Book.pageQuantity;
                currentSave.time = DateTime.UtcNow.ToString();
                SaveGame(currentSave, currentMission, 0);
            }
        }

        // SALVAR JOGO
        private void SaveGame(Save save, int m, int opt)
        {

            BinaryFormatter bf = new BinaryFormatter();
            // opt = -1 -> new game
            // opt = 0 -> continue
            if ((opt == -1 || opt == 0) && PlayerPrefs.HasKey("SaveGame") && PlayerPrefs.GetInt("SaveGame") == 0)
            {
                int numSave = 0;
                if (PlayerPrefs.HasKey("CurrentSaveNumber"))
                {
                    numSave = PlayerPrefs.GetInt("CurrentSaveNumber");
                    if (opt == -1)
                    {
                        numSave++;
                        PlayerPrefs.SetInt("CurrentSaveNumber", numSave);
                        PlayerPrefs.SetInt("MaxSaveNumber", numSave);
                    }
                }
                else
                {
                    PlayerPrefs.SetInt("CurrentSaveNumber", numSave);
                    PlayerPrefs.SetInt("MaxSaveNumber", numSave);
                }

                FileStream file = File.Create(Application.persistentDataPath + "/gamesave" + 0 + "_" + numSave + ".save");
                bf.Serialize(file, save);
                file.Close();

                FileStream fileMission = File.Create(Application.persistentDataPath + "/gamesave" + m + "_" + numSave + ".save");
                bf.Serialize(fileMission, save);
                fileMission.Close();

                Debug.Log("Game Saved " + m);
            }

        }

        // CARREGAR JOGO
        public void LoadGame(int m)
        {
            if (PlayerPrefs.HasKey("CurrentSaveNumber") &&
                File.Exists(Application.persistentDataPath + "/gamesave" + m + "_" + PlayerPrefs.GetInt("CurrentSaveNumber") + ".save"))
            {
                paused = true;

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/gamesave" + m + "_" + PlayerPrefs.GetInt("CurrentSaveNumber") + ".save", FileMode.Open);
                currentSave = (Save)bf.Deserialize(file);
                file.Close();

                Inventory.SetInventory(currentSave.inventory);
                if (currentSave.currentItem != -1) Inventory.SetCurrentItem(currentSave.currentItem);
                if (Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
                {
                    tampaProtectionObject.life = currentSave.lifeTampa;
                }
                if (Inventory.HasItemType(Inventory.InventoryItems.ESCUDO))
                {
                    escudoProtectionObject.life = currentSave.lifeEscudo;
                }
                Inventory.pedraCount = currentSave.pedraCount;
                Inventory.papelCount = currentSave.papelCount;

                Book.pageQuantity = currentSave.numberPages;
                sideQuests = currentSave.sideQuests;

                pathBird = currentSave.pathBird;
                pathCat = currentSave.pathCat;

                mission1AssustaGato = currentSave.mission1AssustaGato;
                mission2ContestaMae = currentSave.mission2ContestaMae;
                mission4QuebraSozinho = currentSave.mission4QuebraSozinho;
                mission10BurnCorredor = currentSave.mission10BurnCorredor;

                SetMission(currentSave.mission);

                Debug.Log("Game Loaded " + m + " [" + PlayerPrefs.GetInt("CurrentSaveNumber") + "]");

                paused = false;
            }
            else
            {
                Debug.Log("No game saved!");
            }
        }

        /************ FUNÇÕES DE MISSÃO ************/

        // INICIALIZAR MISSÃO
        public void SetMission(int m)
        {
            currentMission = m;
            print("MISSAO: " + currentMission);

            switch (currentMission)
            {
                case 1:
                    mission = new Mission1();
                    break;
                case 2:
                    mission = new Mission2();
                    break;
                case 3:
                    mission = new Mission3();
                    break;
                case 4:
                    mission = new Mission4();
                    break;
                case 5:
                    mission = new Mission5();
                    break;
                case 6:
                    mission = new Mission6();
                    break;
                case 7:
                    mission = new Mission7();
                    break;
                case 8:
                    mission = new Mission8();
                    break;
                case 9:
                    mission = new Mission9();
                    break;
                case 10:
                    mission = new Mission10();
                    break;
                case 11:
                    mission = new Mission11();
                    break;
                case 12:
                    mission = new Mission12();
                    break;
                default:
                    break;
            }

            if (mission != null)
            {
                //DeleteAllPlayerPrefs(); - talvez resetar ao mudar de missão, já que é outra noite (mas dá erro)
                if (currentMission != 12)
                {
                    ClearRandomObjectsPlaces();
                    levelText.text = "Chapter  " + m;
                    levelImage.SetActive(true);
                    showMissionStart = true;
                    Invoke("HideLevelImage", startMissionDelay);
                }
                mission.InitMission();
            }
            else
            {
                showMissionStart = false;
            }
        }

        // MUDAR MISSÃO
        public void ChangeMission(int m)
        {
            timer = 0;
            forceReload = true;
            SetMission(m);
            SaveGame(currentMission, 0);
        }

        // FINALIZAR JOGO
        public void GameOver()
        {
            if (!gameOver)
            {
                gameOver = true;
                blocked = true;
                hud.SetActive(false);
                InvertWorld(false);
                if (sideQuest == null)
                {
                    if (Cat.instance != null) Cat.instance.DestroyCat();
                    if (Crow.instance != null) Crow.instance.DestroyRaven();
                }
                LoadScene("GameOver");
            }
        }

        // CONTINUAR JOGO
        public void ContinueGame()
        {
            LoadScene(previousSceneName, true);
        }

        /************ FUNÇÕES ESPECIAIS ************/

        // ALEATORIZAR LOCAIS PARA OBJETOS
        // Exemplo de chamada: GameManager.instance.RandomObjectsPlaces(Inventory.InventoryItems.FLASHLIGHT, new List<string>{ "QuartoKid" });
        // Deve vir antes do GameManager.LoadScene(sceneInit);
        public void RandomObjectsPlaces(Inventory.InventoryItems pickUp, List<string> places = null)
        {
            if (places == null || places.Count == 0)
            {
                places = new List<string> { "Sala", "Corredor", "Jardim", "Cozinha", "QuartoMae", "QuartoKid", "Banheiro", "Porao" };
            }
            int i = UnityEngine.Random.Range(0, places.Count);
            randomPlaces.Add(new RandomPlace(pickUp, places[i], ""));
            print("RANDOM: " + places[i] + "[" + pickUp + "]");
        }

        private void SetPickUps()
        {
            for (int i = 0; i < randomPlaces.Count; i++)
            {
                RandomPlace place = randomPlaces[i];
                if (place.placeChoosen.Equals(currentSceneName))
                {
                    // SE QUISER RETIRAR UM SCENE OBJECT ESPECIFICO DA ALEATORIZAÇÃO, SÓ MUDAR O PARÂMETRO DELE NA MISSÃO
                    if (place.sceneObjectPickUp.Equals(""))
                    {
                        List<GameObject> listScene = new List<GameObject>(GameObject.FindGameObjectsWithTag("SceneObject"));
                        listScene.RemoveAll(obj => obj.GetComponent<SceneObject>() == null || !obj.GetComponent<SceneObject>().canHavePickUp);

                        // GARANTIR QUE OS SCENE OBJECTS DE UMA MESMA CENA TEM NOMES DIFERENTES
                        if (listScene != null && listScene.Count > 0)
                        {
                            int j = UnityEngine.Random.Range(0, listScene.Count);
                            place.sceneObjectPickUp = listScene[j].name;
                        }
                    }

                    if (!place.sceneObjectPickUp.Equals(""))
                    {
                        CreateScenePickUp(place.sceneObjectPickUp, place.objectToPick, i);
                    }
                }

            }

        }

        // CRIA SCENE OBJETO PICK UP A PARTIR DE UM SCENE OBJECT
        public void CreateScenePickUp(string nameObject, Inventory.InventoryItems item, int num = -1)
        {
            print("PICK UP CREATE: " + nameObject + " [" + item + "]");
            GameObject obj = GameObject.Find(nameObject).gameObject;
            obj.tag = "ScenePickUpObject";
            SceneObject sceneObject = obj.GetComponent<SceneObject>();
            sceneObject.enabled = false;
            ScenePickUpObject scenePickUpObject = obj.AddComponent<ScenePickUpObject>();
            scenePickUpObject.sprite1 = sceneObject.sprite1;
            scenePickUpObject.sprite2 = sceneObject.sprite2;
            scenePickUpObject.positionSprite = sceneObject.positionSprite;
            scenePickUpObject.scale = sceneObject.scale;
            scenePickUpObject.isUp = sceneObject.isUp;
            scenePickUpObject.numRandomListed = num;
            scenePickUpObject.item = item;
        }

        // APAGAR OBJETO DA LISTA
        public void ObjectPicked(int i)
        {
            print("OBJECTED PICKED: " + i);
            randomPlaces.RemoveAt(i);
        }

        // LIMPAR DADOS DE ALEATORIZAÇÃO DE LOCAIS PARA OBJETOS
        private void ClearRandomObjectsPlaces()
        {
            randomPlaces.Clear();
        }

        // INVERTER MUNDO
        public void InvertWorld(bool sel)
        {
            invertWorld = sel;
            if (!currentSceneName.Equals("GameOver") && !currentSceneName.Equals("MainMenu") && !currentSceneName.Equals("Credits"))
            {
                if (colorCamera == null)
                {
                    colorCamera = GameObject.Find("MainCamera").GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionLookup>();
                }
                colorCamera.enabled = invertWorld;
            }
        }

        // AUXILIAR PARA RETORNAR NOMES DE ARQUIVO SEGUINDO PADRÃO
        public static string[] GetFilesByPattern(string path, string pattern)
        {
            string[] files = Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                return files;
            }
            return null;
        }

        // AUXILIAR PARA IDENTIFICAÇÃO DA EXISTÊNCIA DE PADRÕES DE ARQUIVO
        public static bool FilePatternExists(string path, string pattern)
        {
            return (GetFilesByPattern(path, pattern) != null);
        }

        // AUXILIAR PARA INVOCAÇÃO NAS MISSÕES
        public new void Invoke(string function, float time)
        {
            invokeFunction = function;
            invokeDelay = time;
            base.Invoke(function, time);
        }

        public void InvokeMission()
        {
            //print("INVOKEMISSION");
            if (currentSceneName.Equals("GameOver"))
            {
                wasInvoked = true;
            }
            else
            {
                wasInvoked = false;
                mission.InvokeMission();
            }
        }

        // AUXILIAR PARA IMPRIMIR NAS MISSÕES
        public void Print(string text)
        {
            print(text);
        }

        // ESCONDER IMAGEM INICIAL
        void HideLevelImage()
        {
            if (levelImage != null) levelImage.SetActive(false);
            showMissionStart = false;
        }

        /************ FUNÇÕES DE ESCOLHA ************/

        // FUNÇÃO APÓS ESCOLHA SER FEITA
        public void OnChoiceMade(int questionId, int choiceID)
        {
            if (questionId == 0)
            { // escolha final da missão 1
                if (choiceID == 0)
                { // assustar gato
                    pathBird += 2;
                    rpgTalk.NewTalk("M1Q0C0", "M1Q0C0End", rpgTalk.txtToParse);
                }
                else
                { // ficar com gato
                    pathCat += 4;
                    rpgTalk.NewTalk("M1Q0C1", "M1Q0C1End", rpgTalk.txtToParse);
                }
            }
            else if (questionId == 1)
            { // escolha final da missão 2
                if (choiceID == 0)
                { // contestar mãe
                    pathBird += 6;
                    rpgTalk.NewTalk("M2Q1C0", "M2Q1C0End", rpgTalk.txtToParse);
                }
                else
                { // respeitar mãe
                    pathCat += 5;
                    rpgTalk.NewTalk("M2Q1C1", "M2Q1C1End", rpgTalk.txtToParse);
                }
            }
            else if (questionId == 2)
            { // escolha final da missão 3
                if (choiceID == 0)
                { // mentir
                    pathBird += 4;
                    rpgTalk.NewTalk("M3Q2C0", "M3Q2C0End", rpgTalk.txtToParse);
                }
                else
                { // contar a verdade
                    pathCat += 4;
                    rpgTalk.NewTalk("M3Q2C1", "M3Q2C1End", rpgTalk.txtToParse);
                }
            }
            else if (questionId == 3)
            { // escolha inicial da missão 4 - escolha de quem vai quebrar o vaso
                if (choiceID == 0)
                { // quebra com o gato
                    pathCat += 4;
                    mission4QuebraSozinho = false;
                    rpgTalk.NewTalk("M4Q3C0", "M4Q3C0End");
                }
                else
                { // quebra sozinho
                    pathBird += 4;
                    mission4QuebraSozinho = true;
                    rpgTalk.NewTalk("M4Q3C1", "M4Q3C1End"); //essa escolha está sem fala definida. falas vazias não devem ser chamadas.
                }
            }
            else if (questionId == 4)
            { // escolha final da missão 4
                if (choiceID == 0)
                { // falar a verdade
                    pathCat += 4;
                    rpgTalk.NewTalk("M4Q4C0", "M4Q4C0End");
                }
                else
                { // mentir
                    pathBird += 6;
                    rpgTalk.NewTalk("M4Q4C1", "M4Q4C1End");
                }
            }
            else if (questionId == 5)
            { // escolha final da missão 5
                if (choiceID == 0)
                { // esconder
                    pathBird += 4;
                    rpgTalk.NewTalk("M5Q5C0", "M5Q5C0End");
                }
                else
                { // investigar
                    pathCat += 5;
                    rpgTalk.NewTalk("M5Q5C1", "M5Q5C1End");
                }
            }
            else if (questionId == 5)
            { // escolha final da missão 5
                if (choiceID == 0)
                { // esconder
                    pathBird += 4;
                    rpgTalk.NewTalk("M5Q5C0", "M5Q5C0End");
                }
                else
                { // investigar
                    pathCat += 5;
                    rpgTalk.NewTalk("M5Q5C1", "M5Q5C1End");
                }
            }
            else if (questionId == 6)
            { // escolha final da missão 7
                if (choiceID == 0)
                { // acusar
                    pathBird += 4;
                    rpgTalk.NewTalk("M7Q6C0", "M5Q6C0End");
                }
                else
                { // preguntar
                    pathCat += 5;
                    rpgTalk.NewTalk("M7Q6C1", "M7Q6C1End");
                }
            }
            else if (questionId == 7)
            { // escolha de dialogo de perguntar ao gato
                switch(choiceID)
                {
                    case 0:
                        rpgTalk.NewTalk("M7Pergunta1Start", "M7Pergunta1End");
                        break;
                    case 1:
                        rpgTalk.NewTalk("M7Pergunta2Start", "M7Pergunta2End");
                        break;
                    default:
                        rpgTalk.NewTalk("M7FineStart", "M7FineEnd");
                        break;
                }
            }
            else if (questionId == 8)
            { // escolha de dialogo de perguntar ao gato
                switch (choiceID)
                {
                    case 0:
                        rpgTalk.NewTalk("M7Pergunta10Start", "M7Pergunta10End");
                        break;
                    default:
                        rpgTalk.NewTalk("M7FineStart", "M7FineEnd");
                        break;
                }
            }
            else if (questionId == 10)
            { // escolha de dialogo de perguntar ao gato
                switch (choiceID)
                {
                    case 0:
                        rpgTalk.NewTalk("M5Q10C2", "M5Q10C2End");
                        break;
                    default:
                        if (pathBird > pathCat)
                        {
                            rpgTalk.NewTalk("M5Q10C1-1", "M5Q10C1-1End");
                        }
                        else
                        {
                            rpgTalk.NewTalk("M5Q10C1-2", "M5Q10C1-2End");
                        }
                        break;
                }

            }


            mission.InvokeMissionChoice(choiceID);
        }

    }
}
