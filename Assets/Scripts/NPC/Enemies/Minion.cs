﻿using UnityEngine;
using CrowShadowManager;

namespace CrowShadowNPCs
{
    public class Minion : Follower
    {

        public int healthLight = 200; //decrementa 1 por colisão
        public int healthMelee = 300;
        public int decrementFaca = 30, decrementBastao = 25, decrementPedra = 20;
        public float addPath = 0.5f; // quanto vai ser adicionado ao somatório das escolhas
        public float timeMaxPower = 3f; // tempo máximo que pode ficar colidindo com o minion para não ativar próximo poder
        public float timeMaxChangeVelocity = 6f, factorDivideSpeed = 1.8f; // tempo máximo com velocidade menor e fator para dividi-la
        public float timeInvertControls = 6f; // tempo adicional para ficar com o controle invertido

        float timeLeftAttack = 0, timePower = 0, timeChangeVelocity = 0;
        int power = 0; // 1 - diminui velocidade, 2 - inverte controles, 3 - morre
        bool onCollision = false, changeVelocity = false;
        MinionEmmitter emmitter;

        protected new void Start()
        {
            base.Start();
            distFollow = 0.1f;
            moveTowards = true;
            emmitter = GetComponentInParent<MinionEmmitter>();
        }

        protected new void Update()
        {
            // Ao patrulhar
            if (!followingPlayer)
            {
                // Se estiver correndo, aumenta a ára de busca
                if (player.GetComponent<Player>().isRunning)
                {
                    GetComponent<CircleCollider2D>().radius = 0.8f;
                }
                else
                {
                    GetComponent<CircleCollider2D>().radius = 0.6f;
                }
            }
            else
            {
                // Condição quando está escondido
                if (!player.GetComponent<Renderer>().enabled &&
                    player.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Kinematic)
                {
                    FollowPlayer(false);
                }
            }

            // Ao colidir
            if (onCollision)
            {
                if (timePower > 0)
                {
                    timePower -= Time.deltaTime;
                }
                else
                {
                    // animaçãozinha de poderzinho atacando (pode ser uma luz)
                    if (GameManager.instance.playerProtected)
                    {
                        print("PROT");
                        if (Inventory.GetCurrentItemType() == Inventory.InventoryItems.TAMPA)
                        {
                            GameObject.Find("Tampa").gameObject.GetComponent<ProtectionObject>().DecreaseLife();
                        }
                        else if (Inventory.GetCurrentItemType() == Inventory.InventoryItems.ESCUDO)
                        {
                            GameObject.Find("Escudo").gameObject.GetComponent<ProtectionObject>().DecreaseLife();
                        }
                    }
                    else
                    {
                        timePower = timeMaxPower;
                        ActivatePower();
                    }
                }
            }

            // Mudança de velocidade do player
            if (changeVelocity)
            {
                if (timeChangeVelocity > 0)
                {
                    timeChangeVelocity -= Time.deltaTime;
                }
                else
                {
                    player.GetComponent<Player>().movespeed = player.GetComponent<Player>().movespeed * factorDivideSpeed;
                    changeVelocity = false;
                }
            }

            if (timeLeftAttack > 0)
            {
                timeLeftAttack -= Time.deltaTime;
            }

            if (healthLight <= 0)
            {
                GameManager.instance.pathCat += addPath;
                if (emmitter) emmitter.currentMinions--;
                Destroy(gameObject);
                // animação + som
            }
            else if (healthMelee <= 0)
            {
                GameManager.instance.pathBird += addPath;
                if (emmitter) emmitter.currentMinions--;
                Destroy(gameObject);
                // animação + som
            }

            base.Update();
        }

        private void ActivatePower()
        {
            print("ACTIVATE " + power);
            switch (power)
            {
                case 0:
                    timeChangeVelocity = timeMaxChangeVelocity;
                    changeVelocity = true;
                    player.GetComponent<Player>().movespeed = player.GetComponent<Player>().movespeed / factorDivideSpeed;
                    power++;
                    break;
                case 1:
                    player.GetComponent<Player>().invertControlsTime += timeInvertControls;
                    power++;
                    break;
                case 2:
                    power = 0;
                    GameManager.instance.GameOver();
                    break;
                default:
                    break;
            }
        }

        protected new void OnTriggerEnter2D(Collider2D collision)
        {
            OnTriggerCalled(collision);
            if (collision.gameObject.tag.Equals("Player") && followingPlayer)
            {
                onCollision = true;
            }
        }

        protected new void OnTriggerStay2D(Collider2D collision)
        {
            OnTriggerCalled(collision);
            //print("Minion: " + collision.tag);
            if ((collision.tag.Equals("Flashlight") && Flashlight.GetState()) || collision.tag.Equals("Lamp"))
            {
                healthLight--;
            }
            else if (collision.tag.Equals("Faca") && collision.GetComponent<AttackObject>().attacking && timeLeftAttack <= 0)
            {
                timeLeftAttack = AttackObject.timeAttack;
                healthMelee -= decrementFaca;
            }
            else if (collision.tag.Equals("Bastao") && collision.GetComponent<AttackObject>().attacking && timeLeftAttack <= 0)
            {
                timeLeftAttack = AttackObject.timeAttack;
                healthMelee -= decrementBastao;
            }
            else if (collision.tag.Equals("Pedra") && collision.GetComponent<FarAttackObject>().attacking)
            {
                collision.GetComponent<FarAttackObject>().hitSuccess = true;
                healthMelee -= decrementPedra;
            }
            else if (collision.tag.Equals("Papel") && collision.GetComponent<FarAttackMiniGameObject>().attacking)
            {
                collision.GetComponent<FarAttackMiniGameObject>().hitSuccess = true;
                if (collision.GetComponent<FarAttackMiniGameObject>().achievedGoal)
                {
                    if (emmitter) emmitter.currentMinions--;
                    Destroy(gameObject);
                }
            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                onCollision = false;
            }
        }

        protected new void OnTriggerCalled(Collider2D collision)
        {
            if (hasActionPatroller)
            {
                print("ActionFollower: " + collision.tag);
                if (collision.gameObject.tag.Equals("Player"))
                {
                    if (followWhenClose && !followingPlayer)
                    {
                        FollowPlayer();
                        GetComponent<CircleCollider2D>().radius = 0.3f;
                    }
                }
            }
        }
    }
}