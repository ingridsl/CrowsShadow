using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowPlayer;

namespace CrowShadowScenery
{
    public class WindowTrigger : MonoBehaviour
    {
        public Sprite aberto;
        public Sprite fechado;
        public bool colliding = false;
        public bool scare = false;

        GameObject birdEmitter;
        SpriteRenderer spriteRenderer;
        BoxCollider2D boxCollider;

        float sizeX, sizeY;
        //float posXdefault;
        bool gameOver = false;
        float timeLeft = 5;

        void Start()
        {
            birdEmitter = transform.Find("BirdEmitter").gameObject;
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = aberto;
            }
            sizeX = boxCollider.size.x / spriteRenderer.bounds.size.x;
            sizeY = boxCollider.size.y / spriteRenderer.bounds.size.y;

            //posXdefault = transform.position.x;
        }

        void Update()
        {
            if (CrossPlatformInputManager.GetButtonDown("keyInteract") && colliding && 
                !GameManager.instance.paused && !GameManager.instance.blocked) //GetKeyDown e GetKeyUp não pode ser usado fora do Update
            {
                ChangeSprite();
            }
            if (gameOver)
            {
                if (timeLeft > 0)
                {
                    timeLeft -= Time.deltaTime;
                }
                else
                {
                    GameManager.instance.GameOver();
                }
            }
        }

        void ChangeSprite()
        {
            if (spriteRenderer.sprite == aberto)
            {
                spriteRenderer.sprite = fechado;
            }
            else if (spriteRenderer.sprite == fechado)
            {
                spriteRenderer.sprite = aberto;
            }

            boxCollider.size = new Vector2(
                sizeX * spriteRenderer.bounds.size.x,
                sizeY * spriteRenderer.bounds.size.y);
        }

        public void ScareTrigger()
        {
            if (spriteRenderer.sprite == aberto && scare && !Inventory.IsCurrentItemType(Inventory.InventoryItems.FLASHLIGHT, true))
            {
                GameManager.instance.scenerySounds.PlayScare(1);
                GameManager.instance.scenerySounds.PlayBird(1);
                GameManager.instance.scenerySounds.PlayBird(4);
                birdEmitter.SetActive(true);
                GameManager.instance.blocked = true;
                gameOver = true;
            }
        }

    }
}