using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission5 : Mission {
    enum enumMission { NIGHT, INICIO, DICA,  FINAL };
    enumMission secao;

    public override void InitMission()
	{
		sceneInit = "Jardim";
		MissionManager.initMission = true;
		MissionManager.initX = (float) 3;
		MissionManager.initY = (float) 0.2;
		MissionManager.initDir = 3;
		SceneManager.LoadScene(sceneInit, LoadSceneMode.Single);
        secao = enumMission.NIGHT;
        if (Cat.instance != null) Cat.instance.DestroyCat();
        if (Corvo.instance != null) Corvo.instance.DestroyRaven();
        MissionManager.instance.invertWorldBlocked = false;
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
        if (secao == enumMission.DICA)
        {
            MissionManager.instance.rpgTalk.NewTalk("Dica5", "Dica5End");
        }
    }

	public override void SetCorredor()
	{
        MissionManager.instance.scenerySounds.StopSound();

      
        //MissionManager.instance.rpgTalk.NewTalk ("M5CorridorSceneStart", "M5CorridorSceneEnd");
    }

	public override void SetCozinha()
	{
        MissionManager.instance.scenerySounds.PlayDrop();
        //MissionManager.instance.rpgTalk.NewTalk ("M5KitchenSceneStart", "M5KitchenSceneEnd");

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
        SpiritManager.canSummom = true;
        if (MissionManager.instance.countGardenDialog == 0)
        {
            MissionManager.instance.rpgTalk.NewTalk("M5GardenSceneStart", "M5GardenSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountGardenDialog");
        }
    }

	public override void SetQuartoKid()
	{
        //MissionManager.instance.rpgTalk.NewTalk ("M5KidRoomSceneStart", "M5KidRoomSceneEnd");

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
		//MissionManager.instance.rpgTalk.NewTalk ("M5MomRoomSceneStart", "M5MomRoomSceneEnd");
	}


	public override void SetSala()
	{
		//MissionManager.instance.AddObject("MovingObject", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
		//GameObject.Find("PickUpLanterna").gameObject.SetActive(false);	
		//	MissionManager.instance.rpgTalk.NewTalk ("M5LivingRoomSceneStart", "M5LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");
	}

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        MissionManager.instance.Print("SECAO: " + secao);
    }

    public void AddCountGardenDialog(){
		MissionManager.instance.countGardenDialog++;

	}
}