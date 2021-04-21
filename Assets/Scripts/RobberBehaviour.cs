using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    BehaviourTree bTree;
    public GameObject diamond;
    public GameObject van;
    public GameObject door;
    public GameObject door2;
    NavMeshAgent navAgent;

    public enum ActionState
    {
        IDLE,
        WORKING
    };

    ActionState state = ActionState.IDLE;
    Node.Status treeStatus = Node.Status.RUNNING;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = this.GetComponent<NavMeshAgent>();

        bTree = new BehaviourTree();
        Sequence steal = new Sequence("Steal Something");
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf goToDoor = new Leaf("Go To Door", GoToDoor);

        Leaf goToVan = new Leaf("Go To Van", GoToVan);

        steal.AddChild(goToDoor);

        steal.AddChild(goToDiamond);
        steal.AddChild(goToDoor);

        steal.AddChild(goToVan);
        bTree.AddChild(steal);




        bTree.PrintTree();

    }

    public Node.Status GoToDiamond()
    {
        return GoToLocation(diamond.transform.position);
    }

    public Node.Status GoToDoor()
    {
        return GoToLocation(door.transform.position);
    }

    public Node.Status GoToVan()
    {
        return GoToLocation(van.transform.position);
    }

    Node.Status GoToLocation(Vector3 destination)
    {
        float distToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE)
        {
            navAgent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(navAgent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    // Update is called once per frame
    void Update()
    {
        if (treeStatus == Node.Status.RUNNING)
            treeStatus = bTree.Process();
    }
}
