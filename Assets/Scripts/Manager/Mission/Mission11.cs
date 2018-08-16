using UnityEngine;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowPlayer;
using CrowShadowScenery;

public class Mission11 : Mission {
    enum enumMission { NIGHT, INICIO, SALA, CORREDOR, QUARTO_MAE, COZINHA, QUARTO_KID, QUARTO_KID_CORVO, QUARTO_KID_CORVO_ATACA, FINAL };
    enumMission secao;

    bool endCat = false;

    public override void InitMission()
    {
        sceneInit = "Jardim";
        GameManager.initMission = true;
        GameManager.initX = (float)1.54;
        GameManager.initY = (float)1.75;
        GameManager.initDir = 2;
        GameManager.LoadScene(sceneInit);
        secao = enumMission.NIGHT;
        if (Cat.instance != null) Cat.instance.DestroyCat();
        if (Crow.instance != null) Crow.instance.DestroyRaven();
        if (GameManager.instance.pathCat >= GameManager.instance.pathBird) endCat = true;

        Book.bookBlocked = true;

        GameManager.instance.invertWorld = false;
        GameManager.instance.invertWorldBlocked = false;
        GameManager.instance.paused = false;

        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        player = GameManager.instance.gameObject;

        GameObject.Find("HUDCanvas").transform.Find("SelectedObject").gameObject.SetActive(false);
        GameObject.Find("HUDCanvas").transform.Find("BoxInventory").gameObject.SetActive(false);
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
        else if (endCat && GameManager.currentSceneName.Equals("Jardim") && Cat.instance == null)
        {
            // Gato, correção de um erro
            GameObject catObject = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(0.92f, 1.46f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            Cat cat = catObject.GetComponent<Cat>();
            cat.followWhenClose = false;
            cat.Stop();
            cat.ChangeDirectionAnimation(5);
        }
        if (secao == enumMission.QUARTO_KID_CORVO_ATACA && !GameManager.instance.scenerySounds.source.isPlaying)
        {
            GameManager.instance.scenerySounds.PlayBird(1);
        }
        if (secao == enumMission.QUARTO_KID_CORVO && !GameManager.instance.scenerySounds.source.isPlaying)
        {
            GameManager.instance.scenerySounds.PlayDemon(1);
        }
    }

    public override void SetCorredor()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 5;
        player.GetComponent<Player>().ChangePositionDefault(0, 0, 0);
        GameObject.Find("MainCamera").transform.position = new Vector3(0, 0, -20);

        if (Cat.instance != null) Cat.instance.DestroyCat();

        if (endCat)
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventCat").gameObject.SetActive(true);

            player.GetComponent<Renderer>().enabled = false;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            player.layer = LayerMask.NameToLayer("PlayerHidden");
            GameManager.instance.blocked = true;
        }
        else
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventBird").gameObject.SetActive(true);

            // Porta bloqueada
            GameObject porta = GameObject.Find("DoorToLivingRoom").gameObject;
            porta.GetComponent<SceneDoor>().isOpened = false;

            // Porta bloqueada
            GameObject portaF = GameObject.Find("DoorToKidRoom").gameObject;
            float portaFDefaultY = portaF.transform.position.y;
            float posXF = portaF.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            portaF.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            portaF.GetComponent<SceneDoor>().isOpened = false;
            portaF.transform.position = new Vector3(portaF.transform.position.x - posXF, portaFDefaultY, portaF.transform.position.z);

            // Porta bloqueada
            GameObject portaMae = GameObject.Find("DoorToMomRoom").gameObject;
            float portaMaeDefaultY = portaMae.transform.position.y;
            float posX = portaMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            portaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            portaMae.GetComponent<SceneDoor>().isOpened = false;
            portaMae.transform.position = new Vector3(portaMae.transform.position.x - posX, portaMaeDefaultY, portaMae.transform.position.z);

            // Porta bloqueada
            GameObject portaK = GameObject.Find("DoorToKitchen").gameObject;
            portaK.GetComponent<SceneDoor>().isOpened = false;

            if (GameManager.instance.mission10BurnCorredor)
            {
                player.GetComponent<Player>().ChangePositionDefault(-6f, -0.4f, 0);
                player.GetComponent<Renderer>().enabled = true;
                player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                player.layer = LayerMask.NameToLayer("Player");
                GameManager.instance.blocked = false;
            }
            else
            {
                player.GetComponent<Renderer>().enabled = false;
                player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                player.layer = LayerMask.NameToLayer("PlayerHidden");
                GameManager.instance.blocked = true;
            }
        }

        if (secao == enumMission.CORREDOR)
        {
            GameManager.instance.Invoke("InvokeMission", 10f);
        }

    }

    public override void SetCozinha()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 3;
        player.GetComponent<Player>().ChangePositionDefault(0, 0, 0);
        GameObject.Find("MainCamera").transform.position = new Vector3(0, 0, -20);

        if (Cat.instance != null) Cat.instance.DestroyCat();

        player.GetComponent<Renderer>().enabled = false;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        player.layer = LayerMask.NameToLayer("PlayerHidden");
        GameManager.instance.blocked = true;

        if (endCat)
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventCat").gameObject.SetActive(true);
        }
        else
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventBird").gameObject.SetActive(true);
        }

        if (secao == enumMission.COZINHA)
        {
            GameManager.instance.Invoke("InvokeMission", 6f);
        }
    }

    public override void SetJardim()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 3;

        if (Cat.instance != null) Cat.instance.DestroyCat();

        GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventTree").gameObject.SetActive(true);

        if (endCat)
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventCat").gameObject.SetActive(true);

            // Porta bloqueada
            GameObject porta = GameObject.Find("DoorToLivingRoom").gameObject;
            porta.GetComponent<SceneDoor>().isOpened = false;

            // Player
            player.GetComponent<Renderer>().enabled = true;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            player.layer = LayerMask.NameToLayer("Player");
            GameManager.instance.blocked = false;

            // Gato
            GameObject cat = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(0.92f, 1.46f, -0.5f), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().followWhenClose = false;
            cat.GetComponent<Cat>().Stop();
            cat.GetComponent<Cat>().ChangeDirectionAnimation(5);

            // Mãe
            GameObject mae = GameManager.instance.AddObject("NPCs/mom", "", new Vector3(0.46f, 1.98f, -0.5f), new Vector3(0.3f, 0.3f, 1));
            mae.GetComponent<Patroller>().Stop();
            mae.GetComponent<Patroller>().ChangeDirectionAnimation(5);
        }
        else
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventBird").gameObject.SetActive(true);

            player.GetComponent<Renderer>().enabled = false;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            player.layer = LayerMask.NameToLayer("PlayerHidden");
            GameManager.instance.blocked = true;
        }

        if (secao == enumMission.NIGHT || secao == enumMission.INICIO)
        {
            GameManager.instance.Invoke("InvokeMission", 15f);
        }
    }

    public override void SetQuartoKid()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 3;
        player.GetComponent<Player>().ChangePositionDefault(0, 0, 0);
        GameObject.Find("MainCamera").transform.position = new Vector3(0, 0, -20);

        if (Cat.instance != null) Cat.instance.DestroyCat();

        player.GetComponent<Renderer>().enabled = false;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        player.layer = LayerMask.NameToLayer("PlayerHidden");
        GameManager.instance.blocked = true;

        if (endCat)
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventCat").gameObject.SetActive(true);
        }
        else
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventBird").gameObject.SetActive(true);

            // Porta bloqueada
            GameObject porta = GameObject.Find("DoorToAlley").gameObject;
            float portaDefaultY = porta.transform.position.y;
            float posX = porta.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            porta.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            porta.GetComponent<SceneDoor>().isOpened = false;
            porta.transform.position = new Vector3(porta.transform.position.x - posX, portaDefaultY, porta.transform.position.z);

            // Gato
            GameObject cat = GameManager.instance.AddObject("NPCs/catFollower", "", new Vector3(2.5f, -1.3f, 0), new Vector3(0.15f, 0.15f, 1));
            cat.GetComponent<Cat>().followWhenClose = false;
            cat.GetComponent<Cat>().Stop();
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

        if (!Inventory.HasItemType(Inventory.InventoryItems.BASTAO))
        {
            GameManager.instance.CreateScenePickUp("Bau", Inventory.InventoryItems.BASTAO);
        }

        if (secao == enumMission.QUARTO_KID)
        {
            GameManager.instance.Invoke("InvokeMission", 10f);
        }
    }

    public override void SetQuartoMae()
    {
        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 3;
        player.GetComponent<Player>().ChangePositionDefault(0, 0, 0);
        GameObject.Find("MainCamera").transform.position = new Vector3(0, -1, -20);

        if (Cat.instance != null) Cat.instance.DestroyCat();

        if (endCat)
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventCat").gameObject.SetActive(true);
            player.GetComponent<Renderer>().enabled = false;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            player.layer = LayerMask.NameToLayer("PlayerHidden");
            GameManager.instance.blocked = true;
        }
        else
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventBird").gameObject.SetActive(true);

            // Mãe
            GameManager.instance.AddObject("NPCs/mom", "", new Vector3(2.04f, 0.94f, -0.5f), new Vector3(0.3f, 0.3f, 1));

            // Porta bloqueada
            GameObject portaMae = GameObject.Find("DoorToAlley").gameObject;
            float portaMaeDefaultY = portaMae.transform.position.y;
            float posX = portaMae.GetComponent<SpriteRenderer>().bounds.size.x / 5;
            portaMae.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/door-closed");
            portaMae.GetComponent<SceneDoor>().isOpened = false;
            portaMae.transform.position = new Vector3(portaMae.transform.position.x - posX, portaMaeDefaultY, portaMae.transform.position.z);

            if (!GameManager.instance.mission10BurnCorredor)
            {
                player.GetComponent<Player>().ChangePositionDefault(2.6f, 1.1f, 0);
                player.GetComponent<Renderer>().enabled = true;
                player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                player.layer = LayerMask.NameToLayer("Player");
                GameManager.instance.blocked = false;
            }
            else {
                player.GetComponent<Renderer>().enabled = false;
                player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                player.layer = LayerMask.NameToLayer("PlayerHidden");
                GameManager.instance.blocked = true;
            }
        }

        if (secao == enumMission.QUARTO_MAE)
        {
            GameManager.instance.Invoke("InvokeMission", 10f);
        }
    }

    public override void SetSala()
    {
        // LUZ DO AMBIENTE
        mainLight.transform.Rotate(new Vector3(50, mainLight.transform.rotation.y, mainLight.transform.rotation.z));
        GameObject.Find("MainCamera").GetComponent<Camera>().orthographicSize = 4;
        player.GetComponent<Player>().ChangePositionDefault(0, -1, 0);
        GameObject.Find("MainCamera").transform.position = new Vector3(0, 0, -20);

        if (Cat.instance != null) Cat.instance.DestroyCat();

        player.GetComponent<Renderer>().enabled = false;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        player.layer = LayerMask.NameToLayer("PlayerHidden");
        GameManager.instance.blocked = true;

        if (endCat)
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventCat").gameObject.SetActive(true);
        }
        else
        {
            GameObject.Find("FireEventHolder").gameObject.transform.Find("FireEventBird").gameObject.SetActive(true);
        }

        if (secao == enumMission.SALA)
        {
            GameManager.instance.Invoke("InvokeMission", 10f);
        }
    }
    public override void SetBanheiro()
    {

    }

    public override void SetPorao()
    {

    }
    public override void EspecificaEnum(int pos)
    {
        secao = (enumMission) pos;
        GameManager.instance.Print("SECAO: " + secao);
        if (secao == enumMission.SALA)
        {
            GameManager.LoadScene("Sala");
        }
        else if (secao == enumMission.CORREDOR)
        {
            GameManager.LoadScene("Corredor");
        }
        else if (secao == enumMission.QUARTO_MAE)
        {
            GameManager.LoadScene("QuartoMae");
        }
        else if (secao == enumMission.COZINHA)
        {
            GameManager.LoadScene("Cozinha");
        }
        else if (secao == enumMission.QUARTO_KID)
        {
            GameManager.LoadScene("QuartoKid");
        }
        else if (secao == enumMission.QUARTO_KID_CORVO)
        {
            GameManager.instance.InvertWorld(true);
            if (endCat)
            {
                GameObject crow = GameManager.instance.AddObject("NPCs/Crow", "", new Vector3(0f, 0f, -0.5f), new Vector3(4.5f, 4.5f, 1));
                crow.GetComponent<SpriteRenderer>().color = Color.gray;
            }
            else
            {
                GameObject crow = GameManager.instance.AddObject("NPCs/Crow", "", new Vector3(0f, 0f, -0.5f), new Vector3(5f, 5f, 1));
                crow.GetComponent<SpriteRenderer>().color = Color.gray;
            }

            GameObject.Find("AreaLightHolder").gameObject.transform.Find("AreaLight").gameObject.SetActive(false);

            GameManager.instance.Invoke("InvokeMission", 8f);
        }
        else if (secao == enumMission.QUARTO_KID_CORVO_ATACA)
        {
            GameManager.instance.scenerySounds.StopSound();
            GameObject emitter = Crow.instance.transform.Find("BirdEmitterCollider").gameObject;
            Crow.instance.timeBirdsFollow = 0f;
            emitter.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            var emAux = emitter.GetComponent<ParticleSystem>();
            var main = emAux.main;
            emitter.GetComponent<ParticleSystemRenderer>().sortingOrder = 2;
            emAux.emission.SetBurst(0, new ParticleSystem.Burst(0, 30, 30, 0, 10));
            main.maxParticles = 30;
            emitter.SetActive(true);

            //se quiser deixar os corvos mais rápidos
            float hSliderValue = 3.0F;
            main.simulationSpeed = hSliderValue;
            main.startSpeed = hSliderValue;

            //mudanças de tamanho
            main.startSize = 1.5F;
            GameManager.instance.Invoke("InvokeMission", 5f);
        }
        else if (secao == enumMission.FINAL)
        {
            // FIM DO JOGO XD
            GameManager.LoadScene("MainMenu");
        }
    }

    public override void InvokeMission()
    {
        if (secao == enumMission.INICIO)
        {
            EspecificaEnum((int)enumMission.SALA);
        }
        else if (secao == enumMission.SALA)
        {
            EspecificaEnum((int)enumMission.CORREDOR);
        }
        else if (secao == enumMission.CORREDOR)
        {
            EspecificaEnum((int)enumMission.QUARTO_MAE);
        }
        else if (secao == enumMission.QUARTO_MAE)
        {
            EspecificaEnum((int)enumMission.COZINHA);
        }
        else if (secao == enumMission.COZINHA)
        {
            EspecificaEnum((int)enumMission.QUARTO_KID);
        }
        else if (secao == enumMission.QUARTO_KID)
        {
            EspecificaEnum((int)enumMission.QUARTO_KID_CORVO);
        }
        else if (secao == enumMission.QUARTO_KID_CORVO)
        {
            EspecificaEnum((int)enumMission.QUARTO_KID_CORVO_ATACA);
        }
        else if (secao == enumMission.QUARTO_KID_CORVO_ATACA)
        {
            EspecificaEnum((int)enumMission.FINAL);
        }
    }
    public override void ForneceDica()
    {

    }


}
