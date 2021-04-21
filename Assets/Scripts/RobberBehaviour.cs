using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    BehaviourTree bTree;
    public GameObject diamond;
    public GameObject van;
    public GameObject frontDoor;
    public GameObject backDoor;
    NavMeshAgent navAgent;

    public enum ActionState
    {
        IDLE,
        WORKING
    };

    ActionState state = ActionState.IDLE;
    Node.Status treeStatus = Node.Status.RUNNING;

    [Range(0, 1000)]
    public int money = 800;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = this.GetComponent<NavMeshAgent>();

        bTree = new BehaviourTree();
        Sequence steal = new Sequence("Steal Something");
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
        Leaf goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);

        Selector opendoor = new Selector("Open Door");

        opendoor.AddChild(goToFrontDoor);
        opendoor.AddChild(goToBackDoor);

        steal.AddChild(hasGotMoney);
        steal.AddChild(opendoor);
        steal.AddChild(goToDiamond);
        //steal.AddChild(goToDoor2);
        steal.AddChild(goToVan);
        bTree.AddChild(steal);

        bTree.PrintTree();

    }


    public Node.Status HasMoney()
    {
        if (money >= 500)
            return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToDiamond()
    {
        Node.Status s = GoToLocation(diamond.transform.position);
        if(s == Node.Status.SUCCESS)
        {
            diamond.transform.parent = this.gameObject.transform;
        }
        return s;
    }

    public Node.Status GoToBackDoor()
    {
        return GoToDoor(backDoor);
    }

    public Node.Status GoToFrontDoor()
    {
        return GoToDoor(frontDoor);
    }

    public Node.Status GoToVan()
    {
        Node.Status s = GoToLocation(van.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            money += 300;
            diamond.SetActive(false);
        }
        return s;
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status s = GoToLocation(door.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked)
            {
                door.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else
            return s;
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
        if (treeStatus != Node.Status.SUCCESS)
            treeStatus = bTree.Process();
    }
}
