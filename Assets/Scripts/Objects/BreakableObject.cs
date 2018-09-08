using UnityEngine;
using CrowShadowPlayer;

namespace CrowShadowObjects
{
    public class BreakableObject : MonoBehaviour
    {
        public bool broke = false;

        SpriteRenderer spriteRenderer;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag.Equals("Pedra") && collision.GetComponent<FarAttackObject>().active)
            {
                collision.GetComponent<FarAttackObject>().hitSuccess = true;
                broke = true;
            }
        }
    }
}