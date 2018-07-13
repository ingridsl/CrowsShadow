using UnityEngine;
using System.Collections.Generic;
using CrowShadowManager;

namespace CrowShadowScenery
{
    public class HelpingLightManager : MonoBehaviour
    {
        private static List<GameObject> lights = new List<GameObject>();

        public void GenerateBorderLightMap(float initX, float initY, List<float> radiusFinalLight, List<float> finalX, List<float> finalY)
        {
            // Luz inicial
            AddLight(1, initX, initY, 1f, false, 0, null);

            // Luzes das áreas com espírito
            for (int i = 0; i < radiusFinalLight.Count; i++)
            {
                AddLight(1, finalX[i], finalY[i], radiusFinalLight[i], false, 0, null, true);
            }
        }

        // Pode adicionar outros parâmetros para mudar (intensidade, cor)
        public void AddLight(int type, float x, float y, float radius, bool active, float speed, Vector3[] targets, bool changeZ = false)
        {
            switch (type)
            {
                case 2:
                    float posEZ = 0;
                    if (changeZ)
                    {
                        // Ajusta para a luz não ocultar espíritos
                        posEZ = -((radius - 2f) / 2f);
                    }
                    GameObject emitterLight = GameManager.instance.AddObjectWithParent("Scenery/EmitterLight", "", new Vector3(x, y, posEZ), new Vector3(1f, 1f, 1), transform);
                    emitterLight.GetComponent<Light>().range = radius + 1f;
                    emitterLight.GetComponent<CircleCollider2D>().radius = radius;
                    emitterLight.GetComponent<HelpingLight>().active = active;
                    emitterLight.GetComponent<HelpingLight>().speed = speed;
                    if (targets != null) emitterLight.GetComponent<HelpingLight>().targets = targets;
                    lights.Add(emitterLight);
                    break;
                default:
                    float posHZ = 0;
                    if (changeZ)
                    {
                        posHZ = -0.5f - ((radius - 2f) / 2f);
                    }
                    GameObject helpingLight = GameManager.instance.AddObjectWithParent("Scenery/HelpingLight", "", new Vector3(x, y, posHZ), new Vector3(1f, 1f, 1), transform);
                    helpingLight.GetComponent<Light>().range = radius + 1f;
                    helpingLight.GetComponent<CircleCollider2D>().radius = radius;
                    helpingLight.GetComponent<HelpingLight>().active = active;
                    helpingLight.GetComponent<HelpingLight>().speed = speed;
                    if (targets != null) helpingLight.GetComponent<HelpingLight>().targets = targets;
                    lights.Add(helpingLight);
                    break;
            }
        }

        public static void KillInDarkness()
        {
            foreach (GameObject light in lights)
            {
                if (light != null && light.GetComponent<HelpingLight>().playerInside)
                {
                    return;
                }
            }
            GameManager.instance.GameOver();
        }
    }
}