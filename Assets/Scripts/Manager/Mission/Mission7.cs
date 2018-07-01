using UnityEngine;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission7 : Mission
{
    enum enumMission { NIGHT, INICIO, PISTA_JARDIM, PISTA_BANHEIRO, PISTA_QUARTO_MAE, FINAL };
    enumMission secao;


    float timeToDeath = 0.5f;
    float portaKidDefaultY, portaKidDefaultX, portaAlleyDefaultY, portaAlleyDefaultX;

    public override void InitMission()
    {
        sceneInit = "Jardim";
        GameManager.initMission = true;
        GameManager.initX = (float)3.5;
        GameManager.initY = (float)1.7;
        //GameManager.initDir = 3;
        GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        Book.bookBlocked = false;

        GameManager.instance.invertWorld = true;
        GameManager.instance.invertWorldBlocked = true;

        SetInitialSettings();
    }

    public override void UpdateMission() //aqui coloca as ações do update específicas da missão
    {
        if (secao == enumMission.NIGHT)
        {
            if (!GameManager.instance.showMissionStart)
            {
                EspecificaEnum((int)enumMission.INICIO);
            }
        }


    }

    public override void SetCorredor()
    {
        GameManager.instance.scenerySounds.StopSound();

        if (GameManager.previousSceneName.Equals("GameOver"))
        {

        }

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

        // porta corredor
        GameObject portaCorredor = GameObject.Find("DoorToKitchen").gameObject;
        portaCorredor.GetComponent<SceneDoor>().isOpened = true;

        // porta sala
        GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
        portaSala.GetComponent<SceneDoor>().isOpened = true;

        // porta quarto criança
        GameObject portaQuarto = GameObject.Find("DoorToKidRoom").gameObject;
        portaQuarto.GetComponent<SceneDoor>().isOpened = true;

        // porta quarto mãe
        GameObject portaQuartoMae = GameObject.Find("DoorToMomRoom").gameObject;
        portaQuartoMae.GetComponent<SceneDoor>().isOpened = true;
    }

    public override void SetCozinha()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameManager.instance.scenerySounds.PlayDrop();

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
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        if (secao == enumMission.INICIO)
        {
        }
        // Problema de performance com os minions
        GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-6f, -3f, 0));
        MinionEmmitter minionEmitter = aux.GetComponent<MinionEmmitter>();
        minionEmitter.numMinions = 10;
        minionEmitter.hydraEffect = true;
        minionEmitter.limitX0 = 0.5f;
        minionEmitter.limitXF = 6.95f;
        minionEmitter.limitY0 = 0f;
        minionEmitter.limitYF = 3f;
    }

    public override void SetQuartoKid()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameManager.instance.rpgTalk.NewTalk("M7KidRoomSceneStart", "M7KidRoomSceneEnd", GameManager.instance.rpgTalk.txtToParse);

        GameObject porta = GameObject.Find("DoorToAlley").gameObject;
        porta.GetComponent<SceneDoor>().isOpened = true;

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

    }

    public override void SetQuartoMae()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameObject porta = GameObject.Find("DoorToAlley").gameObject;
        porta.GetComponent<SceneDoor>().isOpened = true;

        GameObject Luminaria = GameObject.Find("Luminaria").gameObject;
        Luminaria.GetComponent<Light>().enabled = true;

        GameObject camaMae = GameObject.Find("Cama").gameObject;
        camaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/camaMaeDoente");

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            GameManager.instance.ChangeMission(7);
        }
    }


    public override void SetSala()
    {

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

        GameObject portaCorredor = GameObject.Find("DoorToAlley").gameObject;
        portaCorredor.GetComponent<SceneDoor>().isOpened = true;

        GameObject portaG = GameObject.Find("DoorToGarden").gameObject;
        portaG.GetComponent<SceneDoor>().isOpened = true;

        if (GameManager.previousSceneName.Equals("GameOver"))
        {
            // EspecificaEnum((int)enumMission.LUZSALA);
        }
    }
    public override void SetBanheiro()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));

    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        GameManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.INICIO)
        {
            GameManager.instance.rpgTalk.NewTalk("M7GardenSceneStart", "M7GardenSceneEnd", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.FINAL)
        {
            //GameManager.instance.rpgTalk.NewTalk("M7PagePicked", "M7PagePickedEnd", GameManager.instance.rpgTalk.txtToParse);
        }
    }

    public override void ForneceDica()
    {

    }


}