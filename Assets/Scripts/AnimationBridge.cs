using UnityEngine;

public class AnimationBridge : MonoBehaviour
{
    [SerializeField] private TowerEntity parentComponent;

    // Update is called once per frame
    private void Fire()
    {
        if (parentComponent != null)
        {
            parentComponent.Fire();
        }
    }
}
