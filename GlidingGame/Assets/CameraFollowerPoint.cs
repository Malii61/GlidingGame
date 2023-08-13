using UnityEngine;

public class CameraFollowerPoint : MonoBehaviour
{
    [SerializeField] private Transform ballTransform;

    private void Update()
    {
        transform.position = ballTransform.position;
    }
    private void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, ballTransform.rotation.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 3f);
    }


}
