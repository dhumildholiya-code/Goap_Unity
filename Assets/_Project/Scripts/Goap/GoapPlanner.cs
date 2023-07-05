using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    /// <summary>
    /// Plans what actions can be complete in-order to fullfill a goal state.
    /// </summary>
    public class GoapPlanner
    {
        private class Node
        {
            public Node parent;
            public float runningCost;
            public Dictionary<string, Object> state;
            public GoapAction action;

            public Node(Node parent, float runningCost, Dictionary<string, Object> state, GoapAction action)
            {
                this.parent = parent;
                this.runningCost = runningCost;
                this.state = state;
                this.action = action;
            }
        }

        public Queue<GoapAction> Plan(GameObject agent, HashSet<GoapAction> availableAcions,
            Dictionary<string, Object> worldState,
            Dictionary<string, Object> goal)
        {
            foreach (GoapAction action in availableAcions)
            {
                action.DoReset();
            }

            HashSet<GoapAction> usableActions = new HashSet<GoapAction>();
            foreach (var action in availableAcions)
            {
                if (action.CheckProceduralPrecondition(agent))
                    usableActions.Add(action);
            }

            List<Node> leaves = new List<Node>();

            Node start = new Node(null, 0, worldState, null);
            bool success = BuildGraph(start, leaves, usableActions, goal);

            if (!success)
            {
                Debug.LogError("No Plan");
                return null;
            }

            //get cheapest action.
            Node cheapest = null;
            foreach (var leaf in leaves)
            {
                if (cheapest == null)
                    cheapest = leaf;
                else
                {
                    if (leaf.runningCost < cheapest.runningCost)
                        cheapest = leaf;
                }
            }

            List<GoapAction> result = new List<GoapAction>();
            Node n = cheapest;
            while (n != null)
            {
                if (n.action != null)
                {
                    result.Insert(0, n.action);
                }
                n = n.parent;
            }

            Queue<GoapAction> queue = new Queue<GoapAction>();
            foreach (var action in result)
            {
                queue.Enqueue(action);
            }

            return queue;
        }

        private bool BuildGraph(Node parent, List<Node> leaves, HashSet<GoapAction> usableActions, Dictionary<string, Object> goal)
        {
            bool foundOne = false;

            foreach (GoapAction action in usableActions)
            {
                if (InState(action.Preconditions, parent.state))
                {
                    Dictionary<string, Object> currentState = PopulateState(parent.state, action.Effects);
                    Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);

                    if (InState(goal, currentState))
                    {
                        leaves.Add(node);
                        foundOne = true;
                    }
                    else
                    {
                        HashSet<GoapAction> subset = ActionSubset(usableActions, action);
                        bool found = BuildGraph(node, leaves, subset, goal);
                        if (found) foundOne = true;
                    }
                }
            }

            return foundOne;
        }

        private HashSet<GoapAction> ActionSubset(HashSet<GoapAction> usableActions, GoapAction removeMe)
        {
            HashSet<GoapAction> subset = new HashSet<GoapAction>();
            foreach (var action in usableActions)
            {
                if (!action.Equals(removeMe))
                    subset.Add(action);
            }
            return subset;
        }

        private Dictionary<string, Object> PopulateState(Dictionary<string, Object> currentState, Dictionary<string, Object> stateChange)
        {
            Dictionary<string, Object> state = new Dictionary<string, Object>();
            foreach (var keyPair in currentState)
            {
                state.Add(keyPair.Key, keyPair.Value);
            }

            foreach (var keyPair in stateChange)
            {
                if (state.ContainsKey(keyPair.Key))
                {
                    state[keyPair.Key] = keyPair.Value;
                }
                else
                {
                    state.Add(keyPair.Key, keyPair.Value);
                }
            }
            return state;
        }

        private bool InState(Dictionary<string, Object> test, Dictionary<string, Object> state)
        {
            foreach (var keyPair in test)
            {
                if (!state.ContainsKey(keyPair.Key))
                {
                    return false;
                }
            }
            return true;
        }
    }
}