using UnityEngine;
using System.Collections.Generic;
using CrowShadowManager;
using CrowShadowPlayer;

namespace CrowShadowNPCs
{
    public class MinionEmmitter : MonoBehaviour
    {

        public GameObject minionObject = null;
        public List<GameObject> minionList = new List<GameObject>();
        public int numMinions = 5, currentMinions = 0;
        public bool hydraEffect = true, destructible = false, colliding = false;
        public float limitX0 = -3f, limitXF = 3f, limitY0 = -2f, limitYF = 2f;
        public bool stopAll = false;
        Player playerScript;
        Renderer rendererME = null;
        float speedPlayer = 0;
        bool hidden = false;

        private void Start()
        {
            playerScript = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<Player>();
            speedPlayer = playerScript.movespeed;
            rendererME = GetComponent<Renderer>();
            GenerateMinionMap();
        }

        private void Update()
        {
            if (currentMinions < numMinions)
            {
                if (hydraEffect)
                {
                    numMinions++;
                    AddMinion();
                    AddMinion();
                    currentMinions += 2;
                }
                else
                {
                    AddMinion();
                    currentMinions++;
                }
            }

            if (!hidden && !GameManager.instance.invertWorld)
            {
                hidden = true;
                rendererME.enabled = false;
            }
            else if (hidden && GameManager.instance.invertWorld)
            {
                hidden = false;
                rendererME.enabled = true;
            }
        }

        private void GenerateMinionMap()
        {
            for (int i = 0; i < numMinions; i++)
            {
                AddMinion();
            }
            currentMinions = numMinions;
        }

        private void AddMinion()
        {
            if (minionObject != null && !stopAll) {
                GameObject minion = GameManager.instance.AddObjectWithParent(
                    minionObject, "", new Vector3(transform.position.x, transform.position.y, 0), new Vector3(1f, 1f, 1f), transform);
                Minion minionScript = minion.GetComponent<Minion>();
                minionScript.speed = Random.Range(0.15f, 0.3f);
                minionScript.timeMaxPower = Random.Range(2f, 5f);
                minionScript.timeMaxChangeVelocity = Random.Range(4f, 8f);
                minionScript.factorDivideSpeed = Random.Range(1.2f, 1.5f);
                minionScript.timeInvertControls = Random.Range(4f, 8f);

                int numTargets = Random.Range(2, 5);
                Vector3[] targets = new Vector3[numTargets];
                for (int i = 0; i < numTargets - 1; i++)
                {
                    targets[i] = new Vector3(Random.Range(limitX0, limitXF), Random.Range(limitY0, limitYF), 0);
                }
                targets[numTargets - 1] = new Vector3(transform.position.x, transform.position.y, 0);
                minionScript.targets = targets;
                minionList.Add(minion);
            }
        }

        public void StopAllMinions()
        {
            stopAll = true;
            foreach (GameObject minion in minionList)
            {
                if (minion != null)
                {
                    Minion minionScript = minion.GetComponent<Minion>();
                    minionScript.followWhenClose = false;
                    minionScript.followingPlayer = false;
                    minionScript.Stop();
                }
            }
        }

        public void MoveAllMinions(Vector3 target)
        {
            foreach (GameObject minion in minionList)
            {
                if (minion != null)
                {
                    Minion minionScript = minion.GetComponent<Minion>();
                    minionScript.followWhenClose = false;
                    minionScript.followingPlayer = false;
                    minionScript.isPatroller = true;
                    Vector3[] targets = new Vector3[1];
                    targets[0] = target;
                    minionScript.targets = targets;
                }
            }
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                colliding = true;
            }
        }

        protected void OnTriggerStay2D(Collider2D collision)
        {
            if (!hidden && destructible && collision.tag.Equals("Papel") && collision.GetComponent<FarAttackMiniGameObject>().attacking)
            {
                collision.GetComponent<FarAttackMiniGameObject>().hitSuccess = true;
                if (collision.GetComponent<FarAttackMiniGameObject>().achievedGoal)
                {
                    // Colocar animação de destruição e todos os minions morrendo
                    playerScript.movespeed = speedPlayer;
                    playerScript.invertControlsTime = 0;
                    GameManager.instance.Invoke("InvokeMission", 0.2f);
                    Destroy(gameObject);
                }
            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                colliding = false;
            }
        }

    }
}