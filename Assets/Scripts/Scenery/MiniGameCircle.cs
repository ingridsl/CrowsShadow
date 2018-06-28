using UnityEngine;
using CrowShadowManager;
using UnityStandardAssets.CrossPlatformInput;

namespace CrowShadowScenery
{
    public class MiniGameCircle : MonoBehaviour
    {
        public GameObject pointer;
        public float speed = 60;
        public GameObject miniGame;
        public bool inside = false;

        private void Start()
        {
            
            transform.rotation.ToAngleAxis(out angle, out axis);
        }

        private void Update()
        {
            pointer.transform.Rotate(Vector3.back * Time.deltaTime * speed/*, Space.World*/);
            var angleQuart2 = pointer.transform.rotation.z;
            var angleEuler2 = pointer.transform.rotation.eulerAngles.z;
            if (angleEuler2 > 180)
                angleEuler2 /= 2;

            if (CrossPlatformInputManager.GetButtonDown("keyMiniGame") & inside) { 
                  miniGame.SetActive(false);                
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

    }
}
