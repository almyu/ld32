using UnityEngine;
using System.Collections;

public class Visitor : MonoBehaviour {

    public NavMeshAgent agent;
    public Animator animator;
    public Rigidbody leftHand, rightHand;
    public float reach = 1.5f;

    public bool pending, has;
    public float dist, vel;
    public Target target;

    private void Start() {
        StartCoroutine(DoArmUp());
    }

    private void Update() {
        animator.SetFloat("Speed", agent.velocity.magnitude);

        pending = agent.pathPending;
        has = agent.hasPath;
        dist = agent.remainingDistance;
        vel = agent.velocity.sqrMagnitude;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude <= Mathf.Epsilon)) {

            animator.SetTrigger("PathComplete");
        }
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
            yield return StartCoroutine(DoCharge());
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
