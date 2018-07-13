using UnityEngine;
using CrowShadowManager;
using CrowShadowScenery;
using CrowShadowNPCs;

public class SideQuest1 : SideQuest
{
    GameObject person1, person2;
       SpiritManager spiritManager;

    public override void InitSideQuest()
    {
        if (!GameManager.previousSceneName.Equals("GameOver"))
        {
            // Determinar posição do player (sideX e sideY)
            sideX = 0f; sideY = 18f;
            sideDir = 3;
            // Determinar tempo para terminar o nível
            timeEscape = 30f;
            SetInitialSettings();
        }

        // Determinar posição da porta
        SetDoor(0f, 20f);

        // Determinar conjuntos de espíritos
        GameObject holder = GameManager.instance.AddObject("Scenery/SpiritHolder", "", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        spiritManager = holder.GetComponent<SpiritManager>();
        spiritManager.GenerateSpiritMap(3f, 0f, -5f, /*4*/6);
    }

    public override void UpdateSideQuest()
    {
        if (success && counterTimeEscape > 0f && active)
        {
            counterTimeEscape -= Time.deltaTime;
            UpdateTimeToEscape();
            if (counterTimeEscape <= 0f)
            {
                success = false;
                DeleteTimeToEscape();
                GameManager.instance.GameOver();
            }
        }
        // Conferir se todos os SpiritManagers alcançaram sucesso
        else if (spiritManager.success && !success)
        {
            success = true;
            counterTimeEscape = timeEscape;
            SetTimeToEscape();
        }
        ////if (success)
        //{
//
           //EndSideQuest();
        //}
        SpinCamera(5f);
    }

    public override void ShowFlashback()
    {
        GameManager.instance.rpgTalk.NewTalk("M5Side1FlashbackStart", "M5Side1FlashbackEnd", GameManager.instance.rpgTalk.txtToParse);
        // Pessoa 1
        person1 = GameManager.instance.AddObject("NPCs/personShadow", "", new Vector3(2f, 0.5f, -0.5f), new Vector3(0.3f, 0.3f, 1));
        person1.GetComponent<Patroller>().isPatroller = true;
        Vector3 auxP1 = new Vector3(2f, -2f, -0.5f);
        Vector3 auxP2 = new Vector3(-2f, -2f, -0.5f);
        Vector3 auxP3 = new Vector3(-2f, 1f, -0.5f);
        Vector3 auxP4 = new Vector3(-2f, 0f, -0.5f);
        Vector3 auxP5 = new Vector3(-1f, 0f, -0.5f);
        Vector3 auxP6 = new Vector3(-2f, 0f, -0.5f);
        Vector3 auxP7 = new Vector3(-2f, -2f, -0.5f);
        Vector3 auxP8 = new Vector3(2f, -2f, -0.5f);
        Vector3[] p1Pos = { auxP1, auxP2, auxP3, auxP4 , auxP5, auxP6, auxP7, auxP8};
        person1.GetComponent<Patroller>().targets = p1Pos;
        person1.GetComponent<Patroller>().speed = 0.9f;
        person1.GetComponent<Patroller>().stopEndPath = true;
        EndFlashback();
    }

}
