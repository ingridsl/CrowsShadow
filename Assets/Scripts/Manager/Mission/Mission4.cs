using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission4 : Mission {
    enum enumMission { NIGHT, INICIO, GATO_CORREDOR, FRENTE_CRIADO, GRANDE_BARULHO, VASO_GATO, VASO_SOZINHO, INDICACAO_NECESSIDADE, FINAL };
    enumMission secao;

    public override void InitMission()
	{
		sceneInit = "QuartoKid";
		MissionManager.initMission = true;
        MissionManager.initX = (float)-2.5;
        MissionManager.initY = (float)0.7;
        MissionManager.initDir = 1;
		SceneManager.LoadScene(sceneInit, LoadSceneMode.Single);
        secao = enumMission.NIGHT;
        if (Cat.instance != null) Cat.instance.DestroyCat();
    }

	public override void UpdateMission() //aqui coloca as ações do update específicas da missão
	{
        if (secao == enumMission.NIGHT)
        {
            if (!MissionManager.instance.GetMissionStart())
            {
                EspecificaEnum((int)enumMission.INICIO);
                MissionManager.instance.rpgTalk.NewTalk("M4KidRoomSceneStart", "M4KidRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountKidRoomDialog");
           }
        }
        if (secao == enumMission.INICIO && !MissionManager.instance.rpgTalk.isPlaying)
        {
            Cat.instance.speed = 1f;
        }
    }

	public override void SetCorredor()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M4CorridorSceneStart", "M4CorridorSceneEnd");

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z
        
        // Quarto da mãe bloqueado
        GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
        portaMae.GetComponent<Collider2D>().isTrigger = false;

        if (secao == enumMission.INICIO || secao == enumMission.GATO_CORREDOR)
        {
            GameObject trigger = MissionManager.instance.AddObject("AreaTrigger", "", new Vector3(1.3f, 0.4f, 1), new Vector3(1, 1, 1));
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
            EspecificaEnum((int)enumMission.GATO_CORREDOR);
        }


    }

    public override void SetCozinha()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M4KitchenSceneStart", "M4KitchenSceneEnd");

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z

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
        //MissionManager.instance.rpgTalk.NewTalk ("M4GardenSceneStart", "M4GardenSceneEnd");

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z
    }

    public override void SetQuartoKid()
	{
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z
        

    }

    public override void SetQuartoMae()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M4MomRoomSceneStart", "M4MomRoomSceneEnd");

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z
    }


    public override void SetSala()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M4LivingRoomSceneStart", "M4LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");

        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        //GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(true); //utilizar AreaLight para cenas de dia, variar Z
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        MissionManager.instance.Print("SECAO: " + secao);
        if (secao == enumMission.NIGHT || secao == enumMission.INICIO)
        {

            Cat.instance.Patrol();
            Cat.instance.ChangePosition(-1f, 0.5f);
            Transform aux = new GameObject().transform;
            aux.position = new Vector3(1.8f, 0.8f, -0.5f);
            Transform[] catPos = { aux };
            Cat.instance.targets = catPos;
            Cat.instance.speed = 0.3f;
            Cat.instance.destroyEndPath = true;

        }
        else if (secao == enumMission.GATO_CORREDOR)
        {
            GameObject cat = MissionManager.instance.AddObject("catFollower", "", new Vector3(10f, -0.2f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().Patrol();
            Transform aux = new GameObject().transform;
            aux.position = new Vector3(7f, -0.1f, -0.5f);
            Transform[] catPos = { aux };
            cat.GetComponent<Cat>().targets = catPos;
            cat.GetComponent<Cat>().speed = 1.6f;
            cat.GetComponent<Cat>().stopEndPath = true;
        }
        else if (secao == enumMission.FRENTE_CRIADO)
        {
            MissionManager.instance.rpgTalk.NewTalk("frenteCriadoStart", "frenteCriadoEnd");
        }
        else if (secao == enumMission.GRANDE_BARULHO)
        {
            MissionManager.instance.rpgTalk.NewTalk("GrandeBarulhoStart", "GrandeBarulhoEnd");
        }
        else if (secao == enumMission.INDICACAO_NECESSIDADE)
        {
            MissionManager.instance.rpgTalk.NewTalk("IndicarNecessidade", "IndicarNecessidadeEnd");
        }
        else if (secao == enumMission.FINAL)
        {
            MissionManager.instance.rpgTalk.NewTalk("Final", "FinalEnd");
        }
    }

    public void AddCountKidRoomDialog()
    {
        MissionManager.instance.countKidRoomDialog++;
    }
}