using UnityEngine;

public class CalmBehaviour : StateMachineBehaviour {

    public float walkingSpeed = 2f;
    public float runningSpeed = 4f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        var agent = animator.GetComponent<NavMeshAgent>();
        
        agent.speed = walkingSpeed;
        agent.destination = Seat.Pick(Seat.Preferences.none).GetWaypoint(animator.transform.position);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.GetComponent<NavMeshAgent>().speed = runningSpeed;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // drink peacefully
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetIKPosition(AvatarIKGoal.RightHand, animator.GetComponent<NavMeshAgent>().destination);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.5f);
    }
}
