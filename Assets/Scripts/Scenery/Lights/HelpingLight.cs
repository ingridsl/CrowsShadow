using UnityEngine;
using CrowShadowManager;

namespace CrowShadowScenery
{
    public class HelpingLight : MonoBehaviour
    {
        public float speed;
        public Vector3[] targets;

        public bool emitter = false;
        public bool active = false;

        public bool stoped = false;
        public bool destroyEndPath = false;
        public bool stopEndPath = false;

        public bool playerInside = false;

        private int destPoint = 0, lastNumBird;
        private float timeEmitter = 0f, timeTotalEmitter = 10f;

        void Update()
        {
            if (active)
            {
                GotoNextPoint();
            }

            if (emitter && playerInside)
            {
                timeEmitter -= Time.deltaTime;
                if (timeEmitter <= 0f)
                {
                    int numBird = Random.Range(1, 9);
                    if (lastNumBird == numBird)
                    {
                        numBird = ((numBird + 4) % 9);
                    }
                    lastNumBird = numBird;
                    print("BIRDSSS: " + lastNumBird);
                    GameManager.instance.scenerySounds.PlayBird(1);
                    GameObject.Find("CrowBabies (" + numBird + ")").gameObject.transform.Find("BirdEmitterCollider").gameObject.SetActive(true);
                    timeEmitter = timeTotalEmitter;
                }
            }
        }

        void GotoNextPoint()
        {
            float step = speed * Time.deltaTime;
            if (targets.Length == 0) return;

            transform.position = Vector3.MoveTowards(transform.position, targets[destPoint], step);

            float dist = Vector3.Distance(targets[destPoint], transform.position);
            if (dist < 0.4f)
            {
                if (destPoint + 1 == targets.Length && destroyEndPath)
                {
                    Destroy(gameObject);
                }
                else if (destPoint + 1 == targets.Length && stopEndPath)
                {
                    Stop();
                }
                else
                {
                    destPoint = (destPoint + 1) % targets.Length;
                }
            }
        }

        public void Stop()
        {
            active = false;
            stoped = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (gameObject.activeSelf && collision.gameObject.tag.Equals("Player"))
            {
                playerInside = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (gameObject.activeSelf && collision.gameObject.tag.Equals("Player"))
            {
                playerInside = false;
            }
            HelpingLightManager.KillInDarkness();
        }
    }
}