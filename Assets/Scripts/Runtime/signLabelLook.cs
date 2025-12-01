using UnityEngine;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests_PlayMode")]

public class signLabelLook : MonoBehaviour
{
    //variables
    public Transform player; //main camera
    public float radius = 2f;
    private bool playerIsClose = false;
    private SphereCollider sphere;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        sphere = GetComponent<SphereCollider>();

        if (sphere == null)
        {
            sphere = gameObject.AddComponent<SphereCollider>();
        }
        
        sphere.isTrigger = true;
        sphere.radius = radius;

        //Assign camera if not set
        if (player == null && Camera.main != null)
            player = Camera.main.transform;
    }
    void Start()
    {
        if (player == null && Camera.main != null)
            player = Camera.main.transform;
    }
    public bool getPlayerIsClose()
    {
        return playerIsClose;
    }
    // Update is called once per frame
    void Update()
    {
 
        if (playerIsClose && player != null)
        {
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation*Quaternion.Euler(0, 180f, 0), Time.deltaTime * 3f);
        }
    }
    internal void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform == player);
        if (player != null & other.transform == player)
        {
            playerIsClose = true;
        }
    }
    internal void OnTriggerExit(Collider other)
    {
        if(player != null && other.transform == player)
        {
            playerIsClose = false;
        }
    }

<<<<<<< Updated upstream:Assets/Scripts/signLabelLook.cs
    //internal function to emulate update
=======
     //internal function to emulate update
>>>>>>> Stashed changes:Assets/Scripts/Runtime/signLabelLook.cs
    internal void rotateTowardPlayer()
    {
        if (playerIsClose && player != null)
        {
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation*Quaternion.Euler(0, 180f, 0), Time.deltaTime * 3f);
        }
    }
}
