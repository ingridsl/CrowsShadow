using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowPlayer;

namespace CrowShadowScenery
{
    public class Tutorial : MonoBehaviour
    {
        public string keyName1, keyName2;
        public int mission = 1;
        public bool inventoryObject = false;

        public Player.Actions playerAction = Player.Actions.DEFAULT;
        public AudioClip click;
        public AudioSource source { get { return GetComponent<AudioSource>(); } }

        private GameObject key;
        private Player player;

        private int repeatTime = 2;
        private bool exit = false, end = false, invoked = false;
        private bool movingObjectOnArea = false;

        void Awake()
        {
            source.playOnAwake = false;
        }

        private void Start()
        {
            key = gameObject.transform.GetChild(0).gameObject;
            player = GameManager.instance.gameObject.GetComponent<Player>();
        }

        void Update()
        {
            if (movingObjectOnArea && player.playerAction == playerAction)
            {
                exit = false;
                AreaTriggered();
            }

            if (!end && !exit)
            {
                KeyPressed();
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Player") && 
                GameManager.instance.currentMission == mission && player.playerAction == playerAction && !end)
            {
                exit = false;
                AreaTriggered();
            }
            else if ((other.gameObject.tag.Equals("MovingObject") && playerAction == Player.Actions.ON_OBJECT))
            {
                movingObjectOnArea = true;
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Player") && 
                GameManager.instance.currentMission == mission && player.playerAction == playerAction && !end)
            {
                AreaTriggered();
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Player") && GameManager.instance.currentMission == mission)
            {
                exit = true;
                invoked = false;
                key.SetActive(false);
            }
            else if ((other.gameObject.tag.Equals("MovingObject") && playerAction == Player.Actions.ON_OBJECT &&
                player.playerAction != Player.Actions.ANIMATION && player.playerAction != Player.Actions.ON_OBJECT))
            {
                movingObjectOnArea = false;
            }
        }

        void KeyPressed()
        {
            if (CrossPlatformInputManager.GetButtonDown(keyName1) && CrossPlatformInputManager.GetButtonDown(keyName2))
            {
                end = true;
                key.SetActive(false);
            }
        }

        void AreaTriggered()
        {
            if (!inventoryObject)
            {
                InvokeShow();
            }
            else
            {
                if (Inventory.HasItemType(Inventory.InventoryItems.FLASHLIGHT))
                {
                    InvokeShow();
                }
            }
        }

        void InvokeShow()
        {
            if (!source.isPlaying && !invoked)
            {
                Invoke("Show", repeatTime);
                invoked = true;
            }
        }

        void Show()
        {
            if (!end && !exit)
            {
                if (key.activeInHierarchy)
                {
                    key.SetActive(false);
                }
                else
                {
                    key.SetActive(true);
                }
                source.PlayOneShot(click);
                invoked = false;
            }
        }

    }
}