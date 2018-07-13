using UnityEngine;
using CrowShadowManager;
using CrowShadowPlayer;

namespace CrowShadowNPCs
{
    public class Patroller : MonoBehaviour
    {
        public Animator animator;

        public float speed;
        public bool isPatroller = false;
        public bool destroyEndPath = false;
        public bool stopEndPath = false;
        public Vector3[] targets;
        public bool hasActionPatroller = false;
        public float offsetActionPatroller = 0f;

        protected SpriteRenderer spriteRenderer;
        protected CircleCollider2D circleCollider;

        protected int direction = 4, oldDirection = 4;
        protected int destPoint = 0;
        
        //bool quadr1 = false, quadr2 = false, quadr3 = false, quadr4 = false;

        protected void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            if (hasActionPatroller)
            {
                CircleCollider2D[] col = GetComponents<CircleCollider2D>();
                foreach (CircleCollider2D i in col)
                {
                    i.enabled = true;
                }
            }
            circleCollider = GetComponent<CircleCollider2D>();
        }

        protected void Update()
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

            if (isPatroller)
            {
                GotoNextPoint();
            }

            /*if (quadr1 || quadr2 || quadr3 || quadr4)
            {
                float thisObjX = gameObject.transform.position.x;
                float thisObjY = gameObject.transform.position.y;
                float thisObjZ = gameObject.transform.position.z;

                if (quadr1)
                {
                    gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(thisObjX - 1, thisObjY + 1, thisObjZ), speed * Time.deltaTime);
                    quadr1 = false;
                }
                if (quadr2)
                {
                    gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(thisObjX - 1, thisObjY - 1, thisObjZ), speed * Time.deltaTime);
                    quadr2 = false;
                }
                if (quadr3)
                {
                    gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(thisObjX + 1, thisObjY + 1, thisObjZ), speed * Time.deltaTime);
                    quadr3 = false;
                }
                if (quadr4)
                {
                    gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(thisObjX + 1, thisObjY - 1, thisObjZ), speed * Time.deltaTime);
                    quadr4 = false;
                }
            }*/

            SetActionPatrollerDirection();
        }

        protected void GotoNextPoint()
        {
            if (targets.Length == 0 || transform == null) return;
            if (destPoint >= targets.Length) destPoint = 0;

            float step = speed * Time.deltaTime;

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
                    ChangeDirection();
                }
            }
            else
            {
                ChangeDirection();
            }

        }

        protected void ChangeDirection()
        {
            if (Mathf.Abs(targets[destPoint].y - transform.position.y) <
                Mathf.Abs(targets[destPoint].x - transform.position.x))
            {
                if (targets[destPoint].x > transform.position.x)
                {
                    direction = 0;
                }
                else
                {
                    direction = 1;
                }
            }
            else
            {
                if (targets[destPoint].y > transform.position.y)
                {
                    direction = 2;
                }
                else
                {
                    direction = 3;
                }
            }
            ChangeDirectionAnimation();
        }

        protected void SetActionPatrollerDirection()
        {
            if (hasActionPatroller && offsetActionPatroller != 0)
            {
                switch (direction)
                {
                    case 0:
                        circleCollider.offset = new Vector2(offsetActionPatroller, 0f);
                        break;
                    case 1:
                        circleCollider.offset = new Vector2(-offsetActionPatroller, 0f);
                        break;
                    case 2:
                        circleCollider.offset = new Vector2(0f, offsetActionPatroller);
                        break;
                    case 3:
                        circleCollider.offset = new Vector2(0f, -offsetActionPatroller);
                        break;
                    default:
                        circleCollider.offset = new Vector2(0f, 0f);
                        break;
                }
            }
        }

        public int GetDirection()
        {
            return direction;
        }

        public void Stop()
        {
            isPatroller = false;
            direction = 4;
            ChangeDirectionAnimation();
        }

        protected void ChangeDirectionAnimation()
        {
            if (oldDirection != direction && animator != null)
            {
                if (animator == null) animator = GetComponent<Animator>();
                animator.SetInteger("direction", direction);
                animator.SetTrigger("changeDirection");
                oldDirection = direction;
            }
        }

        public void ChangeDirectionAnimation(int d)
        {
            direction = d;
            ChangeDirectionAnimation();
        }

        //Interacoes estao por trigger em vista de nao serem possiveis de identificacao em objeto kinematic
        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (hasActionPatroller)
            {
                print("ActionPatroller: " + collision.tag);
                if (collision.gameObject.tag.Equals("Player") && !collision.gameObject.GetComponent<Player>().hidden)
                {
                    GameManager.instance.GameOver();
                }
            }
        }

        /*void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.gameObject.tag.Equals("FixedObject") || coll.gameObject.tag.Equals("SceneObject")
                || coll.gameObject.tag.Equals("MovingObject") || coll.gameObject.tag.Equals("Player"))
            {
                var thisX = coll.collider.bounds.center.x;
                var thisY = coll.collider.bounds.center.y;
                var otherX = coll.otherCollider.bounds.center.x;
                var otherY = coll.otherCollider.bounds.center.y;

                if (thisX <= otherX && thisY >= otherY)
                {
                    // subir um pouco
                    // um pouco pra esquerda
                    quadr1 = true;
                }
                else if (thisX <= otherX && thisY <= otherY)
                {
                    // descer um pouco
                    // um pouco pra esquerda
                    quadr2 = true;
                }
                else if (thisX >= otherX && thisY >= otherY)
                {
                    // subir um pouco
                    // um pouco pra direita
                    quadr3 = true;
                }
                else if (thisX >= otherX && thisY <= otherY)
                {
                    // descer um pouco
                    // um pouco pra esquerda
                    quadr4 = true;
                }
            }
        }*/

    }
}