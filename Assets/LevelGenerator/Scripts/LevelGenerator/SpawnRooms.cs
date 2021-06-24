using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRooms : MonoBehaviour
{
    public LayerMask whatIsRoom;
    public LevelGeneration levelGen;

    void Update()
    {	
		Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, whatIsRoom);
		if(roomDetection == null && levelGen.stopGeneration == true)
		{
			// spawn random room
			Instantiate(RandomRoom(), transform.position, Quaternion.identity);
			//Destroy(gameObject);
			gameObject.SetActive(false);
		}
    }

	GameObject RandomRoom()
	{
		if (Random.Range(0,2) == 0)
		{
			int rand = Random.Range(0, levelGen.rooms.Length);
			return levelGen.rooms[rand];
		} else {
			int rand = Random.Range(0, levelGen.roomsClosed.Length);
			return levelGen.roomsClosed[rand];
		}
	}
}
