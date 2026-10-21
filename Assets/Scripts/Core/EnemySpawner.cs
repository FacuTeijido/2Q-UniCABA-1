/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-15 

 

*/using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnGroup
    {
        public EnemyType type;
        public int count;
    }

    [SerializeField] SpawnGroup[] groups;
    [SerializeField] float radius = 15f;
    [SerializeField] bool spawnOnStart = true;

    void Start()
    {
        if (spawnOnStart) SpawnAll();
    }

    public void SpawnAll()
    {
        foreach (SpawnGroup g in groups)
        {
            for (int i = 0; i < g.count; i++)
                Spawn(g.type);
        }
    }

    public Enemy Spawn(EnemyType type)
    {
        Vector2 offset = Random.insideUnitCircle * radius;
        Vector3 position = transform.position + new Vector3(offset.x, 0f, offset.y);
        Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        return EnemyFactory.Instance.Create(type, position, rotation);
    }

    // Para probar el pool desde el Inspector (clic derecho en el título del componente, en Play)
    [ContextMenu("Test: spawnear un Melee")]
    void TestSpawnMelee() => Spawn(EnemyType.Melee);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}