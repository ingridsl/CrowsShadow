﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BlinkMainLight : MonoBehaviour {


	GameObject light;
    public bool destroyChangeScene = true;
	public float minWaitTime = 0.2f;
	public float maxWaitTime = 0.3f;
    public int nTimes = -1;
    int cont;

	void Start () {
        cont = 0;
        if(nTimes != -1 && (nTimes % 2) != 0)
        {
            nTimes++;
        }
        print("OP" + nTimes);
        light = GameObject.Find("MainLight").gameObject;
		StartCoroutine(Flashing());
	}

	IEnumerator Flashing ()
	{
		while (nTimes == -1 || nTimes > cont)
		{
			yield return new WaitForSeconds(Random.Range(minWaitTime,maxWaitTime));
			light.SetActive(!light.activeSelf);
            cont++;
            print("C" + cont);
        }
        Destroy(gameObject);
	}
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (destroyChangeScene)
        {
            Destroy(gameObject);
        }
    }

 }