using UnityEngine;
using System.Collections;

public class MomoFeed : MonoBehaviour
{
    [Header("Momo")]
    public Transform momoMover;        // the MomoMover parent (the one that walks)
    public TurtleWander wander;        // the TurtleWander script on MomoMover
    public Transform momoModel;        // the Momo model (child) - used for the happy hop

    [Header("Food")]
    public GameObject foodPrefab;      // any small food model from the food pack
    public Transform foodPoint;        // empty on the floor inside the dome where food lands
    public float dropHeight = 0.6f;

    [Header("Eating position")]
    public float mouthOffset = 0f;     // + = stop further from the food, - = closer

    [Header("Timing")]
    public float walkSpeed = 0.3f;
    public float eatTime = 1.5f;
    public float hopHeight = 0.08f;
    public float turnSpeed = 6f;       // how fast Momo turns while walking to/from the food

    bool busy = false;

    public void Feed()
    {
        if (busy)
        {
            GameManager.instance.ShowTimed("Momo is still eating!", 2f);
            return;
        }
        StartCoroutine(FeedRoutine());
    }

    IEnumerator FeedRoutine()
    {
        busy = true;
        GameManager.instance.ShowTimed("You drop a snack into Momo's dome.", 2.5f);

        // Drop the food in from above
        Vector3 landPos = foodPoint.position;
        GameObject food = Instantiate(foodPrefab, landPos + Vector3.up * dropHeight, foodPoint.rotation);
        foreach (Collider c in food.GetComponentsInChildren<Collider>()) c.enabled = false;
        Vector3 foodScale = food.transform.localScale;
        for (float t = 0; t < 1f; t += Time.deltaTime / 0.5f)
        {
            food.transform.position = Vector3.Lerp(landPos + Vector3.up * dropHeight, landPos, t * t);
            yield return null;
        }
        food.transform.position = landPos;

        // Stop wandering and remember where Momo was (position only)
        wander.enabled = false;
        Vector3 homePos = momoMover.position;

        // How far Momo's head sticks out in front of his centre
        float headDist = 0f;
        if (wander.head)
        {
            Vector3 toHead = wander.head.position - momoMover.position;
            toHead.y = 0;
            headDist = toHead.magnitude;
        }
        float stopDist = Mathf.Max(0.02f, headDist + mouthOffset);

        // Walk towards the food and stop when the head is over it
        Vector3 target = new Vector3(landPos.x, homePos.y, landPos.z);
        yield return WalkTo(target, stopDist);
        yield return TurnToFace(target);

        // Eat: the food shrinks away while Momo nibbles
        Vector3 modelStart = momoModel.localPosition;
        for (float t = 0; t < 1f; t += Time.deltaTime / eatTime)
        {
            food.transform.localScale = foodScale * (1f - t);
            momoModel.localPosition = modelStart + Vector3.up * Mathf.Abs(Mathf.Sin(t * 20f)) * hopHeight * 0.25f;
            yield return null;
        }
        Destroy(food);
        momoModel.localPosition = modelStart;

        // Happy hops
        GameManager.instance.ShowTimed("Momo munches happily!", 2.5f);
        for (int i = 0; i < 2; i++)
        {
            for (float t = 0; t < 1f; t += Time.deltaTime / 0.25f)
            {
                momoModel.localPosition = modelStart + Vector3.up * Mathf.Sin(t * Mathf.PI) * hopHeight;
                yield return null;
            }
        }
        momoModel.localPosition = modelStart;

        // Walk back, then hand over to TurtleWander (it turns him smoothly - no snapping)
        yield return WalkTo(homePos, 0.02f);
        momoMover.position = new Vector3(homePos.x, momoMover.position.y, homePos.z);
        wander.enabled = true;
        busy = false;
    }

    Quaternion FaceRotation(Vector3 dir)
    {
        return Quaternion.LookRotation(wander.flipFacing ? -dir : dir);
    }

    IEnumerator WalkTo(Vector3 target, float stopDistance)
    {
        while (true)
        {
            Vector3 flat = target - momoMover.position;
            flat.y = 0;
            if (flat.magnitude <= stopDistance) break;

            Vector3 dir = flat.normalized;
            momoMover.rotation = Quaternion.Slerp(momoMover.rotation, FaceRotation(dir), Time.deltaTime * turnSpeed);
            momoMover.position = Vector3.MoveTowards(momoMover.position,
                new Vector3(target.x, momoMover.position.y, target.z), walkSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 target)
    {
        Vector3 flat = target - momoMover.position;
        flat.y = 0;
        if (flat.sqrMagnitude < 0.0001f) yield break;

        Quaternion goal = FaceRotation(flat.normalized);
        while (Quaternion.Angle(momoMover.rotation, goal) > 1f)
        {
            momoMover.rotation = Quaternion.Slerp(momoMover.rotation, goal, Time.deltaTime * turnSpeed);
            yield return null;
        }
        momoMover.rotation = goal;
    }
}