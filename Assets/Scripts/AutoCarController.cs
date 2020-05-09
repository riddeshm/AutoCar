using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoCarController : MonoBehaviour
{
    [SerializeField] Transform[] wayPoints;
    [SerializeField] LayerMask layerMask;
    [SerializeField] LineRenderer[] lineRenderers;

    NavMeshAgent agent;
    int currentWaypoint = 0;
    float maxSpeed = 8;

    //LineRenderer laserLineRenderer;
    float laserWidth = 0.1f;
    float laserMaxLength = 10f;

    bool hit = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //laserLineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MoveToWaypoint();

        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };

        foreach(LineRenderer lineRenderer in lineRenderers)
        {
            lineRenderer.SetPositions(initLaserPositions);
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        agent.speed = 0;
        hit = true;
    }

    void MoveToWaypoint()
    {
        agent.SetDestination(wayPoints[currentWaypoint].position);
    } 

    void UpdateCurrentWaypoint()
    {
        if (currentWaypoint >= wayPoints.Length - 1)
        {
            currentWaypoint = 0;
        }
        else
        {
            currentWaypoint++;
        }
    }

    void CheckObstacleAndSlowDown(RaycastHit raycastHit)
    {
        float distanceFromObstacle = Vector3.Distance(transform.position, raycastHit.point);
        if (agent.speed > 0)
        {
            agent.speed -= agent.speed - distanceFromObstacle;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (hit)
            return;

        if (Vector3.Distance(transform.position, wayPoints[currentWaypoint].position) < 1)
        {
            UpdateCurrentWaypoint();
            MoveToWaypoint();
        }

        ShootLaserFromTargetPosition(transform.position, transform.forward, laserMaxLength, lineRenderers[0]);
        //ShootLaserFromTargetPosition(transform.position, transform.forward + new Vector3( 1/Mathf.Tan(45), 0, 0), laserMaxLength, lineRenderers[1]);
        //ShootLaserFromTargetPosition(transform.position, transform.forward + new Vector3(1 / Mathf.Tan(-45), 0, 0), laserMaxLength, lineRenderers[2]);
    }

    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length, LineRenderer laserLineRenderer)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        //if (Physics.Raycast(ray, out raycastHit, length))
        if (Physics.Raycast(ray, out raycastHit, length, layerMask))
        {
            endPosition = raycastHit.point;
            CheckObstacleAndSlowDown(raycastHit);
        }
        else
        {
            agent.speed = 8;
        }

        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }
}
