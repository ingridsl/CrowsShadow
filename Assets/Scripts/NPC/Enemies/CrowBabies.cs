using UnityEngine;
using CrowShadowManager;

namespace CrowShadowNPCs
{
    public class CrowBabies : Follower
    {
        public float timeBirdsFollow = 1f;
        public float timeBurst = 10f;
        public int numBursts = -1;
        public float countTimeBurst = 0f;

        protected GameObject birdEmitter;
        protected ParticleSystem particleSystemCrow;

        protected int countBurst = 0;        
        protected bool activated = false;

        protected new void Start()
        {
            player = GameManager.instance.gameObject;
            birdEmitter = transform.Find("BirdEmitterCollider").gameObject;

            if (numBursts == 1)
            {
                particleSystemCrow = birdEmitter.GetComponent<ParticleSystem>();
                var main = particleSystemCrow.main;
                main.loop = false;
                numBursts = -1;
            }
        }

        protected new void Update()
        {

            if (followingPlayer)
            {
                float dist = Vector3.Distance(player.transform.position, transform.position);

                if (dist > 0.6f)
                {
                    transform.position = Vector3.Lerp(transform.position, player.transform.position, speed * Time.deltaTime);
                }

            }
            else if (isPatroller)
            {
                GotoNextPoint();
            }

            if (birdEmitter.activeSelf)
            {
                if (!activated)
                {
                    birdEmitter.transform.LookAt(player.transform);
                    activated = true;
                    countTimeBurst = 0f;
                    countBurst = 0;
                }
                Vector3 dir = player.transform.position - transform.position;
                Quaternion rot = Quaternion.LookRotation(dir);
                birdEmitter.transform.rotation = Quaternion.Lerp(birdEmitter.transform.rotation, rot, timeBirdsFollow / 2f * Time.deltaTime);

                if (numBursts != -1)
                {
                    countTimeBurst += Time.deltaTime;
                    if (countTimeBurst >= timeBurst)
                    {
                        countBurst++;
                        countTimeBurst = 0f;
                        if (countBurst >= numBursts)
                        {
                            var main = particleSystemCrow.main;
                            main.loop = false;
                        }
                    }
                }
            }
            else if (activated)
            {
                activated = false;
            }

        }

        public void LookAtPlayer()
        {
            if (birdEmitter != null && player != null)
            {
                birdEmitter.transform.LookAt(player.transform);
            }
        }

        public void DestroyCorvBabies()
        {
            Destroy(gameObject);
        }
    }
}