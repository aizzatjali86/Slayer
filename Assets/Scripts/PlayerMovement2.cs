using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement2 : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float animSpeedMult;
    public float rotSpeed = 100.0f;
    public Rigidbody rb;
    public float playerRotation;
    public float playerRotationBefore;
    public float rotChange;
    public float rotChangeBef;
    public float aimRotation;
    public float H2;
    public float V2;
    public float runRotation;
    public float diff;

    private Vector3 movement;
    private Vector3 controllerAim;
    public Vector3 targetDir;
    public Vector3 playerAim = new Vector3(0, 0, 0.1f);
    public Vector3 mouseAim;
    public bool slowMode;
    public bool lowAction;
    public bool highAction;
    public bool specialAction;
    public bool dashstep;
    public bool dashStepBreak;
    private float lastSpeed;
    public int controller; //0 for mouse, 1 for joystick, 2 for mobile

    Animator animator;

    public float DoubleClickInterval = 0.5f;
    public UnityEvent OnDoubleClick;

    float secondClickTimeout = -1;

    void Start()
    {
        Application.targetFrameRate = 30;
        characterController = GetComponent<CharacterController>();
        rb = this.GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.SetFloat("DashMult", animSpeedMult);
    }

    void Update()
    {
        playerAim = transform.forward;

        //Control for Run (fast mode)
        //TODO: Add control scheme for other type
        movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        movement = Vector3.ClampMagnitude(movement, 1);

        //Control for Aim (when in slow mode)
        H2 = Input.GetAxis("H2");
        V2 = Input.GetAxis("V2");
        controllerAim = new Vector3(Input.GetAxis("H2"), 0.0f, Input.GetAxis("V2"));
        controllerAim = Vector3.ClampMagnitude(controllerAim, 1);

        //Control for slow mode (shift for keyboard, L2 for controller)
        //TODO: Add control scheme for other type
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Slow"))
        {
            slowMode = true;
        }
        else
        {
            slowMode = false;
        }

        //Control for action (right button for mouse, R2 for controller)
        //if in slow mode, button can be hold, else button is a trigger
        //TODO: Add control scheme for other type
        if (controller == 0)
        {
            if (slowMode)
            {
                lowAction = Input.GetMouseButton(1);
                highAction = Input.GetMouseButton(0);
            }
            else
            {
                lowAction = Input.GetMouseButtonDown(1);
                highAction = Input.GetMouseButtonDown(0);
            }
        }
        else
        {
            lowAction = Input.GetButton("Low") && controllerAim.magnitude > 0.3f;
            highAction = !Input.GetButton("Low") && controllerAim.magnitude > 0.3f;

            //Double click event
            specialAction = false;
            if (Input.GetButtonDown("Low"))
            {
                if (secondClickTimeout < 0)
                {
                    // This is the first click, calculate the timeout
                    secondClickTimeout = Time.time + DoubleClickInterval;
                }
                else
                {
                    // This is the second click, is it within the interval 
                    if (Time.time < secondClickTimeout)
                    {
                        specialAction = true;

                        // Invoke the event
                        OnDoubleClick.Invoke();

                        // Reset the timeout
                        secondClickTimeout = -1;
                    }
                }

            }

            // If we wait too long for a second click, just cancel the double click
            if (secondClickTimeout > 0 && Time.time >= secondClickTimeout)
            {
                secondClickTimeout = -1;
            }
        }
    }

    private void FixedUpdate()
    {
        // Animator parameter set
        animator.SetFloat("DashMult", animSpeedMult);
        animator.SetBool("Slow", slowMode);
        if (!dashstep)
        {
            animator.SetFloat("Move", movement.magnitude);
            animator.SetBool("LowAction", lowAction);
            animator.SetBool("HighAction", highAction);
        }
        animator.SetBool("SpecialAction", specialAction);

        // Get target direction from aim control
        if (controller == 0)
        {
            targetDir = MouseAim() - transform.position;
        }
        else
        {
            if (controllerAim.magnitude > 0.3f)
            {
                targetDir = ControllerAim();
            }
        }

        //When in slow mode, aim controlled primarily by action
        if (slowMode)
        {
            Vector2 targetDir2 = new Vector2(movement.normalized.x, movement.normalized.z);
            Vector2 playerAim2 = new Vector2(playerAim.x, playerAim.z);
            float angle = Vector2.SignedAngle(targetDir2, playerAim2);
            animator.SetFloat("StrafeLR", Mathf.Sin(angle * Mathf.Deg2Rad));
            animator.SetFloat("StrafeUD", movement.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad));
            // Move and rotate character
            if (!dashstep)
            {
                if (movement.magnitude > 0)
                {
                    if (!dashStepBreak)
                    {
                        MoveCharacter(movement, speed / 1.5f);
                    }
                    else
                    {
                        MoveCharacter(movement, speed / 3);
                    }
                }
                else
                {
                    // Set animation when rotating without moving
                    animator.SetFloat("StrafeLR", Mathf.Sin((aimRotation - transform.eulerAngles.y) * 2 * Mathf.Deg2Rad));
                }

                if (lowAction)
                {
                    Quaternion _targetR = Quaternion.Euler(0, aimRotation, 0);

                    if (_targetR != transform.rotation)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, _targetR, .2f);
                    }
                }
                else if (highAction)
                {
                    Quaternion _targetR = Quaternion.Euler(0, aimRotation, 0);

                    if (_targetR != transform.rotation)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, _targetR, .2f);
                    }
                }
                if (specialAction)
                {
                    dashStepBreak = true;
                    StartCoroutine(Lower());
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Lower"))
            {
                rb.AddForce(playerAim.normalized * speed);
            }
            
        }
        else
        {
            if (!dashstep)
            {
                if (movement.magnitude > 0)
                {
                    MoveCharacter(movement, speed);

                }           
                if (lowAction && controllerAim.magnitude > 0.3f)
                {
                    if (!dashStepBreak)
                    {
                        StartCoroutine(SideStep(targetDir));
                    }
                }
                else if (highAction && controllerAim.magnitude > 0.3f)
                {
                    //dashStepBreak = true;
                    Dash(targetDir);
                }

                if (specialAction)
                {
                    dashStepBreak = true;
                    StartCoroutine(Upper());
                }
            }

            // Movement for dash and step
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Step"))
            {
                rb.AddForce(targetDir.normalized * speed * 3);
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Upper"))
            {
                rb.AddForce(playerAim.normalized * speed);
            }
        }
    }

    void MoveCharacter(Vector3 direction, float speed)
    {
        //rb.velocity = (transform.position + (direction * speed * Time.deltaTime));
        rb.AddForce(direction * speed);
        //rb.velocity = direction * speed;

        if (movement.z >= 0 && movement.x >= 0)
        {
            runRotation = Mathf.Rad2Deg * Mathf.Atan((movement.x) / (movement.z));
        }
        else if (movement.z >= 0 && movement.x < 0)
        {
            runRotation = 360 + Mathf.Rad2Deg * Mathf.Atan((movement.x) / (movement.z));
        }
        else
        {
            runRotation = 180 + Mathf.Rad2Deg * Mathf.Atan((movement.x) / (movement.z));
        }

        Quaternion _targetR = Quaternion.Euler(0, runRotation, 0);

        if (_targetR != transform.rotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetR, .2f);
        }
    }

    Vector3 MouseAim()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        Vector3 target = Vector3.zero;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            target = hit.point;

            if (target.z - transform.position.z >= 0 && target.x - transform.position.x >= 0)
            {
                aimRotation = Mathf.Rad2Deg * Mathf.Atan((target.x - transform.position.x) / (target.z - transform.position.z));
            }
            else if (target.z - transform.position.z >= 0 && target.x - transform.position.x < 0)
            {
                aimRotation = 360 + Mathf.Rad2Deg * Mathf.Atan((target.x - transform.position.x) / (target.z - transform.position.z));
            }
            else
            {
                aimRotation = 180 + Mathf.Rad2Deg * Mathf.Atan((target.x - transform.position.x) / (target.z - transform.position.z));
            }
        }

        return target;
    }

    Vector3 ControllerAim()
    {
        Vector3 target = controllerAim;

        if (target.z >= 0 && target.x >= 0)
        {
            aimRotation = Mathf.Rad2Deg * Mathf.Atan((target.x) / (target.z));
        }
        else if (target.z >= 0 && target.x < 0)
        {
            aimRotation = 360 + Mathf.Rad2Deg * Mathf.Atan((target.x) / (target.z));
        }
        else
        {
            aimRotation = 180 + Mathf.Rad2Deg * Mathf.Atan((target.x) / (target.z));
        }

        return target;
    }


    IEnumerator SideStep(Vector3 direction)
    {
        dashstep = true;
        mouseAim = direction.normalized;
        Vector2 targetDir2 = new Vector2(direction.normalized.x, direction.normalized.z);
        Vector2 playerAim2 = new Vector2(playerAim.x, playerAim.z);
        float angle = Vector2.SignedAngle(targetDir2, playerAim2);
        //print(angle);
        animator.SetFloat("StepLR", direction.normalized.magnitude * Mathf.Sin(angle * Mathf.Deg2Rad));
        animator.SetFloat("StepUD", direction.normalized.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad));
        yield return new WaitForSeconds(0.6f * 1/animSpeedMult);
        dashstep = false;
        dashStepBreak = true;
        animator.SetBool("dashBreak", true);
        yield return new WaitForSeconds(1f);
        dashStepBreak = false;
        animator.SetBool("dashBreak", false);
    }

    void Dash(Vector3 direction)
    {
        lastSpeed = movement.normalized.magnitude;
        //rb.AddForce(direction.normalized * speed * 15);
        mouseAim = direction.normalized;
        Vector2 targetDir2 = new Vector2(direction.normalized.x, direction.normalized.z);
        Vector2 playerAim2 = new Vector2(playerAim.x, playerAim.z);
        float angle = Vector2.SignedAngle(targetDir2, playerAim2);
        //print(angle);
        animator.SetFloat("DashLR", direction.normalized.magnitude * Mathf.Sin(angle * Mathf.Deg2Rad));
        animator.SetFloat("DashUD", direction.normalized.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad));
        //yield return new WaitForSeconds(0.6f * 1/animSpeedMult);
        
    }

    IEnumerator Upper()
    {
        dashstep = true;
        lastSpeed = movement.normalized.magnitude;
        yield return new WaitForSeconds(0.6f * 1 / animSpeedMult);
        dashstep = false;
        dashStepBreak = true;
        yield return new WaitForSeconds(0.5f);
        dashStepBreak = false;
    }

    IEnumerator Lower()
    {
        dashstep = true;
        lastSpeed = movement.normalized.magnitude;
        yield return new WaitForSeconds(0.6f * 1 / animSpeedMult);
        dashstep = false;
        dashStepBreak = true;
        yield return new WaitForSeconds(0.5f);
        dashStepBreak = false;
    }


}
