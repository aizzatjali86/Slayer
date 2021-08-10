using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponController : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public Rigidbody rb;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 startPosition = transform.position;
        }
    }

    void SwingWeapon()
    {
        
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isMouse)
        {
            
        }
    }
}
