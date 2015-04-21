using UnityEngine;
using System.Collections;

public class Visitor : MonoBehaviour {

    public NavMeshAgent agent;
    public Animator animator;
    public Rigidbody leftHand, rightHand;
    public float reach = 1.5f;
    public float sittingOffset = 0f;
    public float warpDuration = 0.5f;
    public float warpHeight = 10f;

    public bool pending, has;
    public float dist, vel;
    public Target target;

    private void Start() {
        WarpToSeat();
    }

    private void Update() {
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (!agent.enabled) return;

        pending = agent.pathPending;
        has = agent.hasPath;
        dist = agent.remainingDistance;
        vel = agent.velocity.sqrMagnitude;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude <= Mathf.Epsilon)) {

            animator.SetTrigger("PathComplete");
        }
    }

    private void WarpToSeat() {
        var seat = Seat.Pick();
        if (!seat) {
            agent.destination = new Vector3(2.8f + Random.Range(-2f, 2f), 0f, 5.5f);
            return;
        }

        agent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        transform.position = seat.transform.TransformPoint(seat.mountPoint).WithY(sittingOffset);
        transform.rotation = seat.transform.rotation;
        seat.enabled = false;

        animator.Play("SitDown");
        StartCoroutine(DoWarpToSeat(transform.position));
    }

    private IEnumerator DoWarpToSeat(Vector3 dest) {
        var start = dest + Vector3.up * warpHeight;

        for (var t = 0f; t <= warpDuration; t += Time.deltaTime) {
            var progress = t / warpDuration;
            progress *= progress;

            transform.position = Vector3.Lerp(start, dest, progress);
            yield return null;
        }

        transform.position = dest;
    }

    public void Enrage() {
        agent.enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        animator.Play("Idle");
        StartCoroutine(DoArmUp());
    }

    private IEnumerator DoGo(Vector3 position) {
        agent.destination = position;

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;
    }

    public Vector3 rightHandTarget;
    public float rightHandWeight;

    private IEnumerator DoArmUp() {
        Weapon wpn;
        do {
            wpn = Weapon.FindClosest(transform.position);
            if (wpn == null) yield break;

            agent.stoppingDistance = 1f;
            yield return StartCoroutine(DoGo(wpn.transform.position));
        }
        while (!wpn.enabled);

        wpn.enabled = false;

        animator.SetTrigger("Pickup");

        rightHandTarget = wpn.transform.position;
        var desiredLook = Quaternion.LookRotation((rightHandTarget - transform.position).WithY(0f));

        for (var t = 0f; t <= 0.5f; t += Time.deltaTime) {
            rightHandWeight = t * 2f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredLook, 10f);
            yield return null;
        }

        wpn.transform.SetParent(rightHand.transform);
        wpn.transform.localPosition = wpn.transform.localPosition.normalized * 0.4f;

        wpn.transform.localRotation = Quaternion.identity;

        var body = wpn.GetComponent<Rigidbody>();
        if (body) body.isKinematic = true;

        var collider = wpn.GetComponentInChildren<Collider>();
        if (collider) {
            var ownCollider = GetComponentInChildren<Collider>();
            if (ownCollider) Physics.IgnoreCollision(collider, ownCollider);
        }

        var obstacle = wpn.GetComponent<NavMeshObstacle>();
        if (obstacle) obstacle.enabled = false;

        /*var joint = wpn.GetComponent<Joint>();
        joint.connectedBody = rightHand;
        joint.connectedAnchor = Vector3.zero;*/

        while (true) {
            if (wpn) yield return StartCoroutine(DoCharge());
            else yield return DoArmUp();
        }
    }

    private IEnumerator DoCharge() {
        var self = GetComponent<Target>();
        if (!self) yield break;

        target = null;
        do {
            if (target == null) {
                target = self.FindClosestEnemy();
                if (!target) {
                    yield return new WaitForSeconds(1f);
                    continue;
                }
            }

            var dest = target.transform.position;
            agent.destination = dest + (transform.position - dest).normalized * reach;

            yield return new WaitForSeconds(0.1f);
        }
        while (!target || agent.remainingDistance > reach);

        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        animator.SetTrigger("Swing");

        yield return new WaitForSeconds(0.7f);
    }

    private void OnAnimatorIK(int layerIndex) {
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
    }
}
