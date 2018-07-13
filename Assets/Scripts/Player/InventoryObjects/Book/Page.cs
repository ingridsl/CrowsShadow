using UnityEngine;
using CrowShadowManager;

namespace CrowShadowPlayer
{
    public class Page : MonoBehaviour
    {

        Renderer rendererPage = null;
        bool hidden = false;

        private void Start()
        {
            rendererPage = GetComponent<Renderer>();
        }

        private void Update()
        {

            if (!hidden && !GameManager.instance.invertWorld)
            {
                hidden = true;
                rendererPage.enabled = false;
            }
            else if (hidden && GameManager.instance.invertWorld)
            {
                hidden = false;
                rendererPage.enabled = true;
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (!hidden && other.gameObject.tag.Equals("Player"))
            {
                // Som de pegar página
                Book.AddPage();
                GameManager.instance.UpdateSave();
                Destroy(gameObject);
            }
        }

    }
}