using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission6 : Mission {
    enum enumMission { NIGHT, INICIO, LIGOULANTERNA, DESLIGOULANTERNA, SALA, GATOCOMFOLHA, DICA,  FINAL };
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

        if (secao == enumMission.DESLIGOULANTERNA && MissionManager.instance.currentSceneName.Equals("Sala"))
        {
            EspecificaEnum((int)enumMission.SALA);
        }
        if(secao == enumMission.SALA && Spirit.maxEvilKilled < 2)
        {
            EspecificaEnum((int)enumMission.DICA);
        }
        if ((secao == enumMission.SALA || secao == enumMission.DICA)) //&& Book.pages[2] == true)
        {
            GameObject portaJardim = GameObject.Find("DoorToGarden").gameObject;
            portaJardim.GetComponent<Collider2D>().isTrigger = true;
        }
        if ((secao == enumMission.SALA || secao == enumMission.DICA ) 
            && MissionManager.instance.currentSceneName.Equals("Jardim"))
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
        GameObject portaJardim = GameObject.Find("DoorToGarden").gameObject;
        portaJardim.GetComponent<Collider2D>().isTrigger = false;
        //MissionManager.instance.AddObject("MovingObject", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        //GameObject.Find("PickUpLanterna").gameObject.SetActive(false);
        GameObject mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
        mainLight.transform.Rotate(new Vector3(20, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        MissionManager.instance.AddObject("Pagina", "", new Vector3(6.2f, 0f, 0), new Vector3(0.535f, 0.483f, 1));

        float x = -7.5f, y = -0.2f;
        int sequencia = 0, aux2 = -1;
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 18; i++)
            {

                int aux = Random.Range(0, 3);
                if(aux2 == aux)
                {
                    sequencia++;
                    if(sequencia == 3)
                    {
                        aux = (aux + 1) % 4; 
                    }
                }
                else
                {
                    sequencia = 0;
                }
                switch (aux)
                {
                    case 1:
                        MissionManager.instance.AddObject("GoodSpiritAux", "", new Vector3(x, y, 0), new Vector3(1f, 1f, 1));
                        break;
                    case 2:
                        MissionManager.instance.AddObject("EvilSpiritAux", "", new Vector3(x, y, 0), new Vector3(1f, 1f, 1));
                        break;
                    default:
                        MissionManager.instance.AddObject("KillerSpirit", "", new Vector3(x, y, 0), new Vector3(1f, 1f, 1));
                        break;
                }
                aux2 = aux;
                x += 0.9f;
            }
            y -= 1f;
            x = -7.5f;
        }
        Spirit.newHealth = 2;
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
        if (secao == enumMission.SALA)
        {
            MissionManager.instance.rpgTalk.NewTalk("M6LivingroomSceneStart", "M6LivingroomSceneEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");
        }
        if (secao == enumMission.DICA)
        {
            MissionManager.instance.rpgTalk.NewTalk("M6LivingroomTipStart", "M6LivingroomTipEnd", MissionManager.instance.rpgTalk.txtToParse, MissionManager.instance, "AddCountLivingroomDialog");
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