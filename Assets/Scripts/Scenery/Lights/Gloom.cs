using UnityEngine;
using CrowShadowManager;
using CrowShadowPlayer;

namespace CrowShadowScenery
{
    public class Gloom : MonoBehaviour
    {
        AudioSource audioSource;

        private int maxRoar = 5;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.Stop();
        }

        void Update()
        {
            if (Inventory.IsCurrentItemType(Inventory.InventoryItems.FLASHLIGHT, true) && !audioSource.isPlaying)
            {
                gameObject.GetComponent<AudioSource>().Play();
                maxRoar--;
                if (maxRoar < 0)
                {
                    GameManager.instance.GameOver();
                }
            }
        }
    }
}