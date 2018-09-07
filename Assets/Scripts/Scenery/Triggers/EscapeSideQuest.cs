using UnityEngine;
using CrowShadowManager;

namespace CrowShadowScenery
{
    public class EscapeSideQuest : MonoBehaviour
    {

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                ChangeScene();
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.tag.Equals("Player") && !GameManager.instance.paused)
            {
                ChangeScene();
            }
        }

        private void ChangeScene()
        {
            GameManager.instance.blocked = true;
            GameManager.instance.sideQuest.EndSideQuest();
        }
    }
}