/***************************************************
 * Written By: Anton Huber
 * Purpose: Time display element of Ui
 * Data Created: 8/10
 * Last Modified: 8/10
 **************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemTime : BaseUi
{
    //*************************************************************************************
    // This will specifically be called to adjust the time. It should be based on percentages
    // rather then whole numbers. ie The problem will fail in 15 seconds. Bar is full at start
    // every frame(?) Ui will turn the current time passed into a percentage (time left / total time)
    // It will then check how many sprites it has active against the number above (sprites showing / total sprites)
    // If time passed is less then sprites shown it will remove 1 sprite. Only 1 though.
    // In cases where the time moves quickly what will happen is the timer will appear to move 'smoothly'
    // If their are enough sprite frames.
    //*************************************************************************************



   
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
