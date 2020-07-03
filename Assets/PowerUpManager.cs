using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PowerUpManager : MonoBehaviour
{
    public PathCreator pathCreator;
    public GameObject zone;

    // Start is called before the first frame update
    void Start()
    {
        float distance = Random.Range(0, pathCreator.path.length);
        GameObject new_zone = Instantiate(zone, pathCreator.path.GetPointAtDistance(distance), pathCreator.path.GetRotationAtDistance(distance));
        new_zone.name = "New Zone";
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator usePowerup(int PowerUpType)
    {
        switch (PowerUpType)
        {
            //Booster
            case 0:
                GameObject.FindObjectOfType<Player>().SetSpeed(20.0f);
                yield return new WaitForSeconds(2);
                GameObject.FindObjectOfType<Player>().SetSpeed(10.0f);
                break;

            //Shockwave
            /* braucht noch Enemy
            case 1:
                if (Vector3.Distance(GameObject.FindObjectOfType<Player>().transform.position, GameObject.FindObjectOfType<Enemy>().transform.position) < 2)
                {
                    GameObject.FindObjectOfType<Enemy>().SetSpeed(0.0f);
                    yield return new WaitForSeconds(5);
                    GameObject.FindObjectOfType<Enemy>().SetSpeed(10.0f);
                }
                
                break;

            //Homing Shot
            case 2: 
                if (Vector3.Distance(GameObject.FindObjectOfType<Player>().transform.position, GameObject.FindObjectOfType<Enemy>().transform.position) < 5)
                {
                    GameObject.FindObjectOfType<Enemy>().SetSpeed(0.0f);
                    yield return new WaitForSeconds(3);
                    GameObject.FindObjectOfType<Enemy>().SetSpeed(10.0f);
                }
                break;
            */
        }
    }
}
