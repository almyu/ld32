using UnityEngine;
using System.Collections;

public class Visitor : MonoBehaviour {

    public NavMeshAgent agent;
    public Animator animator;
    public Rigidbody leftHand, rightHand;

    public bool pending, has;
    public float dist, vel;

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

        rightHandTarget = wpn.transform.position;

        for (var t = 0f; t <= 10f; t += Time.deltaTime) {
            rightHandWeight = t * 0.1f;
            yield return null;
        }

        wpn.transform.SetParent(rightHand.transform);
        wpn.transform.localPosition = wpn.transform.localPosition.normalized * 0.4f;
        /*var joint = wpn.GetComponent<Joint>();
        joint.connectedBody = rightHand;
        joint.connectedAnchor = Vector3.zero;*/

        while (true) {
            agent.destination = new Vector3(Random.Range(-5f, 1f), 0f, Random.Range(-8f, 5f));
            yield return new WaitForSeconds(5f);
        }
    }

    private void OnAnimatorIK(int layerIndex) {
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
    }
}
