using System;
using UnityEngine;

public static class CrosshairController
{
    public static Action<bool> CrosshairAction;

    public static void ControllCroshair(bool active)
    {
        CrosshairAction?.Invoke(active);
    }
}
