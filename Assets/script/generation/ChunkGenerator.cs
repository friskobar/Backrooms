using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform ppos;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject[] Rooms;

    [Header("Variables")]
    [SerializeField] const int chunkSize = 32;
    [SerializeField] float viewDist = 4f;
    [SerializeField] List<GameObject> chunks = new List<GameObject>();
    [SerializeField] public Vector2 curchunk;
    Vector2 lastChunk;


    void Start()
    {
        GenerateChunks();
    }

    void Update()
    {
        curchunk = FindPlayerCurChunk();
        GenerateChunks();
        lastChunk = curchunk;
    }

    void GenerateChunks()
    {
        var room = Rooms[Random.Range(0, Rooms.Length)];
        for (int x = (int)curchunk.x - (int)viewDist/2; x < curchunk.x + viewDist/2; x++)
        {
            for (int y = (int)curchunk.y - (int)viewDist/2; y < curchunk.y + viewDist/2; y++)
            {
                if(!Physics.CheckBox(new Vector3(chunkSize*x + .6f , 0, chunkSize*y - .6f + chunkSize), new Vector3(.5f, .01f, .5f)))
                {
                    var chunk = Instantiate(room, new Vector3(chunkSize * x , 0, chunkSize * y + chunkSize), new Quaternion(0, 0, 0, 0));
                    chunks.Add(chunk);
                }
                room = Rooms[Random.Range(0, Rooms.Length)];
            }
        }
    }

    Vector2 FindPlayerCurChunk()
    {
        Vector2 curc = Vector2.zero;
        if(ppos.position.x < 0) curc.x = Mathf.Ceil(ppos.position.x/chunkSize);
        else if(ppos.position.x > 0) curc.x = Mathf.Floor(ppos.position.x / chunkSize);
        else curc.x = 0;
        if(ppos.position.z < 0) curc.y = Mathf.Ceil(ppos.position.z/chunkSize);
        else if(ppos.position.z > 0) curc.y = Mathf.Floor(ppos.position.z / chunkSize);
        else curc.y = 0;
        return curc;
    }
}
