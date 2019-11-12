/***************************************************
 * Written By: Eric Brkic
 * Purpose: Add basic utility functions used commonly
 * Data Created: 24th Oct, 2019
 * Last Modified: 24th Oct, 2019
 **************************************************/
using UnityEngine;

public static class Utility
{
	public static void ResetTransform(this Transform a_transform)
	{
		// This makes my caramello koala extra tasty
		a_transform.localPosition = Vector3.zero;
		a_transform.localEulerAngles = Vector3.zero;
		a_transform.localScale = Vector3.one;
	}
}
