using System.Collections.Generic;
using Image = UnityEngine.UI.Image;
using Text = UnityEngine.UI.Text;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;

namespace CrowShadowPlayer
{
    public class Inventory : MonoBehaviour
    {

        [System.Serializable]
        public enum InventoryItems { DEFAULT, FLASHLIGHT, FOSFORO, ISQUEIRO, FACA, BASTAO, TAMPA, ESCUDO, PEDRA, PAPEL, VELA, RACAO, LIVRO };
        public class DataItems
        {
            public InventoryItems type;
            public string file;

            public DataItems(InventoryItems type, string file)
            {
                this.type = type;
                this.file = file;
            }
        }
        public static bool open = false;
        public static int pedraCount = 0;
        public static int papelCount = 0;
        public AudioClip sound;

        GameObject menu, slotsPanel, imagesPanel;
        static GameObject menuItem, counterValue, hud, player;
        Sprite box, selectedBox;

        private AudioSource source { get { return GetComponent<AudioSource>(); } }
        private static List<DataItems> listItems;
        private static int currentItem = -1, lastItem = -1;
        private static InventoryObject currentItemObject = null;
        private int previousItem = -1;
        private int slotPedra = -1, slotPapel = -1;
        private bool activeShow = false;

        private void Awake()
        {
            hud = GameObject.Find("HUDCanvas");
            menu = hud.transform.Find("InventoryMenu").gameObject;
            menu.SetActive(false);

            menuItem = hud.transform.Find("SelectedObject").gameObject;
            counterValue = menuItem.transform.Find("CurrentTextCounter").gameObject;
            slotsPanel = hud.transform.Find("InventoryMenu/SlotsPanel").gameObject;
            imagesPanel = hud.transform.Find("InventoryMenu/ImagesPanel").gameObject;

            box = Resources.Load<Sprite>("Sprites/UI/box");
            selectedBox = Resources.Load<Sprite>("Sprites/UI/box-select");

            player = GameObject.FindGameObjectWithTag("Player");

            if (listItems == null) listItems = new List<DataItems>();
        }

        void Start()
        {
            // Adiciona todos os objetos, para testar
            // DELETAR PARA A VERSÃO FINAL
            /*NewItem(InventoryItems.LIVRO);
            NewItem(InventoryItems.RACAO);
            NewItem(InventoryItems.VELA);
            NewItem(InventoryItems.PAPEL);
            NewItem(InventoryItems.PEDRA);
            NewItem(InventoryItems.ESCUDO);
            NewItem(InventoryItems.TAMPA);
            NewItem(InventoryItems.BASTAO);
            NewItem(InventoryItems.FACA);
            NewItem(InventoryItems.FOSFORO);
            NewItem(InventoryItems.ISQUEIRO);
            NewItem(InventoryItems.FLASHLIGHT);*/

            gameObject.AddComponent<AudioSource>();
            source.clip = sound;
            source.playOnAwake = false;
        }

        void Update()
        {
            if (!GameManager.instance.showMissionStart && !GameManager.instance.blocked &&
                !GameManager.instance.pausedObject && !Book.show)
            {
                if (!menu.activeSelf && CrossPlatformInputManager.GetButtonDown("keyInventory"))
                {
                    ShowInventoryMenu();
                }
                else if (menu.activeSelf)
                {
                    if (CrossPlatformInputManager.GetButtonDown("keyInventory"))
                    {
                        ShowInventoryMenu(false);
                    }
                    else if (CrossPlatformInputManager.GetButtonDown("keyUseObject"))
                    {
                        ShowInventoryMenu(true);
                    }
                }
            }

            if (menu.activeSelf && currentItem != -1)
            {
                if (Input.GetButtonDown("Horizontal"))
                {
                    if (CrossPlatformInputManager.GetAxisRaw("Horizontal") > 0 && currentItem < listItems.Count - 1)
                    {
                        currentItem++;
                    }
                    else if (CrossPlatformInputManager.GetAxisRaw("Horizontal") < 0 && currentItem > 0)
                    {
                        currentItem--;
                    }
                }
                if (Input.GetButtonDown("Vertical"))
                {
                    if (CrossPlatformInputManager.GetAxisRaw("Vertical") < 0 && currentItem < listItems.Count - 4)
                    {
                        currentItem += 4;
                    }
                    else if (CrossPlatformInputManager.GetAxisRaw("Vertical") > 0 && currentItem > 3)
                    {
                        currentItem -= 4;
                    }
                }

                if (previousItem != currentItem)
                {
                    GameObject previousSlot = slotsPanel.transform.Find("Slot (" + previousItem + ")").gameObject;
                    previousSlot.GetComponent<Image>().sprite = box;
                    GameObject currentSlot = slotsPanel.transform.Find("Slot (" + currentItem + ")").gameObject;
                    currentSlot.GetComponent<Image>().sprite = selectedBox;
                    previousItem = currentItem;
                }
            }

            if (currentItemObject != null && !activeShow && currentItemObject.active)
            {
                menuItem.GetComponent<Image>().color = Color.red;
                activeShow = true;
            }
            else if (activeShow && ((currentItemObject != null && !currentItemObject.active) || currentItemObject == null))
            {
                menuItem.GetComponent<Image>().color = Color.white;
                activeShow = false;
            }
        }

        // MOSTRAR INVENTÁRIO
        void ShowInventoryMenu(bool change = false)
        {
            if (!source.isPlaying)
            {
                source.PlayOneShot(sound);
            }

            if (menu.activeSelf)
            {
                open = false;
                menu.SetActive(false);
                GameManager.instance.PauseGame(false);
                if (currentItem != -1 && change)
                {
                    GameObject currentSlot = slotsPanel.transform.Find("Slot (" + currentItem + ")").gameObject;
                    currentSlot.GetComponent<Image>().sprite = box;
                    previousItem = currentItem;
                    SetCurrentItem(currentItem);
                }
                DeleteCounterMenu();
            }
            else
            {
                open = true;
                menu.SetActive(true);
                GameManager.instance.PauseGame(true);

                int count = 0;
                foreach (DataItems i in listItems)
                {
                    GameObject slot = imagesPanel.transform.Find("Slot (" + count + ")").gameObject;
                    slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Objects/Inventory/" + i.file);
                    slot.GetComponent<Image>().preserveAspect = true;
                    SetCounterMenu(i.type, slot, count);
                    slot.SetActive(true);
                    count++;
                }
                for (; count < 16; count++)
                {
                    GameObject slot = imagesPanel.transform.Find("Slot (" + count + ")").gameObject;
                    slot.SetActive(false);
                }
                if (currentItem != -1)
                {
                    GameObject currentSlot = slotsPanel.transform.Find("Slot (" + currentItem + ")").gameObject;
                    currentSlot.GetComponent<Image>().sprite = selectedBox;
                    previousItem = currentItem;
                }
            }

        }

        // RETORNA ITEM ATUAL
        public static int GetCurrentItem()
        {
            return currentItem;
        }

        // RETORNA TIPO DO ITEM
        public static InventoryItems GetItemType(int i)
        {
            if (i == -1 || i >= listItems.Count) return InventoryItems.DEFAULT;
            return listItems[i].type;
        }

        // RETORNA TIPO DO ITEM ATUAL
        public static InventoryItems GetCurrentItemType()
        {
            return GetItemType(currentItem);
        }

        // RETORNA SE O ITEM ATUAL É DE UM TIPO ESPECÍFICO
        public static bool IsCurrentItemType(InventoryItems item, bool isActive = false)
        {
            if (isActive)
            {
                return (currentItemObject != null && currentItemObject.active && GetItemType(currentItem) == item);
            }
            return (GetItemType(currentItem) == item);
        }

        // RETORNA INVENTÁRIO
        public static List<DataItems> GetInventory()
        {
            return listItems;
        }

        // RETORNA TIPOS DOS ITENS DO INVENTÁRIO
        public static List<InventoryItems> GetInventoryItems()
        {
            List<InventoryItems> list = new List<InventoryItems>();

            if (listItems != null)
            {
                for (int i = 0; i < listItems.Count; i++)
                {
                    list.Add(listItems[i].type);
                }
            }

            return list;
        }

        // VERIFICA SE ITEM ESTÁ NO INVENTÁRIO
        public static bool HasItemType(InventoryItems item)
        {
            if (listItems != null && listItems.Count > 0)
            {
                foreach (DataItems i in listItems)
                {
                    if (item == i.type)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // DEFINE ITEM ATUAL
        public static void SetCurrentItem(int pos)
        {
            if (lastItem != -1)
            {
                EnableItem(GetItemType(lastItem), false);
            }
            currentItem = pos;
            EnableItem(GetCurrentItemType());

            if (pos != -1)
            {
                if (menuItem == null)
                {
                    menuItem = hud.transform.Find("SelectedObject").gameObject;
                }
                if (!menuItem.activeSelf && !(GameManager.instance.mission is Mission9))
                {
                    menuItem.SetActive(true);
                }
                menuItem.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Objects/Inventory/" + listItems[pos].file);
                menuItem.GetComponent<Image>().preserveAspect = true;
                SetCurrentValue();
            }

            lastItem = currentItem;
        }

        // HABILITA ITEM NO PLAYER
        private static void EnableItem(InventoryItems item, bool e = true)
        {
            GameObject itemObject;
            switch (item)
            {
                case InventoryItems.FLASHLIGHT:
                    itemObject = player.transform.Find("Flashlight").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.FOSFORO:
                    itemObject = player.transform.Find("Fosforo").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.ISQUEIRO:
                    itemObject = player.transform.Find("Isqueiro").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.FACA:
                    itemObject = player.transform.Find("Faca").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.BASTAO:
                    itemObject = player.transform.Find("Bastao").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.TAMPA:
                    itemObject = player.transform.Find("Tampa").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.ESCUDO:
                    itemObject = player.transform.Find("Escudo").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.PEDRA:
                    itemObject = player.transform.Find("Pedra").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.PAPEL:
                    itemObject = player.transform.Find("Papel").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.VELA:
                    itemObject = player.transform.Find("Vela").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.RACAO:
                    itemObject = player.transform.Find("Racao").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                case InventoryItems.LIVRO:
                    itemObject = player.transform.Find("Livro").gameObject;
                    if (e) currentItemObject = itemObject.GetComponent<InventoryObject>();
                    itemObject.SetActive(e);
                    break;
                default:
                    break;
            }
        }

        // DETERMINA INVENTÁRIO
        public static void SetInventory(List<InventoryItems> invItems)
        {
            listItems = new List<DataItems>();
            if (invItems != null)
            {
                for (int i = 0; i < invItems.Count; i++)
                {
                    NewItem(invItems[i]);
                }
            }
            else
            {
                currentItem = -1;
                currentItemObject = null;
                pedraCount = 0;
                papelCount = 0;
            }
        }

        // ADICIONA NOVO ITEM
        public static void NewItem(InventoryItems selectItem)
        {

            // Não permite ter mais de um mesmo objeto no inventário
            if (listItems == null) listItems = new List<DataItems>();
            if (selectItem == InventoryItems.PEDRA)
            {
                pedraCount++;
                SetCurrentValue();
                if (pedraCount > 1) return;
            }
            else if (selectItem == InventoryItems.PAPEL)
            {
                papelCount ++;
                SetCurrentValue();
                if (papelCount > 1) return;
            }
            else
            {
                if (listItems.Count > 0)
                {
                    foreach (DataItems i in listItems)
                    {
                        if (selectItem == i.type)
                        {
                            return;
                        }
                    }
                }
            }

            string file = "";
            switch (selectItem)
            {
                case InventoryItems.FLASHLIGHT:
                    file = "lanterna";
                    break;
                case InventoryItems.FOSFORO:
                    file = "caixa_fosforo_maior";
                    break;
                case InventoryItems.ISQUEIRO:
                    file = "isqueiro";
                    break;
                case InventoryItems.FACA:
                    file = "faca";
                    break;
                case InventoryItems.BASTAO:
                    file = "bastao";
                    break;
                case InventoryItems.TAMPA:
                    file = "tampa";
                    player.transform.Find("Tampa").gameObject.GetComponent<ProtectionObject>().life = 100;
                    break;
                case InventoryItems.ESCUDO:
                    file = "escudo";
                    player.transform.Find("Escudo").gameObject.GetComponent<ProtectionObject>().life = 80;
                    break;
                case InventoryItems.PEDRA:
                    file = "pedra";
                    break;
                case InventoryItems.PAPEL:
                    file = "papel";
                    break;
                case InventoryItems.VELA:
                    file = "vela";
                    break;
                case InventoryItems.RACAO:
                    file = "saco_racao";
                    break;
                case InventoryItems.LIVRO:
                    file = "book";
                    break;
                default:
                    break;
            }

            DataItems novoItem = new DataItems(selectItem, file);
            listItems.Add(novoItem);
            SetCurrentItem(listItems.Count - 1);
        }

        // DELETA ITEM
        public static void DeleteItem(InventoryItems selectItem)
        {
            int count = 0;
            foreach (DataItems i in listItems)
            {
                if (i.type == selectItem)
                {
                    if (selectItem == InventoryItems.PEDRA)
                    {
                        pedraCount--;
                        SetCurrentValue();
                        if (pedraCount > 0)
                        {
                            return;
                        }
                    }
                    else if (selectItem == InventoryItems.PAPEL)
                    {
                        papelCount--;
                        SetCurrentValue();
                        if (papelCount > 0)
                        {
                            return;
                        }
                    }

                    if (lastItem != -1)
                    {
                        EnableItem(GetItemType(lastItem), false);
                        lastItem = -1;
                    }

                    if (selectItem == listItems[currentItem].type)
                    {
                        listItems.RemoveAt(count);

                        if (listItems.Count > 0)
                        {
                            SetCurrentItem(listItems.Count - 1);
                        }
                        else
                        {
                            menuItem.SetActive(false);
                            currentItem = -1;
                            currentItemObject = null;
                        }
                    }
                    else
                    {
                        listItems.RemoveAt(count);
                    }
                    break;
                }
                count++;
            }

            EnableItem(selectItem, false);

        }

        // DETERMINA VALOR ATUAL DA PEDRA/PAPEL
        public static void SetCurrentValue()
        {
            if (GetCurrentItemType() == InventoryItems.PEDRA)
            {
                if (!counterValue.activeSelf)
                {
                    counterValue.SetActive(true);
                }
                counterValue.GetComponent<Text>().text = "" + pedraCount;
            }
            else if (GetCurrentItemType() == InventoryItems.PAPEL)
            {
                if (!counterValue.activeSelf)
                {
                    counterValue.SetActive(true);
                }
                counterValue.GetComponent<Text>().text = "" + papelCount;
            }
            else
            {
                if (counterValue == null)
                {
                    counterValue = menuItem.transform.Find("CurrentTextCounter").gameObject;
                }
                if (counterValue != null && counterValue.activeSelf)
                {
                    counterValue.SetActive(false);
                }
            }
        }

        // ATUALIZA CONTADOR DA PEDRA/PAPEL NO MENU
        private void SetCounterMenu(InventoryItems item, GameObject slot, int pos)
        {
            if (item == InventoryItems.PEDRA)
            {
                GameObject text = GameManager.instance.AddObjectWithParent("UI/TextCounter", slot.transform);
                text.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                text.name = "TextPedra";
                text.GetComponent<Text>().text = "" + pedraCount;
                slotPedra = pos;
            }
            else if (item == InventoryItems.PAPEL)
            {
                GameObject text = GameManager.instance.AddObjectWithParent("UI/TextCounter", slot.transform);
                text.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                text.name = "TextPapel";
                text.GetComponent<Text>().text = "" + papelCount;
                slotPapel = pos;
            }
        }

        // APAGA CONTADOR DA PEDRA/PAPEL NO MENU
        private void DeleteCounterMenu()
        {
            if (slotPedra != -1)
            {
                GameObject slot = imagesPanel.transform.Find("Slot (" + slotPedra + ")").gameObject;
                GameObject text = slot.transform.Find("TextPedra").gameObject;
                Destroy(text);
                slotPedra = -1;
            }
            if (slotPapel != -1)
            {
                GameObject slot = imagesPanel.transform.Find("Slot (" + slotPapel + ")").gameObject;
                GameObject text = slot.transform.Find("TextPapel").gameObject;
                Destroy(text);
                slotPapel = -1;
            }
        }

    }
}