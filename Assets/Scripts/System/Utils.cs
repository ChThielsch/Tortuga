using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Calculates the normalized direction vector pointing from the 'fromPosition' to the 'toPosition'.
    /// </summary>
    /// <param name="_fromPosition">The starting position.</param>
    /// <param name="_toPosition">The target position.</param>
    /// <returns>The normalized direction vector from 'fromPosition' to 'toPosition'.</returns>
    public static Vector3 GetDirectionBetweenPoints(Vector3 _fromPosition, Vector3 _toPosition)
    {
        return (_toPosition - _fromPosition).normalized;
    }

    /// <summary>
    /// Converts the given angle to the range of -180 to 180 degrees.
    /// </summary>
    /// <param name="_angle">The angle to be converted, in degrees.</param>
    /// <returns>The angle within the range of -180 to 180 degrees.</returns>
    public static float ConvertAngleToRange(float _angle)
    {
        if (_angle > 180f)
        {
            _angle -= 360f;
        }

        return _angle;
    }

    /// <summary>
    /// Performs a weighted interpolation between two rotations, blending from 'startRotation' to 'targetRotation'.
    /// </summary>
    /// <param name="_rotationA">The starting rotation.</param>
    /// <param name="_rotationB">The target rotation.</param>
    /// <param name="_weight">The weight of the interpolation (between 0 and 1).</param>
    /// <returns>The resulting rotation after the weighted interpolation.</returns>
    public static Quaternion WeightedRotation(Quaternion _rotationA, Quaternion _rotationB, float _weight)
    {
        // Ensure the weight is between 0 and 1
        _weight = Mathf.Clamp01(_weight);

        // Perform the weighted interpolation between the two rotations
        Quaternion newRotation = Quaternion.Lerp(_rotationA, _rotationB, _weight);

        // Return the resulting rotation
        return newRotation;
    }
}
