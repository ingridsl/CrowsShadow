using UnityEngine;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowPlayer;

public abstract class Mission {

    protected GameObject mainLight, camera, player, fosforo, isqueiro, faca, pedra;
    protected MiniGameObject fosforoMiniGame, isqueiroMiniGame;
    protected Book book;

    public string sceneInit = "";

    public int tipTimerSmall = 40;
    public int tipTimerMedium = 60;
    public int tipTimerLonger = 80;
    public bool usedTip1 = false;

    public void LoadMission()
    {

    }

    public void LoadMissionScene()
    {
        if (!GameManager.currentSceneName.Equals("MainMenu") && !GameManager.currentSceneName.Equals("GameOver") && 
            !GameManager.currentSceneName.Equals("Credits"))
        {
            mainLight = GameObject.Find("MainLight").gameObject; // Variar X (-50 - claro / 50 - escuro) - valor original: 0-100 (-50)
            camera = GameObject.Find("MainCamera").gameObject;
        }

        if (GameManager.currentSceneName.Equals("Corredor"))
        {
            SetCorredor();
        }
        else if (GameManager.currentSceneName.Equals("Cozinha"))
        {
            SetCozinha();
        }
        else if (GameManager.currentSceneName.Equals("Jardim"))
        {
            SetJardim();
        }
        else if (GameManager.currentSceneName.Equals("QuartoKid"))
        {
            SetQuartoKid();
        }
        else if (GameManager.currentSceneName.Equals("QuartoMae"))
        {
            SetQuartoMae();
        }
        else if (GameManager.currentSceneName.Equals("Sala"))
        {
            SetSala();
        }
        else if (GameManager.currentSceneName.Equals("Banheiro"))
        {
            SetBanheiro();
        }
        else if (GameManager.currentSceneName.Equals("Porao"))
        {
            SetPorao();
        }
    }

    public abstract void InitMission();

    public abstract void UpdateMission();

    public abstract void SetCorredor();

    public abstract void SetCozinha();

    public abstract void SetJardim();

    public abstract void SetQuartoKid();

    public abstract void SetQuartoMae();

    public abstract void SetSala();

    public abstract void SetBanheiro();

    public abstract void SetPorao();

    public abstract void EspecificaEnum(int pos);

    public abstract void ForneceDica();

    public virtual void AreaTriggered(string tag) { } // para chamar quando uma área é ativada

    public virtual void InvokeMission() { } // para chamar após um determinado tempo

    public virtual void InvokeMissionChoice(int id) { } // para chamar após uma escolha ser feita

    public void SpinCamera(float speed)
    {
        if (camera != null)
        {
            camera.transform.Rotate(0, 0, speed * Time.deltaTime);
        }
    }

    public void SetInitialSettings()
    {
        if (Cat.instance != null) Cat.instance.DestroyCat();
        if (Crow.instance != null) Crow.instance.DestroyRaven();

        GameManager.instance.paused = false;
        GameManager.instance.blocked = false;

        player = GameManager.instance.gameObject;
        fosforo = player.transform.Find("Fosforo").gameObject;
        isqueiro = player.transform.Find("Isqueiro").gameObject;
        faca = player.transform.Find("Faca").gameObject;
        pedra = player.transform.Find("Pedra").gameObject;
        book = player.GetComponent<Book>();

        fosforoMiniGame = fosforo.GetComponent<MiniGameObject>();
        isqueiroMiniGame = isqueiro.GetComponent<MiniGameObject>();

        fosforoMiniGame.achievedGoal = false;
        isqueiroMiniGame.achievedGoal = false;

        fosforoMiniGame.activated = false;
        isqueiroMiniGame.activated = false;

        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }
    }

}
