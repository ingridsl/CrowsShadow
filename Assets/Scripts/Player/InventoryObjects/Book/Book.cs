using Image = UnityEngine.UI.Image;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;

namespace CrowShadowPlayer
{
    public class Book : MonoBehaviour
    {
        public static int pageQuantity = 0;
        public static bool bookBlocked = true;
        public static bool show = false;

        GameObject book, page1, page2, pagebonus;
        Sprite pg1, pg2, pg3, pg4, pg5, pg6, pg78;

        private int pageShowing; // 0 - 2
        private bool lastPageSeen = false, seenAll = false, invertWorldAux = false;

        void Start()
        {
            GameObject hud = GameObject.Find("HUDCanvas");
            book = hud.transform.Find("Book").gameObject;
            page1 = hud.transform.Find("Book/Page 1").gameObject;
            page2 = hud.transform.Find("Book/Page 2").gameObject;
            pagebonus = hud.transform.Find("Book/Page Bonus").gameObject;
            invertWorldAux = GameManager.instance.invertWorld;
            pageShowing = 0;

            if (PlayerPrefs.HasKey("Language") && PlayerPrefs.GetString("Language").Equals("PR_BR"))
            {
                //portugês
                pg1 = Resources.Load<Sprite>("Sprites/UI/book/page1");
                pg2 = Resources.Load<Sprite>("Sprites/UI/book/page2");
                pg3 = Resources.Load<Sprite>("Sprites/UI/book/page3");
                pg4 = Resources.Load<Sprite>("Sprites/UI/book/page4");
                pg5 = Resources.Load<Sprite>("Sprites/UI/book/page5");
                pg6 = Resources.Load<Sprite>("Sprites/UI/book/page6");
                pg78 = Resources.Load<Sprite>("Sprites/UI/book/page7-8");
            }
            else
            {
                //inglês
                pg1 = Resources.Load<Sprite>("Sprites/UI/book/page1");
                pg2 = Resources.Load<Sprite>("Sprites/UI/book/page2");
                pg3 = Resources.Load<Sprite>("Sprites/UI/book/page3");
                pg4 = Resources.Load<Sprite>("Sprites/UI/book/page4");
                pg5 = Resources.Load<Sprite>("Sprites/UI/book/page5");
                pg6 = Resources.Load<Sprite>("Sprites/UI/book/page6");
                pg78 = Resources.Load<Sprite>("Sprites/UI/book/page7-8");
            }
        }
        
        void Update()
        {
            if ((CrossPlatformInputManager.GetButtonDown("keyJournal") ||
                (CrossPlatformInputManager.GetButtonDown("keyUseObject") && Inventory.GetCurrentItemType() == Inventory.InventoryItems.LIVRO))
                && !Inventory.open && !GameManager.instance.showMissionStart && !GameManager.instance.blocked &&
                !GameManager.instance.pausedObject && !bookBlocked)
            {
                ShowBook();
            }

            if (invertWorldAux != GameManager.instance.invertWorld)
            {
                invertWorldAux = GameManager.instance.invertWorld;
                ShowBookPages();
            }

            if (book.activeSelf && CrossPlatformInputManager.GetButtonDown("Horizontal"))
            {
                // Som de folha mexendo
                page1.SetActive(false); page2.SetActive(false);
                pagebonus.SetActive(false);
                if (CrossPlatformInputManager.GetAxisRaw("Horizontal") > 0)
                {
                    if (pageShowing < 2 || (pageShowing < 3 && pageQuantity > 5))
                    {
                        pageShowing++;
                    }
                }
                if (CrossPlatformInputManager.GetAxisRaw("Horizontal") < 0)
                {
                    if (pageShowing > 0)
                    {
                        pageShowing--;
                    }
                }
                ShowBookPages();
            }
        }

        public void ShowBook()
        {
            if (book.activeSelf)
            {
                GameManager.instance.scenerySounds2.PlayPaper(3);
                if (lastPageSeen) seenAll = true;
                book.SetActive(false);
                GameManager.instance.PauseGame(false);
                show = false;
            }
            else
            {
                GameManager.instance.scenerySounds2.PlayPaper(2);
                book.SetActive(true);
                GameManager.instance.PauseGame(true);
                show = true;
                ShowBookPages();
            }
        }

        private void ShowBookPages()
        {
            if (book.activeSelf && pageQuantity > 0 && GameManager.instance.invertWorld)
            {
                if (pageShowing == 0)
                {
                    if (pageQuantity > 0)
                    {
                        page1.SetActive(true);
                        page1.GetComponent<Image>().sprite = pg1;
                    }
                    if (pageQuantity > 1)
                    {
                        page2.SetActive(true);
                        page2.GetComponent<Image>().sprite = pg2;
                    }
                }
                else if (pageShowing == 1)
                {
                    if (pageQuantity > 2)
                    {
                        page1.SetActive(true);
                        page1.GetComponent<Image>().sprite = pg3;
                    }
                    if (pageQuantity > 3)
                    {
                        page2.SetActive(true);
                        page2.GetComponent<Image>().sprite = pg4;
                    }
                }
                else if (pageShowing == 2)
                {
                    if (pageQuantity > 4)
                    {
                        page1.SetActive(true);
                        page1.GetComponent<Image>().sprite = pg5;
                    }
                    if (pageQuantity > 5)
                    {
                        page2.SetActive(true);
                        page2.GetComponent<Image>().sprite = pg6;
                    }
                }
                else if (pageShowing == 3)
                {
                    lastPageSeen = true;
                    pagebonus.SetActive(true);
                    pagebonus.GetComponent<Image>().sprite = pg78;
                }
            }
            else
            {
                page1.SetActive(false);
                page2.SetActive(false);
                pagebonus.SetActive(false);
            }
        }

        public static void AddPage()
        {
            pageQuantity++;
        }

        public bool SeenAll()
        {
            return seenAll;
        }

    }
}