using UnityEngine;
using System.Collections.Generic;
using CrowShadowManager;
using CrowShadowScenery;

public class SideQuest3 : SideQuest
{
    SpiritManager spiritManager;
    HelpingLightManager lightManager;

    public override void InitSideQuest()
    {
        if (!GameManager.previousSceneName.Equals("GameOver"))
        {
            // Determinar posição do player (sideX e sideY)
            sideX = 0f; sideY = 18f;
            sideDir = 3;
            // Determinar tempo para terminar o nível
            timeEscape = 10f;
            SetInitialSettings();
        }

        // Determinar posição da porta
        SetDoor(0f, 20f);

        // Determinar conjuntos de espíritos
        List<float> radius = new List<float>(), originX = new List<float>(), originY = new List<float>();
        radius.Add(5f); originX.Add(0f); originY.Add(-10f);

        GameObject holderSpirit = GameManager.instance.AddObject("Scenery/SpiritHolder", "", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        spiritManager = holderSpirit.GetComponent<SpiritManager>();
        spiritManager.GenerateSpiritMap(radius[0], originX[0], originY[0], 6, 2, 1, true, 4);

        // Determinar posição das luzes de borda (posição inicial do player e locais dos espíritos
        GameObject holderLight = GameManager.instance.AddObject("Scenery/LightHolder", "", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        lightManager = holderLight.GetComponent<HelpingLightManager>();
        lightManager.GenerateBorderLightMap(sideX, sideY, radius, originX, originY);

        // Determinar luzes
        lightManager.AddLight(2, sideX, sideY - 4, 3f, false, 0, null);
        lightManager.AddLight(1, sideX, sideY - 8, 4f, false, 0, null);
        lightManager.AddLight(2, sideX, sideY - 12, 3f, false, 0, null);
        lightManager.AddLight(1, sideX, sideY - 16, 4f, false, 0, null);
        lightManager.AddLight(2, sideX, sideY - 20, 3f, false, 0, null);
        lightManager.AddLight(1, sideX, sideY - 22, 1f, false, 0, null);
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
    }

    public override void ShowFlashback()
    {
        EndFlashback();
    }

}
