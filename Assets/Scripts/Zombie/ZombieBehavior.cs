using UnityEngine;
using UnityEngine.AI;

namespace Zombie
{
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
    private PlayerHealthManager healthManager;
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
      animator = GetComponent<Animator>();
      player = GameObject.FindGameObjectWithTag("Player");
      healthManager = player.GetComponent<PlayerHealthManager>();
      GotoNextPoint();
    }

    
    // Update is called once per frame
    void Update()
    {
      // animator.SetFloat("Speed",agent.speed);
      //if(SeePlayer())
      {
        // Debug.Log(agent.transform.position);
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

    private void OnCollisionEnter(Collision col)
    {
      if (col.collider.gameObject.CompareTag("Player"))
      {
        Debug.Log("Ouch!");
        StopMovement();
        // HitPlayer();
        transform.LookAt(player.transform.position);
        isPlayerInAttackRange = true;
        // animator.SetBool("Attack", true);
        Invoke("ResumeMovement",2.0f);
        InvokeRepeating("HitPlayer", 0f, 1f);
      }
    }
    private void OnCollisionExit(Collision col)
    {
      if (col.collider.gameObject.CompareTag("Player"))
      {
        isPlayerInAttackRange = false;
        CancelInvoke("HitPlayer");
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
      animator.SetFloat("MoveSpeed", agent.speed);
      agent.isStopped = false; // was agent.Stop();
    }
    public void HitPlayer()
    {
      healthManager.DecrementHealth();

      // if (isPlayerInAttackRange) {
      //   Debug.Log("Player Hit");
      //   GameManager.Health -= 10;
      // }
    }
  }
}