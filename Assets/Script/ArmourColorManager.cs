using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourColorManager : MonoBehaviour
{
    private static ArmourColorManager Instance;

    [SerializeField]
    private ArmourColor[] armourColors;

    private void Awake()
    {
        Instance = this;
    }

    public static Color GetArmourColor(Enums.ArmourType type)
    {
        foreach (ArmourColor ac in Instance.armourColors)
        {
            if (ac.type == type)
            {
                return ac.color;
            }
        }
        return new Color(1, 0, 1, 1);
    }

    [Serializable]
    public struct ArmourColor
    {
        public Enums.ArmourType type;
        public Color color;
    }
}
