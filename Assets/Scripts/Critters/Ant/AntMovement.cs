using UnityEngine;
using System.Collections.Generic;

public class AntMovement : CritterMovement
{
    private List<Vector3> trail;
    private float lockedY;
    public int TrailIndex { get; private set; }

    public void SetupWithTrail(List<Vector3> sharedTrail, float y)
    {
        trail = sharedTrail;
        lockedY = y;
        TrailIndex = 0;
    }

    public override void SetTarget(Transform target) { }

    private void Update()
    {
        if (isStunned || trail == null || TrailIndex >= trail.Count) return;

        Vector3 goal = trail[TrailIndex];
        goal.y = lockedY;
        MoveTowards(goal);

        float dist = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(goal.x, goal.z)
        );

        if (dist < 0.12f)
            TrailIndex++;
    }
}