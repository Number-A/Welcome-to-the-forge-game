using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private AreaDoorController areaDoorController;

    private void OnDestroy()
    {
        if (this.gameObject.GetComponent<WerewolfBoss>() != null)
        {
            AreaDoorController.SetBossDefeated(1);
        }
    }

}