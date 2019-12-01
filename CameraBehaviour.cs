using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform target;
    public float high;
    public float zDistance;
    public Animator anim;


    public void Shake()
    {
        anim.SetTrigger("Shake");
    }

    void FixedUpdate()
    {
        Vector3 destination;

        destination = new Vector3(target.position.x, high, target.position.z - zDistance);

        transform.position = Vector3.Lerp(transform.position, destination, 5 * Time.fixedDeltaTime);
    }
}
