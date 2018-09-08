using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;

namespace CrowShadowPlayer
{
    public class AttackObject : InventoryObject
    {
        public Inventory.InventoryItems item;
        public static float timeAttack = 1f;

        Player player;
        CircleCollider2D colliderComponent;

        float timeLeftAttack = 0;

        new void Start()
        {
            base.Start();
            player = GetComponentInParent<Player>();
            colliderComponent = GetComponent<CircleCollider2D>();
        }

        new void Update()
        {
            if (timeLeftAttack > 0)
            {
                timeLeftAttack -= Time.deltaTime;
            }
            else if (CrossPlatformInputManager.GetButtonDown("keyUseObject") && Inventory.GetCurrentItemType() == item &&
                !GameManager.instance.paused && !GameManager.instance.blocked && !GameManager.instance.pausedObject)
            {
                timeLeftAttack = timeAttack;
                active = true;
                // animação + som

                string anim = "";
                if (item == Inventory.InventoryItems.BASTAO)
                {
                    anim = "bastao";
                }
                else if (item == Inventory.InventoryItems.FACA)
                {
                    anim = "faca";
                }

                switch (player.direction)
                {
                    case Player.Directions.EAST:
                        anim += "East";
                        break;
                    case Player.Directions.WEST:
                        anim += "West";
                        break;
                    case Player.Directions.NORTH:
                        anim += "North";
                        break;
                    case Player.Directions.SOUTH:
                        anim += "South";
                        break;
                    default:
                        break;
                }

                player.PlayAnimation(anim);
            }
            else if (timeLeftAttack <= 0)
            {
                active = false;
            }

            switch (player.direction)
            {
                case Player.Directions.EAST:
                    colliderComponent.offset = new Vector2(0.5f, 0);
                    break;
                case Player.Directions.WEST:
                    colliderComponent.offset = new Vector2(-0.5f, 0);
                    break;
                case Player.Directions.NORTH:
                    colliderComponent.offset = new Vector2(0, 0.5f);
                    break;
                case Player.Directions.SOUTH:
                    colliderComponent.offset = new Vector2(0, -0.5f);
                    break;
                default:
                    break;
            }

        }

    }
}