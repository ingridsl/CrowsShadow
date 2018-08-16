using UnityEngine;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;
using UnityStandardAssets.CrossPlatformInput;

public class Mission10 : Mission {
    enum enumMission { NIGHT, INICIO, CORVO_APARECE_CAT, CORVO_ATACA_CAT_INIT, CORVO_ATACA_CAT, MAE_CAT, FINAL_CAT,
        CORVO_APARECE_BIRD, CORVO_ATACA_BIRD_INIT, CORVO_ATACA_BIRD, BOTIJAO_BIRD, FINAL_BIRD, FINAL };
    enumMission secao;

    GameObject luminaria, fireEvent;

    bool hasPanela = false, endCat = false, invertLocal = false, gameOverSet = false, stopMiniGame = false;
    bool estanteTrigger = false, poltronaTrigger = false, sofaTrigger = false;
    bool estanteBurn = false, poltronaBurn = false, sofaBurn = false;
    bool falaMae = false, falaGato = false;
    bool birdsActive = false;
    float portaDefaultX, portaDefaultY;
    bool fireBasement = false;

    // ANALISAR DIFICULDADE DO NIVEL E DOS DIFERENTES OBJETOS - FACA, PEDRO, FOSFORO, ISQUEIRO
    public override void InitMission()
    {
        sceneInit = "Porao";
        GameManager.initMission = true;
        GameManager.initX = (float) 1.5;
        GameManager.initY = (float) 0.2;
        GameManager.initDir = 3;
        GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = false;

        GameManager.instance.invertWorld = false;
        GameManager.instance.invertWorldBlocked = false;

        SetInitialSettings();

        hasPanela = Inventory.HasItemType(Inventory.InventoryItems.TAMPA);
        if (GameManager.instance.pathCat >= GameManager.instance.pathBird) endCat = true;
    }

    public override void UpdateMission() //aqui coloca as ações do update específicas da missão
    {
        if (GameManager.currentSceneName.Equals("GameOver") && !stopMiniGame)
        {
            if (fosforo != null && Inventory.GetCurrentItemType() == Inventory.InventoryItems.FOSFORO)
                fosforoMiniGame.StopMiniGame();
            if (isqueiro != null && Inventory.GetCurrentItemType() == Inventory.InventoryItems.ISQUEIRO)
                isqueiroMiniGame.StopMiniGame();
            stopMiniGame = true;
        }
        else if (GameManager.previousSceneName.Equals("GameOver") && stopMiniGame)
        {
            stopMiniGame = false;
        }

        if (GameManager.instance.invertWorld && !invertLocal)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }
        else if (!GameManager.instance.invertWorld && invertLocal)
        {
            invertLocal = false;
            mainLight.transform.Rotate(new Vector3(40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
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
            //if (book.SeenAll())
            //{
                if (endCat)
                {
                    EspecificaEnum((int)enumMission.CORVO_APARECE_CAT);
                }
                else
                {
                    EspecificaEnum((int)enumMission.CORVO_APARECE_BIRD);
                }
            //}
        }
        else if (secao == enumMission.CORVO_APARECE_CAT)
        {
            if (!GameManager.instance.rpgTalk.isPlaying)
            {
                EspecificaEnum((int)enumMission.CORVO_ATACA_CAT_INIT);
            }
        }
        else if (secao == enumMission.CORVO_APARECE_BIRD)
        {
            if (!GameManager.instance.rpgTalk.isPlaying)
            {
                EspecificaEnum((int)enumMission.CORVO_ATACA_BIRD_INIT);
            }
        }
        else if (secao == enumMission.CORVO_ATACA_CAT || secao == enumMission.MAE_CAT)
        {
            if (fosforoMiniGame.achievedGoal || isqueiroMiniGame.achievedGoal)
            {
                fosforoMiniGame.achievedGoal = false;
                isqueiroMiniGame.achievedGoal = false;

                fosforoMiniGame.activated = false;
                isqueiroMiniGame.activated = false;

                if (estanteTrigger)
                {
                    estanteBurn = true;
                    fireEvent.transform.Find("FogoEstante2").gameObject.SetActive(true);
                }
                if (poltronaTrigger)
                {
                    poltronaBurn = true;
                    fireEvent.transform.Find("FogoPoltrona").gameObject.SetActive(true);
                }
                if (sofaTrigger)
                {
                    sofaBurn = true;
                    fireEvent.transform.Find("FogoSofa").gameObject.SetActive(true);
                }

                if (estanteBurn && poltronaBurn && sofaBurn)
                {
                    EspecificaEnum((int)enumMission.FINAL_CAT);
                }
                else if ((estanteBurn && poltronaBurn) ||
                    (estanteBurn && sofaBurn) ||
                    (poltronaBurn && sofaBurn))
                {
                    EspecificaEnum((int)enumMission.MAE_CAT);
                }
            }
        }
        else if (secao == enumMission.CORVO_ATACA_BIRD)
        {
            // Achieved Goal Faca Pedra
            EspecificaEnum((int) enumMission.BOTIJAO_BIRD);
        }
        else if (secao == enumMission.BOTIJAO_BIRD)
        {
            if (GameManager.currentSceneName.Equals("Corredor") && luminaria != null)
            {
                if (luminaria.GetComponent<Lamp>().Changed())
                {
                    GameManager.instance.mission10BurnCorredor = true;
                    EspecificaEnum((int)enumMission.FINAL_BIRD);
                }
            }
            else if (GameManager.currentSceneName.Equals("QuartoMae") && luminaria != null)
            {
                if (luminaria.GetComponent<Lamp>().Changed())
                {
                    GameManager.instance.mission10BurnCorredor = false;
                    EspecificaEnum((int)enumMission.FINAL_BIRD);
                }
            }
        }

        if (GameObject.Find("BirdEmitterCollider"))
        {
            birdsActive = GameObject.Find("BirdEmitterCollider").gameObject.activeInHierarchy;
            if (birdsActive && !GameManager.instance.scenerySounds.source.isPlaying)
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
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        GameManager.instance.scenerySounds.StopSound();

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            gameOverSet = true;
            GameManager.instance.Invoke("InvokeMission", 2f);
        }

        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

        if (secao == enumMission.BOTIJAO_BIRD)
        {
            // Luminaria com faisca
            luminaria = GameObject.Find("Luminaria").gameObject;

            // FireEvent
            fireEvent = GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventBird").gameObject;
        }
    }

    public override void SetCozinha()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        GameManager.instance.scenerySounds.PlayDrop();

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            gameOverSet = true;
            GameManager.instance.Invoke("InvokeMission", 2f);
        }

        // Panela para caso ainda não tenha
        if (!hasPanela)
        {
            GameObject panela = GameObject.Find("Panela").gameObject;
            panela.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/panela_tampa");
            panela.GetComponent<ScenePickUpObject>().enabled = true;
        }

        if (secao == enumMission.CORVO_ATACA_BIRD)
        {
            // Botijao
            GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-4.1f, 1f, 0), new Vector3(1, 1, 1));
            trigger.name = "GasTrigger";
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        }
    }

    public override void SetJardim()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            gameOverSet = true;
            GameManager.instance.Invoke("InvokeMission", 2f);
        }

        // Pedra, caso não tenha
        if (!Inventory.HasItemType(Inventory.InventoryItems.PEDRA))
        {
            GameObject pedra1 = GameObject.Find("monte_pedra").gameObject;
            pedra1.tag = "ScenePickUpObject";
            ScenePickUpObject scenePickUpObject = pedra1.AddComponent<ScenePickUpObject>();
            scenePickUpObject.sprite1 = pedra1.GetComponent<SpriteRenderer>().sprite;
            scenePickUpObject.sprite2 = pedra1.GetComponent<SpriteRenderer>().sprite;
            scenePickUpObject.blockAfterPick = true;
            scenePickUpObject.item = Inventory.InventoryItems.PEDRA;

            GameObject pedra2 = GameObject.Find("monte_pedra (1)").gameObject;
            pedra2.tag = "ScenePickUpObject";
            ScenePickUpObject scenePickUpObject2 = pedra2.AddComponent<ScenePickUpObject>();
            scenePickUpObject2.sprite1 = pedra2.GetComponent<SpriteRenderer>().sprite;
            scenePickUpObject2.sprite2 = pedra2.GetComponent<SpriteRenderer>().sprite;
            scenePickUpObject2.blockAfterPick = true;
            scenePickUpObject2.item = Inventory.InventoryItems.PEDRA;

            GameObject pedra = GameManager.instance.AddObject("Objects/PickUp", "Sprites/Objects/Inventory/pedra", new Vector3((float)-3.59, (float)-0.45, 0), new Vector3(0.6f, 0.6f, 1f));
            pedra.GetComponent<PickUpObject>().item = Inventory.InventoryItems.PEDRA;
        }

        if (!Inventory.HasItemType(Inventory.InventoryItems.FACA) && !Inventory.HasItemType(Inventory.InventoryItems.PEDRA))
        {
            GameManager.instance.rpgTalk.NewTalk("M8PedraJardim", "M8PedraJardimEnd", GameManager.instance.rpgTalk.txtToParse, GameManager.instance, "", false);
        }

        if (secao == enumMission.FINAL_CAT)
        {
            EspecificaEnum((int) enumMission.FINAL);
        }
    }

    public override void SetQuartoKid()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            gameOverSet = true;
            GameManager.instance.Invoke("InvokeMission", 2f);
        }

        if (GameManager.instance.mission2ContestaMae)
        {
            // Arranhao
            GameManager.instance.AddObject("Scenery/Garra", "", new Vector3(-1.48f, 1.81f, 0), new Vector3(0.1f, 0.1f, 1));
        }
        else
        {
            // Vela
            GameObject velaFixa = GameObject.Find("velaMesa").gameObject;
            velaFixa.transform.GetChild(0).gameObject.SetActive(true);
            velaFixa.transform.GetChild(1).gameObject.SetActive(true);
            velaFixa.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = 140;
        }

        if (!Inventory.HasItemType(Inventory.InventoryItems.BASTAO))
        {
            GameManager.instance.CreateScenePickUp("Bau", Inventory.InventoryItems.BASTAO);
        }

        if (secao == enumMission.NIGHT || secao == enumMission.INICIO)
        {
            // Porta momentaneamente travada
            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            portaDefaultX = porta.transform.position.x;
            portaDefaultY = porta.transform.position.y;
            float posX = porta.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            porta.GetComponent<SceneDoor>().isOpened = false;
            porta.transform.position = new Vector3(porta.transform.position.x - posX, portaDefaultY, porta.transform.position.z);
        }
    }

    public override void SetQuartoMae()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            gameOverSet = true;
            GameManager.instance.Invoke("InvokeMission", 2f);
        }

        // Mae sumida
        GameObject triggerM = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-2.21f, -3.39f, 0), new Vector3(1, 1, 1));
        triggerM.name = "MaeTrigger";
        triggerM.GetComponent<Collider2D>().offset = new Vector2(0, 0);
        triggerM.GetComponent<BoxCollider2D>().size = new Vector2(1.8f, 1f);

        if (secao == enumMission.BOTIJAO_BIRD)
        {
            // Luminaria com faisca
            luminaria = GameObject.Find("Luminaria").gameObject;

            // FireEvent
            fireEvent = GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventBird").gameObject;
        }
    }

    public override void SetSala()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(30, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

        if (GameManager.instance.invertWorld)
        {
            invertLocal = true;
            mainLight.transform.Rotate(new Vector3(-40, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        }

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            gameOverSet = true;
            GameManager.instance.Invoke("InvokeMission", 2f);
        }

        if (secao == enumMission.CORVO_ATACA_CAT || secao == enumMission.MAE_CAT)
        {
            GameManager.instance.rpgTalk.NewTalk("M8LivingroomSceneStart", "M8LivingroomSceneEnd", false);

            if (secao == enumMission.MAE_CAT)
            {
                EspecificaEnum((int)enumMission.MAE_CAT);
            }

            // Isqueiro
            GameManager.instance.CreateScenePickUp("escrivaninha", Inventory.InventoryItems.ISQUEIRO);

            estanteTrigger = poltronaTrigger = sofaTrigger = false;

            // Estante
            GameObject triggerE = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-5.71f, 1.64f, 0), new Vector3(1, 1, 1));
            triggerE.name = "EstanteTrigger";
            triggerE.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            triggerE.GetComponent<BoxCollider2D>().size = new Vector2(1.8f, 1.6f);

            // Poltrona
            GameObject triggerP = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-1f, 1.6f, 0), new Vector3(1, 1, 1));
            triggerP.name = "PoltronaTrigger";
            triggerP.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            triggerP.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1.2f);

            // Sofa
            GameObject triggerS = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(5.53f, 0.4f, 0), new Vector3(1, 1, 1));
            triggerS.name = "SofaTrigger";
            triggerS.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            triggerS.GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1f);

            // FireEvent
            fireEvent = GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventCat").gameObject;
            fireEvent.SetActive(true);
            fireEvent.transform.Find("FumacaArmario").gameObject.SetActive(false);
            fireEvent.transform.Find("FogoArmario").gameObject.SetActive(false);
            fireEvent.transform.Find("FumacaSofa").gameObject.SetActive(false);
            fireEvent.transform.Find("FogoSofa").gameObject.SetActive(false);
            fireEvent.transform.Find("FumacaCriado").gameObject.SetActive(false);
            fireEvent.transform.Find("FogoCriado").gameObject.SetActive(false);
            fireEvent.transform.Find("FumacaEstante").gameObject.SetActive(false);
            fireEvent.transform.Find("FogoEstante1").gameObject.SetActive(false);
            fireEvent.transform.Find("FogoEstante2").gameObject.SetActive(false);
            fireEvent.transform.Find("FogoEstante3").gameObject.SetActive(false);
            fireEvent.transform.Find("FogoCadeira").gameObject.SetActive(false);
            fireEvent.transform.Find("FumacaPoltrona").gameObject.SetActive(false);
            fireEvent.transform.Find("FogoPoltrona").gameObject.SetActive(false);

            if (estanteBurn)
            {
                fireEvent.transform.Find("FogoEstante2").gameObject.SetActive(true);
            }
            if (poltronaBurn)
            {
                fireEvent.transform.Find("FogoPoltrona").gameObject.SetActive(true);
            }
            if (sofaBurn)
            {
                fireEvent.transform.Find("FogoSofa").gameObject.SetActive(true);
            }

            if (Cat.instance == null)
            {
                // Gato
                GameObject cat = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(2.5f, -1.3f, 0), new Vector3(0.15f, 0.15f, 1));
                cat.GetComponent<Cat>().speed = 2.4f;
                cat.GetComponent<Cat>().FollowPlayer();
            }
        }
        else if (secao == enumMission.CORVO_ATACA_BIRD || secao == enumMission.BOTIJAO_BIRD)
        {
            // Fala sobre gato sumido
            GameObject triggerG = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-4.96f, -2f, 0), new Vector3(1, 1, 1));
            triggerG.name = "GatoTrigger";
            triggerG.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            triggerG.GetComponent<BoxCollider2D>().size = new Vector2(6f, 1f);
            
        }
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        GameManager.instance.Print("SECAO: " + secao);

        if(secao == enumMission.INICIO)
        {
            GameManager.instance.rpgTalk.NewTalk("M8KidRoomSceneStart", "M8KidRoomSceneEnd", GameManager.instance.rpgTalk.txtToParse, GameManager.instance, "", false);
        }
        else if (secao == enumMission.CORVO_APARECE_CAT)
        {
            GameManager.instance.rpgTalk.NewTalk("Dica8PC", "Dica8PCEnd", GameManager.instance.rpgTalk.txtToParse, GameManager.instance, "", false);

            CreateCorvoCat();

            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            porta.GetComponent<SceneDoor>().isOpened = false;
        }
        else if (secao == enumMission.CORVO_APARECE_BIRD)
        {
            GameManager.instance.rpgTalk.NewTalk("Dica8PB", "Dica8PBEnd", GameManager.instance.rpgTalk.txtToParse, GameManager.instance, "", false);

            CreateCorvoBird();

            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            porta.GetComponent<SceneDoor>().isOpened = false;
        }
        else if (secao == enumMission.CORVO_ATACA_CAT_INIT || secao == enumMission.CORVO_ATACA_BIRD_INIT)
        {
            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            porta.GetComponent<SceneDoor>().isOpened = true;
            GameManager.instance.scenerySounds2.PlayDoorOpen(1);
            porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-opened");
            porta.transform.position = new Vector3(portaDefaultX, portaDefaultY, porta.transform.position.z);

            GameManager.instance.Invoke("InvokeMission", 3f);
        }
        else if (secao == enumMission.CORVO_ATACA_CAT || secao == enumMission.CORVO_ATACA_BIRD)
        {
            if (Crow.instance != null) {
                Crow.instance.transform.Find("BirdEmitterCollider").gameObject.SetActive(true);
                Crow.instance.FollowPlayer();
            }
        }
        else if (secao == enumMission.MAE_CAT)
        {
            GameManager.instance.rpgTalk.NewTalk("M8MomCat", "M8MomCatEnd", GameManager.instance.rpgTalk.txtToParse, GameManager.instance, "", false);

            GameManager.instance.AddObject("NPCs/mom", "", new Vector3(-3.1f, 1.3f, -0.5f), new Vector3(0.3f, 0.3f, 1));
        }
        else if (secao == enumMission.FINAL_CAT)
        {
            Crow.instance.Stop();
            Crow.instance.transform.Find("BirdEmitterCollider").gameObject.SetActive(false);

            GameManager.instance.rpgTalk.NewTalk("M8LivingroomSceneRepeat", "M8LivingroomSceneRepeatEnd", false);
        }
        else if (secao == enumMission.BOTIJAO_BIRD)
        {
            GameManager.instance.rpgTalk.NewTalk("M8KitchenSceneRepeat", "M8KitchenSceneRepeatEnd", false);
        }
        else if (secao == enumMission.FINAL_BIRD)
        {
            fireEvent.SetActive(true);
            Crow.instance.Stop();
            Crow.instance.transform.Find("BirdEmitterCollider").gameObject.SetActive(false);
            GameManager.instance.blocked = true;
            GameManager.instance.Invoke("InvokeMission", 5f);
        }
        else if (secao == enumMission.FINAL)
        {
            if (Cat.instance != null) Cat.instance.DestroyCat();
            GameManager.instance.ChangeMission(9);
        }
    }

    public override void AreaTriggered(string tag)
    {

        if (tag.Equals("PoraoTrigger") && (Inventory.HasItemType(Inventory.InventoryItems.ISQUEIRO) || Inventory.HasItemType(Inventory.InventoryItems.FOSFORO)))
        {
            if (CrossPlatformInputManager.GetButtonDown("keyInteract") && !fireBasement) {
                GameObject fire = GameManager.instance.AddObject(
               "ScenerySounds/FogoChamine", "", new Vector3(-1.19f, 0.9f, -2f), new Vector3(1f, 1f, 0.5242185f));
                fireBasement = true;
            }
        }
        if (tag.Equals("EnterMomTrigger") && !falaMae)
        {
            GameManager.instance.rpgTalk.NewTalk("M8MomRoomSceneStart", "M8MomRoomSceneEnd", false);
            falaMae = true;
        }
        else if (tag.Equals("EnterGatoTrigger") && !falaGato)
        {
            GameManager.instance.rpgTalk.NewTalk("M8LivingroomSceneRepeat2", "M8LivingroomSceneRepeat2End", false);
            falaGato = true;
        }
        else if (secao == enumMission.CORVO_ATACA_CAT || secao == enumMission.MAE_CAT)
        {
            if (tag.Equals("EnterEstanteTrigger") && !estanteBurn)
            {
                fosforoMiniGame.activated = true;
                isqueiroMiniGame.activated = true;
                fosforoMiniGame.posFlareX = -5.71f;
                fosforoMiniGame.posFlareY = 1.64f;
                isqueiroMiniGame.posFlareX = -5.71f;
                isqueiroMiniGame.posFlareY = 1.64f;
                estanteTrigger = true;
            }
            else if (tag.Equals("EnterPoltronaTrigger") && !poltronaBurn)
            {
                fosforoMiniGame.activated = true;
                isqueiroMiniGame.activated = true;
                fosforoMiniGame.posFlareX = -1f;
                fosforoMiniGame.posFlareY = 1.6f;
                isqueiroMiniGame.posFlareX = -1f;
                isqueiroMiniGame.posFlareY = 1.6f;
                poltronaTrigger = true;
            }
            else if (tag.Equals("EnterSofaTrigger") && !sofaBurn)
            {
                fosforoMiniGame.activated = true;
                isqueiroMiniGame.activated = true;
                fosforoMiniGame.posFlareX = 5.53f;
                fosforoMiniGame.posFlareY = 0.4f;
                isqueiroMiniGame.posFlareX = 5.53f;
                isqueiroMiniGame.posFlareY = 0.5f;
                sofaTrigger = true;
            }
            else if (tag.Equals("ExitEstanteTrigger") || tag.Equals("ExitPoltronaTrigger") || tag.Equals("ExitSofaTrigger"))
            {
                fosforoMiniGame.activated = false;
                isqueiroMiniGame.activated = false;
                estanteTrigger = poltronaTrigger = sofaTrigger = false;
            }
        }
        else if (secao == enumMission.CORVO_ATACA_BIRD)
        {
            if (tag.Equals("EnterGasTrigger"))
            {
                if (!Inventory.HasItemType(Inventory.InventoryItems.FACA) && !Inventory.HasItemType(Inventory.InventoryItems.PEDRA))
                {
                    GameManager.instance.rpgTalk.NewTalk("M8PedraCozinha", "M8PedraCozinhaEnd", GameManager.instance.rpgTalk.txtToParse, GameManager.instance, "", false);
                }
            }
            else if (tag.Equals("ExitGasTrigger"))
            {

            }
        }
    }
    public override void SetBanheiro()
    {

    }

    public override void SetPorao()
    {
        GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-1.2f, 0.75f, 0), new Vector3(1, 1, 1));
        trigger.name = "PoraoTrigger";
        trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
        trigger.GetComponent<BoxCollider2D>().size = new Vector2(2f, 1f);


        GameObject aux = GameManager.instance.AddObject("Effects/MasterRiftEmitter", new Vector3(-2.9f, 0.67f, 0));
    }

    public override void InvokeMission()
    {
        if (gameOverSet)
        {
            gameOverSet = false;
            if (secao == enumMission.CORVO_ATACA_CAT || secao == enumMission.MAE_CAT || secao == enumMission.FINAL_CAT)
            {
                GameObject crow = CreateCorvoCat();
                crow.GetComponent<Crow>().ChangePosition(
                    player.GetComponent<Player>().GetCorvoPositionX(), player.GetComponent<Player>().GetCorvoPositionY());
                crow.transform.Find("BirdEmitterCollider").gameObject.SetActive(true);
                crow.GetComponent<Crow>().FollowPlayer();
                if (secao == enumMission.FINAL_CAT) EspecificaEnum((int)enumMission.FINAL_CAT);
            }
            else if (secao == enumMission.CORVO_ATACA_BIRD || secao == enumMission.BOTIJAO_BIRD)
            {
                GameObject crow = CreateCorvoBird();
                crow.GetComponent<Crow>().ChangePosition(
                    player.GetComponent<Player>().GetCorvoPositionX(), player.GetComponent<Player>().GetCorvoPositionY());
                crow.transform.Find("BirdEmitterCollider").gameObject.SetActive(true);
                crow.GetComponent<Crow>().FollowPlayer();
            }
        }
        else if  (secao == enumMission.CORVO_ATACA_CAT_INIT)
        {
            EspecificaEnum((int)enumMission.CORVO_ATACA_CAT);
        }
        else if (secao == enumMission.CORVO_ATACA_BIRD_INIT)
        {
            EspecificaEnum((int)enumMission.CORVO_ATACA_BIRD);
        }
        else if (secao == enumMission.FINAL_BIRD)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }
    }

    //PathBird e PathCat variam entre 0 e 30
    //Quando mais no caminho do gato, mais fraco o crow
    public GameObject CreateCorvoCat()
    {
        GameObject crow = GameManager.instance.AddObject("NPCs/Crow", "", new Vector3(-1.7f, 0.6f, -0.5f), new Vector3(4.5f, 4.5f, 1));
        crow.GetComponent<Crow>().LookAtPlayer();
        crow.GetComponent<Crow>().speed = 0.1f - (GameManager.instance.pathCat/1000); // velocidade do crow
        crow.GetComponent<Crow>().timeBirdsFollow = 0.7f - (GameManager.instance.pathCat/100); // tempo que os pássaros analisam onde o player está, quando menor, o delay será maior
        var em = crow.transform.Find("BirdEmitterCollider").gameObject.GetComponent<ParticleSystem>();
        var main = em.main;
        em.emission.SetBurst(0, new ParticleSystem.Burst(0, 8, 12, 0, 10)); // min, max pássaros por burst e tempo para outro ciclo
        main.startSpeed = 1f; // velocidade dos pássaros
        main.duration = 10f; // tempo do ciclo de ataque dos pássaros        
        main.startLifetime = 18f; // tempo de vida dos pássaros, tem que ser menor que o ciclo
        main.maxParticles = 20;

        return crow;
    }

    //Quando mais no caminho do crow, mais forte ele será
    public GameObject CreateCorvoBird()
    {
        GameObject crow = GameManager.instance.AddObject("NPCs/Crow", "", new Vector3(-1.7f, 0.6f, -0.5f), new Vector3(4.8f, 4.8f, 1));
        crow.GetComponent<Crow>().LookAtPlayer();
        crow.GetComponent<Crow>().speed = 0.08f + (GameManager.instance.pathBird/1000);
        crow.GetComponent<Crow>().timeBirdsFollow = 0.5f + (GameManager.instance.pathBird/100);
        var em = crow.transform.Find("BirdEmitterCollider").gameObject.GetComponent<ParticleSystem>();
        var main = em.main;
        em.emission.SetBurst(0, new ParticleSystem.Burst(0, 8, 12, 0, 8));
        main.startSpeed = 1f;
        main.duration = 8f;
        main.startLifetime = 14f;
        main.maxParticles = 20;

        return crow;
    }

    public override void ForneceDica()
    {

    }

}
