using System.Collections;
using UnityEngine;

public class SquirrelMovement : CritterMovement
{
    [Header("Twitching")]
    [SerializeField] private int twitchCount = 3;
    [SerializeField] private float twitchDuration = 0.4f;   // how long each twitch dash lasts
    [SerializeField] private float pauseDuration = 0.3f;    // how long it stops between twitches
    [SerializeField] private float twitchAngle = 35f;       // max degrees off-center per twitch
    [SerializeField] private float twitchStepDistance = 3f; // how far each twitch moves it

    [Header("Charge")]
    [SerializeField] private float chargeSpeedMultiplier = 2f; // charge is faster than normal speed

    private Transform target;
    private bool isCharging = false;

    public override void Initialize(CritterSettingsSO settings)
    {
        base.Initialize(settings);
        StartCoroutine(TwitchRoutine());
    }

    public override void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if (isStunned || !isCharging || target == null) return;
        MoveTowards(target.position);
    }

    private IEnumerator TwitchRoutine()
    {
        yield return null;

        for (int i = 0; i < twitchCount; i++)
        {
            yield return StartCoroutine(DashTwitch());

            StopMoving();
            yield return new WaitForSeconds(pauseDuration);
        }

        speedMultiplier = chargeSpeedMultiplier;
        isCharging = true;
    }

    private IEnumerator DashTwitch()
    {
        if (target == null) yield break;

        Vector3 toTarget = (target.position - transform.position);
        toTarget.y = 0f;
        float baseAngle = Mathf.Atan2(toTarget.z, toTarget.x) * Mathf.Rad2Deg;
        float randomOffset = Random.Range(-twitchAngle, twitchAngle);
        float dashAngle = (baseAngle + randomOffset) * Mathf.Deg2Rad;

        Vector3 dashDir = new Vector3(Mathf.Cos(dashAngle), 0f, Mathf.Sin(dashAngle)).normalized;
        Vector3 dashTarget = transform.position + dashDir * twitchStepDistance;
        dashTarget.y = transform.position.y;

        float elapsed = 0f;
        while (elapsed < twitchDuration)
        {
            if (isStunned)
            {
                StopMoving();
                yield return new WaitUntil(() => !isStunned);
            }

            MoveTowards(dashTarget);
            elapsed += Time.deltaTime;
            yield return null;
        }

        StopMoving();
    }

    protected override void OnStunEnd()
    {
        if (isCharging) return;
    }
}
