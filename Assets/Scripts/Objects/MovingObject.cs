using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowPlayer;

namespace CrowShadowObjects
{
    public class MovingObject : MonoBehaviour
    {
        public bool canMoveUp = false;
        public bool colliding = false;
        public string prefName = ""; // Padrão: identificador do objeto (MO) + _ + nome da cena + _ + identificador
        public float scaleMoveUp = 4f;

        GameObject player;
        Player scriptPlayer;
        Renderer playerRenderer;
        SpriteRenderer spriteRenderer;
        Rigidbody2D rb;
        BoxCollider2D colliderMO, colliderPlayer;

        bool moving = false;
        float distanceWantedX = 0.4f;
        float distanceWantedY = 0.45f;
        Player.Directions originalDirection;
        float originalX, originalY;
        float aproxTimerMult = 1f;
        float colliderFactorX, colliderFactorY;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            colliderMO = GetComponent<BoxCollider2D>();
            colliderFactorX = colliderMO.size.x / 2;
            colliderFactorY = colliderMO.size.y / 2;
        }

        private void Start()
        {
            player = GameManager.instance.gameObject;
            scriptPlayer = player.GetComponent<Player>();
            playerRenderer = player.GetComponent<SpriteRenderer>();
            colliderPlayer = player.GetComponent<BoxCollider2D>();
        }

        void Update()
        {
            if (scriptPlayer.playerAction != Player.Actions.ON_OBJECT)
            {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
            }
            else
            {
                spriteRenderer.sortingOrder = playerRenderer.sortingOrder - 1;
            }

            if (colliding && !GameManager.instance.paused && !GameManager.instance.pausedObject && !GameManager.instance.blocked)
            {
                bool facingObject = false;
                var relativePoint = transform.InverseTransformPoint(player.transform.position);

                if (relativePoint.x < 0.0 && scriptPlayer.direction == Player.Directions.EAST)
                {
                    if ((player.transform.position.y > transform.position.y - colliderFactorY) && 
                        (player.transform.position.y < transform.position.y + colliderFactorY))
                    {
                        print("Object is to the left");
                        facingObject = true;
                    }
                }
                else if (relativePoint.x > 0.0 && scriptPlayer.direction == Player.Directions.WEST)
                {
                    if ((player.transform.position.y > transform.position.y - colliderFactorY) &&
                        (player.transform.position.y < transform.position.y + colliderFactorY))
                    {
                        print("Object is to the right");
                        facingObject = true;
                    }
                }
                else if (relativePoint.y < 0.0 && scriptPlayer.direction == Player.Directions.NORTH)
                {
                    if ((player.transform.position.x > transform.position.x - colliderFactorX) &&
                        (player.transform.position.x < transform.position.x + colliderFactorX))
                    {
                        print("Object is down");
                        facingObject = true;
                    }
                }
                else if (relativePoint.y > 0.0 && scriptPlayer.direction == Player.Directions.SOUTH)
                {
                    if ((player.transform.position.x > transform.position.x - colliderFactorX) &&
                        (player.transform.position.x < transform.position.x + colliderFactorX))
                    {
                        print("Object is up");
                        facingObject = true;
                    }
                }

                if (facingObject)
                {
                    if (CrossPlatformInputManager.GetButton("keySpecial") && canMoveUp)
                    {
                        if (CrossPlatformInputManager.GetButtonDown("keyMove"))
                        {
                            MoveUp();
                        }
                    }
                    else if (scriptPlayer.playerAction != Player.Actions.ON_OBJECT)
                    {
                        if (CrossPlatformInputManager.GetButtonDown("keyMove")) //GetKeyDown e GetKeyUp não pode ser usado fora do Update
                        {
                            InitMove();
                        }
                        else if (CrossPlatformInputManager.GetButton("keyMove"))
                        {
                            Move();
                        }
                        else if (CrossPlatformInputManager.GetButtonUp("keyMove"))
                        {
                            EndMove();
                        }
                    }
                    else if (moving)
                    {
                        EndMove();
                    }
                }
            }
            else if (moving)
            {
                EndMove();
            }
        }

        public void InitMove()
        {
            //print("INITMOVE");
            moving = true;
            scriptPlayer.playerAction = Player.Actions.MOVING_OBJECT;
            scriptPlayer.animator.SetTrigger("movingObject");
            aproxTimerMult = 0f;
        }

        public void Move()
        {
            if (scriptPlayer.playerAction != Player.Actions.MOVING_OBJECT)
            {
                InitMove();
            }

            if (!GameManager.instance.scenerySounds2.source.isPlaying)
            {
                GameManager.instance.scenerySounds2.PlaySlide(1);
            }
            //print("MOVE");
            var relativePoint = transform.InverseTransformPoint(player.transform.position);
            //para ver se esta na esquerda ou direta, em cima ou baixo
            //para nao dar problema com a colisão do MovingObject em bordas, faze-las grandes (maiores do que ele)

            if ((scriptPlayer.direction == Player.Directions.EAST && relativePoint.x < 0.0)
                || (scriptPlayer.direction == Player.Directions.WEST && relativePoint.x > 0.0)
                || (scriptPlayer.direction == Player.Directions.EAST && relativePoint.x < 0.0 && scriptPlayer.wantedDirection == Player.Directions.WEST)
                || (scriptPlayer.direction == Player.Directions.WEST && relativePoint.x > 0.0 && scriptPlayer.wantedDirection == Player.Directions.EAST)
                )
            {
                Vector3 diff = new Vector3(rb.position.x, rb.position.y) - player.transform.position;
                diff.y = 0; // ignore Y
                diff.z = 0;
                rb.position = Vector2.Lerp(rb.position, player.transform.position + diff.normalized * distanceWantedX, aproxTimerMult * Time.deltaTime);
            }
            else if ((scriptPlayer.direction == Player.Directions.NORTH && relativePoint.y < 0.0)
                || (scriptPlayer.direction == Player.Directions.SOUTH && relativePoint.y > 0.0)
                || (scriptPlayer.direction == Player.Directions.NORTH && relativePoint.y < 0.0 && scriptPlayer.wantedDirection == Player.Directions.SOUTH)
                || (scriptPlayer.direction == Player.Directions.SOUTH && relativePoint.y > 0.0 && scriptPlayer.wantedDirection == Player.Directions.NORTH))
            {
                Vector3 diff = new Vector3(rb.position.x, rb.position.y) - player.transform.position;
                diff.x = 0; // ignore X
                diff.z = 0;
                rb.position = Vector2.Lerp(rb.position, player.transform.position + diff.normalized * distanceWantedY, aproxTimerMult * Time.deltaTime);
            }

            if (aproxTimerMult < 100f)
            {
                aproxTimerMult += (10 * Time.deltaTime);
            }
        }

        public void EndMove()
        {
            //print("ENDMOVE");
            moving = false;
            GameManager.instance.scenerySounds2.StopSound();
            scriptPlayer.playerAction = Player.Actions.DEFAULT;
            scriptPlayer.animator.SetTrigger("changeDirection");
        }

        public void MoveUp()
        {
            //print("MOVEUP");
            scriptPlayer.StopMovement();
            if (scriptPlayer.playerAction != Player.Actions.ON_OBJECT)
            {
                originalDirection = scriptPlayer.direction;
                if (originalDirection != Player.Directions.SOUTH)
                {
                    scriptPlayer.playerAction = Player.Actions.ANIMATION;
                    originalX = player.transform.position.x;
                    originalY = player.transform.position.y;
                    colliderMO.enabled = false;
                    colliderPlayer.enabled = false;
                    if (originalDirection == Player.Directions.EAST)
                    {
                        scriptPlayer.ChangePositionDefault(rb.position.x - (spriteRenderer.bounds.size.x / (float)1.5), rb.position.y, -1);
                        scriptPlayer.MoveUpAnimation(this, "playerClimbEast", rb.position.x, rb.position.y + (spriteRenderer.bounds.size.y / scaleMoveUp), (int)originalDirection);
                    }
                    else if (originalDirection == Player.Directions.WEST)
                    {
                        scriptPlayer.ChangePositionDefault(rb.position.x + (spriteRenderer.bounds.size.x / (float)1.5), rb.position.y, -1);
                        scriptPlayer.MoveUpAnimation(this, "playerClimbWest", rb.position.x, rb.position.y + (spriteRenderer.bounds.size.y / scaleMoveUp), (int)originalDirection);
                    }
                    else if (originalDirection == Player.Directions.NORTH)
                    {
                        scriptPlayer.ChangePositionDefault(rb.position.x, rb.position.y, -1);
                        scriptPlayer.MoveUpAnimation(this, "playerClimbNorth", rb.position.x, rb.position.y + (spriteRenderer.bounds.size.y / scaleMoveUp), (int)originalDirection);
                    }
                    /*else if (originalDirection == Player.Directions.SOUTH)
                    {
                        scriptPlayer.ChangePositionDefault(this, rb.position.x, rb.position.y, -1);
                        scriptPlayer.MoveUpAnimation("playerClimbSouth", rb.position.x, rb.position.y + (spriteRenderer.bounds.size.y / scaleMoveUp), (int)originalDirection);
                    }*/
                }
            }
            else
            {
                if (originalDirection == Player.Directions.EAST)
                {
                    scriptPlayer.ChangePositionDefault(rb.position.x - (spriteRenderer.bounds.size.x / (float)1.5), rb.position.y + (spriteRenderer.bounds.size.y / scaleMoveUp), 1);
                    scriptPlayer.MoveDownAnimation("playerDownWest", originalX, originalY, -1);
                }
                else if (originalDirection == Player.Directions.WEST)
                {
                    scriptPlayer.ChangePositionDefault(rb.position.x + (spriteRenderer.bounds.size.x / (float)1.5), rb.position.y + (spriteRenderer.bounds.size.y / scaleMoveUp), 0);
                    scriptPlayer.MoveDownAnimation("playerDownEast", originalX, originalY, -1);
                }
                else if (originalDirection == Player.Directions.NORTH)
                {
                    scriptPlayer.ChangePositionDefault(rb.position.x, rb.position.y, 3);
                    scriptPlayer.MoveDownAnimation("playerDownSouth", originalX, originalY, -1);
                }
                /*else if (originalDirection == Player.Directions.SOUTH)
                {
                scriptPlayer.ChangePositionDefault(this, rb.position.x, rb.position.y + (spriteRenderer.bounds.size.y / 4), 2);
                    scriptPlayer.MoveDownAnimation("playerDownNorth", originalX, originalY, -1);
                }*/
            }
        }

        public void ChangePosition(float x, float y)
        {
            rb.position = new Vector2(x, y);
        }

    }
}