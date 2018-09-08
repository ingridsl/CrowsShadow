using UnityStandardAssets.CrossPlatformInput;

namespace CrowShadowPlayer
{
    public class PlaceObject : InventoryObject
    {
        public Inventory.InventoryItems item;
        public bool inArea = false;

        new void Start()
        {
            base.Start();
        }

        new void Update()
        {
            if (Inventory.GetCurrentItemType() == item && inArea && CrossPlatformInputManager.GetButtonDown("keyUseObject"))
            {
                Inventory.DeleteItem(item);
            }
        }
    }
}