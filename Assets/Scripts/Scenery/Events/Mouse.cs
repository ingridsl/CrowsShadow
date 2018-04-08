﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public AudioClip squeak;
    private AudioSource source { get { return GetComponent<AudioSource>(); } }
    
    public string animationName = "Mouse";
    public bool always = true; //after all this time? always
    bool playedAlready = false;
    public Animation animationMouse { get { return GetComponent<Animation>(); } }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player") && !playedAlready)
        {
            if (!always) {
                if (MissionManager.instance.currentMission % 2 == 0)
                {
                    source.clip = squeak;
                    source.PlayDelayed(0.5f);
                    animationMouse.Play(animationName);
                    playedAlready = true;
                }

            }

            else{
                source.clip = squeak;
                source.PlayDelayed(0.5f);
                animationMouse.Play(animationName);
                playedAlready = true;

            }
        }
    }
}