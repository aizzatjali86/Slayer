using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    Animator animator;
    public GameObject player;
    PlayerMovement2 playerControl;
    public GameObject attackObject;
    public ParticleSystem particleMain;
    public ParticleSystem particleTrail;
    public ParticleSystem particleSpeck;

    public ParticleSystem particleTrailB;
    public ParticleSystem particleSpeckB;

    // Start is called before the first frame update
    void Start()
    {
        playerControl = player.GetComponent<PlayerMovement2>();
        animator = player.GetComponent<Animator>();
        //particleMain.Play();
        //particleTrail.Play();
        //particleSpeck.Play();
        //particleTrailB.Play();
        //particleSpeckB.Play();

        StopEmit();
        attackObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("StrongAttack") || 
            animator.GetCurrentAnimatorStateInfo(0).IsName("Upper") || 
            animator.GetCurrentAnimatorStateInfo(0).IsName("Lower"))
        {
            attackObject.SetActive(true);
            EmitParticle();
        }
        else
        {
            StopEmit();
            attackObject.SetActive(false);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Step"))
        {
            EmitParticleB();
        }
        else
        {
            StopEmitB();
        }
    }

    void EmitParticle()
    {
        ParticleSystem.EmissionModule emMain = particleMain.emission;
        emMain.enabled = true;
        ParticleSystem.EmissionModule emTrail = particleTrail.emission;
        emTrail.enabled = true;
        ParticleSystem.EmissionModule emSpeck = particleSpeck.emission;
        emSpeck.enabled = true;
    }

    void StopEmit()
    {
        ParticleSystem.EmissionModule emMain = particleMain.emission;
        emMain.enabled = false;
        ParticleSystem.EmissionModule emTrail = particleTrail.emission;
        emTrail.enabled = false;
        ParticleSystem.EmissionModule emSpeck = particleSpeck.emission;
        emSpeck.enabled = false;
    }

    void EmitParticleB()
    {
        ParticleSystem.EmissionModule emTrail = particleTrailB.emission;
        emTrail.enabled = true;
        ParticleSystem.EmissionModule emSpeck = particleSpeckB.emission;
        emSpeck.enabled = true;
    }

    void StopEmitB()
    {
        ParticleSystem.EmissionModule emTrail = particleTrailB.emission;
        emTrail.enabled = false;
        ParticleSystem.EmissionModule emSpeck = particleSpeckB.emission;
        emSpeck.enabled = false;
    }
}
