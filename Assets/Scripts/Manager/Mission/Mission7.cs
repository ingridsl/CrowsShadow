using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission7 : Mission {
    enum enumMission { NIGHT, INICIO, APARECELUZ, LUZCHEIA, LUZANDANDO, LUZCORREDOR, LUZCORREDORANDANDO, LUZSALA,
        LUZSALAANDANDO, LUZFALOUGATO, LUZCORREDOR2, LUZCORREDOR2ANDANDO, LUZKID, LUZKIDANDANDO, FINAL };
    enumMission secao;

    //Luz do quarto da mãe
    GameObject luz1;
    //Luz corredor
    GameObject[] luz2;
    //Luz quarto criança
    GameObject[] luz3;
    //Luz sala
    GameObject[] luz4;
    //Luz corredor 2
    GameObject luz5;
    private float timeToDeath = 0.5f;

    float portaKidDefaultY, portaKidDefaultX, portaAlleyDefaultY, portaAlleyDefaultX;

    public override void InitMission()
	{
		sceneInit = "QuartoMae";
		MissionManager.initMission = true;
		MissionManager.initX = (float) 3.5;
		MissionManager.initY = (float) 1.7;
		MissionManager.initDir = 3;
		SceneManager.LoadScene(sceneInit, LoadSceneMode.Single);
        secao = enumMission.NIGHT;
        Book.bookBlocked = false;

        MissionManager.instance.invertWorld = false;
        MissionManager.instance.invertWorldBlocked = false;

        SetInitialSettings();

        // Adiciona as páginas
        Book.pages[0] = Book.pages[1] = Book.pages[2] = Book.pages[3] = true;
        Book.pages[4] = false;
        Book.pageQuantity = 4;
    }

	public override void UpdateMission() //aqui coloca as ações do update específicas da missão
	{
        if (secao == enumMission.NIGHT)
        {
            if (!MissionManager.instance.GetMissionStart())
            {
                EspecificaEnum((int)enumMission.INICIO);
            }
        }

        if(secao == enumMission.INICIO && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.APARECELUZ);
        }
        if(secao == enumMission.APARECELUZ)
        {
            if(luz1.GetComponent<Light>().range < 0.8)
            {
                luz1.GetComponent<Light>().range += Time.deltaTime;
            }
            else
            {
                EspecificaEnum((int)enumMission.LUZCHEIA);
            }
        }
        if(secao == enumMission.LUZCHEIA && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.LUZANDANDO);
        }

        if (secao == enumMission.LUZANDANDO && luz1 != null)
        {
            if (luz1.GetComponent<HelpingLight>().stoped)
            {
                GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
                portaCorredor.GetComponent<Collider2D>().isTrigger = true;
                portaCorredor.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-opened");
                portaCorredor.transform.position = new Vector3(portaAlleyDefaultX, portaAlleyDefaultY, portaCorredor.transform.position.z);
            }
            if (!luz1.GetComponent<HelpingLight>().PlayerInside)
            {
                KillInDarkness();
            }
            else
            {
                timeToDeath = 0.5f;
            }
        }

        //////////////// CORREDOR //////////////////

        if (secao == enumMission.LUZANDANDO && MissionManager.instance.currentSceneName.Equals("Corredor"))
        {
            EspecificaEnum((int)enumMission.LUZCORREDOR);
        }
        if (secao == enumMission.LUZCORREDOR && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.LUZCORREDORANDANDO);
        }

        if (secao == enumMission.LUZCORREDORANDANDO && luz2[0] != null && luz2[1] != null)
        {
            if (luz2[0].GetComponent<HelpingLight>().stoped)
            {
                GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
                portaSala.GetComponent<Collider2D>().isTrigger = true;
            }
            if (luz2[1].GetComponent<HelpingLight>().stoped)
            {
                GameObject portaQuarto = GameObject.Find("DoorToKidRoom").gameObject;
                portaQuarto.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-opened");
                portaQuarto.GetComponent<Collider2D>().isTrigger = true;
                portaQuarto.transform.position = new Vector3(portaKidDefaultX, portaKidDefaultY, portaQuarto.transform.position.z);
            }

            if (!luz2[0].GetComponent<HelpingLight>().PlayerInside &&
                !luz2[1].GetComponent<HelpingLight>().PlayerInside)
            {
                KillInDarkness();
            }
            else
            {
                timeToDeath = 0.5f;
            }
        }
        if ((secao == enumMission.LUZCORREDORANDANDO || secao == enumMission.LUZCORREDOR2ANDANDO)
            && MissionManager.instance.currentSceneName.Equals("QuartoKid"))
        {
            if (secao == enumMission.LUZCORREDORANDANDO)
            {
                MissionManager.instance.pathBird += 4;
            }
            else
            {
                MissionManager.instance.pathCat += 4;
            }
            EspecificaEnum((int)enumMission.LUZKID);
        }
        if (secao == enumMission.LUZKID && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.LUZKIDANDANDO);
        }
        if(secao == enumMission.LUZKIDANDANDO && Book.pageQuantity == 5)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }
        if (secao == enumMission.FINAL && !MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.ChangeMission(8);
        }

        /////////////SALA/////////////////

        if (secao == enumMission.LUZCORREDORANDANDO && MissionManager.instance.currentSceneName.Equals("Sala"))
        {
            EspecificaEnum((int)enumMission.LUZSALA);
        }
        if (secao == enumMission.LUZSALA && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.LUZSALAANDANDO);
        }
        if ((secao == enumMission.LUZSALAANDANDO || secao == enumMission.LUZFALOUGATO)
            && luz4[0] != null && luz4[1] != null)
        {
            if (!luz4[0].GetComponent<HelpingLight>().PlayerInside &&
                !luz4[1].GetComponent<HelpingLight>().PlayerInside)
            {
                KillInDarkness();
            }
            else
            {
                timeToDeath = 0.5f;
            }

            if (secao == enumMission.LUZSALAANDANDO && luz4[1].GetComponent<HelpingLight>().PlayerInside)
            {
                EspecificaEnum((int)enumMission.LUZFALOUGATO);
            }
        }

        // CORREDOR 2
        if(secao == enumMission.LUZFALOUGATO && MissionManager.instance.currentSceneName.Equals("Corredor"))
        {
            EspecificaEnum((int)enumMission.LUZCORREDOR2);
        }

        if (secao == enumMission.LUZCORREDOR2 && !MissionManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.LUZCORREDOR2ANDANDO);
        }

        if (secao == enumMission.LUZCORREDOR2ANDANDO && luz5 != null)
        {
            if (luz5.GetComponent<HelpingLight>().stoped)
            {
                GameObject portaQuarto = GameObject.Find("DoorToKidRoom").gameObject;
                portaQuarto.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-opened");
                portaQuarto.GetComponent<Collider2D>().isTrigger = true;
                portaQuarto.transform.position = new Vector3(portaKidDefaultX, portaKidDefaultY, portaQuarto.transform.position.z);
            }
            if (!luz5.GetComponent<HelpingLight>().PlayerInside)             
            {
                KillInDarkness();
            }
            else
            {
                timeToDeath = 0.5f;
            }
        }
    }


    public override void SetCorredor()
	{
        MissionManager.instance.scenerySounds.StopSound();

        if (MissionManager.instance.previousSceneName.Equals("GameOver"))
        {
            if (secao == enumMission.LUZCORREDOR2 || secao == enumMission.LUZCORREDOR2ANDANDO)
            {
                EspecificaEnum((int)enumMission.LUZCORREDOR2);
            }
            else {
                EspecificaEnum((int)enumMission.LUZCORREDOR);
            }
        }

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

        // porta corredor
        GameObject portaCorredor = GameObject.Find("DoorToKitchen").gameObject;
        portaCorredor.GetComponent<Collider2D>().isTrigger = false;

        // porta sala
        GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
        portaSala.GetComponent<Collider2D>().isTrigger = false;

        // porta quarto criança
        GameObject portaQuarto = GameObject.Find("DoorToKidRoom").gameObject;
        portaKidDefaultY = portaQuarto.transform.position.y;
        portaKidDefaultX = portaQuarto.transform.position.x;
        float posKidX = portaQuarto.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        portaQuarto.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        portaQuarto.GetComponent<Collider2D>().isTrigger = false;
        portaQuarto.transform.position = new Vector3(portaQuarto.transform.position.x + posKidX, portaKidDefaultY, portaQuarto.transform.position.z);

        // porta quarto mãe
        GameObject portaQuartoMae = GameObject.Find("DoorToMomRoom").gameObject;
        float portaMaeDefaultY = portaQuartoMae.transform.position.y;
        float portaMaeDefaultX = portaQuartoMae.transform.position.x;
        float posX = portaQuartoMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        portaQuartoMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        portaQuartoMae.GetComponent<Collider2D>().isTrigger = false;
        portaQuartoMae.transform.position = new Vector3(portaQuartoMae.transform.position.x + posX, portaMaeDefaultY, portaQuartoMae.transform.position.z);
    }

	public override void SetCozinha()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        MissionManager.instance.scenerySounds.PlayDrop();

        // Panela para caso ainda não tenha
        if (!Inventory.HasItemType(Inventory.InventoryItems.TAMPA))
        {
            GameObject panela = GameObject.Find("Panela").gameObject;
            panela.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/panela_tampa");
            panela.GetComponent<ScenePickUpObject>().enabled = true;
        }
    }

	public override void SetJardim()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
    }

    public override void SetQuartoKid()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        MissionManager.instance.rpgTalk.NewTalk("M7KidRoomSceneStart", "M7KidRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountKidRoomDialog");

        GameObject porta = GameObject.Find("DoorToAlley").gameObject;
        float portaDefaultY = porta.transform.position.y;
        float portaDefaultX = porta.transform.position.x;
        float posMaeX = porta.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        porta.GetComponent<Collider2D>().isTrigger = false;
        porta.transform.position = new Vector3(porta.transform.position.x + posMaeX, portaDefaultY, porta.transform.position.z);

        if (MissionManager.instance.mission2ContestaMae)
        {
            // Arranhao
            MissionManager.instance.AddObject("Garra", "", new Vector3(-1.48f, 1.81f, 0), new Vector3(0.1f, 0.1f, 1));
        }
        else
        {
            // Vela
            GameObject velaFixa = MissionManager.instance.AddObject("EmptyObject", "", new Vector3(0.125f, -1.1f, 0), new Vector3(2.5f, 2.5f, 1));
            velaFixa.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Inventory/vela_acesa1");
            velaFixa.GetComponent<SpriteRenderer>().sortingOrder = 140;
            GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true);
        }

        MissionManager.instance.AddObject("HelpingLight", "", new Vector3(1f, -0.3f, -0.4f), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("HelpingLight", "", new Vector3(1.73f, -0.87f, -0.2f), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("HelpingLight", "", new Vector3(2.38f, -0.3f, -0.4f), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("HelpingLight", "", new Vector3(1.73f, -1.3f, -0.2f), new Vector3(1f, 1f, 1));
        luz3 = GameObject.FindGameObjectsWithTag("HelpingLight");

        MissionManager.instance.AddObject("Pagina", "", new Vector3(1.7f, -0.6f, 0), new Vector3(0.535f, 0.483f, 1));    
    }

	public override void SetQuartoMae()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameObject porta = GameObject.Find("DoorToAlley").gameObject;
        portaAlleyDefaultY = porta.transform.position.y;
        portaAlleyDefaultX = porta.transform.position.x;
        float posX = porta.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        porta.GetComponent<Collider2D>().isTrigger = false;
        porta.transform.position = new Vector3(porta.transform.position.x + posX, portaAlleyDefaultY, porta.transform.position.z);

        GameObject Luminaria = GameObject.Find("Luminaria").gameObject;
        Luminaria.GetComponent<Light>().enabled = false;

        GameObject camaMae = GameObject.Find("Cama").gameObject;
        camaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/camaMaeDoente");

        if (MissionManager.instance.previousSceneName.Equals("GameOver"))
        {
            MissionManager.instance.ChangeMission(7);
        }
    }


    public override void SetSala()
	{
        MissionManager.instance.scenerySounds.PlayCat(3);

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
        portaCorredor.GetComponent<Collider2D>().isTrigger = false;

        GameObject portaG = GameObject.Find("DoorToGarden").gameObject;
        portaG.GetComponent<Collider2D>().isTrigger = false;

        //MissionManager.instance.AddObject("MovingObject", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        //GameObject.Find("PickUpLanterna").gameObject.SetActive(false);	
        //	MissionManager.instance.rpgTalk.NewTalk ("M7LivingRoomSceneStart", "M7LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");

        if (MissionManager.instance.previousSceneName.Equals("GameOver"))
        {
            EspecificaEnum((int)enumMission.LUZSALA);
        }
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        MissionManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.INICIO)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7MomRoomSceneStart", "M7MomRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }
        if (secao == enumMission.APARECELUZ)
        {
            MissionManager.instance.AddObject("HelpingLight", "", new Vector3(3.5f, 1.7f, -0.03f), new Vector3(1f, 1f, 1));
            luz1 = GameObject.Find("HelpingLight(Clone)").gameObject;
            luz1.GetComponent<Light>().range = 0;
        }
        if (secao == enumMission.LUZCHEIA)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7LightFull", "M7LightFullEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }
        if (secao == enumMission.LUZANDANDO)
        {
            luz1.GetComponent<HelpingLight>().targets = new Vector3[3];
            luz1.GetComponent<HelpingLight>().targets[0] = new Vector3(3.5f, -1, -0.03f);
            luz1.GetComponent<HelpingLight>().targets[1] = new Vector3(-0.9f, -2.07f, -0.03f);
            luz1.GetComponent<HelpingLight>().targets[2] = new Vector3(-3.8f, -0.23f, -0.03f);

            luz1.GetComponent<HelpingLight>().active = true;
            luz1.GetComponent<HelpingLight>().stopEndPath = true;
        }
        if (secao == enumMission.LUZCORREDOR)
        {
            MissionManager.instance.scenerySounds.PlayCat(3);

            MissionManager.instance.AddObject("HelpingLight", "", new Vector3(-2.083f, -0.372f, -0.03f), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("HelpingLight", "", new Vector3(-1.14f, -0.372f, -0.03f), new Vector3(1f, 1f, 1));
            luz2 = GameObject.FindGameObjectsWithTag("HelpingLight");

            luz2[0].GetComponent<Light>().color = new Color(0.8f,0.1f,0.1f);
            luz2[0].GetComponent<Light>().intensity = 30;
            MissionManager.instance.rpgTalk.NewTalk("M7CorridorSceneStart", "M7CorridorSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }
        if (secao == enumMission.LUZCORREDORANDANDO)
        {
            luz2[0].GetComponent<HelpingLight>().targets = new Vector3[1];
            luz2[0].GetComponent<HelpingLight>().targets[0] = new Vector3(-9.8f, -0.62f, -0.03f);
            luz2[0].GetComponent<HelpingLight>().active = true;
            luz2[0].GetComponent<HelpingLight>().stopEndPath = true;

            luz2[1].GetComponent<HelpingLight>().targets = new Vector3[1];
            luz2[1].GetComponent<HelpingLight>().targets[0] = new Vector3(11.88f, -0.462f, -0.03f);
            luz2[1].GetComponent<HelpingLight>().active = true;
            luz2[1].GetComponent<HelpingLight>().stopEndPath = true;
        }

        if (secao == enumMission.LUZKID)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7KidRoomSceneStart", "M7KidRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }

        if (secao == enumMission.LUZKIDANDANDO) {
            Vector3 aux1 = new Vector3(1f, -0.3f, -0.4f);
            Vector3 aux2 = new Vector3(1.73f, -0.87f, -0.2f);
            Vector3 aux3 = new Vector3(2.38f, -0.3f, -0.4f);
            Vector3 aux4 = new Vector3(1.73f, -1.3f, -0.2f);

            luz3[0].GetComponent<HelpingLight>().targets = new Vector3[4];
            luz3[1].GetComponent<HelpingLight>().targets = new Vector3[4];
            luz3[2].GetComponent<HelpingLight>().targets = new Vector3[4];
            luz3[3].GetComponent<HelpingLight>().targets = new Vector3[4];

            luz3[0].GetComponent<HelpingLight>().targets[0] = aux2; luz3[0].GetComponent<HelpingLight>().targets[1] = aux3;
            luz3[0].GetComponent<HelpingLight>().targets[2] = aux4; luz3[0].GetComponent<HelpingLight>().targets[3] = aux1;

            luz3[1].GetComponent<HelpingLight>().targets[0] = aux3; luz3[1].GetComponent<HelpingLight>().targets[1] = aux4;
            luz3[1].GetComponent<HelpingLight>().targets[2] = aux1; luz3[1].GetComponent<HelpingLight>().targets[3] = aux2;

            luz3[2].GetComponent<HelpingLight>().targets[0] = aux4; luz3[2].GetComponent<HelpingLight>().targets[1] = aux1;
            luz3[2].GetComponent<HelpingLight>().targets[2] = aux2; luz3[2].GetComponent<HelpingLight>().targets[3] = aux3;

            luz3[3].GetComponent<HelpingLight>().targets[0] = aux1; luz3[3].GetComponent<HelpingLight>().targets[1] = aux2;
            luz3[3].GetComponent<HelpingLight>().targets[2] = aux3; luz3[3].GetComponent<HelpingLight>().targets[3] = aux4;

            luz3[0].GetComponent<HelpingLight>().active = true;
            luz3[1].GetComponent<HelpingLight>().active = true;
            luz3[2].GetComponent<HelpingLight>().active = true;
            luz3[3].GetComponent<HelpingLight>().active = true;
        }

        if (secao == enumMission.LUZSALA)
        {
            MissionManager.instance.AddObject("catFollower", "", new Vector3(0.76f, 0.5f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            GameObject catAux = GameObject.Find("catFollower(Clone)").gameObject;
            catAux.GetComponent<Cat>().followWhenClose = false;

            MissionManager.instance.AddObject("HelpingLight", "", new Vector3(-3.17f, 1f, -0.03f), new Vector3(1f, 1f, 1));
            MissionManager.instance.AddObject("HelpingLight", "", new Vector3(0.76f, 0.5f, -0.03f), new Vector3(1f, 1f, 1));

            luz4 = GameObject.FindGameObjectsWithTag("HelpingLight");

            luz4[0].GetComponent<Light>().color = new Color(0, 0.5f, 0.5f);
            luz4[1].GetComponent<Light>().color = new Color(0,0.5f,0);
            luz4[1].GetComponent<Light>().range *= 0.7f;
            luz4[1].GetComponent<CircleCollider2D>().radius = 0.2f;
            luz4[0].GetComponent<Light>().intensity = 30;
            luz4[1].GetComponent<Light>().intensity = 30;

            MissionManager.instance.rpgTalk.NewTalk("M7LivingroomSceneStart", "M7LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");

        }

        if (secao == enumMission.LUZSALAANDANDO)
        {
            luz4[0].GetComponent<HelpingLight>().targets = new Vector3[7];

            luz4[0].GetComponent<HelpingLight>().targets[0] = new Vector3(-3.15f, 1.29f, -0.03f);
            luz4[0].GetComponent<HelpingLight>().targets[1] = new Vector3(-3.15f, -0.33f, -0.03f);
            luz4[0].GetComponent<HelpingLight>().targets[2] = new Vector3(-6f, -0.33f, -0.03f);
            luz4[0].GetComponent<HelpingLight>().targets[3] = new Vector3(-6f, -1.4f, -0.03f);
            luz4[0].GetComponent<HelpingLight>().targets[4] = new Vector3(4.43f, -1.4f, -0.03f);
            luz4[0].GetComponent<HelpingLight>().targets[5] = new Vector3(4f, 0.32f, -0.03f);
            luz4[0].GetComponent<HelpingLight>().targets[6] = new Vector3(0.76f, 0.29f, -0.03f);

            luz4[0].GetComponent<HelpingLight>().speed = 1f;
            luz4[0].GetComponent<HelpingLight>().active = true;
        }

        if(secao == enumMission.LUZFALOUGATO)
        {
            MissionManager.instance.scenerySounds.PlayCat(3);

            MissionManager.instance.rpgTalk.NewTalk("M7TalkedAtze", "M7TalkedAtzeEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");

            GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
            portaCorredor.GetComponent<Collider2D>().isTrigger = true;
        }

        if(secao == enumMission.LUZCORREDOR2)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7Corridor2", "M7Corridor2End", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");

            MissionManager.instance.AddObject("HelpingLight", "", new Vector3(-9.85f, -1f, -0.03f), new Vector3(1f, 1f, 1));
            luz5 = GameObject.Find("HelpingLight(Clone)").gameObject;
            luz5.GetComponent<Light>().color = new Color(0.5f, 0.5f, 0);
            luz5.GetComponent<Light>().intensity = 30;
        }
        if (secao == enumMission.LUZCORREDOR2ANDANDO)
        {
            luz5.GetComponent<HelpingLight>().targets = new Vector3[2];

            luz5.GetComponent<HelpingLight>().targets[0] = new Vector3(-9.85f, -0.35f, -0.03f);
            luz5.GetComponent<HelpingLight>().targets[1] = new Vector3(12f, -0.35f, -0.03f);
            luz5.GetComponent<HelpingLight>().active = true;
            luz5.GetComponent<HelpingLight>().stopEndPath = true;
            luz5.GetComponent<HelpingLight>().speed = 1.5f;
        }

        if (secao == enumMission.FINAL)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7PagePicked", "M7PagePickedEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }
    }

    private void KillInDarkness()
    {
        timeToDeath -= Time.deltaTime;
        if (timeToDeath <= 0)
        {
            MissionManager.instance.GameOver();
        }
        
    }
}