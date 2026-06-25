using UnityEngine;

public class BearFoodDestroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlatformObject>(out PlatformObject obj))
        {
            Destroy(obj.gameObject);
            // Maybe try to update next target at this point?
        }
    }
}
