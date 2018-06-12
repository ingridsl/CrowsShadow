﻿using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;
using CrowShadowPlayer;

namespace CrowShadowScenery
{
    public class Tutorial : MonoBehaviour
    {
        public string keyName1, keyName2;
        public int mission = 1;
        public Player.Actions playerAction = Player.Actions.DEFAULT;
        public AudioClip click;
        public AudioSource source { get { return GetComponent<AudioSource>(); } }

        private Player player;
        private int repeatTime = 2;
        private bool exit = false, end = false, invoked = false;

        public bool inventoryObject = false;

        void Awake()
        {
            source.playOnAwake = false;
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        void Update()
        {
            if (!end && !exit)
            {
                KeyPressed();
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Player") && GameManager.instance.currentMission == mission && player.playerAction == playerAction && !end)
            {
                exit = false;
                if (!inventoryObject)
                {
                    InvokeShow();
                }
                else
                {
                    if (Inventory.HasItemType(Inventory.InventoryItems.FLASHLIGHT))
                    {
                        InvokeShow();
                    }
                }
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Player") && GameManager.instance.currentMission == mission && !end)
            {
                if (!inventoryObject)
                {
                    InvokeShow();
                }   
                else
                {
                    if (Inventory.HasItemType(Inventory.InventoryItems.FLASHLIGHT))
                    {
                        InvokeShow();
                    }
                }
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Player") && GameManager.instance.currentMission == mission)
            {
                exit = true;
                invoked = false;
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        void KeyPressed()
        {
            if (CrossPlatformInputManager.GetButtonDown(keyName1) && CrossPlatformInputManager.GetButtonDown(keyName2))
            {
                end = true;
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        void InvokeShow()
        {
            if (!source.isPlaying && !invoked)
            {
                Invoke("Show", repeatTime);
                invoked = true;
            }
        }

        void Show()
        {
            if (!end && !exit)
            {
                if (gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    gameObject.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
                source.PlayOneShot(click);
                invoked = false;
            }
        }

    }
}