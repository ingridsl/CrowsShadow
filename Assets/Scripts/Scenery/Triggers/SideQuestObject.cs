using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;

namespace CrowShadowScenery
{
    public class SideQuestObject : MonoBehaviour
    {
        public int numSideQuest = 0;

        Renderer rendererPage = null;
        bool hidden = false;
        private bool triggered = false;

        private void Start()
        {
            rendererPage = GetComponent<Renderer>();
        }

        private void Update()
        {
            if (!hidden && !GameManager.instance.invertWorld)
            {
                hidden = true;
                rendererPage.enabled = false;
            }
            else if (hidden && GameManager.instance.invertWorld)
            {
                hidden = false;
                rendererPage.enabled = true;
            }

            if (!hidden && triggered && CrossPlatformInputManager.GetButtonDown("keyInteract"))
            {
                ExtrasManager.InitSideQuest(numSideQuest);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                triggered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                triggered = false;
            }
        }
    }

}