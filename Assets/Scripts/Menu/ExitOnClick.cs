﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class ExitOnClick : MonoBehaviour {
    public Image black;
    public Animator anim;

    public void Quit()
	{
        StartCoroutine(FadingExit());

	}

    IEnumerator FadingExit()
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        UnityEditor.EditorApplication.isPlaying = false;
        #if UNITY_EDITOR
        #else
		Application.Quit ();
        #endif

    }

}