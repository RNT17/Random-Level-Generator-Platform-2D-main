using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

public class LevelEvent : UnityEvent<Vector2> { }

public class LevelGeneration : MonoBehaviour
{
    public static LevelGeneration instance = null;
    public Transform[] startingPositions;
	public GameObject[] rooms; // index 0 --> closed, index 1 --> LR, index 2 --> LRB, index 3 --> LRT, index 4 --> LRBT
	public GameObject[] roomsClosed;
	public GameObject roomFirst;
	public GameObject exit;
	public float moveIncrement;
    public float minX;
    public float maxX;
    public float minY;
    public LayerMask whatIsRoom;
	public LevelEvent firtsRoomEvent = new LevelEvent();
	public LevelEvent lastRoomEvent = new LevelEvent();
    [HideInInspector] public bool stopGeneration;

    private int direction;
    private int downCounter;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            // Then destroy this.This enforces our singleton pattern, meaning there can only ever be one instance of a LevelGeneration.
            Destroy(gameObject);

        // Sets this to not be destroyed when reloading scene
        //DontDestroyOnLoad(gameObject);   
		
		firtsRoomEvent.AddListener(PlayerController.OnFirstRoomWasCreated);
		lastRoomEvent.AddListener(OnLastRoom);
    }

    void Start()
    {
        int randStartingPos = Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randStartingPos].position;

        Instantiate(roomFirst, transform.position, Quaternion.identity);
        direction = Random.Range(1, 6);
		firtsRoomEvent.Invoke(transform.position);
    }

    void Update()
    {
        // Only for tests purpose. To generate a new Random Level.
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (stopGeneration == false)
            Move();
    }

    private void Move()
    {
    	if (direction == 1 || direction == 2)
        { 
        	if(transform.position.x < maxX)
        	{
        		MoveRight();
        	} else {
        		direction = 5;
        	}
        } 
        else if (direction == 3 || direction == 4)
        { 
        	if(transform.position.x > minX)
        	{
        		MoveLeft();
        	} else {
        		direction = 5;
        	}
        } 
        else if (direction == 5)
        {
        	downCounter++;
         	if(transform.position.y > minY)
         	{
				 MoveDown();
            } else {
            	// STOP LEVEL GENERATION!!!	
            	stopGeneration = true;
				lastRoomEvent?.Invoke(transform.position);
            }
        }
    }

	void MoveRight()
	{
		//Debug.Log("Move Right");
		downCounter = 0;
		Vector2 pos = new Vector2(transform.position.x + moveIncrement, transform.position.y);
		transform.position = pos;

		int rand = Random.Range(0, rooms.Length);
		Instantiate(rooms[rand], transform.position, Quaternion.identity);

		direction = Random.Range(1, 6);
		if(direction == 3) 
			direction = 2;
		else if (direction == 4)
			direction = 5;
	}

	void MoveLeft()
	{
		//Debug.Log("Move Left");
		downCounter = 0;
		Vector2 pos = new Vector2(transform.position.x - moveIncrement, transform.position.y);
		transform.position = pos;

		int rand = Random.Range(0, rooms.Length);
		Instantiate(rooms[rand], transform.position, Quaternion.identity);

		direction = Random.Range(3, 6);
	}

	void MoveDown()
	{
		Debug.Log("Move Down");
		Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, whatIsRoom);
		RoomType roomType = roomDetection?.GetComponent<RoomType>();

		if(roomType?.type != 1 && roomType?.type != 3)
		{
			roomDetection.GetComponent<RoomType>()?.RoomDestruction();
			
			if(downCounter >= 2)
			{
				Instantiate(rooms[3], transform.position, Quaternion.identity);
			} else {

				int randBottomRoom = Random.Range(1, 4);
				if(randBottomRoom == 2)
					randBottomRoom = 1;

				Instantiate(rooms[randBottomRoom], transform.position, Quaternion.identity);
			}
		}

		Vector2 pos = new Vector2(transform.position.x, transform.position.y - moveIncrement);
		transform.position = pos;

		int rand = Random.Range(2, 4);
		Instantiate(rooms[rand], transform.position, Quaternion.identity);
	
		direction = Random.Range(1 ,6);
	}

	void OnLastRoom(Vector2 position)
	{
		Debug.Log("Last Room Was Created: " + position);
		Instantiate(exit, position, Quaternion.identity);
	}

}
