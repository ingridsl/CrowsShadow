using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission7 : Mission {
    enum enumMission { NIGHT, INICIO, APARECELUZ, LUZCHEIA, LUZANDANDO, FINAL };
    enumMission secao;
    GameObject luz1;
    private float timeToDeath = 1.5f;

    public override void InitMission()
	{
		sceneInit = "QuartoMae";
		MissionManager.initMission = true;
		MissionManager.initX = (float) 3.5;
		MissionManager.initY = (float) 1.7;
		MissionManager.initDir = 3;
		SceneManager.LoadScene(sceneInit, LoadSceneMode.Single);
        secao = enumMission.NIGHT;
        if (Cat.instance != null) Cat.instance.DestroyCat();
        if (Corvo.instance != null) Corvo.instance.DestroyRaven();
        Book.bookBlocked = false;

        MissionManager.instance.invertWorld = false;
        MissionManager.instance.invertWorldBlocked = false;

        Book.pages[0] = true;
        Book.pages[1] = true;
        Book.pages[2] = true;
        Book.pages[3] = true;
        Book.pageQuantity = 4;


        if (MissionManager.instance.rpgTalk.isPlaying)
        {
            MissionManager.instance.rpgTalk.EndTalk();
        }
    }

	public override void UpdateMission() //aqui coloca as ações do update específicas da missão
	{
        if (secao != enumMission.NIGHT && secao != enumMission.INICIO 
            && secao != enumMission.APARECELUZ && secao != enumMission.LUZCHEIA && !HelpingLight.PlayerInside)
        {
            timeToDeath -= Time.deltaTime;
        }
        else
        {
            timeToDeath = 1.5f;
        }

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

        if (secao == enumMission.LUZANDANDO && luz1.GetComponent<HelpingLight>().stoped)
        {
            GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
            portaCorredor.GetComponent<Collider2D>().isTrigger = true;
        }
    }

	public override void SetCorredor()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        MissionManager.instance.scenerySounds.StopSound();

        //MissionManager.instance.rpgTalk.NewTalk ("M7CorridorSceneStart", "M7CorridorSceneEnd");
    }

	public override void SetCozinha()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        MissionManager.instance.scenerySounds.PlayDrop();

        //MissionManager.instance.rpgTalk.NewTalk ("M7KitchenSceneStart", "M7KitchenSceneEnd");

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

        //MissionManager.instance.rpgTalk.NewTalk ("M7GardenSceneStart", "M7GardenSceneEnd");
    }

    public override void SetQuartoKid()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (MissionManager.instance.countLivingroomDialog == 0)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7KidRoomSceneStart", "M7KidRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountKidRoomDialog");
        }

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
    }

	public override void SetQuartoMae()
	{
        GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
        portaCorredor.GetComponent<Collider2D>().isTrigger = false;
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject Luminaria = GameObject.Find("Luminaria").gameObject;
        Luminaria.GetComponent<Light>().enabled = false;

        GameObject camaMae = GameObject.Find("Cama").gameObject;
        camaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/camaMaeDoente");
    }


    public override void SetSala()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        //MissionManager.instance.AddObject("MovingObject", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        //GameObject.Find("PickUpLanterna").gameObject.SetActive(false);	
        //	MissionManager.instance.rpgTalk.NewTalk ("M7LivingRoomSceneStart", "M7LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");
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
            MissionManager.instance.AddObject("HelpingLight", "", new Vector3(3.5f, 1.7f, 0), new Vector3(1f, 1f, 1));
            luz1 = GameObject.Find("HelpingLight(Clone)").gameObject;
            luz1.GetComponent<Light>().range = 0;
        }

        if (secao == enumMission.APARECELUZ)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7MomRoomSceneStart", "M7MomRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }
        if(secao == enumMission.LUZCHEIA)
        {
            MissionManager.instance.rpgTalk.NewTalk("M7MomRoomSceneStart", "M7MomRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountMomRoomDialog");
        }
        if (secao == enumMission.LUZANDANDO)
        {
            luz1.GetComponent<HelpingLight>().targets = new Vector3[3];
            luz1.GetComponent<HelpingLight>().targets[0] = new Vector3(3.5f, -1, 0);
            luz1.GetComponent<HelpingLight>().targets[1] = new Vector3(-0.9f, -2.07f, 0);
            luz1.GetComponent<HelpingLight>().targets[2] = new Vector3(-3.8f, -0.23f, 0);

            luz1.GetComponent<HelpingLight>().active = true;
            luz1.GetComponent<HelpingLight>().stopEndPath = true;
        }
    }

    public void AddCountKidRoomDialog(){
		MissionManager.instance.countKidRoomDialog++;
	}
    public void AddCountMomRoomDialog()
    {
        MissionManager.instance.countMomRoomDialog++;
    }
    public void AddCountCorridorDialog()
    {
        MissionManager.instance.countCorridorDialog++;
    }
}