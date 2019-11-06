/***************************************************
 * Written By: Anton Huber
 * Made Pretty By: Ciaran Coppell
 * Purpose: Contains all the information about a single problem.
 * Data Created: 27/09
 * Last Modified: 09/10
 **************************************************/




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[System.Serializable]
public class ProblemList
{
    //*************************************************************************************
    // This class is just a bunch of getters and variables that a single problem will have.
    // Note that if you want a singe entity to be able to have several different problems
    // you will have to attach a second script to the same object.
    //*************************************************************************************

    [SerializeField] private string m_name;
    [SerializeField] private GameObject m_activeState; // Place the problems active state model here.

    [Header("Problem Is Fixed By")] // All Things Revolving Around the Problem and Solution
    [SerializeField] private E_Quality m_fixableProblem; // Will be able to set the list size and the problems from the unity editor.
    [Header("In")]
    [SerializeField] private int m_maxButtonPresses; // How many wacks it takes to fix the problem. Assaigned every activation. Per event.

    [Header("Animation Controller")]
    [SerializeField] private Animator m_animationController;

    [Header("Broken State")] // All states of problems

     


    
    [Header("Pending")]
 
    [SerializeField] private List<GameObject> m_breakProjectile; // What particle effects take place. 1 to 1 ration with animation and Fixable problem.  // Could probably be changed to game object that is turned on and off.
    [SerializeField] private float m_startDirection; // The particel will start at (x,y,z) and then move towards the problem
    //[SerializeField] private Vector3 m_particleStartDirection;
    [SerializeField] private float m_startDistance = 3f; // How far away will it spawn.
    [SerializeField] private float m_fallSpeed = 0.5f; // Controls the speed of the particle moving towards the problem.


    [Header("Active")]
    [SerializeField] private ProblemPhsyicsObject m_breakPhysics; // The physics object -- Will create a sepearate script for handling of physics.    [Header("Pending")]
    [SerializeField] private List<ParticleSystem> m_whileBrokenParticles; // What plays while the problem is now active.
    // While broken animation?
    
    

    [Header("Succesfully Fixed and failed to fix")]
    [SerializeField] private List<ParticleSystem> m_fixedParticles; // What plays when the problem is fixed.
    [SerializeField] private List<ParticleSystem> m_failParticles; // What plays when the problem fails.


    [Header("Sounds for pending, active, fail and success for this problem.")]
    [SerializeField] private List<SoundRequester> m_sounds;

    //*************************************************************************************
    // Getters to easily access the things contained within.
    //*************************************************************************************


    public E_Quality GetQuality()
    {
        return m_fixableProblem;
    }
    public int GetMaxButtonPresses()
    {
        return m_maxButtonPresses;
    }
    public GameObject GetActiveState()
    {
        return m_activeState;
    }
    public List<GameObject> GetBreakParticleEffect()
    {
        return m_breakProjectile;
    }
    public float GetParticleStartDirection()
    {
        return m_startDirection;
    }
    public float GetParticleStartDistance()
    {
        return m_startDistance;
    }
    public float GetParticleFallSpeed()
    {
        return m_fallSpeed;
    }
    public ProblemPhsyicsObject GetPhsyicsObject()
    {
        return m_breakPhysics;
    }
    public List<ParticleSystem> GetWhileBrokenParticle()
    {
        return m_whileBrokenParticles;
    }
    public List<ParticleSystem> GetFixedParticle()
    {
        return m_fixedParticles;
    }
    public List<ParticleSystem> GetFailedParticle()
    {
        return m_failParticles;
    }
    public Animator GetAnimationController()
    {
        return m_animationController;
    }
    public List<SoundRequester> GetSounds()
    {
        return m_sounds;
    }

}
//[CustomEditor(typeof(ProblemList))]
//public class ProblemListEditor : Editor
//{
//    public float labelWidth = 100.0f;
//    public override void OnInspectorGUI()
//    {
//        //draws a default inspector
//        base.DrawDefaultInspector();
//        //Custom Form for problems list UI

//        ProblemList problemList = (ProblemList) target;

//        //Label of GUI
//        GUILayout.Space(20.0f);
//        GUILayout.Label("Problem List Editor", EditorStyles.boldLabel);

//        //Label for Preferences
//        GUILayout.Space(10.0f);
//        GUILayout.Label("Problem Preference");

//        //UwU
//        GUILayout.BeginHorizontal();
//        GUILayout.Label("Fixable Problem:", GUILayout.Width(labelWidth));


//    }
//}



