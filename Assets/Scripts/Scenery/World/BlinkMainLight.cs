using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace CrowShadowScenery
{
    public class BlinkMainLight : MonoBehaviour
    {
        public bool destroyChangeScene = true;
        public float minWaitTime = 0.2f;
        public float maxWaitTime = 0.3f;
        public int nTimes = -1;

        GameObject lightMain;

        int cont;

        void Start()
        {
            cont = 0;
            if (nTimes != -1 && (nTimes % 2) != 0)
            {
                nTimes++;
            }
            print("OP" + nTimes);
            lightMain = GameObject.Find("MainLight").gameObject;
            StartCoroutine(Flashing());
        }

        IEnumerator Flashing()
        {
            while (nTimes == -1 || nTimes > cont)
            {
                yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
                lightMain.SetActive(!lightMain.activeSelf);
                cont++;
                //print("C" + cont);
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
}