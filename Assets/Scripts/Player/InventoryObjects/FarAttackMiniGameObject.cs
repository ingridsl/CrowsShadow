using UnityEngine;

namespace CrowShadowPlayer
{
    public class FarAttackMiniGameObject : FarAttackObject
    {
        public bool miniGameBlocked = false;
        public bool miniGameUnlocked = false;
        public bool achievedGoal = false;

        protected GameObject fireHolder, fire;
        protected MiniGameObject miniGameObject;

        protected bool init = false;

        protected new void Start()
        {
            base.Start();
            miniGameObject = GetComponent<MiniGameObject>();
            fireHolder = transform.Find("FirePaperHolder").gameObject;
            fire = fireHolder.transform.Find("Fire").gameObject;
            if (fire != null)
            {
                addObject = false;
                SetMiniGame();
           }
        }

        protected new void Update()
        {
            if (!miniGameUnlocked)
            {
                base.Update();
                if (initPositioning)
                {
                    fire.SetActive(true);
                    fire.transform.localPosition = new Vector3(0f, 0f, 0f);
                }
                else if (attacking && !init)
                {
                    init = true;
                }
                else if (init && !attacking)
                {
                   // EndFire();
                }
                else if (achievedGoal && !initAttack && !attacking)
                {
                    if (Inventory.GetCurrentItemType() != item)
                    {
                        EndThrow();
                        EndFire();
                    }
                    switch (player.direction)
                    {
                        case Player.Directions.EAST:
                            fire.SetActive(true);
                            fire.transform.localPosition = new Vector3(-0.3f, -0.6f, 0f);
                            break;
                        case Player.Directions.WEST:
                            fire.SetActive(false);
                            break;
                        case Player.Directions.NORTH:
                            fire.SetActive(true);
                            fire.transform.localPosition = new Vector3(0.3f, -0.6f, 0f);
                            break;
                        case Player.Directions.SOUTH:
                            fire.SetActive(true);
                            fire.transform.localPosition = new Vector3(-0.3f, -0.6f, 0f);
                            break;
                        default:
                            break;
                    }

                }
            }
            else
            {
                miniGameObject.posFlareX = transform.position.x;
                miniGameObject.posFlareY = transform.position.y;
                achievedGoal = miniGameObject.achievedGoal;
                if (achievedGoal)
                {
                    SetMiniGame(false);
                    ShowFire();
                }
            }
        }

        public void UnlockMiniGame()
        {
            miniGameBlocked = false;
            addObject = false;
            SetMiniGame();
        }

        protected void SetMiniGame(bool e = true)
        {
            miniGameUnlocked = e;
            miniGameObject.activated = e;
            miniGameObject.paper = e;
            miniGameObject.enabled = e;
        }

        protected void ShowFire(bool e = true)
        {
            fireHolder.SetActive(e);
        }

        protected void EndFire()
        {
            if (achievedGoal)
            {
                SetMiniGame();
            }
            achievedGoal = false;
            miniGameUnlocked = false;
            ShowFire(false);
        }
    }
}