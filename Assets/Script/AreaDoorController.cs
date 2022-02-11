using UnityEngine;

public class AreaDoorController : MonoBehaviour
{
    private static int numAreas = 3;

    private static bool[] doorUnlocked = { true, false, false };
    private static bool[] bossDeafeated = { false, false, false };
    [SerializeField]

    public static void SetBossDefeated(int bossIndex) 
    {
        if (IsValidIndex(bossIndex)) 
        {
            if (bossDeafeated[bossIndex] == false)
            {
                if (bossIndex != 2)
                {
                    doorUnlocked[bossIndex + 1] = true;
                }
            }
            bossDeafeated[bossIndex] = true;
        }

    }

    public static void ResetData()
    {
        bossDeafeated[0] = false;
        for (int i = 1; i < doorUnlocked.Length; i++)
        {
            doorUnlocked[i] = false;
            bossDeafeated[i] = false;
        }
    }

    public static bool IsDoorUnlocked(int doorIndex) 
    {
        return doorUnlocked[doorIndex];
    }

    public static bool GetBossDeafeated(int bossIndex)
    {
        return bossDeafeated[bossIndex];
    }

    private static bool IsValidIndex(int index)
    {
        if (index >= 0 && index < numAreas)
        {
            return true;
        }
        return false;
    }
}
