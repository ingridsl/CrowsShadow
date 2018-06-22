using UnityEngine;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission1 : Mission {
    enum enumMission { NIGHT, INICIO, GATO_QUARTO, GATO_APARECEU, GATO_CORREDOR,
        GATO_COZINHA, GATO_SALA, LANTERNA_ENCONTRADA, CORVO_VISTO, SMILE, MAE_QUARTO, FAZER_ESCOLHA, FINAL };
    enumMission secao;

    SceneObject window;
    SickCrow sickCrow;
    
    bool areaTriggered = false, birdsActive = false;
    float portaDefaultX, portaDefaultY;

    public override void InitMission()
    {
        sceneInit = "QuartoKid";
        GameManager.initMission = true;
        GameManager.initX = (float) -2.5;
        GameManager.initY = (float) 0.7;
        GameManager.initDir = 0;
        GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = true;
        GameManager.instance.invertWorld = false;
        GameManager.instance.invertWorldBlocked = true;

        PlayerPrefs.DeleteKey("MO_Corredor_0X");
        PlayerPrefs.DeleteKey("MO_Corredor_0Y");
        
        SetInitialSettings();
    }

    public override void UpdateMission() //aqui coloca as ações do update específicas da missão
    {
        if ((int)GameManager.instance.timer == tipTimerSmall || (int)GameManager.instance.timer == tipTimerMedium || (int)GameManager.instance.timer == tipTimerLonger)
        {
            ForneceDica();
        }
        if (secao == enumMission.NIGHT)
        {
            if (!GameManager.instance.showMissionStart)
            {
                EspecificaEnum((int)enumMission.INICIO);
            }
        }
        else if (secao == enumMission.INICIO)
        {
            if (sickCrow.fly)
            {
                EspecificaEnum((int) enumMission.GATO_QUARTO);
            }
        }
        else if (secao == enumMission.GATO_QUARTO)
        {
            if (GameManager.currentSceneName.Equals("QuartoKid") && !GameManager.instance.rpgTalk.isPlaying)
            {
                EspecificaEnum((int)enumMission.GATO_APARECEU);
            }
        }
        else if (secao == enumMission.GATO_APARECEU)
        {
            if (GameManager.currentSceneName.Equals("Corredor") && !GameManager.instance.rpgTalk.isPlaying)
            {
                EspecificaEnum((int)enumMission.GATO_CORREDOR);
            } 
        }
        else if (secao == enumMission.GATO_SALA)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.FLASHLIGHT))
            {
                EspecificaEnum((int)enumMission.LANTERNA_ENCONTRADA);
            }
        }
        else if (secao == enumMission.MAE_QUARTO && GameManager.currentSceneName.Equals("QuartoKid"))
        {
            if (!GameManager.instance.rpgTalk.isPlaying)
            {
                EspecificaEnum((int)enumMission.FAZER_ESCOLHA);
            }
        }

        if (secao == enumMission.GATO_SALA || secao == enumMission.LANTERNA_ENCONTRADA)
        {
            if (areaTriggered && !Flashlight.GetState() && !birdsActive)
            {
                GameManager.instance.scenerySounds.PlayBird(1);
                GameObject birds = GameObject.Find("BirdEmitterHolder(Sala)").gameObject;
                birds.transform.Find("BirdEmitterCollider").gameObject.SetActive(true);
                birdsActive = true;

            }
            if(birdsActive && !GameManager.instance.scenerySounds.source.isPlaying && (!GameManager.currentSceneName.Equals("GameOver") || !GameManager.currentSceneName.Equals("MainMenu")))
            {
                GameManager.instance.scenerySounds.StopSound();
                float value = Random.value;
                if (value > 0)
                    GameManager.instance.scenerySounds.PlayBird(1);
                else
                    GameManager.instance.scenerySounds.PlayBird(4);
            }
            
        }
    }

    public override void SetCorredor()
    {
        GameManager.instance.scenerySounds.StopSound();

        if (secao == enumMission.GATO_APARECEU)
        {
            GameManager.instance.rpgTalk.NewTalk("M1CorridorSceneStart", "M1CorridorSceneEnd", GameManager.instance.rpgTalk.txtToParse);
            GameManager.instance.scenerySounds.PlayCat(2);
        }

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        // Porta Mae
        GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
        float portaMaeDefaultY = portaMae.transform.position.y;
        float posX = portaMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        portaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        portaMae.GetComponent<SceneDoor>().isOpened = false;
        portaMae.transform.position = new Vector3(portaMae.transform.position.x - posX, portaMaeDefaultY, portaMae.transform.position.z);

        // Porta Banheiro
        GameObject portaBanheiro = GameObject.Find("DoorToBathroom").gameObject;
        float portaBanheiroDefaultY = portaBanheiro.transform.position.y;
        posX = portaBanheiro.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        portaBanheiro.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        portaBanheiro.GetComponent<SceneDoor>().isOpened = false;
        portaBanheiro.transform.position = new Vector3(portaBanheiro.transform.position.x - posX, portaBanheiroDefaultY, portaBanheiro.transform.position.z);

        // Objeto movel que atrapalha
        GameObject chair = GameManager.instance.AddObject("Objects/MovingObject", "Sprites/Objects/Scene/vaso",
            new Vector3((float)-3.59, (float)-0.45, 0), new Vector3((float)1.2, (float)1.2, 1));
        chair.GetComponent<MovingObject>().prefName = "MO_Corredor_0";

        // Tutorial
        GameManager.instance.AddObject("Tutorial/C-vase");

        if (secao == enumMission.GATO_APARECEU)
        {
            GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(8.3f, -0.6f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 4;
            GameObject.Find("MainCamera").GetComponent<Camera>().transform.position = new Vector3(0f, 0f, -20f);
            // Porta Sala
            GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
            portaSala.GetComponent<SceneDoor>().isOpened = false;

            // Porta QuartoKid
            GameObject portaKid = GameObject.Find("DoorToKidRoom").gameObject;
            portaKid.GetComponent<SceneDoor>().isOpened = false;
            float portaKidDefaultY = portaKid.transform.position.y;
            float portaKidposX = portaKid.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            portaKid.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            portaKid.transform.position = new Vector3(portaKid.transform.position.x - posX, portaKidDefaultY, portaKid.transform.position.z);

        }
        else if (secao == enumMission.GATO_COZINHA)
        {
            GameObject cat = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(-0.7f, -0.6f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().Patrol();
            Vector3 aux = new Vector3(-9.8f, -0.4f, -0.5f);
            Vector3[] catPos = { aux };
            cat.GetComponent<Cat>().targets = catPos;

            GameManager.instance.pausedObject = true;
            GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 4;
            GameObject.Find("MainCamera").GetComponent<Camera>().transform.position = new Vector3(0f, 0f, -20f);
            GameManager.instance.Invoke("InvokeMission", 2.5f);

            // gato andando para sala
            GameManager.instance.scenerySounds.PlayCat(3);
        }
    }

    public override void SetCozinha()
    {
        GameManager.instance.scenerySounds.StopSound();
        GameManager.instance.scenerySounds.PlayDrop();
        //GameManager.instance.rpgTalk.NewTalk ("M1KitchenSceneStart", "M1KitchenSceneEnd");

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        // Panela com tampa
        GameObject panela = GameObject.Find("Panela").gameObject;
        panela.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/panela_tampa");

        if (secao == enumMission.GATO_CORREDOR)
        {
            EspecificaEnum((int)enumMission.GATO_COZINHA);
        }
    }

    public override void SetJardim()
    {
        GameManager.instance.scenerySounds.StopSound();
        //GameManager.instance.rpgTalk.NewTalk ("M1GardenSceneStart", "M1GardenSceneEnd");

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        /*GameObject areaLight = GameObject.Find("AreaLightHolder").gameObject; //utilizar AreaLight para cenas de dia, variar Z do Holder
        areaLight.transform.Find("AreaLight").gameObject.SetActive(true);
        areaLight.transform.position = new Vector3(areaLight.transform.position.x, areaLight.transform.position.y, -20);*/
        GameManager.instance.scenerySounds.PlayWolf(2);
    }

    public override void SetQuartoKid()
    {
        GameManager.instance.scenerySounds.StopSound();
        
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true);

        if (secao == enumMission.NIGHT || secao == enumMission.INICIO) {
            // Tutorial
            GameManager.instance.AddObject("Tutorial/Z-key1");
            GameManager.instance.AddObject("Tutorial/C-key");
            GameManager.instance.AddObject("Tutorial/CtrlC-key");

            // Pássaro
            GameObject littleCrow = GameManager.instance.AddObject("MissionEvents/CrowHolder", "", new Vector3(-0.66f, 1.62f, 0f), new Vector3(0.5f, 0.5f, 1));
            sickCrow = littleCrow.GetComponent<SickCrow>();
        }

        if (secao == enumMission.NIGHT || secao == enumMission.INICIO || secao == enumMission.CORVO_VISTO)
        {
            // Porta
            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            portaDefaultX = porta.transform.position.x;
            portaDefaultY = porta.transform.position.y;
            float posX = porta.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            porta.GetComponent<SceneDoor>().isOpened = false;
            porta.transform.position = new Vector3(porta.transform.position.x - posX, portaDefaultY, porta.transform.position.z);
        }
        
        if (secao == enumMission.MAE_QUARTO)
        {
            GameObject.Find("Flashlight").gameObject.GetComponent<Flashlight>().EnableFlashlight(false);
            GameManager.instance.GetComponent<Player>().ChangePositionDefault(-2.5f, 0.7f, 0);
            GameManager.instance.AddObject("NPCs/mom", "", new Vector3(1.7f, 0.6f, -0.5f), new Vector3(0.3f, 0.3f, 1));

            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            porta.GetComponent<SceneDoor>().isOpened = false;

            GameManager.instance.rpgTalk.NewTalk("M1KidRoomSceneRepeat", "M1KidRoomSceneRepeatEnd", GameManager.instance.rpgTalk.txtToParse);
        }
    }

    public override void SetQuartoMae()
    {
        GameManager.instance.scenerySounds.StopSound();
        //GameManager.instance.rpgTalk.NewTalk ("M1MomRoomSceneStart", "M1MomRoomSceneEnd");

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z
    }

    public override void SetSala()
    {
        GameManager.instance.scenerySounds.StopSound();
        //GameManager.instance.rpgTalk.NewTalk ("M1LivingroomSceneStart", "M1LivingroomSceneEnd");

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLightBooks").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        // Lanterna - Criado Mudo
        GameManager.instance.CreateScenePickUp("CriadoMudoSala", Inventory.InventoryItems.FLASHLIGHT);

        // Porta Jardim
        GameObject portaJardim = GameObject.Find("DoorToGarden").gameObject;
        portaJardim.GetComponent<SceneDoor>().isOpened = false;

        // Porta Corredor
        GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
        portaCorredor.GetComponent<SceneDoor>().isOpened = false;

        // Triggers
        GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(0f, 0f, 0), new Vector3(1, 1, 1));
        trigger.name = "AreaTrigger";
        trigger.GetComponent<Collider2D>().offset = new Vector2(2.68f, 0);
        trigger.GetComponent<BoxCollider2D>().size = new Vector2(10.2f, 5f);

        GameObject trigger2 = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(0f, 0f, 0), new Vector3(1, 1, 1));
        trigger2.name = "TVTrigger";
        trigger2.GetComponent<Collider2D>().offset = new Vector2(7f, -0.4f);
        trigger2.GetComponent<BoxCollider2D>().size = new Vector2(1.55f, 0.8f);

        // Tutorial
        GameManager.instance.AddObject("Tutorial/Z-key2");
        GameManager.instance.AddObject("Tutorial/X-key");

        areaTriggered = false;
        birdsActive = false;

        if (secao == enumMission.GATO_COZINHA)
        {
            EspecificaEnum((int)enumMission.GATO_SALA);
        }
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission) pos;
        GameManager.instance.timer = 0;
        GameManager.instance.Print("SECAO: " + secao);

        if(secao == enumMission.INICIO)
        {
            GameManager.instance.timer = 0;
            GameManager.instance.rpgTalk.NewTalk("M1KidRoomSceneStart", "M1KidRoomSceneEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.GATO_QUARTO)
        {
            // Porta abrindo
            GameManager.instance.scenerySounds2.PlayDoorOpen(2);
            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-opened");
            porta.GetComponent<SceneDoor>().isOpened = true;
            porta.transform.position = new Vector3(portaDefaultX, portaDefaultY, porta.transform.position.z);
            
            GameManager.instance.rpgTalk.NewTalk("M1KidRoomSceneCat", "M1KidRoomSceneCatEnd", GameManager.instance.rpgTalk.txtToParse);
            
            // Gato entrando
            GameManager.instance.scenerySounds.PlayCat(2);
            GameObject cat = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(1.8f, 1f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().Patrol();
            Vector3 aux = new Vector3(1f, 0f, -0.5f);
            Vector3[] catPos = { aux };
            cat.GetComponent<Cat>().targets = catPos;
            cat.GetComponent<Cat>().stopEndPath = true;
            cat.GetComponent<Cat>().destroyEndPath = false;
        }
        else if (secao == enumMission.GATO_APARECEU)
        {
            GameManager.instance.scenerySounds.PlayCat(2);
            Cat.instance.GetComponent<Cat>().Patrol();
            Vector3 aux = new Vector3(1.8f, 1f, -0.5f);
            Vector3[] catPos = { aux };
            Cat.instance.targets = catPos;
            Cat.instance.destroyEndPath = true;
        }
        else if (secao == enumMission.GATO_CORREDOR)
        {
            Cat.instance.GetComponent<Cat>().Patrol();
            Vector3 aux = new Vector3(2.6f, -0.7f, -0.5f);
            Vector3[] catPos = { aux };
            Cat.instance.GetComponent<Cat>().targets = catPos;
            Cat.instance.destroyEndPath = false;
            Cat.instance.stopEndPath = true;
            Cat.instance.speed = 1.4f;
            GameManager.instance.Invoke("InvokeMission", 6.5f);
            GameManager.instance.pausedObject = true;
        }
        else if (secao == enumMission.CORVO_VISTO)
        {
            GameManager.instance.scenerySounds.PlayBird(1);
            GameManager.instance.blocked = true;

            //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLightTV").gameObject.SetActive(true);
            mainLight.transform.Rotate(new Vector3(-25, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
            GameManager.instance.AddObject("Effects/BlinkMainLight", "", new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
            GameObject.Find("TV").gameObject.GetComponent<SceneMultipleObject>().ChangeSprite();
            GameManager.instance.Invoke("InvokeMission", 5f);
        }
        else if (secao == enumMission.SMILE)
        {
            GameManager.instance.scenerySounds.PlayScare(3);
            
            GameObject darkness = GameObject.Find("DarknessHolder").gameObject;
            darkness.transform.Find("Darkness").gameObject.SetActive(true);
            GameManager.instance.Invoke("InvokeMission", 3f);
        }
        else if (secao == enumMission.MAE_QUARTO)
        {
            GameManager.LoadScene("QuartoKid");
        }
        else if (secao == enumMission.FAZER_ESCOLHA)
        {
            GameObject.Destroy(GameObject.Find("mom(Clone)").gameObject);
            GameManager.instance.Invoke("InvokeMission", 4f);
        }
    }

    public override void ForneceDica()
    {
        if (GameManager.currentSceneName.Equals("QuartoKid") && secao == enumMission.INICIO)
        {
            GameManager.instance.timer = 0;
            GameManager.instance.rpgTalk.NewTalk("M1CorvoArmarioStart", "M1CorvoArmarioEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (GameManager.currentSceneName.Equals("Sala") && secao == enumMission.GATO_SALA && !usedTip1 && GameManager.instance.timer == tipTimerSmall)
        {
            GameManager.instance.timer = 0;
            GameManager.instance.rpgTalk.NewTalk("M1LanternaArmario1Start", "M1LanternaArmario1End", GameManager.instance.rpgTalk.txtToParse);
            usedTip1 = true;
        }
        else if (GameManager.currentSceneName.Equals("Sala") && secao == enumMission.GATO_SALA && usedTip1 && GameManager.instance.timer == tipTimerMedium)
        {
            GameManager.instance.timer = 0;
            GameManager.instance.rpgTalk.NewTalk("M1LanternaArmario2Start", "M1LanternaArmario2End", GameManager.instance.rpgTalk.txtToParse);
        }
    }

    public override void AreaTriggered(string tag)
    {
        if (tag.Equals("AreaTrigger"))
        {
            areaTriggered = true;
        }
        else if (tag.Equals("TVTrigger") && secao == enumMission.LANTERNA_ENCONTRADA && !birdsActive)
        {
            EspecificaEnum((int)enumMission.CORVO_VISTO);
        }
    }

    public override void InvokeMission()
    {
        if (secao == enumMission.GATO_CORREDOR || secao == enumMission.GATO_COZINHA)
        {
            GameObject.Destroy(GameObject.Find("catFollower(Clone)").gameObject);
            GameManager.instance.pausedObject = false;
            GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 2;
        }
        else if (secao == enumMission.CORVO_VISTO)
        {
            EspecificaEnum((int)enumMission.SMILE);
        }
        else if (secao == enumMission.SMILE)
        {
            EspecificaEnum((int)enumMission.MAE_QUARTO);
        }
        else if (secao == enumMission.FAZER_ESCOLHA)
        {
            GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(1.7f, 0.7f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            GameManager.instance.rpgTalk.NewTalk("M1KidRoomSceneChoice", "M1KidRoomSceneChoiceEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.FINAL)
        {
            PlayerPrefs.DeleteKey("Corredor_0X");
            PlayerPrefs.DeleteKey("Corredor_0Y");
            GameManager.instance.ChangeMission(2);
        }
    }

    public override void InvokeMissionChoice(int id)
    {
        if (secao == enumMission.FAZER_ESCOLHA)
        {
            EspecificaEnum((int)enumMission.FINAL);
            if(id == 0)
            {
                GameManager.instance.mission1AssustaGato = true;
                GameObject.Destroy(GameObject.Find("catFollower(Clone)").gameObject);
            }
            else
            {
                GameManager.instance.mission1AssustaGato = false;
                GameObject.Find("catFollower(Clone)").gameObject.GetComponent<Cat>().FollowPlayer();
            }
            GameManager.instance.Invoke("InvokeMission", 8f);
        }
    }

}
