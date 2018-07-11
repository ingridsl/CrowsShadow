using UnityEngine;
using Image = UnityEngine.UI.Image;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowScenery;

namespace CrowShadowPlayer
{
    public class MiniGameObject : MonoBehaviour
    {
        public Inventory.InventoryItems item;
        public bool activated = false;
        public bool achievedGoal = false;
        public float timeMax = 5;
        public int counterMax = 20;
        public bool refreshTimeMax = true;
        public float posFlareX = 0, posFlareY = 0;

        GameObject anim, flare;
        RectTransform animRect;
        Image animImage;

        private bool otherItem = true;
        private bool playing = false;
        private float timeLeft;
        private int counter;
        public bool paper = false;

        public GameObject miniGame;

        void Start()
        {
          
        }

        public void Update()
        {
            //if(activated && paper)
           // {
           //     miniGame.SetActive(true);

           // }
            if (activated && CrossPlatformInputManager.GetButtonDown("keyUseObject") && !achievedGoal)
            {
                miniGame.SetActive(true);
            }
            if (activated && miniGame.activeSelf && !achievedGoal)
            {
                var miniGameCircle = miniGame.transform.GetChild(1).gameObject.GetComponent<MiniGameCircle>();
                if (miniGameCircle) {
                    if (miniGameCircle.goalAchieved)
                    {
                        achievedGoal = true;
                        StopMiniGame();

                    }
                }

            }
          

        }

        public void StopMiniGame()
        {

            miniGame.SetActive(false);
            //print("STOPMINIMAGE" + item);
            GameManager.instance.pausedObject = false;
           // timeLeft = 0;
           /* if (refreshTimeMax)
            {
                counter = 0;
                InitImage();
            }*/
          //  if (anim != null) anim.SetActive(false);
           // if (flare != null) Destroy(flare);
            playing = false;
            miniGame.transform.GetChild(1).gameObject.GetComponent<MiniGameCircle>().goalAchieved = false;
        }

        private void InitImage()
        {
            //print("INITMINIGAME" + item);
            if (Inventory.GetCurrentItemType() == Inventory.InventoryItems.FOSFORO || Inventory.GetCurrentItemType() == Inventory.InventoryItems.PAPEL)
            {
                animImage.sprite = Resources.Load<Sprite>("Sprites/Objects/Inventory/fosforo");
                animRect.rotation = Quaternion.Euler(new Vector3(0, 0, -90f));
                animRect.sizeDelta = new Vector2(50, 50);
                animRect.anchoredPosition = new Vector3(80, animRect.anchoredPosition.y);
            }
            else if (Inventory.GetCurrentItemType() == Inventory.InventoryItems.ISQUEIRO)
            {
                animImage.sprite = Resources.Load<Sprite>("Sprites/Objects/Inventory/isqueiro_faisca");
                animRect.rotation = Quaternion.Euler(new Vector3(0, 0, 45f));
                animRect.sizeDelta = new Vector2(50, 100);
                animRect.anchoredPosition = new Vector3(80, animRect.anchoredPosition.y);
            }
            else if (Inventory.GetCurrentItemType() == Inventory.InventoryItems.FACA)
            {
                animImage.sprite = Resources.Load<Sprite>("Sprites/Objects/Inventory/faca");
                animRect.rotation = Quaternion.Euler(new Vector3(180, 0, 180));
                animRect.sizeDelta = new Vector2(100, 20);
                animRect.anchoredPosition = new Vector3(80 - counter * (160 / counterMax), animRect.anchoredPosition.y);
            }
            else if (Inventory.GetCurrentItemType() == Inventory.InventoryItems.PEDRA)
            {
                animImage.sprite = Resources.Load<Sprite>("Sprites/Objects/Inventory/pedra");
                animRect.rotation = Quaternion.Euler(new Vector3(0, 0, -20));
                animRect.sizeDelta = new Vector2(60, 40);
                animRect.anchoredPosition = new Vector3(80 - counter * (160 / counterMax), animRect.anchoredPosition.y);
            }

        }
    }
}