using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public NavMeshAgent agent;
    public Transform target;

    private void Update() {
        agent.destination = target.position;
    }
}
