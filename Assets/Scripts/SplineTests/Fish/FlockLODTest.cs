using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudFine.FlockBox;

[RequireComponent(typeof(FlockBox))]
public class FlockLODTest : MonoBehaviour
{
    private FlockBox flockBox;
    public Transform player;
    public float radius;

    HashSet<int> list;

    private void Awake()
    {
        flockBox = GetComponent<FlockBox>();
        list = new HashSet<int>();
    }
    private void FixedUpdate()
    {
        //for (int x = 0; x < (flockBox.dimensions_x > 0 ? flockBox.dimensions_x : 1); x++)
        //{
        //    for (int y = 0; y < (flockBox.dimensions_y > 0 ? flockBox.dimensions_y : 1); y++)
        //    {
        //        for (int z = 0; z < (flockBox.dimensions_z > 0 ? flockBox.dimensions_z : 1); z++)
        //        {
        //            Vector3 corner = new Vector3(x, y, z) * flockBox.cellSize;
        //            int cell = flockBox.WorldPositionToHash(corner);
        //            list.Add(cell);
        //        }
        //    }
        //}

        HashSet<int> newlist = new HashSet<int>();

        flockBox.GetCellsOverlappingSphere(player.position, radius, newlist);

        foreach (int l in list)
        {
            if (newlist.Contains(l)) newlist.Remove(l);
            else
            {
                HashSet<Agent> agents= flockBox.cellToAgents[l];
                foreach (Agent a in agents)
                    a.gameObject.SetActive(false);
            }
        }
        foreach (int l in newlist)
        {
            HashSet<Agent> agents = flockBox.cellToAgents[l];
            foreach (Agent a in agents)
                a.gameObject.SetActive(true);
        }

    }
}
