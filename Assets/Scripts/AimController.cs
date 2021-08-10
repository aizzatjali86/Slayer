using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public Rigidbody rb;
    public float aimRotation;

    private Vector3 movement;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            Vector3 target = hit.point;

            if (target.z - transform.position.z >= 0)
            {
                aimRotation = Mathf.Rad2Deg * Mathf.Atan((target.x - transform.position.x) / (target.z - transform.position.z));
            }
            else
            {
                aimRotation = 180 + Mathf.Rad2Deg * Mathf.Atan((target.x - transform.position.x) / (target.z - transform.position.z));
            }
        }
    }

    private void FixedUpdate()
    {
        //AimCharacter(aimRotation);
    }

    void AimCharacter(float aimrotation)
    {
        Quaternion rotation = Quaternion.Euler(0, aimrotation, 0);

        rb.MoveRotation(rotation);
    }
}
