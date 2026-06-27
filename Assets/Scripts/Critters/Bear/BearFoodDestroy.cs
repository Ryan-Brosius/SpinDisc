using UnityEngine;

public class BearFoodDestroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlatformObject>(out PlatformObject obj))
        {
            GameManager.Instance.RemoveTowerFromList(obj.gameObject);
            Destroy(obj.gameObject);
            // Maybe try to update next target at this point?
        }
    }
}
