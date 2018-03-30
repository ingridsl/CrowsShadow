﻿using UnityEngine.SceneManagement;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class MissionManager : MonoBehaviour {

    public static MissionManager instance;

    // MISSÕES
    public Mission mission;
    public int currentMission, unlockedMission;
    public static bool initMission = false;
    public static float initX = 0, initY = 0;
    public static int initDir = 0;

    // CENAS
    public string previousSceneName, currentSceneName;

    // EXTRAS
    public int numberPages, sideQuests;

    // ESCOLHAS
    public float pathBird, pathCat;

    // ESCOLHAS ESPECÍFICAS
    public bool mission1AssustaGato = false;
    public bool mission2ContestaMae = false;
    public bool mission4QuebraSozinho = false;
    public bool mission8BurnCorredor = false;

    // CONIDÇÕES DO JOGO
    public bool paused = false;
    public bool pausedObject = false;
    public bool blocked = false;
    public bool invertWorld = false;
    public bool invertWorldBlocked = true;
    public bool playerProtected = false;

    // HUD - INÍCIO MISSÃO
    public bool showMissionStart = true;
    float startMissionDelay = 3f;
    private GameObject hud;
    private GameObject levelImage;
    private Text levelText;

    // RPG TALK
    public RPGTalk rpgTalk;

    // SONS
    public ScenerySounds scenerySounds;
    public ScenerySounds2 scenerySounds2;

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

            currentMission = PlayerPrefs.GetInt("Mission");
            if (currentMission == -1)
            {
                currentMission = 1;
                unlockedMission = 1;

                Inventory.SetInventory(null);
                GameObject.Find("Player").gameObject.transform.Find("Tampa").gameObject.GetComponent<ProtectionObject>().life = 80;

                numberPages = 0;
                sideQuests = 0;

                pathBird = 0;
                pathCat = 0;

                mission1AssustaGato = false;
                mission2ContestaMae = false;
                mission4QuebraSozinho = false;
                mission8BurnCorredor = false;

                SetMission(currentMission);
                SaveGame(0);
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
        if (!showMissionStart)
        {

            if (mission != null)
            {
                mission.UpdateMission();
            }

            if (CrossPlatformInputManager.GetButtonDown("EndText"))
            {
                rpgTalk.EndTalk();
            }

            if (!blocked && !paused && CrossPlatformInputManager.GetButtonDown("keyInvert") && !invertWorldBlocked)
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
            LoadScene(0);
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
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            LoadGame(0);
        }

    }

    /************ FUNÇÕES DE CENA ************/

    // FUNÇÕES DE MUDANÇA DE CENA
    public static void LoadScene(string name)
    {
        SaveMovingObjectsPosition();
        SceneManager.LoadScene(name);
    }

    public static void LoadScene(int index)
    {
        SaveMovingObjectsPosition();
        SceneManager.LoadScene(index);
    }

    // FUNÇÕES PARA CONTAR MUDANÇA DE CENA
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // FUNÇÃO APÓS MUDANÇA DE CENA
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        previousSceneName = currentSceneName;
        currentSceneName = scene.name;
        print("OLDSCENE" + previousSceneName);
        print("NEWSCENE" + currentSceneName);

        if (!initMission) {
            GetComponent<Player>().ChangePosition();
        }
        else {
            GetComponent<Player>().ChangePositionDefault(initX, initY, initDir);
            if (currentSceneName.Equals(mission.sceneInit))
            {
                initMission = false;
                initX = initY = 0;
            }
        }

        if (previousSceneName.Equals("GameOver"))
        {
            GetComponent<Player>().enabled = true;
            GetComponent<Renderer>().enabled = true;
        }

        if (currentSceneName.Equals("GameOver"))
        {
            GetComponent<Player>().enabled = false;
            GetComponent<Renderer>().enabled = false;
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
            if (Corvo.instance != null) Corvo.instance.DestroyRaven();
            if (CorvBabies.instance != null) CorvBabies.instance.DestroyCorvBabies();
        }

        InvertWorld(invertWorld);

        if(mission != null) mission.LoadMissionScene();

        GameObject[] list = GameObject.FindGameObjectsWithTag("MovingObject");
        foreach (GameObject i in list)
        {
            if (!i.GetComponent<MovingObject>().prefName.Equals(""))
            {
                print("POSNEW: " + i.GetComponent<MovingObject>().prefName);
                i.GetComponent<MovingObject>().ChangePosition(
                    PlayerPrefs.GetFloat(i.GetComponent<MovingObject>().prefName + "X"),
                    PlayerPrefs.GetFloat(i.GetComponent<MovingObject>().prefName + "Y"));
            }
        }
    }

    /************ FUNÇÕES DE OBJETO ************/

    // ADICIONAR OBJETO NA CENA
    public GameObject AddObject(string name, string sprite, Vector3 position, Vector3 scale)
    {
        GameObject moveInstance =
            Instantiate(Resources.Load("Prefab/" + name),
            position, Quaternion.identity) as GameObject;
        if (sprite != "") moveInstance.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sprite);
        moveInstance.transform.localScale = scale;
        return moveInstance;
    }

    // SALVAR POSIÇÃO DE OBJETOS MÓVEIS
    private static void SaveMovingObjectsPosition()
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("MovingObject");
        foreach (GameObject i in list)
        {
            if (!i.GetComponent<MovingObject>().prefName.Equals(""))
            {
                PlayerPrefs.SetFloat(i.GetComponent<MovingObject>().prefName + "X", i.GetComponent<Rigidbody2D>().position.x);
                PlayerPrefs.SetFloat(i.GetComponent<MovingObject>().prefName + "Y", i.GetComponent<Rigidbody2D>().position.y);
            }
        }
    }

    // DELETAR TODAS AS POSIÇÕES DE OBJETOS MÓVEIS
    public void DeleteAllPlayerPrefs()
    {
        string language = "";
        if (PlayerPrefs.HasKey("Language"))
        {
            language = PlayerPrefs.GetString("Language");
        }
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.SetInt("Mission", currentMission);
    }

    /************ FUNÇÕES DE SAVE ************/

    // CRIAR SAVE
    private Save CreateSaveGameObject()
    {
        Save save = new Save();

        save.mission = currentMission;
        if (unlockedMission <= currentMission) {
            save.unlockedMission = currentMission;
        }
        else
        {
            save.unlockedMission = unlockedMission;
        }

        save.inventory = Inventory.GetInventoryItems();
        save.currentItem = Inventory.GetCurrentItem();
        if (Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
        {
            save.lifeTampa = GameObject.Find("Player").gameObject.transform.Find("Tampa").gameObject.GetComponent<ProtectionObject>().life;
        }
        else
        {
            save.lifeTampa = 0;
        }


        save.numberPages = numberPages;
        save.sideQuests = sideQuests;

        save.pathBird = pathBird;
        save.pathCat = pathCat;
        
        save.mission1AssustaGato = mission1AssustaGato;
        save.mission2ContestaMae = mission2ContestaMae;
        save.mission4QuebraSozinho = mission4QuebraSozinho;
        save.mission8BurnCorredor = mission8BurnCorredor;

        return save;
    }

    // SALVAR JOGO
    public void SaveGame(int m)
    {
        Save save = CreateSaveGameObject();
        
        BinaryFormatter bf = new BinaryFormatter();
        // m = 0 -> continue
        // m > 0 -> select mission
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave" + m + ".save");
        bf.Serialize(file, save);
        file.Close();
        
        Debug.Log("Game Saved " + m);
    }

    // CARREGAR JOGO
    public void LoadGame(int m)
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave" + m + ".save"))
        {
            paused = true;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave" + m + ".save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();
            
            Inventory.SetInventory(save.inventory);
            if (save.currentItem != -1) Inventory.SetCurrentItem(save.currentItem);
            if (Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
            {
                GameObject.Find("Player").gameObject.transform.Find("Tampa").gameObject.GetComponent<ProtectionObject>().life = save.lifeTampa;
            }

            numberPages = save.numberPages;
            sideQuests = save.sideQuests;

            pathBird = save.pathBird;
            pathCat = save.pathCat;
           
            mission1AssustaGato = save.mission1AssustaGato;
            mission2ContestaMae = save.mission2ContestaMae;
            mission4QuebraSozinho = save.mission4QuebraSozinho;
            mission8BurnCorredor = save.mission8BurnCorredor;

            unlockedMission = save.unlockedMission;
            SetMission(save.mission);
            SaveGame(0);

            Debug.Log("Game Loaded " + m);

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

		switch(currentMission){
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
        }

        ExtrasManager.SideQuestsManager();
        ExtrasManager.PagesManager();
        
        levelImage = hud.transform.Find("LevelImage").gameObject;
        levelText = levelImage.transform.Find("LevelText").GetComponent<Text>();

        if (mission != null)
        {
            levelText.text = "Chapter  " + m;
            levelImage.SetActive(true);
            showMissionStart = true;
            mission.InitMission();
            Invoke("HideLevelImage", startMissionDelay);
        }
        else {
            showMissionStart = false;
        }
    }

    // MUDAR MISSÃO
    public void ChangeMission(int m)
    {
        SetMission(m);
        SaveGame(0);
        SaveGame(currentMission);
    }

    // FINALIZAR JOGO
    public void GameOver()
    {
        blocked = true;
        hud.SetActive(false);
        InvertWorld(false);
        if (Cat.instance != null) Cat.instance.DestroyCat();
        if (Corvo.instance != null) Corvo.instance.DestroyRaven();
        if (CorvBabies.instance != null) CorvBabies.instance.DestroyCorvBabies();
        LoadScene("GameOver");
    }

    // CONTINUAR JOGO
    public void ContinueGame()
    {
        LoadScene(previousSceneName);
        blocked = false;
        hud.SetActive(true);
    }

    /************ FUNÇÕES ESPECIAIS ************/

    // INVERTER MUNDO
    public void InvertWorld(bool sel)
    {
        invertWorld = sel;
        if (!currentSceneName.Equals("GameOver") && !currentSceneName.Equals("MainMenu"))
        {
            GameObject.Find("MainCamera").GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionLookup>().enabled = invertWorld;
        }
    }

    // AUXILIAR PARA INVOCAÇÃO NAS MISSÕES
    public void InvokeMission()
    {
        print("INVOKEMISSION");
        mission.InvokeMission();
    }

    // AUXILIAR PARA IMPRIMIR NAS MISSÕES
    public void Print(string text)
    {
        print(text);
    }

    // ESCONDER IMAGEM INICIAL
    void HideLevelImage()
    {
        levelImage.SetActive(false);
        showMissionStart = false;
    }

    /************ FUNÇÕES DE ESCOLHA ************/

    // FUNÇÃO APÓS ESCOLHA SER FEITA
    public void OnChoiceMade(int questionId, int choiceID)
    {
		if (questionId == 0) { // escolha final da missão 1
			if (choiceID == 0) { // assustar gato
				pathBird += 2;
                rpgTalk.NewTalk ("M1Q0C0", "M1Q0C0End", rpgTalk.txtToParse);;
			} else { // ficar com gato
				pathCat += 4;
                rpgTalk.NewTalk ("M1Q0C1", "M1Q0C1End", rpgTalk.txtToParse);;
			}
		}
		else if (questionId == 1) { // escolha final da missão 2
			if (choiceID == 0) { // contestar mãe
				pathBird += 6;
				rpgTalk.NewTalk ("M2Q1C0", "M2Q1C0End", rpgTalk.txtToParse);;
			} else { // respeitar mãe
				pathCat += 5;
				rpgTalk.NewTalk ("M2Q1C1", "M2Q1C1End", rpgTalk.txtToParse);;
			}
		}
        else if (questionId == 2) { // escolha final da missão 3
            if (choiceID == 0) { // mentir
                pathBird += 4;
                rpgTalk.NewTalk ("M3Q2C0", "M3Q2C0End", rpgTalk.txtToParse);;
            } else { // contar a verdade
                pathCat += 4;
                rpgTalk.NewTalk ("M3Q2C1", "M3Q2C1End", rpgTalk.txtToParse);;
            }
        }
        else if (questionId == 3) { // escolha inicial da missão 4 - escolha de quem vai quebrar o vaso
            if (choiceID == 0) { // quebra com o gato
                pathCat += 4;
                mission4QuebraSozinho = false;
                rpgTalk.NewTalk("M4Q3C0", "M4Q3C0End");
            }
            else{ // quebra sozinho
                pathBird += 4;
                mission4QuebraSozinho = true;
                rpgTalk.NewTalk("M4Q3C1", "M4Q3C1End"); //essa escolha está sem fala definida. falas vazias não devem ser chamadas.
            }
        }
        if (questionId == 4) { // escolha final da missão 4
            if (choiceID == 0) { // falar a verdade
                pathCat += 4;
                rpgTalk.NewTalk("M4Q4C0", "M4Q4C0End");
            }
            else { // mentir
                pathBird += 6;
                rpgTalk.NewTalk("M4Q4C1", "M4Q4C1End");
            }
        }
        if (questionId == 5) { // escolha final da missão 5
            if (choiceID == 0) { // esconder
                pathBird += 4;
                rpgTalk.NewTalk("M5Q5C0", "M5Q5C0End");
            }
            else { // investigar
                pathCat += 5;
                rpgTalk.NewTalk("M5Q5C1", "M5Q5C1End");
            }
        }
        mission.InvokeMissionChoice(choiceID);
    }
    
}
