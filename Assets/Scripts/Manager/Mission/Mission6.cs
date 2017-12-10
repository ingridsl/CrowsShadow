using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission6 : Mission {
    enum enumMission { NIGHT, INICIO, LIGOULANTERNA, DESLIGOULANTERNA, GATOCOMFOLHA,  FINAL };
    enumMission secao;

    public override void InitMission()
	{
		sceneInit = "QuartoMae";
		MissionManager.initMission = true;
		MissionManager.initX = (float) 3;
		MissionManager.initY = (float) 0.2;
		MissionManager.initDir = 3;
		SceneManager.LoadScene(sceneInit, LoadSceneMode.Single);
        secao = enumMission.NIGHT;
        if (Cat.instance != null) Cat.instance.DestroyCat();
        if (Corvo.instance != null) Corvo.instance.DestroyRaven();
        Book.bookBlocked = false;
        MissionManager.instance.invertWorld = true;
        MissionManager.instance.invertWorldBlocked = true;
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
        if (secao == enumMission.INICIO && Flashlight.GetState())
        {
            EspecificaEnum((int)enumMission.LIGOULANTERNA);
        }

        if (secao == enumMission.LIGOULANTERNA && !Flashlight.GetState())
        {
            EspecificaEnum((int)enumMission.DESLIGOULANTERNA);
        }

        if (secao == enumMission.DESLIGOULANTERNA && MissionManager.instance.currentSceneName.Equals("Jardim"))
        {
            EspecificaEnum((int)enumMission.GATOCOMFOLHA);
        }

    }

	public override void SetCorredor()
	{
        MissionManager.instance.scenerySounds.StopSound();
        GameObject portaCozinha = GameObject.Find("DoorToKitchen").gameObject;
        portaCozinha.GetComponent<Collider2D>().isTrigger = false;
        GameObject portaQuarto = GameObject.Find("DoorToKidRoom").gameObject;
        portaQuarto.GetComponent<Collider2D>().isTrigger = false;
        //MissionManager.instance.rpgTalk.NewTalk ("M6CorridorSceneStart", "M6CorridorSceneEnd");
    }

	public override void SetCozinha()
	{
        MissionManager.instance.scenerySounds.PlayDrop();
        //MissionManager.instance.rpgTalk.NewTalk ("M6KitchenSceneStart", "M6KitchenSceneEnd");

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
        //MissionManager.instance.rpgTalk.NewTalk ("M6GardenSceneStart", "M6GardenSceneEnd");
        MissionManager.instance.AddObject("Leafs", "", new Vector3(1.8f, 2.4f, -0.5f), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("catFollower", "", new Vector3(2f, 1.5f, -0.5f), new Vector3(0.15f, 0.15f, 1));
    }

    public override void SetQuartoKid()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M6KidRoomSceneStart", "M6KidRoomSceneEnd");

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
        MissionManager.instance.scenerySounds.StopSound();
        GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
        portaCorredor.GetComponent<Collider2D>().isTrigger = false;

        GameObject camaMae = GameObject.Find("Cama").gameObject;
        camaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/camaMaeDoente");
        MissionManager.instance.rpgTalk.NewTalk("M6MomRoomSceneStart", "M6MomRoomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");

        MissionManager.instance.AddObject("Gloom", "", new Vector3(4.26f, 1.98f, 0), new Vector3(1f, 1f, 1));
    }


    public override void SetSala()
	{
        //MissionManager.instance.AddObject("MovingObject", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        //GameObject.Find("PickUpLanterna").gameObject.SetActive(false);
        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(-3.878462f, -0.537204f, 0), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(-5.441667f, -0.802685f, 0), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(-2.008199f, 0.3697177f, 0), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(-0.2775087f, -1.360973f, 0), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(2.178955f, 0.1743171f, 0), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(2.402271f, -1.27723f, 0), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(-3.013116f, -1.388887f, 0), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(-6.418669f, -0.02108344f, 0), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(3.128043f, 1.067577f, 0), new Vector3(1f, 1f, 1));
        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(0.6436655f, 1.430463f, 0), new Vector3(1f, 1f, 1));
    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        MissionManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.LIGOULANTERNA)
        {
            MissionManager.instance.rpgTalk.NewTalk("M6AfterFlashlight", "M6AfterFlashlightEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");

        }
        if (secao == enumMission.DESLIGOULANTERNA)
        {
            MissionManager.instance.rpgTalk.NewTalk("M6AfterFlashlightShutdown", "M6AfterFlashlightShutdownEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");
            GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
            portaCorredor.GetComponent<Collider2D>().isTrigger = true;
        }
        if (secao == enumMission.GATOCOMFOLHA)
        {
            MissionManager.instance.rpgTalk.NewTalk("M6GardenSceneStart", "M6GardenSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");

        }
    }

    public void AddCountLivingroomDialog(){
		MissionManager.instance.countLivingroomDialog++;

	}
}