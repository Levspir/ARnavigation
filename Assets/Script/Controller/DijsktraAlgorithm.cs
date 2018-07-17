using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DijsktraAlgorithm : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool FindShortestPath(GameObject startNode, GameObject finishNode)
    /* reset and change data in node for navigate 
	can only navigate in same floor*/
    {
        GameObject floorObject = startNode.GetComponent<NodeData>().GetParentObjectData().GetParentFloorObject();
        ResetAllVertexData(floorObject);
        FloorData currentFloor = floorObject.GetComponent<FloorData>();

        List<GameObject> unVisitedList = new List<GameObject>();
        foreach (GameObject node in floorObject.GetComponent<FloorData>().GetNodesList())
        {
            unVisitedList.Add(node);
        }

        bool isFounded = false;
        float costToadjacentNode = 0;
        GameObject currentNode = startNode;
        NodeData currentNodeData = currentNode.GetComponent<NodeData>();
        currentNodeData.cost = 0;

        Debug.Log(" first node cost 0");

        Debug.Log(" - - - - " + (unVisitedList.Count > 0));
        while (currentNode != finishNode && (unVisitedList.Count >= 0)) //unVisitedList.Count.CompareTo(0)  ||  (unvisitedLeft > 0)   
        {
            //check adjacentNode
            foreach (GameObject adjacentObject in currentNodeData.adjacentNodeList)
            {
                NodeData adjacentNodeData = adjacentObject.GetComponent<NodeData>();
                Debug.Log(" -@ " + currentNodeData.nodeID +
                        "  -Adjacent |" + adjacentNodeData.nodeID +
                        "| End at " + finishNode.GetComponent<NodeData>().nodeID);

                if (adjacentNodeData.GetParentObjectData().roomName == finishNode.GetComponent<NodeData>().GetParentObjectData().roomName)
                { // if adjacent is final node  (adjacentObject == finishNode)
                    adjacentNodeData.cost = Vector3.Distance(currentNodeData.position, adjacentNodeData.position) + currentNodeData.cost;
                    unVisitedList.Remove(currentNode);
                    unVisitedList.Remove(adjacentObject);
                    adjacentNodeData.predecessor = currentNode;
                    currentNode = adjacentObject;
                    currentNodeData = currentNode.GetComponent<NodeData>();
                    isFounded = true;
                    Debug.Log(" - - Break - -");
                    break;
                }
                else if (unVisitedList.Contains(adjacentObject))
                { //neightbor are not visited, Update it
                    costToadjacentNode = Vector3.Distance(currentNodeData.position, adjacentNodeData.position) + currentNodeData.cost;
                    if (costToadjacentNode < adjacentNodeData.cost)
                    {
                        Debug.Log("cost from here are less than older " + costToadjacentNode + "<" + adjacentNodeData.cost);
                        adjacentNodeData.cost = costToadjacentNode;
                        adjacentNodeData.predecessor = currentNode;
                        Debug.Log("Predecessor of " + adjacentNodeData.nodeID + " are " + adjacentNodeData.predecessor.GetComponent<NodeData>().nodeID);
                    }

                    Debug.Log("  Update " + adjacentNodeData.nodeID + "  to " + adjacentNodeData.cost);
                }
            }
            if (isFounded) { break; }

            // find Least cost  and choose to current node
            GameObject leastCostNode = finishNode;
            foreach (GameObject unVisitedObj in unVisitedList)
            {
                NodeData unVisitedNode = unVisitedObj.GetComponent<NodeData>();
                if (unVisitedNode.cost < leastCostNode.GetComponent<NodeData>().cost && unVisitedObj != currentNode)
                {
                    leastCostNode = unVisitedObj;
                }
                Debug.Log("Compare " + unVisitedNode.nodeID + "-  " + unVisitedNode.cost + "<" + leastCostNode.GetComponent<NodeData>().cost
                    + "  Least cost are:" + leastCostNode.GetComponent<NodeData>().nodeID + " cost:" + leastCostNode.GetComponent<NodeData>().cost);
            }
            unVisitedList.Remove(currentNode);
            Debug.Log("change predecessor of leastcostnode|" + leastCostNode.GetComponent<NodeData>().nodeID +
                 "| form " + leastCostNode.GetComponent<NodeData>().predecessor + " To " + currentNode);
            currentNode = leastCostNode;
            currentNodeData = currentNode.GetComponent<NodeData>();
            Debug.Log(" =====" + " Unvisited left " + unVisitedList.Count + " >=0 is " + (unVisitedList.Count >= 0));
            Debug.Log("===== CurrentNode are " + currentNode.GetComponent<NodeData>().nodeID);

        }

        /* set successor from reverse finishNode's preDecessor */
        currentNode = finishNode;
        Debug.Log(" From : " + finishNode.GetComponent<NodeData>().nodeID);
        while (currentNode != startNode)
        {
            currentNode.GetComponent<NodeData>().predecessor.GetComponent<NodeData>().successor = currentNode;
            currentNode = currentNode.GetComponent<NodeData>().predecessor;
            Debug.Log(currentNode.GetComponent<NodeData>().nodeID);
        }
        Debug.Log(" To : " + startNode.GetComponent<NodeData>().nodeID);

        return isFounded;
    }

    // private GameObject FindGameObjectFromMarkerData(GameObject floorObject, MarkerData markerData)
    // {
    // 	foreach (GameObject markerob in floorObject.GetComponent<FloorData>().markerList)
    // 	{
    // 		if(markerob.GetComponent<MarkerData>().markerName == markerData.markerName)
    // 		{
    // 			return markerob;
    // 		}
    // 	}
    // 	return floorObject.GetComponent<FloorData>().markerList[0].gameObject;
    // }

    public void ResetAllVertexData(GameObject floorObject)
    {
        foreach (GameObject node in floorObject.GetComponent<FloorData>().GetNodesList())
        {
            NodeData nodedt = node.GetComponent<NodeData>();
            nodedt.cost = Single.PositiveInfinity;
            nodedt.predecessor = null;
            nodedt.successor = null;
        }

        // foreach (GameObject roomlist in floorObject.GetComponent<FloorData>().GetRoomsList())
        // {
        //     Debug.Log(roomlist.name);
        //     foreach (Transform nodetrans in roomlist.transform)
        //     {
        //         Debug.Log(nodetrans.name);
        //         NodeData nodedt = nodetrans.gameObject.GetComponent<NodeData>();
        //         nodedt.cost = Single.PositiveInfinity;
        //         nodedt.predecessor = null;
        //         nodedt.successor = null;
        //     }
        // }
    }


}
