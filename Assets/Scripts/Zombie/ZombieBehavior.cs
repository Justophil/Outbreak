using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class ZombieBehavior : MonoBehaviour
{
    public GameObject player;
    public bool isPlayerInAttackRange;
    public NavMeshAgent agent;
    public float maxAngle = 45;
    // public float maxAngle = 180;
    public float maxDistance = 20;
    // public Transform[] points;
    private int destPoint = 0;
    // private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
      // animator = GetComponent<Animator>();
      GotoNextPoint();
    }

    
    // Update is called once per frame
    void Update()
    {
      // animator.SetFloat("Speed",agent.speed);
      //if(SeePlayer())
      {
        Debug.Log(agent.transform.position);
        agent.destination = player.transform.position;
        agent.speed = 3.0f;
        // Go to Player
      }
      /*else {
        if(HasReachedDestination()) {
          StopMovement();
        }
      }*/
    }

    void GotoNextPoint() {
      ResumeMovement();
      // Returns if no points have been set up
      // Set the agent to go to the currently selected destination.
      agent.destination = player.transform.position;
      // points[destPoint].GetComponent<Renderer>().material.color = Color.red;
      // Choose the next point in the array as the destination,
      // cycling to the start if necessary.
      // destPoint = (destPoint + 1) % points.Length;
    }

    public bool SeePlayer()
    {
      Vector3 vecPlayerTurret = player.transform.position - transform.position;
      if (vecPlayerTurret.magnitude > maxDistance)
      {
        return false;
      }
      Vector3 normVecPlayerTurret = Vector3.Normalize(vecPlayerTurret);
      float dotProduct = Vector3.Dot(transform.forward,normVecPlayerTurret);
      var angle = Mathf.Acos(dotProduct);
      float deg = angle * Mathf.Rad2Deg;
      if (deg < maxAngle)
      {
        RaycastHit hit;
        Ray ray = new Ray(transform.position,vecPlayerTurret);
        if (Physics.Raycast(ray, out hit))
        {
          if (hit.collider.tag == "Player")
          {
            return true;
          }
        }
      }
      return false;
    }
    
    public bool HasReachedDestination()
    {
      if(Vector3.Distance(agent.destination, agent.transform.position) < 1.5f) {
        return true;
      }
		  return false;
    }

    private void OnTriggerEnter(Collider col)
    {
      if (col.gameObject.tag == "Player")
      {
        StopMovement();
        transform.LookAt(player.transform.position);
        isPlayerInAttackRange = true;
        // animator.SetBool("Attack", true);
        Invoke("ResumeMovement",2.0f);
      }
    }
    private void OnTriggerExit(Collider col)
    {
      if (col.gameObject.tag == "Player")
      {
        isPlayerInAttackRange = false;
        // animator.SetBool("Attack", false);
        ResumeMovement();
      }
    }
    public void StopMovement()
    {
      agent.isStopped = true; // was agent.Stop();
      agent.velocity = Vector3.zero;
    }
    public void ResumeMovement()
    {
      agent.isStopped = false; // was agent.Stop();
    }
    public void HitPlayer()
    {
      if (isPlayerInAttackRange) {
        GameManager.Health -= 10;
      }
    }
}