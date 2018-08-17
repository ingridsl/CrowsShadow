using UnityEngine;
using UnityEngine.UI;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowPlayer;

public abstract class SideQuest
{
    public bool success = false;
    protected bool active = true, showingFlashback = false;
    protected bool hasCat = false, hasRaven = false, invertBlocked = false, wasInverted = false;
    protected string oldScene = "";

    protected float timeEscape = 10f, counterTimeEscape = 0f;
    protected GameObject timerSideQuest, camera;

    protected float oldX = 0, oldY = 0;
    protected int oldDir = 0;

    public float sideX = 0f, sideY = 8f;
    public int sideDir = 3;

    public abstract void InitSideQuest();

    public abstract void UpdateSideQuest();

    public abstract void ShowFlashback();

    protected void SetInitialSettings()
    {
        oldScene = GameManager.previousSceneName;
        camera = GameObject.Find("MainCamera").gameObject;

        GameObject player = GameObject.Find("Player").gameObject;
        oldX = player.transform.position.x;
        oldY = player.transform.position.y;
        oldDir = (int)player.GetComponent<Player>().direction;

        GameManager.initX = sideX;
        GameManager.initY = sideY;
        GameManager.initDir = sideDir;

        if (Cat.instance != null)
        {
            Cat.instance.GetComponent<Cat>().gameObject.SetActive(false);
            hasCat = true;
        }
        if (Crow.instance != null)
        {
            Crow.instance.gameObject.SetActive(false);
            hasRaven = true;
        }

        invertBlocked = GameManager.instance.invertWorldBlocked;
        GameManager.instance.invertWorldBlocked = false;
        wasInverted = GameManager.instance.invertWorld;
        GameManager.instance.InvertWorld(false);
        GameManager.instance.blocked = false;

        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }
    }

    protected void SetDoor(float x, float y)
    {
        GameObject door = GameObject.Find("ExitSideQuestDoor").gameObject;
        door.transform.position = new Vector3(x, y, 0f);
        switch (oldScene)
        {
            case "Sala":
                door.tag = "DoorToLivingroom";
                break;
            case "Corredor":
                door.tag = "DoorToAlley";
                break;
            case "Jardim":
                door.tag = "DoorToGarden";
                break;
            case "Cozinha":
                door.tag = "DoorToKitchen";
                break;
            case "QuartoMae":
                door.tag = "DoorToMomRoom";
                break;
            case "QuartoKid":
                door.tag = "DoorToKidRoom";
                break;
            case "Banheiro":
                door.tag = "DoorToBathroom";
                break;
            case "Porao":
                door.tag = "DoorToBasement";
                break;
        }
    }

    protected void SetTimeToEscape()
    {
        timerSideQuest = GameManager.instance.AddObjectWithParent("UI/TimerSideQuest", GameObject.Find("HUDCanvas").transform);
        timerSideQuest.GetComponent<RectTransform>().transform.localScale = new Vector3(1, 1, 1);
        timerSideQuest.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, -40, 0);
    }

    protected void UpdateTimeToEscape()
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(counterTimeEscape);
        string timerFormatted = string.Format("{0:D2}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
        timerSideQuest.GetComponent<Text>().text = timerFormatted;
    }

    protected void DeleteTimeToEscape()
    {
        if (timerSideQuest != null)
        {
            GameObject.Destroy(timerSideQuest);
            timerSideQuest = null;
        }
    }

    protected void SpinCamera(float speed)
    {
        if (camera != null)
        {
            camera.transform.Rotate(0, 0, speed * Time.deltaTime);
        }
        else if (GameManager.currentSceneName.Equals("SideQuest"))
        {
            camera = GameObject.Find("MainCamera").gameObject;
        }
    }

    public void EndSideQuest()
    {
        GameManager.initX = oldX;
        GameManager.initY = oldY;
        GameManager.initDir = oldDir;

        if (hasCat)
        {
            Cat.instance.GetComponent<Cat>().gameObject.SetActive(true);
        }
        if (hasRaven)
        {
            Crow.instance.gameObject.SetActive(true);
        }

        GameManager.instance.blocked = true;

        if (GameManager.instance.rpgTalk.isPlaying)
        {
            GameManager.instance.rpgTalk.EndTalk();
        }

        if (success)
        {
            GameManager.instance.sideQuests++;
            GameManager.instance.UpdateSave();
            ShowFlashback();
        }
        else
        {
            GameManager.instance.invertWorldBlocked = invertBlocked;
            GameManager.instance.InvertWorld(true);
            EndFlashback();
        }

        active = false;
        DeleteTimeToEscape();
    }

    protected void EndFlashback()
    {
        GameManager.instance.invertWorldBlocked = invertBlocked;
        GameManager.instance.InvertWorld(wasInverted);
        GameManager.instance.blocked = false;
        GameManager.instance.paused = false; // ERRO TÁ AQUI
        ExtrasManager.EndSideQuest();
        GameManager.instance.Print("EndFlashback");
    }

}
