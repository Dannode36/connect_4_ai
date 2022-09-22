using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    public GameObject board;

    public GameObject yDisk;
    public GameObject rDisk;

    public float density = 0.7f;

    void Start()
    {
        StartCoroutine(SpawnBoards());
        StartCoroutine(SpawnDisks());
    }

    IEnumerator SpawnBoards()
    {
        while (true)
        {
            Quaternion randRot = new Quaternion
            {
                eulerAngles = new Vector3(Random.Range(-360, 360), -90, 90)
            };
            Instantiate(board, new Vector3(Random.Range(-100, 100), 100, 75), randRot);
            yield return new WaitForSeconds(density);
        }
    }
    IEnumerator SpawnDisks()
    {
        while (true)
        {
            Quaternion randRot = new Quaternion
            {
                eulerAngles = new Vector3(Random.Range(-360, 360), -90, 90)
            };
            Instantiate(yDisk, new Vector3(Random.Range(-100, 100), 100, 75), randRot);

            yield return new WaitForSeconds(density);

            randRot = new Quaternion
            {
                eulerAngles = new Vector3(Random.Range(-360, 360), -90, 90)
            };
            Instantiate(rDisk, new Vector3(Random.Range(-100, 100), 100, 75), randRot);
        }
    }
}
