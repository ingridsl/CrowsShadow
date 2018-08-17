using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowNPCs;
using CrowShadowObjects;

namespace CrowShadowPlayer
{
    public class Player : MonoBehaviour
    {
        public enum Actions { DEFAULT, MOVING_OBJECT, ON_OBJECT, ANIMATION };
        [Header("Conditions")]
        public Actions playerAction;
        public enum States { DEFAULT, FLASHLIGHT, PROTECTED_TAMPA, PROTECTED_ESCUDO };
        public States playerState;

        [Header("Variables")]
        public float movespeed = 0.01f;
        public float runningFactor = 3f;
        public float invertControlsTime = 0;

        [Header("Data")]
        public bool hidden = false;
        public bool isRunning = false;
        public enum Directions { EAST, WEST, NORTH, SOUTH, NONE }; //0 = east, 1 = west, 2 = north, 3 = south
        public Directions direction = Directions.EAST, wantedDirection = Directions.EAST;

        [Header("External Sources")]
        public Animator animator;
        public AudioClip steps, stepsGrass;
        public AudioSource source { get { return GetComponent<AudioSource>(); } }

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D colliderPlayer;
        private Rigidbody2D rb;
        private MovingObject auxOnObject;

        private float stepsControl = 0.5f;
        private string lastSceneGameOver = "", corvoScene = "";
        private float corvoPositionX, corvoPositionY;
        private Directions oldDirection;

        void Start()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            colliderPlayer = GetComponent<BoxCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            source.playOnAwake = false;
        }

        void Update()
        {
            if (invertControlsTime > 0)
            {
                invertControlsTime -= Time.deltaTime;
            }

            if (!GameManager.instance.paused && !GameManager.instance.blocked && !GameManager.instance.pausedObject)
            {
                bool isWalking = false;
                float move = movespeed;

                //Ordem do layer determinada pelo eixo y
                spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

                if (playerAction == Actions.DEFAULT || playerAction == Actions.MOVING_OBJECT)
                {
                    if (CrossPlatformInputManager.GetButton("keyRun"))
                    {
                        move = runningFactor * movespeed;
                        isRunning = true;
                    }
                    else
                    {
                        isRunning = false;
                    }

                    if (CrossPlatformInputManager.GetAxis("Horizontal") > 0)
                    {
                        if (invertControlsTime > 0)
                        {
                            rb.position = new Vector2(rb.position.x - move, rb.position.y);
                        }
                        else
                        {
                            rb.position = new Vector2(rb.position.x + move, rb.position.y);
                        }
                        isWalking = true;
                        direction = Directions.EAST;
                    }
                    else if (CrossPlatformInputManager.GetAxis("Horizontal") < 0)
                    {
                        if (invertControlsTime > 0)
                        {
                            rb.position = new Vector2(rb.position.x + move, rb.position.y);
                        }
                        else
                        {
                            rb.position = new Vector2(rb.position.x - move, rb.position.y);
                        }
                        isWalking = true;
                        direction = Directions.WEST;
                    }
                    else if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
                    {
                        if (invertControlsTime > 0)
                        {
                            rb.position = new Vector2(rb.position.x, rb.position.y - move);
                        }
                        else
                        {
                            rb.position = new Vector2(rb.position.x, rb.position.y + move);
                        }
                        isWalking = true;
                        direction = Directions.NORTH;
                    }
                    else if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
                    {
                        if (invertControlsTime > 0)
                        {
                            rb.position = new Vector2(rb.position.x, rb.position.y + move);
                        }
                        else
                        {
                            rb.position = new Vector2(rb.position.x, rb.position.y - move);
                        }
                        isWalking = true;
                        direction = Directions.SOUTH;
                    }

                    if (isRunning)
                    {
                        if (!isWalking) isRunning = false;
                    }
                    animator.SetBool("isWalking", isWalking);
                    animator.SetBool("isRunning", isRunning);

                    if (playerAction == Actions.MOVING_OBJECT)
                    {
                        wantedDirection = direction;
                        direction = oldDirection;
                    }

                    //barulho dos passos
                    if (GameManager.currentSceneName.Equals("Jardim"))
                    {
                        source.clip = stepsGrass;
                        stepsControl = 0.1f;
                    }
                    else
                    {
                        source.clip = steps;
                        stepsControl = 0.2f;
                    }
                    if (isWalking && !source.isPlaying)
                    {
                        source.PlayDelayed(stepsControl * 2);
                    }
                    else if (isRunning && !source.isPlaying)
                    {
                        source.PlayDelayed(stepsControl);
                    }
                    else if (!(isWalking || isRunning))
                    {
                        source.Stop();
                    }
                }
                else if (playerAction == Actions.ON_OBJECT)
                {
                    if (CrossPlatformInputManager.GetButton("keySpecial"))
                    {
                        if (CrossPlatformInputManager.GetButtonDown("keyMove"))
                        {
                            auxOnObject.MoveUp();
                        }
                    }
                    if (CrossPlatformInputManager.GetAxis("Horizontal") > 0)
                    {
                        direction = Directions.EAST;
                    }
                    else if (CrossPlatformInputManager.GetAxis("Horizontal") < 0)
                    {
                        direction = Directions.WEST;
                    }
                    else if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
                    {
                        direction = Directions.NORTH;
                    }
                    else if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
                    {
                        direction = Directions.SOUTH;
                    }
                }

                animator.SetInteger("direction", (int)direction);
                if (oldDirection != direction)
                {
                    animator.SetTrigger("changeDirection");
                    oldDirection = direction;
                }

            }
            else if (oldDirection != Directions.NONE)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                animator.SetTrigger("changeDirection");
                oldDirection = Directions.NONE;
            }
        }

        public void StopMovement()
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }

        public void ChangeState(int newState)
        {
            playerState = (States)newState;
            animator.SetInteger("playerState", newState);
            animator.SetTrigger("changeDirection");
        }

        public void ChangeDirection(int newDirection)
        {
            direction = (Directions)newDirection;
            animator.SetInteger("direction", (int)direction);
            animator.SetTrigger("changeDirection");
            oldDirection = direction;
        }

        public void ChangePositionDefault(float x, float y, int dir)
        {
            transform.position = new Vector3(x, y, -1f);
            if (dir != -1) ChangeDirection(dir);
        }

        public void ChangePosition()
        {
            playerAction = Actions.DEFAULT;

            string previousSceneName = "";
            if (GameManager.previousSceneName.Equals("GameOver"))
            {
                previousSceneName = lastSceneGameOver;
                invertControlsTime = 0;
                movespeed = 0.01f;
            }
            else if (!GameManager.currentSceneName.Equals("GameOver"))
            {
                lastSceneGameOver = GameManager.previousSceneName;
                previousSceneName = GameManager.previousSceneName;
            }

            if (!GameManager.currentSceneName.Equals(GameManager.previousSceneName))
            {
                if (GameManager.currentSceneName.Equals("Corredor"))
                {
                    if (previousSceneName.Equals("Sala"))
                    {
                        transform.position = new Vector3(-9.8f, -0.6f, -1f);
                    }
                    else if (previousSceneName.Equals("QuartoMae"))
                    {
                        transform.position = new Vector3(-1.6f, -0.3f, -1f);
                        ChangeDirection((int)Directions.SOUTH);
                    }
                    else if (previousSceneName.Equals("Cozinha"))
                    {
                        transform.position = new Vector3(2.95f, -0.6f, -1f);
                    }
                    else if (previousSceneName.Equals("Banheiro"))
                    {
                        transform.position = new Vector3(-11.3f, -0.3f, -1f);
                        ChangeDirection((int)Directions.SOUTH);
                    }
                    //else if (previousSceneName.Equals("QuartoKid"))
                    else
                    {
                        transform.position = new Vector3(11.9f, -0.3f, -1f);
                        ChangeDirection((int)Directions.SOUTH);
                    }
                }
                else if (GameManager.currentSceneName.Equals("Cozinha"))
                {
                    transform.position = new Vector3(1.5f, 0.7f, -1f);
                }
                else if (GameManager.currentSceneName.Equals("Jardim"))
                {
                    if (previousSceneName.Equals("Porao"))
                    {
                        transform.position = new Vector3(6.0f, 2.5f, -1f);
                        ChangeDirection((int)Directions.SOUTH);
                    }
                    else
                    {
                        transform.position = new Vector3(2.9f, 2.3f, -1f);
                    }
                }
                else if (GameManager.currentSceneName.Equals("Porao"))
                {
                    transform.position = new Vector3(3.2f, 0.5f, -1f);
                    ChangeDirection((int)Directions.WEST);
                }
                else if (GameManager.currentSceneName.Equals("QuartoKid"))
                {
                    transform.position = new Vector3(1.75f, 0.65f, -1f);
                    ChangeDirection((int)Directions.SOUTH);
                }
                else if (GameManager.currentSceneName.Equals("QuartoMae"))
                {
                    transform.position = new Vector3(-3.8f, -0.45f, -1f);
                    ChangeDirection((int)Directions.SOUTH);
                }
                else if (GameManager.currentSceneName.Equals("Banheiro"))
                {
                    transform.position = new Vector3(2.171f, 0.284f, -1f);
                    ChangeDirection((int)Directions.SOUTH);
                }
                else if (GameManager.currentSceneName.Equals("Sala"))
                {
                    if (previousSceneName.Equals("Jardim"))
                    {
                        transform.position = new Vector3(2.35f, -2.0f, -1f);
                    }
                    //else if (previousSceneName.Equals("Corredor"))
                    else
                    {
                        transform.position = new Vector3(-3.15f, 0.85f, -1f);
                        ChangeDirection((int)Directions.SOUTH);
                    }
                }
                else if (GameManager.currentSceneName.Equals("SideQuest"))
                {
                    transform.position = new Vector3(GameManager.instance.sideQuest.sideX, GameManager.instance.sideQuest.sideY);
                    ChangeDirection(GameManager.instance.sideQuest.sideDir);
                }
                GameManager.instance.paused = false;
            }

            if (Cat.instance != null)
            {
                Cat.instance.GetComponent<Cat>().ChangePosition(transform.position.x - 0.6f, transform.position.y - 0.3f);
            }

            if ((GameManager.instance.mission is Mission8) &&
                !GameManager.previousSceneName.Equals("GameOver") &&
                !GameManager.currentSceneName.Equals("GameOver"))
            {
                corvoPositionX = transform.position.x;
                corvoPositionY = transform.position.y;
                corvoScene = GameManager.currentSceneName;
                if (Crow.instance != null)
                {
                    Crow.instance.gameObject.SetActive(false);
                    Invoke("ChangeCorvoPosition", 2f);
                }
            }

        }

        public void ChangeCorvoPosition()
        {
            if (GameManager.currentSceneName.Equals(corvoScene) && Crow.instance != null)
            {
                Crow.instance.ChangePosition(corvoPositionX, corvoPositionY);
                Crow.instance.gameObject.SetActive(true);
                Crow.instance.LookAtPlayer();
            }
        }

        public float GetCorvoPositionX()
        {
            return corvoPositionX;
        }

        public float GetCorvoPositionY()
        {
            return corvoPositionY;
        }

        public void MoveUpAnimation(MovingObject aux, string anim, float x, float y, int dir)
        {
            auxOnObject = aux;
            animator.Play(anim);
            StartCoroutine(WaitCoroutineUpDown(2, x, y, dir, false));
        }

        public void MoveDownAnimation(string anim, float x, float y, int dir)
        {
            animator.Play(anim);
            StartCoroutine(WaitCoroutineUpDown(1, x, y, dir, true));
        }

        public void PlayAnimation(string anim, float time = 1f)
        {
            //print("ANIM:" + anim + time);
            animator.Play(anim);
            StartCoroutine(WaitCoroutineAnim(time));
        }

        IEnumerator WaitCoroutineUpDown(float time, float x, float y, int dir, bool down)
        {
            Debug.Log("about to yield return WaitForSeconds(" + time + ") " + down);
            yield return new WaitForSeconds(time);
            Debug.Log("Animation ended " + down);
            ChangePositionDefault(x, y, dir);
            if (down)
            {
                playerAction = Actions.DEFAULT;
                auxOnObject.GetComponent<Collider2D>().enabled = true;
                colliderPlayer.enabled = true;
            } else
            {
                playerAction = Actions.ON_OBJECT;
            }
            yield break;
            //Debug.Log("You'll never see this"); // produces a dead code warning
        }

        IEnumerator WaitCoroutineAnim(float time)
        {
            Debug.Log("about to yield return WaitForSeconds(" + time + ")");
            yield return new WaitForSeconds(time);
            Debug.Log("Animation ended");
            animator.SetTrigger("changeDirection");
            yield break;
            //Debug.Log("You'll never see this"); // produces a dead code warning
        }

    }
}