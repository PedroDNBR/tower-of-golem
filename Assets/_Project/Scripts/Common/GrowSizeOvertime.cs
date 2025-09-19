using UnityEngine;

namespace TW
{
    public class GrowSizeOvertime : MonoBehaviour
    {
        [SerializeField]
        private float startingSize = 1f;

        [SerializeField]
        private float targetSize = 2;

        private Vector3 sizeGrowthVector;

        private void OnEnable()
        {
            transform.localScale = new Vector3(startingSize, startingSize, startingSize);
            sizeGrowthVector = new Vector3(targetSize, targetSize, targetSize);
        }

        public void FixedUpdate()
        {
            transform.localScale += sizeGrowthVector * Time.deltaTime;
        }
    }
}