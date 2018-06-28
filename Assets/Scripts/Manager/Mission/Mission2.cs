using UnityEngine;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission2 : Mission {
    enum enumMission { NIGHT, INICIO_GATO, INICIO_SOZINHO, ENCONTRA_MAE, CONTESTA_MAE, CONTESTA_MAE2, RESPEITA_MAE, RESPEITA_MAE2,
        FINAL_CONTESTA, FINAL_CONTESTA_CORVO, FINAL_CONTESTA_GATO, FINAL_CONTESTA_ATAQUE,
        FINAL_RESPEITA, FINAL_RESPEITA_VELA, FINAL_RESPEITA_FOSFORO, FINAL };
    enumMission secao;

    GameObject crowBabies;
    GameObject vela, velaFixa;

    public override void InitMission()
    {
        sceneInit = "QuartoKid";
        GameManager.initMission = true;
        GameManager.initX = (float)1.5;
        GameManager.initY = (float)-1.0;
        GameManager.initDir = 1;
        GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = true;

        GameManager.instance.invertWorld = false;
        GameManager.instance.invertWorldBlocked = true;

        SetInitialSettings();
    }

    public override void UpdateMission() //aqui coloca as ações do update específicas da missão
    {
        if (secao == enumMission.NIGHT)
        {
            if (!GameManager.instance.showMissionStart)
            {
                if (GameManager.instance.mission1AssustaGato)
                {
                    EspecificaEnum((int)enumMission.INICIO_SOZINHO);
                }
                else
                {
                    EspecificaEnum((int)enumMission.INICIO_GATO);
                }
                GameManager.instance.rpgTalk.NewTalk("M2KidRoomSceneStart", "M2KidRoomSceneEnd", GameManager.instance.rpgTalk.txtToParse);
            }
        }
        else if (secao == enumMission.RESPEITA_MAE2)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.VELA) && Inventory.HasItemType(Inventory.InventoryItems.FOSFORO))
            {
                EspecificaEnum((int)enumMission.FINAL_RESPEITA);
            }
        }
        else if(secao == enumMission.CONTESTA_MAE2)
        {
            if (Inventory.HasItemType(Inventory.InventoryItems.FACA) && Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
            {
                EspecificaEnum((int)enumMission.FINAL_CONTESTA);
            }
        }
        else if (secao == enumMission.FINAL_RESPEITA)
        {
            if (!Inventory.HasItemType(Inventory.InventoryItems.VELA))
            {
                EspecificaEnum((int)enumMission.FINAL_RESPEITA_VELA);
            }
        }
        else if (secao == enumMission.FINAL_RESPEITA_VELA)
        {
            if (fosforoMiniGame.achievedGoal)
            {
                EspecificaEnum((int)enumMission.FINAL_RESPEITA_FOSFORO);
            }
        }
    }

    public override void SetCorredor()
    {
        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        GameManager.instance.scenerySounds.StopSound();

        if (GameManager.previousSceneName.Equals("QuartoKid") &&
            (secao == enumMission.CONTESTA_MAE2 || secao == enumMission.RESPEITA_MAE2))
        {
            GameManager.instance.rpgTalk.NewTalk("M2CorridorSceneRepeat", "M2CorridorSceneRepeatEnd");
        }

        if (secao == enumMission.INICIO_SOZINHO)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(10.8f, -0.3f, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().followWhenClose = true;
        }

        if (secao == enumMission.INICIO_SOZINHO || secao == enumMission.INICIO_GATO)
        {
            // Porta Cozinha
            GameObject portaCozinha = GameObject.Find("DoorToKitchen").gameObject;
            portaCozinha.GetComponent<SceneDoor>().isOpened = false;

            // Porta Sala
            GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
            portaSala.GetComponent<SceneDoor>().isOpened = false;

            // Porta Quarto Mae
            GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject; float portaMaeDefaultY = portaMae.transform.position.y;
            float posX = portaMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            portaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            portaMae.GetComponent<SceneDoor>().isOpened = false;
            portaMae.transform.position = new Vector3(portaMae.transform.position.x - posX, portaMaeDefaultY, portaMae.transform.position.z);

            // Mae
            GameManager.instance.AddObject("NPCs/mom", "", new Vector3(-1.5f, 0f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-1.5f, 0f, 1), new Vector3(1, 1, 1));
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
        }
        else if (secao == enumMission.CONTESTA_MAE || secao == enumMission.RESPEITA_MAE ||
            secao == enumMission.CONTESTA_MAE2 || secao == enumMission.RESPEITA_MAE2 || 
            secao == enumMission.FINAL_CONTESTA || secao == enumMission.FINAL_RESPEITA)
        {
            // Porta Mae
            GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
            float portaMaeDefaultY = portaMae.transform.position.y;
            float posX = portaMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            portaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            portaMae.GetComponent<SceneDoor>().isOpened = false;
            portaMae.transform.position = new Vector3(portaMae.transform.position.x - posX, portaMaeDefaultY, portaMae.transform.position.z);

            if (secao == enumMission.CONTESTA_MAE2 || secao == enumMission.RESPEITA_MAE2 ||
            secao == enumMission.FINAL_CONTESTA || secao == enumMission.FINAL_RESPEITA)
            {
                // Mae patrulha
                GameObject mom = GameManager.instance.AddObject("NPCs/mom", "", new Vector3(-2f, -0.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
                mom.GetComponent<Patroller>().isPatroller = true;
                Vector3 target1 = new Vector3(6f, -0.3f, -0.5f);
                Vector3 target2 = new Vector3(6f, 0.5f, -0.5f);
                Vector3 target3 = new Vector3(6f, -0.3f, -0.5f);
                Vector3 target4 = new Vector3(7f, -0.3f, -0.5f);
                if (Random.value > 0) target4 = new Vector3(8f, -0.3f, -0.5f);
                Vector3 target5 = new Vector3(-3f, -0.3f, -0.5f);
                Vector3 target6 = new Vector3(5f, -0.3f, -0.5f);
                if (Random.value > 0) target6 = new Vector3(3f, -0.3f, -0.5f);
                Vector3 target7 = new Vector3(-3f, -0.3f, -0.5f);
                Vector3[] momTargets = { target1, target2, target3, target4, target5, target6, target7 };
                mom.GetComponent<Patroller>().targets = momTargets;
                mom.GetComponent<Patroller>().hasActionPatroller = true;
            }
        }
    }

	public override void SetCozinha()
	{
        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        GameManager.instance.scenerySounds.PlayDrop();
        //GameManager.instance.rpgTalk.NewTalk ("M2KitchenSceneStart", "M2KitchenSceneEnd");

        // Panela
        GameObject panela = GameObject.Find("Panela").gameObject;
        panela.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/panela_tampa");

        if (secao == enumMission.CONTESTA_MAE2)
        {
            panela.GetComponent<ScenePickUpObject>().enabled = true;

            // Faca
            GameManager.instance.CreateScenePickUp("Armario1", Inventory.InventoryItems.FACA);
        }
        else if (secao == enumMission.RESPEITA_MAE2)
        {
            // Fosforo
            GameManager.instance.CreateScenePickUp("Armario1", Inventory.InventoryItems.FOSFORO);
        }
    }

	public override void SetJardim()
	{
		//GameManager.instance.rpgTalk.NewTalk ("M2GardenSceneStart", "M2GardenSceneEnd");
	}

    public override void SetQuartoKid()
    {
        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        if (GameManager.previousSceneName.Equals("GameOver") && 
            (secao == enumMission.FINAL_CONTESTA_CORVO || secao == enumMission.FINAL_CONTESTA_GATO))
        {
            secao = enumMission.FINAL_CONTESTA; // está fora do EspecificaEnum pq não é para chamar a fala de lá e aí ficava mais fácil
        }

        if ((secao == enumMission.NIGHT && !GameManager.instance.mission1AssustaGato) || secao == enumMission.INICIO_GATO)
        {
            GameObject cat = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(2.5f, -0.8f, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }
        else if (secao == enumMission.RESPEITA_MAE)
        {
            EspecificaEnum((int)enumMission.RESPEITA_MAE2);
        }
        else if (secao == enumMission.CONTESTA_MAE)
        {
            EspecificaEnum((int)enumMission.CONTESTA_MAE2);
        }
        else if (secao == enumMission.RESPEITA_MAE2)
        {
            GameObject windowTrigger = GameObject.Find("WindowTrigger").gameObject;
            windowTrigger.tag = "WindowTrigger";
            WindowTrigger trigger = windowTrigger.GetComponent<WindowTrigger>();
            trigger.enabled = true;
            SceneObject sceneObject = windowTrigger.GetComponent<SceneObject>();
            sceneObject.enabled = false;

            if (!Inventory.HasItemType(Inventory.InventoryItems.VELA))
            {
                GameManager.instance.rpgTalk.NewTalk("M2KidRoomSceneVela", "M2KidRoomSceneVelaEnd", GameManager.instance.rpgTalk.txtToParse);
            }
            else if (!Inventory.HasItemType(Inventory.InventoryItems.FOSFORO))
            {
                GameManager.instance.rpgTalk.NewTalk("M2KidRoomSceneFosforo", "M2KidRoomSceneFosforoEnd", GameManager.instance.rpgTalk.txtToParse);
            }
        }
        else if (secao == enumMission.CONTESTA_MAE2)
        {
            if (!Inventory.HasItemType(Inventory.InventoryItems.FACA))
            {
                GameManager.instance.rpgTalk.NewTalk("M2KidRoomSceneFaca", "M2KidRoomSceneFacaEnd", GameManager.instance.rpgTalk.txtToParse);
            }
            else if (!Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
            {
                GameManager.instance.rpgTalk.NewTalk("M2KidRoomSceneTampa", "M2KidRoomSceneTampaEnd", GameManager.instance.rpgTalk.txtToParse);
            }
        }
        else if (secao == enumMission.FINAL_RESPEITA || secao == enumMission.FINAL_CONTESTA)
        {
            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            float portaDefaultY = porta.transform.position.y;
            float posX = porta.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            porta.GetComponent<SceneDoor>().isOpened = false;
            porta.transform.position = new Vector3(porta.transform.position.x - posX, portaDefaultY, porta.transform.position.z);

            if (secao == enumMission.FINAL_RESPEITA)
            {
                // Mini-game vela
                vela = player.transform.Find("Vela").gameObject;
                GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(0.125f, -1.38f, 0), new Vector3(1, 1, 1));
                trigger.name = "VelaTrigger";
                trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
                trigger.GetComponent<BoxCollider2D>().size = new Vector2(1.8f, 1f);

                GameManager.instance.rpgTalk.NewTalk("M2KidRoomSceneRepeat", "M2KidRoomSceneRepeatEnd", GameManager.instance.rpgTalk.txtToParse);
            }
            else if (secao == enumMission.FINAL_CONTESTA)
            {
                // Crow atacando
                GameManager.instance.Print("CORVO");
                crowBabies = GameManager.instance.AddObject("NPCs/CrowBabies", "", new Vector3(-1.97f, 1.42f, -0.5f), new Vector3(3f, 3f, 1));
                crowBabies.GetComponent<CrowBabies>().speed = 0.1f;
                crowBabies.GetComponent<CrowBabies>().timeBirdsFollow = 0.5f;
                var main = crowBabies.transform.Find("BirdEmitterCollider").gameObject.GetComponent<ParticleSystem>().main;
                main.startSpeed = 1.5f;

                GameManager.instance.Invoke("InvokeMission", 5f);
            }
        }

    }

    public override void SetQuartoMae()
	{
		//GameManager.instance.rpgTalk.NewTalk ("M2MomRoomSceneStart", "M2MomRoomSceneEnd");
	}

    public override void SetSala()
    {
        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (secao == enumMission.RESPEITA_MAE2)
        {
            // Vela
            GameManager.instance.CreateScenePickUp("Armario", Inventory.InventoryItems.VELA);
        }
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        GameManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.ENCONTRA_MAE)
        {
            GameManager.instance.rpgTalk.NewTalk ("M2CorridorSceneStart", "M2CorridorSceneEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.RESPEITA_MAE)
        {
            GameManager.instance.mission2ContestaMae = false;
        }
        else if (secao == enumMission.CONTESTA_MAE)
        {
            GameManager.instance.mission2ContestaMae = true;
        }
        else if (secao == enumMission.RESPEITA_MAE2)
        {
            GameManager.instance.rpgTalk.NewTalk("M2Q1C1_2", "M2Q1C1_2End", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.CONTESTA_MAE2)
        {
            GameManager.instance.rpgTalk.NewTalk("M2Q1C0_2", "M2Q1C0_2End", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.FINAL_RESPEITA)
        {
            GameManager.instance.rpgTalk.NewTalk("M2AllObjectsRespeita", "M2AllObjectsRespeitaEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.FINAL_RESPEITA_VELA)
        {
            velaFixa = GameManager.instance.AddObject("Objects/EmptyObject", "", new Vector3(0.125f, -1.1f, 0), new Vector3(2.5f, 2.5f, 1));
            velaFixa.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Inventory/vela");
            velaFixa.GetComponent<SpriteRenderer>().sortingOrder = 140;

            GameObject.Find("Player").gameObject.transform.Find("Fosforo").gameObject.GetComponent<MiniGameObject>().posFlareX = 0.125f;
            GameObject.Find("Player").gameObject.transform.Find("Fosforo").gameObject.GetComponent<MiniGameObject>().posFlareY = -1.05f;
        }
        else if (secao == enumMission.FINAL_RESPEITA_FOSFORO)
        {
            velaFixa.SetActive(false);
            // Vela
            velaFixa = GameObject.Find("velaMesa").gameObject;
            velaFixa.transform.GetChild(0).gameObject.SetActive(true);
            velaFixa.transform.GetChild(1).gameObject.SetActive(true);
            velaFixa.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = 140;
            GameManager.instance.Invoke("InvokeMission", 4f);
        }
        else if (secao == enumMission.FINAL_CONTESTA)
        {
            GameManager.instance.rpgTalk.NewTalk("M2AllObjectsContesta", "M2AllObjectsContestaEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.FINAL_CONTESTA_CORVO)
        {
            GameManager.instance.scenerySounds.PlayBird(1);
            //crowBabies.GetComponent<CrowBabies>().FollowPlayer();
            crowBabies.transform.Find("BirdEmitterCollider").gameObject.SetActive(true);
            GameManager.instance.Invoke("InvokeMission", 40f);
        }
        else if (secao == enumMission.FINAL_CONTESTA_GATO)
        {
            crowBabies.transform.Find("BirdEmitterCollider").gameObject.SetActive(false);
            crowBabies.GetComponent<CrowBabies>().Stop();

            Cat.instance.followWhenClose = false;
            Cat.instance.stopEndPath = true;
            Cat.instance.GetComponent<Cat>().Patrol();

            Vector3 target1 = crowBabies.transform.position;
            Vector3 target2 = new Vector3(-2f, 0.6f, -0.5f);
            Vector3[] targets = { target1, target2 };
            Cat.instance.targets = targets;

            crowBabies.GetComponent<CrowBabies>().Patrol();
            Vector3[] targetsCorvo = { target2 };
            crowBabies.GetComponent<CrowBabies>().targets = targetsCorvo;
            crowBabies.GetComponent<CrowBabies>().speed = 0.6f;

            GameManager.instance.rpgTalk.NewTalk("M2AtaqueContesta", "M2AtaqueContestaEnd", GameManager.instance.rpgTalk.txtToParse);

            GameManager.instance.Invoke("InvokeMission", 2.5f);
        }
        else if (secao == enumMission.FINAL_CONTESTA_ATAQUE)
        {
            GameManager.instance.AddObject("Scenery/Garra", "", new Vector3(-1.48f, 1.81f, 0), new Vector3(0.1f, 0.1f, 1));
            crowBabies.GetComponent<CrowBabies>().DestroyCorvBabies();
            Cat.instance.Stop();
            GameManager.instance.Invoke("InvokeMission", 6f);
        }
        else if (secao == enumMission.FINAL)
        {
            GameManager.instance.ChangeMission(3);
        }
    }

    public override void AreaTriggered(string tag)
    {
        if (tag.Equals("AreaTrigger(Clone)") && (secao == enumMission.INICIO_GATO || secao == enumMission.INICIO_SOZINHO))
        {
            EspecificaEnum((int)enumMission.ENCONTRA_MAE);
        }
        else if (secao == enumMission.FINAL_RESPEITA)
        {
            if (tag.Equals("EnterVelaTrigger"))
            {
                vela.GetComponent<PlaceObject>().inArea = true;
            }
            else if (tag.Equals("ExitVelaTrigger"))
            {
                vela.GetComponent<PlaceObject>().inArea = false;
            }
        }
        else if (secao == enumMission.FINAL_RESPEITA_VELA)
        {
            if (tag.Equals("EnterVelaTrigger"))
            {
                fosforo.GetComponent<MiniGameObject>().activated = true;
            }
            else if (tag.Equals("ExitVelaTrigger"))
            {
                fosforo.GetComponent<MiniGameObject>().activated = false;
            }
        }
    }

    public override void InvokeMission()
    {
        if (secao == enumMission.FINAL_RESPEITA_FOSFORO)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }
        else if (secao == enumMission.FINAL_CONTESTA)
        {
            EspecificaEnum((int)enumMission.FINAL_CONTESTA_CORVO);
        }
        else if (secao == enumMission.FINAL_CONTESTA_CORVO)
        {
            EspecificaEnum((int)enumMission.FINAL_CONTESTA_GATO);
        }
        else if (secao == enumMission.FINAL_CONTESTA_GATO)
        {
            EspecificaEnum((int)enumMission.FINAL_CONTESTA_ATAQUE);
        }
        else if (secao == enumMission.FINAL_CONTESTA_ATAQUE)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }
    }

    public override void InvokeMissionChoice(int id)
    {
        if (secao == enumMission.ENCONTRA_MAE)
        {
            if (id == 0)
            {
                EspecificaEnum((int)enumMission.CONTESTA_MAE);
            }
            else
            {
                EspecificaEnum((int)enumMission.RESPEITA_MAE);
            }
        }
    }

    public override void ForneceDica()
    {
        
    }

}