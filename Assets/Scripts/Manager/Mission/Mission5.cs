using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission5 : Mission
{
    enum enumMission { NIGHT, INICIO, CLOSED, OPENED, INSIDE, FINISHED, ALMOST_ATTACK, END_ATTACK, ATTACK_MOM, ATTACK_CAT, FINAL };
    enumMission secao;

    private MinionEmmitter minionEmitter = null;
    private GameObject mom = null;

    private float timeToTip = 2;
    private int timesInMI = 0;

    public override void InitMission()
    {
        sceneInit = "QuartoKid";//"QuartoKid";
        GameManager.initMission = true;
        GameManager.initX = (float)3.0;
        GameManager.initY = (float)0.2;
        GameManager.initDir = 3;
        GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;//enumMission.NIGHT;
        Book.bookBlocked = false;

        GameManager.instance.invertWorld = false;
        GameManager.instance.invertWorldBlocked = false;

        SetInitialSettings();
    }

    public override void UpdateMission() //aqui coloca as ações do update específicas da missão
    {
        if ((int)GameManager.instance.timer == tipTimerSmall ||
            (int)GameManager.instance.timer == tipTimerMedium || (int)GameManager.instance.timer == tipTimerLonger)
        {
            ForneceDica();
        }
        if (secao == enumMission.NIGHT)
        {
            if (!GameManager.instance.showMissionStart)
            {
                EspecificaEnum((int)enumMission.INICIO);
            }
        }
        else if (secao == enumMission.INICIO && Book.show)
        {
            EspecificaEnum((int)enumMission.OPENED);
        }
        else if (secao == enumMission.OPENED && (CrossPlatformInputManager.GetButtonDown("keyJournal")))
        {
            EspecificaEnum((int)enumMission.CLOSED);
        }
        else if (secao == enumMission.CLOSED && GameManager.currentSceneName.Equals("SideQuest"))
        {
            EspecificaEnum((int)enumMission.INSIDE);
        }
        else if (secao == enumMission.INSIDE && GameManager.instance.sideQuests == 1)
        {
            EspecificaEnum((int)enumMission.FINISHED);
        }
        else if (secao == enumMission.FINISHED && GameManager.currentSceneName.Equals("Jardim")
            && minionEmitter != null && (minionEmitter.numMinions >= 40 || minionEmitter.colliding))
        {
            EspecificaEnum((int)enumMission.END_ATTACK);
        }
        else if (secao == enumMission.END_ATTACK && !GameManager.instance.rpgTalk.isPlaying)
        {
            EspecificaEnum((int)enumMission.ALMOST_ATTACK);
        }
        else if ((secao == enumMission.ATTACK_MOM || secao == enumMission.ATTACK_CAT) && !GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.Invoke("InvokeMission", 4f);
        }
    }

    public override void SetCorredor()
    {
        GameManager.instance.scenerySounds.StopSound();

        // Porta Mae
        GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
        float portaMaeDefaultY = portaMae.transform.position.y;
        float posX = portaMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
        portaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
        portaMae.GetComponent<SceneDoor>().isOpened = false;
        portaMae.transform.position = new Vector3(portaMae.transform.position.x - posX, portaMaeDefaultY, portaMae.transform.position.z);

        GameObject.Find("VasoNaoEmpurravel").gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");

        //GameManager.instance.rpgTalk.NewTalk ("M5CorridorSceneStart", "M5CorridorSceneEnd");
    }

    public override void SetCozinha()
    {
        GameManager.instance.scenerySounds.PlayDrop();
        //GameManager.instance.rpgTalk.NewTalk ("M5KitchenSceneStart", "M5KitchenSceneEnd");

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
        //GameObject portaSala = GameObject.Find("DoorToLivingRoom").gameObject;
        //portaSala.GetComponent<SceneDoor>().isOpened = false;

        GameManager.instance.rpgTalk.NewTalk("M5KidRoomSceneStart", "M5KidRoomSceneEnd"); // tirar isso depois
        // Problema de performance com os minions
        GameObject aux = GameManager.instance.AddObject("NPCs/MinionEmitter", new Vector3(6.05f, 3.15f, 0));
        minionEmitter = aux.GetComponent<MinionEmmitter>();
        minionEmitter.numMinions = 20;
        minionEmitter.hydraEffect = true;
        minionEmitter.limitX0 = 0.5f;
        minionEmitter.limitXF = 6.95f;
        minionEmitter.limitY0 = 0f;
        minionEmitter.limitYF = 3f;
    }

    public override void SetQuartoKid()
    {
        if (secao == enumMission.NIGHT)
        {
            GameManager.instance.rpgTalk.NewTalk("M5KidRoomSceneStart", "M5KidRoomSceneEnd");
        }

        if (secao <= enumMission.FINISHED)
        {
            // Porta
            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            float portaDefaultX = porta.transform.position.x;
            float portaDefaultY = porta.transform.position.y;
            float posX = porta.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            porta.GetComponent<SceneDoor>().isOpened = false;
            porta.transform.position = new Vector3(porta.transform.position.x - posX, portaDefaultY, porta.transform.position.z);
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

        if (Cat.instance == null)
        {
            // Gato
            GameObject cat = GameManager.instance.AddObject(
                "NPCs/catFollower", "", new Vector3(player.transform.position.x + 0.6f, player.transform.position.y, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().FollowPlayer();
        }
    }

    public override void SetQuartoMae()
    {
        //GameManager.instance.rpgTalk.NewTalk ("M5MomRoomSceneStart", "M5MomRoomSceneEnd");
    }


    public override void SetSala()
    {

    }
    public override void SetBanheiro()
    {

    }
    public override void SetPorao()
    {

    }

    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission)pos;
        GameManager.instance.Print("SECAO: " + secao);

        if (secao == enumMission.OPENED)
        {
            ExtrasManager.canActivateSide1 = true;
            ExtrasManager.SideQuestsManager();
        }
        else if (secao == enumMission.CLOSED)
        {
            GameManager.instance.rpgTalk.NewTalk("M5KidRoomSceneSideStart", "M5KidRoomSceneSideEnd");
        }
        else if (secao == enumMission.INSIDE)
        {
            GameManager.instance.rpgTalk.NewTalk("M5Side1Start", "M5Side1End");
        }
        else if (secao == enumMission.FINISHED)
        {
            GameManager.instance.scenerySounds.PlayBird(1);
            //GameManager.instance.rpgTalk.NewTalk("M5KidRoomSceneRepeat", "M5KidRoomSceneRepeatEnd");
        }
        else if (secao == enumMission.END_ATTACK)
        {
            GameManager.instance.blocked = true;
            minionEmitter.StopAllMinions();
            GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 6;
            GameObject.Find("MainCamera").GetComponent<Camera>().transform.position = new Vector3(0f, 0f, -20f);
            mom = GameManager.instance.AddObject("NPCs/mom", "", new Vector3(2.65f, 2.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            GameManager.instance.rpgTalk.NewTalk("M5HelpMomStart", "M5HelpMomEnd");

        }else if (secao == enumMission.ALMOST_ATTACK)
        {
            GameManager.instance.rpgTalk.NewTalk("M5HelpMom2Start", "M5HelpMom2End");

        }
        else if (secao == enumMission.ATTACK_MOM)
        {
            //final ruim
            minionEmitter.MoveAllMinions(mom.transform.position);
        }
        else if (secao == enumMission.ATTACK_CAT)
        {
            //final bom
        }
        else if (secao == enumMission.FINAL)
        {
            GameManager.instance.ChangeMission(7);
        }
    }

    public override void ForneceDica()
    {

        if (secao == enumMission.INICIO || secao == enumMission.NIGHT)
        {
            GameManager.instance.timer = 0;
            GameManager.instance.rpgTalk.NewTalk("DicaMundoInvertido5Start", "DicaMundoInvertido5End", GameManager.instance.rpgTalk.txtToParse);
        }
        else if (secao == enumMission.CLOSED)
        {
            GameManager.instance.timer = 0;
            GameManager.instance.rpgTalk.NewTalk("DicaSide5Start", "DicaSide5End", GameManager.instance.rpgTalk.txtToParse);
        }
    }

    public override void InvokeMissionChoice(int id)
    {
        if (secao == enumMission.ALMOST_ATTACK)
        {
            if (id == 0)
            {
                EspecificaEnum((int)enumMission.ATTACK_CAT);
            }
            else
            {
                EspecificaEnum((int)enumMission.ATTACK_MOM);
            }
            GameManager.instance.Invoke("InvokeMission", 8f);
        }
    }
    public override void InvokeMission()
    {
        if (secao == enumMission.ATTACK_MOM || secao == enumMission.ATTACK_CAT)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }
    }

}