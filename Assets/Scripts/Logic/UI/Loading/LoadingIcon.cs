namespace Logic.UI.Loading
{
    using UnityEngine;

    public class LoadingIcon : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(Vector3.back, 0.5f);
        }
    }
}
