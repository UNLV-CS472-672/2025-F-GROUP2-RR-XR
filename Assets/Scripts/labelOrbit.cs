using UnityEngine;

public class labelOrbit : MonoBehaviour
{
    public Transform center;
    public float radius = 1f;
    public float speed = 30f;
    private float angle = 0f;
    private Vector3 rotationAxis = Vector3.up;
    private Vector3 offset;
    private signLabelLook lookScript;
    void Start()
    {
        offset = new Vector3(0, 0.2f, radius); 
        transform.position = center.position + offset;
    }
    // Update is called once per frame
    void Update()
    {
        //  when player is close enough, run the signLabelLook script and 
        //  ignore this script.
        if (lookScript != null && lookScript.getPlayerIsClose())
            return;
        
        
        if (center == null)
            return;
        //To make the object rotate in X, Y, Z.    
        transform.Rotate(rotationAxis * speed * Time.deltaTime, Space.Self);
        //TO ORBIT AROUND CERTAIN OBJECT
        //transform.RotateAround(center.position, Vector3.up, speed * Time.deltaTime);

        //Vector3 direction = transform.position - center.position;
        //if (direction != Vector3.zero)
        //  transform.rotation = Quaternion.LookRotation(direction);

        //Vector3 euler = transform.eulerAngles;
        //euler.x = 0f;
        //transform.eulerAngles = euler;
    }
}
