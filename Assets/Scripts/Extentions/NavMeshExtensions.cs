using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CanTemplate.Extensions
{
    public static class NavMeshExtensions
    {
        public static bool ReachedDestination(this NavMeshAgent agent, bool returnFalseIfDestinationSelf = true)
        {
            if (returnFalseIfDestinationSelf && agent.destination == agent.transform.position) return false;
            if (agent.pathPending) return false;
            if (!(agent.remainingDistance <= agent.stoppingDistance)) return false;
            return !agent.hasPath || agent.velocity.sqrMagnitude == 0f;
        }
    }
}