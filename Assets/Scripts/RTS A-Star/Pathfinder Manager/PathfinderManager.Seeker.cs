using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using AStar;

namespace AStar {

    public partial class PathfinderManager : MonoBehaviour {

        //Starts the pathfinding process
        public void FindPath(Vector3 pathStart, Vector3 pathEnd, Action<List<Node>, Vector3[], bool> callback, PathType pathType) {
            List<Node> nodeWaypoints = new List<Node>();
            Vector3[] waypoints = new Vector3[0];                       //All waypoints
            bool pathSuccess = false;                                   //By default the pathSuccess will be false

            Node startNode = Grid.main.NodeFromWorldPoint(pathStart);   //The starting node
            Node endNode = Grid.main.NodeFromWorldPoint(pathEnd);       //The ending node


            if(pathType == PathType.PointToPoint) {
                if(CreatePath(startNode, endNode)) {
                    List<Node> waypointNodes = RetracePath2(startNode, endNode);
                    List<Node> waypointSimpilifyed = SimplifyPath2(waypointNodes);
                    waypoints = ExtractPath(waypointSimpilifyed);
                }
            } else if(pathType == PathType.AllPoints) {
                if(CreatePath(startNode, endNode)) {
                    List<Node> waypointNodes = RetracePath2(startNode, endNode);
                    //nodeWaypoints = waypointNodes;
                    waypoints = ExtractPath(waypointNodes);
                }
            } else if(pathType == PathType.EndOfTheLine) {
                if(CreatePath(startNode, endNode, true)) {
                    List<Node> waypointNodes = RetracePath2(startNode, endNode);

                    //If the last node is unwalkable (Special feature)
                    if(!waypointNodes[0].isWalkable) {
                        UnityEngine.Debug.Log("Last node is unwalkable!");

                        List<Node> waypointNodes2 = new List<Node>();
                        bool delNodes = true;

                        //Node: WAYPOINTS IS BACKWARDS RIGHT NOW
                        foreach(Node node in waypointNodes) {
                            //The first iteration should always be true VVV
                            if(!node.isWalkable && delNodes) {

                            } else {
                                delNodes = false;
                                waypointNodes2.Add(node);
                            }
                        }



                        Node startWaypointNode = waypointNodes2[waypointNodes2.Count-1];
                        Node endWaypointNode = waypointNodes2[0];

                        //Normal PointToPoint
                        //if(CreatePath(startWaypointNode, endWaypointNode)) {
                        //	List<Node> waypointNodes3 = RetracePath2(startWaypointNode, endWaypointNode);
                        //	List<Node> waypointSimpilifyed = SimplifyPath2(waypointNodes3);
                        //	waypoints = ExtractPath(waypointSimpilifyed);
                        //}

                        //AllPoints
                        if(CreatePath(startWaypointNode, endWaypointNode)) {
                            List<Node> waypointNodes3 = RetracePath2(startWaypointNode, endWaypointNode);
                            waypoints = ExtractPath(waypointNodes3);
                        }

                        //waypoints = ExtractPath(waypointNodes2);

                    }
                    //If the last node is walkable (Special feature not applyed)
                    else {
                        UnityEngine.Debug.Log("Last node is walkable");

                        //Normal PointToPoint
                        if(CreatePath(startNode, endNode)) {
                            List<Node> waypointNodes2 = RetracePath2(startNode, endNode);
                            List<Node> waypointSimpilifyed = SimplifyPath2(waypointNodes2);
                            waypoints = ExtractPath(waypointSimpilifyed);
                        }
                    }






                    //Node: WAYPOINTS IS BACKWARDS RIGHT NOW
                    for(int i = 0; i < waypointNodes.Count; i++) {
                        Node currentNode = waypointNodes[i];

                        if(!currentNode.isWalkable && i == 0) {
                            //UnityEngine.Debug.Log("Last node is unwalkable");
                        }

                    }

                }
            }

            if(waypoints.Length > 0) {
                pathSuccess = true;
            } else {
                pathSuccess = false;
            }


            //Method message
            OnFinishedProcessingPath(new PathResult(nodeWaypoints, waypoints, pathSuccess, callback));   //Method on other page
        }

        /*
        [Obsolete("Use FindPath() instead.")]
        public void FindPathOld(PathRequest request) {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;

            Node startNode = Grid.main.NodeFromWorldPoint(request.pathStart);
            Node targetNode = Grid.main.NodeFromWorldPoint(request.pathEnd);

            //PATHFINDING MATH
            if(startNode.isWalkable) { //&& targetNode.isWalkable
                Heap<Node> openSet = new Heap<Node>(Grid.main.maxSize);
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);

                while(openSet.Count() > 0) {
                    Node currentNode = openSet.RemoveFirst();

                    closedSet.Add(currentNode);

                    //We have found the final path (End of path)
                    if(currentNode == targetNode) {
                        sw.Stop();
                        //print("Path found: " + sw.ElapsedMilliseconds + " ms");

                        pathSuccess = true;
                        break;
                    }

                    foreach(Node neighbor in Grid.main.GetNeighbours(currentNode)) {

                        if(!neighbor.isWalkable || closedSet.Contains(neighbor)) {  //If the current neighbor is not walkable OR if it is in the closedSet list
                            continue;
                        }

                        //New gCost
                        int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;

                        if(newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
                            neighbor.gCost = newMovementCostToNeighbor;
                            neighbor.hCost = GetDistance(neighbor, targetNode);
                            neighbor.parent = currentNode;

                            //If openSet does not contain the current node then add it to the list
                            if(!openSet.Contains(neighbor)) {
                                openSet.Add(neighbor);
                            } else {
                                openSet.UpdateItem(neighbor);
                            }
                        }

                        if(!pathSuccess) {
                            //UnityEngine.Debug.Log("Failed");
                            if(currentNode.nodeType == NodeType.Building) {
                                UnityEngine.Debug.Log("Building node found: " + currentNode.gameObject.name);
                                //pathSuccess = true;
                            }
                        }
                    }
                }

            }
            //END OF MATH

            if(pathSuccess) {   //If the path was a success
                waypoints = RetracePath(startNode, targetNode);

                if(waypoints.Length > 0) {
                    pathSuccess = true;
                } else {
                    pathSuccess = false;
                }
            } else {
                //UnityEngine.Debug.Log("Start Node: " + startNode.isWalkable + "\nEnd Node: " + targetNode.isWalkable);
            }

            //Method message
            OnFinishedProcessingPath(new PathResult(waypoints, pathSuccess, request.callback));   //Method on other page
        }
        */


        //Pathfinding Math
        public bool CreatePath(Node startNode, Node targetNode, bool ignoreIsWalkable = false) {
            Heap<Node> openSet = new Heap<Node>(Grid.main.maxSize);     //The Nodes to check
            HashSet<Node> closedSet = new HashSet<Node>();              //The Nodes already checked

            openSet.Add(startNode);

            while(openSet.Count() > 0) {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                //We have found the final path (End of path)
                if(currentNode == targetNode) {
                    return true;
                }

                foreach(Node neighbor in Grid.main.GetNeighbours(currentNode)) {
                    if(!neighbor.isWalkable && !ignoreIsWalkable) {
                        continue;
                    }
                    if(closedSet.Contains(neighbor)) {
                        continue;
                    }


                    //New gCost
                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;

                    if(newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        //If openSet does not contain the current node then add it to the list
                        if(!openSet.Contains(neighbor)) {
                            openSet.Add(neighbor);
                        } else {
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
            return false;
        }



        //Create the waypoint nodes
        [Obsolete("Dont use anymore")]
        public Vector3[] RetracePath(Node startNode, Node endNode) {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while(currentNode != startNode) {
                path.Add(currentNode);
                //UnityEngine.Debug.Log(currentNode.worldPosition);
                currentNode = currentNode.parent;
                //UnityEngine.Debug.Log(currentNode.worldPosition);
                //UnityEngine.Debug.Log("Parent: " + currentNode.parent.worldPosition);

                if(currentNode == startNode) {
                    //UnityEngine.Debug.Log("start Node found");
                    //path.Add(currentNode);
                    //UnityEngine.Debug.Log(currentNode.worldPosition);
                }
                if(currentNode.worldPosition == null) {
                    UnityEngine.Debug.Log("Null");
                }
                //UnityEngine.Debug.Log(currentNode.worldPosition);
            }

            //Store the waypoints
            Vector3[] waypoints;
            waypoints = SimplifyPath(path);

            //Reverse the array because it is backwards right now
            Array.Reverse(waypoints);


            List<Vector3> allWaypoints = new List<Vector3>();
            foreach(Node n in path) {
                allWaypoints.Add(n.worldPosition);
            }

            Vector3[] allWaypointsArray = allWaypoints.ToArray();
            Array.Reverse(allWaypointsArray);


            //return allWaypointsArray;
            return waypoints;
        }

        public List<Node> RetracePath2(Node startNode, Node endNode) {
            //Note: We dont need all of the waypoints because we will be useing the Node's "parent" variable

            List<Node> waypointNodes = new List<Node>();    //Store the waypoint nodes
            Node currentNode = endNode;                     //Start backwards because of the Node's "parent" variable

            //We dont know how long it is going to run so I am using a "do while" loop
            //The waypoints will be added backwards in this loop
            do {
                //Add the node to the list
                waypointNodes.Add(currentNode);

                //Set the current node to be the current node's parent
                currentNode = currentNode.parent;

                //Prevent a null error
                if(currentNode.worldPosition == null) {
                    UnityEngine.Debug.Log("Null Position");
                }
            } while(currentNode != startNode);

            return waypointNodes;
            //Node: THE WAYPOINTS ARE BACKWARDS
        }



        [Obsolete("Dont use anymore")]
        public Vector3[] SimplifyPath(List<Node> path) {
            List<Vector3> waypoints = new List<Vector3>();

            Vector2 directionOld = Vector2.zero;
            for(int i = 0; i < path.Count-1; i++) {
                Vector2 directionNew = new Vector2(path[i].gridX - path[i+1].gridX, path[i].gridY - path[i+1].gridY);

                //If the direction has changed from the last waypoint
                if(directionNew != directionOld) {
                    waypoints.Add(path[i+1].worldPosition);
                }
                //Adds the first node to the list
                else if(i == path.Count-2) {
                    //waypoints.Add(path[path.Count-1].worldPosition);
                    //UnityEngine.Debug.Log(path[i-1].worldPosition);
                }

                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }

        public List<Node> SimplifyPath2(List<Node> oldWaypoints) {
            //Node: THE WAYPOINTS ARE BACKWARDS

            List<Node> waypoints = new List<Node>();

            Vector2 directionOld = Vector2.zero;
            for(int i = 0; i < oldWaypoints.Count-1; i++) {
                Vector2 directionNew = new Vector2(oldWaypoints[i].gridX - oldWaypoints[i+1].gridX, oldWaypoints[i].gridY - oldWaypoints[i+1].gridY);

                //If the direction has changed from the last waypoint
                if(directionNew != directionOld) {
                    waypoints.Add(oldWaypoints[i]);
                }
                //Adds the first node to the list (Not quite the first node but the next node next to the first node)
                else if(i == oldWaypoints.Count-2) {
                    waypoints.Add(oldWaypoints[i+1]);
                }

                directionOld = directionNew;
            }

            return waypoints;
            //Node: THE WAYPOINTS ARE BACKWARDS
        }



        //Extracts the vector3's out of the node list and reverses all of the waypoints
        public Vector3[] ExtractPath(List<Node> waypointNodes) {
            //Store the waypoint's position
            Vector3[] waypoints = new Vector3[waypointNodes.Count];

            //Loop through all of the nodes and add them to the waypoint array
            for(int i = 0; i < waypointNodes.Count; i++) {
                waypoints[i] = waypointNodes[i].worldPosition;
            }

            //Reverse the array because it is backwards right now
            Array.Reverse(waypoints);

            return waypoints;
        }



        //Returns the disatance between two nodes
        public int GetDistance(Node nodeA, Node nodeB) {
            int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if(distanceX > distanceY) {
                return 14 * distanceY + 10 * (distanceX - distanceY);   //14y + 10(x - y)
            } else {
                return 14 * distanceX + 10 * (distanceY - distanceX);   //14x + 10(y - x)
            }
        }

    }

}