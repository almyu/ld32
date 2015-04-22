using UnityEngine;
using System.Collections;

public class Visitor : MonoBehaviour {

    public NavMeshAgent agent;
    public Animator animator;
    public Rigidbody leftHand, rightHand;
    public Transform weaponMark;
    public float reach = 1.5f;
    public float sittingOffset = 0f;
    public float warpDuration = 0.5f;
    public float warpHeight = 10f;

    public Weapon weapon;

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
            agent.destination = new Vector3(2.8f + Random.Range(-2f, 2f), 0f, 5f);
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
        animator.Play("Idle");

        if (!weapon) {
            StopAllCoroutines();
            StartCoroutine(DoArmUp());
        }
    }

    public void Disarm() {
        if (weapon) {
            var joint = weapon.GetComponent<FixedJoint>();
            if (joint) DestroyImmediate(joint);

            weapon.SetHeld(false);
            weapon.SetBeingAnimated(false);
        }
        weapon = null;

        animator.Play("Calm", 1);
    }

    public void Leave() {
        agent.speed = 2f;

        StopAllCoroutines();
        StartCoroutine(DoLeave());
    }

    private void OnDestroy() {
        if (weapon) {
            var joint = weapon.GetComponent<FixedJoint>();
            if (joint) DestroyImmediate(joint);

            weapon.SetHeld(false);
        }
    }

    private IEnumerator DoGo(Vector3 position) {
        agent.destination = position;

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;
    }

    private static Vector3 RollPosition() {
        return new Vector3(Random.value * 11f, 0f, Random.value * 5f);
    }

    private IEnumerator DoLeave() {
        Disarm();
        yield return StartCoroutine(DoGo(Entrance.instance.transform.position));
        Destroy(gameObject);
    }

    private IEnumerator DoArmUp() {
        agent.stoppingDistance = 1f;

        Disarm();

        Weapon wpn;
        do {
            wpn = Weapon.FindClosest(transform.position);
            if (wpn) yield return StartCoroutine(DoGo(wpn.transform.position));
            else yield return StartCoroutine(DoGo(RollPosition()));
        }
        while (!wpn || !wpn.enabled);

        wpn.enabled = false;
        weapon = wpn;

        var collider = wpn.GetComponentInChildren<Collider>();
        if (collider) {
            var ownCollider = GetComponentInChildren<Collider>();
            if (ownCollider) Physics.IgnoreCollision(collider, ownCollider);
        }

        animator.SetTrigger("Pickup");

        var desiredLook = Quaternion.LookRotation((wpn.transform.position - transform.position).WithY(0f));

        for (var t = 0f; t <= 0.5f; t += Time.deltaTime) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredLook, 10f);
            yield return null;
        }

        wpn.SetHeld(true);
        wpn.SetBeingAnimated(true);

        var xf = wpn.transform;
        var handXf = rightHand.transform;

        var initialRot = xf.rotation;
        var relRot = Quaternion.Euler(wpn.handleRotation);

        for (var t = 0f; t <= 0.5f; t += Time.deltaTime) {
            if (!xf) break;

            var progress = t * 2f;
            var dstRot = transform.rotation * relRot;

            xf.rotation = Quaternion.Slerp(initialRot, dstRot, progress);
            xf.position = handXf.position - xf.rotation * (wpn.handleOffset * progress);
            yield return null;
        }

        if (wpn) {
            var joint = wpn.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = rightHand;

            wpn.SetBeingAnimated(false);
        }
        
        while (weapon) yield return StartCoroutine(DoCharge());
        yield return StartCoroutine(DoArmUp());
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

        yield return new WaitForSeconds(Random.Range(0.6f, 0.8f));
    }
}
