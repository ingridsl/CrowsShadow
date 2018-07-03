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

    bool pistaJardim = false, pistaBanheiro = false, pistaQuartoMae = false;

    GameObject person1, person2;

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
        GameManager.instance.invertWorldBlocked = false;

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
        if (secao == enumMission.PISTA_JARDIM && !GameManager.instance.rpgTalk.isPlaying)
        {
            person1.SetActive(false);
            person2.SetActive(false);
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

        if (!pistaJardim)
        {
            GameObject trigger = GameManager.instance.AddObject("Scenery/AreaTrigger", "", new Vector3(-6f, -3f, 0), new Vector3(1, 1, 1));
            trigger.name = "JardimTrigger";
            trigger.GetComponent<Collider2D>().offset = new Vector2(0, 0);
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
        }

        /*
        // Problema de performance com os minions
        GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(-6f, -3f, 0));
        MinionEmmitter minionEmitter = aux.GetComponent<MinionEmmitter>();
        minionEmitter.numMinions = 2;
        minionEmitter.hydraEffect = true;
        minionEmitter.limitX0 = 0.5f;
        minionEmitter.limitXF = 6.95f;
        minionEmitter.limitY0 = 0f;
        minionEmitter.limitYF = 3f;*/
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
        }else if (secao ==  enumMission.PISTA_JARDIM)
        {

            GameManager.instance.rpgTalk.NewTalk("M7GardenShadowSceneStart", "M7GardenShadowSceneEnd", GameManager.instance.rpgTalk.txtToParse);
            // Pessoa 1
            person1 = GameManager.instance.AddObject("NPCs/personShadow", "", new Vector3(-6f, 4f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            person1.GetComponent<Patroller>().isPatroller = true;
            Vector3 auxP1 = new Vector3(-6f, -3f, -0.5f);
            Vector3 auxP2 = new Vector3(-5f, -3f, -0.5f);
            Vector3 auxP3 = new Vector3(-7f, -3f, -0.5f);
            Vector3 auxP4 = new Vector3(-7f, -2f, -0.5f);
            Vector3[] p1Pos = { auxP1, auxP2, auxP3, auxP4};
            person1.GetComponent<Patroller>().targets = p1Pos;
            person1.GetComponent<Patroller>().speed = 0.9f;
            person1.GetComponent<Patroller>().stopEndPath = true;

            // Pessoa 2
            person2 = GameManager.instance.AddObject("NPCs/personShadow", "", new Vector3(0f, -3.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            person2.GetComponent<Patroller>().isPatroller = true;
            Vector3 aux2P1 = new Vector3(-3f, -3f, -0.5f);
            Vector3 aux2P2 = new Vector3(-4f, -3f, -0.5f);
            Vector3 aux2P3 = new Vector3(-4f, -2f, -0.5f);
            Vector3 aux2P4 = new Vector3(-6f, -2f, -0.5f);
            Vector3[] p2Pos = { aux2P1, aux2P2, aux2P3, aux2P4 };
            person2.GetComponent<Patroller>().targets = p2Pos;
            person2.GetComponent<Patroller>().speed = 0.6f;
            person2.GetComponent<Patroller>().stopEndPath = true;

        }
    }

    public override void ForneceDica()
    {

    }

    public override void AreaTriggered(string tag)
    {
        if (tag.Equals("JardimTrigger") && !pistaJardim)
        {
            pistaJardim = true;
            EspecificaEnum((int)enumMission.PISTA_JARDIM);
        }
    }

}