using UnityEngine;
using System;
public static class NoiseSystem
{
    public static event Action<Vector3, float> OnNoise;

    public static void MakeNoise(Vector3 position, float range)
    {
        OnNoise?.Invoke(position, range);
    }
}
