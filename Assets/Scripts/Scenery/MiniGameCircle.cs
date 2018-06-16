using UnityEngine;
using CrowShadowManager;
/// <summary>
/// https://www.youtube.com/watch?v=BGe5HDsyhkY
/// </summary>
namespace CrowShadowScenery
{
    public class MiniGameCircle : MonoBehaviour
    {
        private float RotateSpeed = 5f;
        private float Radius = 0.1f;

        private Vector2 _centre;
        private float _angle;

        private void Start()
        {
            _centre = transform.position;
        }

        private void Update()
        {

            _angle += RotateSpeed * Time.deltaTime;

            var offset = new Vector2(Mathf.Sin(_angle), Mathf.Cos(_angle)) * Radius;
            transform.position = _centre + offset;
        }

    }
}
