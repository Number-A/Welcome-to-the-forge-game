using System;
using UnityEngine;

[Serializable]
public class EncounterData
{
    private EnemyData[] enemyData;

    public EncounterData(Encounter.EncounterEnemy[] enemies) 
    {
        enemyData = new EnemyData[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyData[i] = new EnemyData(enemies[i]);
        }
    }

    public EnemyData[] GetEnemyData() { return enemyData; }

    [Serializable]
    public class EnemyData
    {
        public string prefabName;
        public float health;
        public float[] position;
        public EnemyData(Encounter.EncounterEnemy enemy)
        {
            prefabName = enemy.enemyPrefab.name;
            position = new float[2];
            position[0] = enemy.positionInRoom.x;
            position[1] = enemy.positionInRoom.y;
            health = enemy.enemyPrefab.GetComponent<Entity>().GetMaxHealth();
        }

        public Vector2 GetPosition() { return new Vector2(position[0], position[1]); }

        public void UpdateHealth(float newHealth)
        {
            health = newHealth;
        }
    }
}
