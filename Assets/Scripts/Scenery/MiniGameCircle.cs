using UnityEngine;
using CrowShadowManager;
using UnityStandardAssets.CrossPlatformInput;

namespace CrowShadowScenery
{
    public class MiniGameCircle : MonoBehaviour
    {
        public GameObject pointer;
        public float speed = 300;
        public GameObject miniGame;
        public bool inside = false;
        private bool playing = false;
        public bool goalAchieved = false;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (CrossPlatformInputManager.GetButtonDown("keyUseObject") && !playing) //GetKeyDown e GetKeyUp não pode ser usado fora do Update
            {
                GameManager.instance.pausedObject = true;
                playing = true;
            }
            pointer.transform.Rotate(Vector3.back * Time.deltaTime * speed);

            if (CrossPlatformInputManager.GetButtonDown("keyMiniGame") & inside) {
                StopMiniGame();
            }


        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("MiniGameRightArea"))
            {

                inside = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("MiniGameRightArea"))
            {

                inside = false;
            }
        }

        public void StopMiniGame()
        {

            GameManager.instance.pausedObject = false;

            goalAchieved = true;
            playing = false;
        }


    }
}
