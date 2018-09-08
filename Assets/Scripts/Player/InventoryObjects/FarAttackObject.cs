using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowObjects;

namespace CrowShadowPlayer
{
    public class FarAttackObject : InventoryObject
    {
        public Inventory.InventoryItems item;
        public bool hitSuccess = false, addObject = true;

        protected Player player;
        protected SpriteRenderer spriteRenderer;

        protected float distance = 0, maxDistance = 6f, startX = 0, startY = 0;
        protected float speed = 3f, translateSpeed = 5f;
        protected float arcHeight = 0.8f; // altura
        protected float timeLeftPedra = 0, maxTimePedra = 0.2f;
        protected int directionAttack = 0;
        protected bool changeDirection = false, initPositioning = false, initAttack = false, triggered = false;
        protected Vector3 posAttack = new Vector3(0, 0, 0), oldParentPosition;

        protected new void Start()
        {
            base.Start();
            player = GetComponentInParent<Player>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            active = false;
        }

        protected new void Update()
        {
            if (timeLeftPedra > 0)
            {
                timeLeftPedra -= Time.deltaTime;
            }

            if (hitSuccess)
            {
                EndThrow();
                // som acerto
            }
            else if (active && timeLeftPedra <= 0)
            {
                EndThrowAdd();
                // som chão
            }

            print("INIT: " + initAttack + active);
            if (Inventory.GetCurrentItemType() == item && !initAttack && !active &&
                !GameManager.instance.paused && !GameManager.instance.blocked && !GameManager.instance.pausedObject)
            {
                if (CrossPlatformInputManager.GetButtonDown("keyUseObject"))
                {
                    initPositioning = true;
                    spriteRenderer.enabled = true;
                    spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
                }
                else if (CrossPlatformInputManager.GetButton("keyUseObject"))
                {
                    if (changeDirection)
                    {
                        distance -= speed * Time.deltaTime;
                    }
                    else
                    {
                        distance += speed * Time.deltaTime;
                    }

                    if (distance >= maxDistance)
                    {
                        changeDirection = true;
                    }
                    else if (distance <= 0f)
                    {
                        changeDirection = false;
                    }
                }
                else if (CrossPlatformInputManager.GetButtonUp("keyUseObject"))
                {
                    posAttack = new Vector3(transform.position.x, transform.position.y, 0);

                    switch (player.direction)
                    {
                        case Player.Directions.EAST:
                            directionAttack = 0;
                            break;
                        case Player.Directions.WEST:
                            directionAttack = 1;
                            break;
                        case Player.Directions.NORTH:
                            directionAttack = 2;
                            break;
                        case Player.Directions.SOUTH:
                            directionAttack = 3;
                            break;
                        default:
                            break;
                    }

                    transform.localPosition = new Vector3(0, 0, 0);
                    if (directionAttack == 0 || directionAttack == 1)
                    {
                        spriteRenderer.color = new Color(1f, 1f, 1f, 0.75f);
                    }
                    else
                    {
                        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                    }
                    initPositioning = false;
                    initAttack = true;
                    startX = transform.position.x;
                    startY = transform.position.y;
                }
            }

            if (initAttack)
            {
                if (directionAttack == 0 || directionAttack == 1)
                {
                    float dist = posAttack.x - startX;
                    float nextX = Mathf.MoveTowards(transform.position.x, posAttack.x, translateSpeed * Time.deltaTime);
                    float baseY = Mathf.Lerp(startY, posAttack.y, (nextX - startX) / dist);
                    float arc = arcHeight * (nextX - startX) * (nextX - posAttack.x) / (-0.25f * dist * dist);
                    Vector3 nextPos = new Vector3(nextX, baseY + arc, transform.position.z);
                    transform.rotation = LookAt2D(nextPos - transform.position);
                    transform.position = nextPos;

                    if (Vector3.Distance(transform.position, posAttack) <= 1f) EndAnimation();
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, posAttack, translateSpeed * Time.deltaTime);
                    if (Mathf.Abs(transform.position.y) < Mathf.Abs(posAttack.y / 2))
                    {
                        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f + (Mathf.Abs(transform.position.y) / Mathf.Abs(posAttack.y)));
                    }
                    else
                    {
                        spriteRenderer.color = new Color(1f, 1f, 1f, 1f - (Mathf.Abs(transform.position.y) / Mathf.Abs(posAttack.y) / 2));
                    }

                    if (transform.position == posAttack) EndAnimation();
                }

            }
            else if (active)
            {
                transform.position = Vector3.MoveTowards(transform.position, posAttack, translateSpeed * Time.deltaTime);
            }
            else
            {
                switch (player.direction)
                {
                    case Player.Directions.EAST:
                        transform.localPosition = new Vector3(distance, 0, 0);
                        break;
                    case Player.Directions.WEST:
                        transform.localPosition = new Vector3(-distance, 0, 0);
                        break;
                    case Player.Directions.NORTH:
                        transform.localPosition = new Vector3(0, distance, 0);
                        break;
                    case Player.Directions.SOUTH:
                        transform.localPosition = new Vector3(0, -distance, 0);
                        break;
                    default:
                        break;
                }
            }
        }

        protected Quaternion LookAt2D(Vector2 forward)
        {
            return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
        }

        protected void EndAnimation()
        {
            initAttack = false;
            hitSuccess = false;
            active = true;
            timeLeftPedra = maxTimePedra;
        }

        protected void EndThrow()
        {
            spriteRenderer.enabled = false;
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = new Quaternion(0, 0, 0, 0);
            distance = 0;
            hitSuccess = false;
            active = false;
            timeLeftPedra = 0;
            Inventory.DeleteItem(item);
        }

        protected void EndThrowAdd()
        {
            if (!triggered && addObject)
            {
                string nameItem = "";
                if (item == Inventory.InventoryItems.PEDRA)
                {
                    nameItem = "pedra";
                }
                else if (item == Inventory.InventoryItems.PAPEL)
                {
                    nameItem = "papel";
                }
                GameObject pick = GameManager.instance.AddObject("Objects/PickUp", "Sprites/Objects/Inventory/" + nameItem, transform.position, new Vector3(0.6f, 0.6f, 1f));
                pick.GetComponent<PickUpObject>().item = item;
                pick.GetComponent<SpriteRenderer>().sortingLayerName = "BackLayer";
            }
            EndThrow();
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            //print("Pedra: " + collision.tag);
            if (collision.tag.Equals("Background"))
            {
                triggered = true;
            }
            if (collision.tag.Equals("MinnionEmitter"))
            {
                collision.gameObject.SetActive(false);

            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            //print("Pedra: " + collision.tag);
            if (collision.tag.Equals("Background"))
            {
                triggered = false;
            }
        }
    }
}