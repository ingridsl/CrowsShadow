using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;

namespace CrowShadowPlayer
{
    public class ProtectionObject : InventoryObject
    {
        public Inventory.InventoryItems item;
        public int life = 60;

        Player player;

        new void Start()
        {
            base.Start();
            player = GetComponentInParent<Player>();
        }

        new void Update()
        {
            if (life <= 0)
            {
                GameManager.instance.playerProtected = false;
                Inventory.DeleteItem(item);
            }
            else
            {
                if (Inventory.GetCurrentItemType() == item && !GameManager.instance.paused &&
                    !GameManager.instance.blocked && !GameManager.instance.pausedObject &&
                    CrossPlatformInputManager.GetButtonDown("keyUseObject"))
                {
                    EnterProtection(!active);
                }
                else if (Inventory.GetCurrentItemType() != item && active)
                {
                    EnterProtection(false);
                }
            }
        }

        private void EnterProtection(bool e = true)
        {
            active = e;
            GameManager.instance.playerProtected = e;
            // mudar imagem do player
            if (active)
            {
                if (item == Inventory.InventoryItems.TAMPA)
                {
                    player.ChangeState((int)Player.States.PROTECTED_TAMPA);
                }
                else
                {
                    player.ChangeState((int)Player.States.PROTECTED_ESCUDO);
                }
            }
            else
            {
                player.ChangeState((int)Player.States.DEFAULT);
            }
        }

        public void DecreaseLife()
        {
            life--;
        }
    }
}