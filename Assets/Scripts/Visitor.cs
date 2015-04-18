using UnityEngine;

public class Visitor : MonoBehaviour {

    public NavMeshAgent agent;
    public Animator animator;
    public Transform leftHand, rightHand;
    
    private void Update() {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}
