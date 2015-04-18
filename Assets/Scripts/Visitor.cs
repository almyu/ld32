using UnityEngine;

public class Visitor : MonoBehaviour {

    public NavMeshAgent agent;
    public Animator animator;
    public Transform leftHand, rightHand;

    public bool pending, has;
    public float dist, vel;

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
}
