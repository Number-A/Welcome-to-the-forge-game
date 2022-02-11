using System;
using UnityEngine;

[Serializable]
public class Encounter
{
    [SerializeField]
    private EncounterEnemy[] enemies;

    public EncounterData GetData() { return new EncounterData(enemies); }

    [Serializable]
    public struct EncounterEnemy
    {
        public GameObject enemyPrefab;
        public Vector2 positionInRoom;
    }
}
