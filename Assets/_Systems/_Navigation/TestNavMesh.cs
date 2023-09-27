using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D;

public class TestNavMesh : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    [Button]
    void BuildNavMesh(){
        if(TryGetComponent(out Unity.AI.Navigation.NavMeshSurface surface)){
            surface.BuildNavMesh();
        }
    }
}
