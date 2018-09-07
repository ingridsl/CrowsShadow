using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowPlayer;

namespace CrowShadowScenery
{
    /**
     * 
     * senha cofre corredor: 159
     * senha cadeado porão: 376
     * 
     falta:
        - objeto a ser inserido quando abre cofre atras do quadro na parede
         
         **/
    public class LockerController : MonoBehaviour
    {
        public Text row1;
        public Text row2;
        public Text row3;

        public string password;
        public string boardName;

        public bool isBasement = false;

        public AudioClip click;
        public AudioClip success;

        private AudioSource source { get { return GetComponent<AudioSource>(); } }

        GameObject locker, basement;
        SceneDoor sceneDoor;
        public SpriteRenderer rendererBasement;

        int selectedRow = 1;
        int selectedNumber1 = 1;
        int selectedNumber2 = 1;
        int selectedNumber3 = 1;

        bool opened = false;
        bool tried = false;

        public void Start()
        {
            source.clip = click;
            locker = transform.GetChild(0).gameObject;
            if (isBasement)
            {
                basement = GameObject.Find("Jardim").gameObject.transform.GetChild(0).gameObject;
                sceneDoor = basement.GetComponent<SceneDoor>();
                //rendererBasement = basement.GetComponent<SpriteRenderer>();
            }
        }

        public void Update()
        {
            if (locker.activeSelf)
            {
                if (Input.GetButtonDown("Vertical"))
                { //up(positive) e down(negative)
                    if (CrossPlatformInputManager.GetAxisRaw("Vertical") < 0)
                    {
                        DownRow();
                    }
                    else if (CrossPlatformInputManager.GetAxisRaw("Vertical") > 0)
                    {
                        UpRow();
                    }
                }
                else if (Input.GetButtonDown("Horizontal"))
                {//right(positive) e left(negative)
                    if (CrossPlatformInputManager.GetAxisRaw("Horizontal") < 0)
                    {
                        NumbersRight();
                    }
                    else if (CrossPlatformInputManager.GetAxisRaw("Horizontal") > 0)
                    {
                        NumbersLeft();
                    }
                }
                if (locker.activeSelf)
                {
                    if (row1.text[3] == password[0] && row2.text[3] == password[1] && row3.text[3] == password[2] && !opened)
                    {
                        opened = true;
                        if (!isBasement && Book.pageQuantity >= 6) //cofre atrás do quadro - descomentar na versão final. comentário apenas para testes
                        {
                            //adicionar algo do inventário
                            source.clip = success;
                            source.PlayOneShot(success);
                            Invoke("Show", 0.5f);
                        }
                        else if (Book.pageQuantity >= 4 && isBasement)
                        { // porta do porão
                            source.clip = success;
                            source.PlayOneShot(success);

                            Invoke("Show", 0.5f);
                            //sceneDoor.isOpened = true;
                            var doorToBasement = GameObject.Find("Jardim").gameObject.transform.GetChild(0).gameObject;
                            doorToBasement.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/vasoPlanta_quebrado");
                            doorToBasement.GetComponent<SceneDoor>().isOpened = true;

                        }
                    }
                }
            }
        }


        void DownRow()
        {
            GameObject selectorRow;
            if (selectedRow == 3)
                selectedRow = 1;
            else
                selectedRow++;

            switch (selectedRow)
            {
                case 1:
                    selectorRow = locker.transform.Find("SelectorRow3").gameObject;
                    selectorRow.SetActive(false);
                    selectorRow = locker.transform.Find("SelectorRow1").gameObject;
                    selectorRow.SetActive(true);
                    break;
                case 2:
                    selectorRow = locker.transform.Find("SelectorRow1").gameObject;
                    selectorRow.SetActive(false);
                    selectorRow = locker.transform.Find("SelectorRow2").gameObject;
                    selectorRow.SetActive(true);
                    break;
                case 3:
                    selectorRow = locker.transform.Find("SelectorRow2").gameObject;
                    selectorRow.SetActive(false);
                    selectorRow = locker.transform.Find("SelectorRow3").gameObject;
                    selectorRow.SetActive(true);
                    break;
            }
            source.PlayOneShot(click);
        }

        void UpRow()
        {
            GameObject selectorRow;
            if (selectedRow == 1)
                selectedRow = 3;
            else
                selectedRow--;

            switch (selectedRow)
            {
                case 1:
                    selectorRow = locker.transform.Find("SelectorRow2").gameObject;
                    selectorRow.SetActive(false);
                    selectorRow = locker.transform.Find("SelectorRow1").gameObject;
                    selectorRow.SetActive(true);
                    break;
                case 2:
                    selectorRow = locker.transform.Find("SelectorRow3").gameObject;
                    selectorRow.SetActive(false);
                    selectorRow = locker.transform.Find("SelectorRow2").gameObject;
                    selectorRow.SetActive(true);
                    break;
                case 3:
                    selectorRow = locker.transform.Find("SelectorRow1").gameObject;
                    selectorRow.SetActive(false);
                    selectorRow = locker.transform.Find("SelectorRow3").gameObject;
                    selectorRow.SetActive(true);
                    break;


            }
            source.PlayOneShot(click);

        }
        void NumbersLeft()
        {
            if (selectedRow == 1)
            {
                if (selectedNumber1 == 10)
                    selectedNumber1 = 1;
                else
                    selectedNumber1++;
            }
            else if (selectedRow == 2)
            {
                if (selectedNumber2 == 10)
                    selectedNumber2 = 1;
                else
                    selectedNumber2++;
            }
            else if (selectedRow == 3)
            {
                if (selectedNumber3 == 10)
                    selectedNumber3 = 1;
                else
                    selectedNumber3++;
            }
            ChangeText();
        }

        void NumbersRight()
        {
            if (selectedRow == 1)
            {
                if (selectedNumber1 == 1)
                    selectedNumber1 = 10;
                else
                    selectedNumber1--;
            }
            else if (selectedRow == 2)
            {
                if (selectedNumber2 == 1)
                    selectedNumber2 = 10;
                else
                    selectedNumber2--;
            }
            else if (selectedRow == 3)
            {
                if (selectedNumber3 == 1)
                    selectedNumber3 = 10;
                else
                    selectedNumber3--;
            }
            ChangeText();
        }

        void ChangeText()
        {

            int selectedNumber = 0;
            if (selectedRow == 1)
                selectedNumber = selectedNumber1;
            else if (selectedRow == 2)
                selectedNumber = selectedNumber2;
            else if (selectedRow == 3)
                selectedNumber = selectedNumber3;

            source.PlayOneShot(click);
            switch (selectedNumber)
            {
                case 1:
                    if (selectedRow == 1)
                        row1.text = "9  0  1";
                    else if (selectedRow == 2)
                        row2.text = "9  0  1";
                    else if (selectedRow == 3)
                        row3.text = "9  0  1";
                    break;
                case 2:
                    if (selectedRow == 1)
                        row1.text = "0  1  2";
                    else if (selectedRow == 2)
                        row2.text = "0  1  2";
                    else if (selectedRow == 3)
                        row3.text = "0  1  2";
                    break;
                case 3:
                    if (selectedRow == 1)
                        row1.text = "1  2  3";
                    else if (selectedRow == 2)
                        row2.text = "1  2  3";
                    else if (selectedRow == 3)
                        row3.text = "1  2  3";
                    break;
                case 4:
                    if (selectedRow == 1)
                        row1.text = "2  3  4";
                    else if (selectedRow == 2)
                        row2.text = "2  3  4";
                    else if (selectedRow == 3)
                        row3.text = "2  3  4";
                    break;
                case 5:
                    if (selectedRow == 1)
                        row1.text = "3  4  5";
                    else if (selectedRow == 2)
                        row2.text = "3  4  5";
                    else if (selectedRow == 3)
                        row3.text = "3  4  5";
                    break;
                case 6:
                    if (selectedRow == 1)
                        row1.text = "4  5  6";
                    else if (selectedRow == 2)
                        row2.text = "4  5  6";
                    else if (selectedRow == 3)
                        row3.text = "4  5  6";
                    break;
                case 7:
                    if (selectedRow == 1)
                        row1.text = "5  6  7";
                    else if (selectedRow == 2)
                        row2.text = "5  6  7";
                    else if (selectedRow == 3)
                        row3.text = "5  6  7";
                    break;
                case 8:
                    if (selectedRow == 1)
                        row1.text = "6  7  8";
                    else if (selectedRow == 2)
                        row2.text = "6  7  8";
                    else if (selectedRow == 3)
                        row3.text = "6  7  8";
                    break;
                case 9:
                    if (selectedRow == 1)
                        row1.text = "7  8  9";
                    else if (selectedRow == 2)
                        row2.text = "7  8  9";
                    else if (selectedRow == 3)
                        row3.text = "7  8  9";
                    break;
                case 10:
                    if (selectedRow == 1)
                        row1.text = "8  9  0";
                    else if (selectedRow == 2)
                        row2.text = "8  9  0";
                    else if (selectedRow == 3)
                        row3.text = "8  9  0";
                    break;
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            GameObject board;

            if (!tried && other.gameObject.tag.Equals("Player") && 
                CrossPlatformInputManager.GetButton("keyInteract") && !locker.activeSelf)
            {

                if (GameManager.currentSceneName.Equals("Corridor"))
                {
                    board = GameObject.Find(boardName).gameObject;
                    board.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/cofre");
                }
                else if (GameManager.currentSceneName.Equals("Jardim"))
                {
                    isBasement = true;
                }

                Invoke("Show", 1f);
                tried = true;

            }
            else if (tried && other.gameObject.tag.Equals("Player") && 
                CrossPlatformInputManager.GetButton("keyInteract") && locker.activeSelf)
            {
                if (GameManager.currentSceneName.Equals("Corridor"))
                {
                    board = GameObject.Find(boardName).gameObject;
                    board.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Objects/Scene/quadroInutil2");
                }
                else if (GameManager.currentSceneName.Equals("Jardim"))
                {
                    isBasement = true;
                }
                Invoke("Show", 0.2f);
                tried = false;
            }


        }

        void Show()
        {
            if (locker.activeSelf)
            {
                locker.SetActive(false);
                GameManager.instance.blocked = false;
            }
            else
            {
                locker.SetActive(true);
                GameManager.instance.blocked = true;
            }
        }
        
    }
}