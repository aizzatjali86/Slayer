using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Shader shader1;
    public Shader shader2;
    Renderer rend;
    Rigidbody rb;
    bool startDissolve;

    public float time;
    
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startDissolve)
        {
            rb.constraints = RigidbodyConstraints.None;
            time += Time.deltaTime;
            rend.material.SetFloat("Vector1_9B1AD561", time);
        }
        if (rend.material.GetFloat("Vector1_1B625DDF") * time > 1)
        {
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "weapon")
        {
            startDissolve = true;
        }
    }
}
