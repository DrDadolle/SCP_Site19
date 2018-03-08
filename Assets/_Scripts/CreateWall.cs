using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateWall : MonoBehaviour {

	//The instance for access purposes
	public static CreateWall Instance;

	//Indicate if currently build a wall of not
	bool dragAndDropping;

	//Store the starting position of the drag and drop
	private Vector3 startPosition;

	private Vector3 it;
	private Vector3 lastIteration;

	/*keep in memory which axis we were building on;
	 * 0 = none
	 * 1 = X
	 * 2 = Z
	 */
	private int buildingAxis = 0;

	//Show the starting point of the wall
	private GameObject startPole;

	//Show the ending point of the wall
	private GameObject endPole;

	// Array of provisiorary wall's GO
	private List<GameObject> tempGOList;

	//Public game objects models
	public GameObject polePrefab;
	public GameObject wallPrefab;

	// Materials
	private Object[] tmp_materials;
	protected static string PATH_TO_WALL_MATERIALS = "Materials_Wall";
	public static Material default_material;
	public static Material ghostly_blue;
	public static Material ghostly_red;

	//Value of the layer
	public static int VALUE_OF_WALLS_LAYER = 9;


	//On Awake
	void Awake(){
		Instance = this;
		//Create a temp WallGameObject list
		tempGOList = new List<GameObject> ();

		//Loading the 3 differents materials for walls
		loadingWallMaterials ();
	}

	//Loading of Wall Materials Resources
	private void loadingWallMaterials()
	{
		tmp_materials = Resources.LoadAll (PATH_TO_WALL_MATERIALS, typeof(Material));
		foreach (var t in tmp_materials)
		{
			if (t.name == "Walls"){
				default_material = (Material)t;
			} else if (t.name == "Wall_Ghost_Blue") {
				ghostly_blue = (Material)t;
			} else if (t.name == "Wall_Ghost_Red"){
				ghostly_red = (Material)t;
			}
		}

		if (default_material == null || ghostly_blue == null || ghostly_red == null)
			Debug.LogError ("Loading of Wall Materials failed");
	}


	//Main method to create walls
	public void creationOfWalls(ShowMousePosition pointer){

		//When clicking for the first time, Sets the starting pole and all the variables
		if (Input.GetMouseButtonDown (0)) {
			Instance.startWall(pointer);
		}else if(Input.GetMouseButtonUp(0) && Instance.dragAndDropping){
			Instance.setWall();
		}

		if(Instance.dragAndDropping){
			//If right click is pressed to cancel
			if (Input.GetMouseButtonDown (1)) {
				// Remove the starting Pole
				Destroy (Instance.startPole);

				// Remove all walls
				clearAndDestroyAllGO ();

				// undrag
				Instance.dragAndDropping = false;

			} else {
				Instance.updateWall (pointer);
			}
		}
	}


	// Change the properties of the materials and physics for a specific piece of wall
	public static void changePropertiesOfWall(GameObject wall, Material mat, bool setIsTrigger)
	{
		// Set the materials
		wall.GetComponent<Renderer> ().material = mat;

		//Set the trigger
		Collider tmp_collider = wall.GetComponent<Collider> ();
		tmp_collider.isTrigger = setIsTrigger;

		//If we are removing the trigger, it means we are finally building a wall for real !
		if (setIsTrigger == false) {
			//Changing its layer from GhostWalls (10) to Walls (9)
			wall.layer = VALUE_OF_WALLS_LAYER;
			Rigidbody tmp_rigid = wall.AddComponent<Rigidbody> ();
			tmp_rigid.isKinematic = true;
		}

	}





	//Initiate the wall segment creation
	void startWall(ShowMousePosition pointer){

		// Set the starting Position
		Vector3 startPos = pointer.getWorldPoint();
		startPosition = pointer.snapPosition (startPos);
		dragAndDropping = true;
		lastIteration = startPosition;

		// Show the starting position with a gameObject
		startPole = Instantiate (polePrefab, startPosition, Quaternion.identity);
		startPole.transform.position = new Vector3 (startPosition.x, startPosition.y, startPosition.z);
	}


	//Effectively create the wall
	void setWall(){
		// Show the finishing position with a gameObject
		endPole = Instantiate (polePrefab, lastIteration, Quaternion.identity);
		endPole.transform.position = new Vector3 (lastIteration.x, lastIteration.y, lastIteration.z);

		if (lastIteration.Equals (startPosition)) {
			Destroy (Instance.startPole);
			Destroy (Instance.endPole);
		}
			
		// Change the material of walls
		foreach (var w in tempGOList) {
			destroyCollidingWalls (w);
			changePropertiesOfWall (w, default_material, false);
		}

		//Clear the previous arraylist
		tempGOList.Clear();

		//Finish Drag And drop
		dragAndDropping = false;
	}


	private void destroyCollidingWalls(GameObject wall){
		//The wall is still red because of a collision
		if (wall.GetComponent<Renderer> ().sharedMaterial.Equals(ghostly_red)) {
			Destroy (wall);
		}
	}



	//Show the future position of the wall
	void updateWall(ShowMousePosition pointer){

		//Get the current mouse position 
		Vector3 currentPosition = pointer.getWorldPoint ();
		currentPosition = pointer.snapPosition (currentPosition);
		currentPosition = new Vector3 (currentPosition.x, currentPosition.y, currentPosition.z);

		if (!currentPosition.Equals(lastIteration)) {
			recursiveWallBuilder(startPosition, currentPosition);
		}
	}




	/**
	 * Create recursively walls between a start position and an end position
	 * StartPos and finishPos are both snapped already
	 * 
	 */
	void recursiveWallBuilder(Vector3 startPos, Vector3 finishPos)
	{
		int _max_x = Mathf.RoundToInt (finishPos.x - startPos.x);
		int max_x = Mathf.Abs (_max_x); 
		int _max_z = Mathf.RoundToInt (finishPos.z - startPos.z);
		int max_z = Mathf.Abs (_max_z); 

			//if it is an x-axis building
			//Default case
		if (max_x >= max_z) {
			if (max_x > tempGOList.Count) {
				lastIteration = startPos;
				for (int i = 0; i < max_x; i++) {
					buildingAxis = 1;
					Instance.createWallSegment (i + 1, _max_x > 0);
				}
			} else {
				destroyObsoleteGO (_max_x);
		
			}
		}
		//else if its an z-axis building
		else {
			if (max_z > tempGOList.Count) {
				lastIteration = startPos;
				for (int i = 0; i < max_z; i++) {
					buildingAxis = 2;
					Instance.createWallSegment (i + 1, _max_z > 0);
				}
			} else {
				destroyObsoleteGO (_max_z);
			}
		}
	}

	//==========================================
	//Create effectively a Segment of the Wall
	void createWallSegment(int iteration, bool isPositive){

		if (!isPositive)
			iteration = -iteration;

		//Check that all the object are build on the same axis (x or z one)
		checkAllObjectOnXorZAxis (iteration);

		//Check that all the object are build on the same side (negative or positive one)
		checkAllObjectPositiveOrNegativeAxis ();

		// checks if it is a new item
		if (Mathf.Abs(iteration) > tempGOList.Count) {

			//Get the middle between the 2 last points
			Vector3 middle = Vector3.Lerp (lastIteration, it, 0.5f);

			//Create the wall
			GameObject newWall = Instantiate (wallPrefab, middle, Quaternion.identity);

			//Add it to the arraylist
			tempGOList.Add (newWall);

			// Rotate the model so the forward axis points in the right direction
			Vector3 YRotation = new Vector3 (0, 90, 0);
			newWall.transform.LookAt (startPole.transform);
			newWall.transform.Rotate (YRotation);

			//Destroy previous endPole
			/*
			if (endPole == null) {
				endPole = Instantiate (polePrefab, it, Quaternion.identity);
			} else if (!endPole.transform.position.Equals (it)){
				endPole.transform.position = new Vector3 (it.x, it.y, it.z);
			}*/

		}
		//Update the last iteration
		lastIteration = it;
	}
		
	//Check that all the object are build on the same axis (x or z one)
	void checkAllObjectOnXorZAxis(int iteration){
		
		//Create the it vector indicating the new point of the wall
		if (buildingAxis == 1) {
			it = new Vector3 (startPosition.x + iteration, startPosition.y, startPosition.z);
		} else {
			it = new Vector3 (startPosition.x, startPosition.y, startPosition.z + iteration);
		}
			
		// Check if we are still building on the right axis
		if (tempGOList.Count > 0) {
			if (buildingAxis == 1 && tempGOList [0].transform.position.z != startPosition.z) {
				buildingAxis = 2;
				clearAndDestroyAllGO ();
			} else if (buildingAxis == 2 && tempGOList [0].transform.position.x != startPosition.x) {
				buildingAxis = 1;
				clearAndDestroyAllGO ();
			}
		}
	}

	//Check that all the object are build on the same side (negative or positive one)
	void checkAllObjectPositiveOrNegativeAxis(){
		if (tempGOList.Count > 0) {
			if (buildingAxis == 1) {
				if (it.x > startPosition.x && tempGOList [0].transform.position.x < startPosition.x)
					clearAndDestroyAllGO ();
				else if (it.x < startPosition.x && tempGOList [0].transform.position.x > startPosition.x)
					clearAndDestroyAllGO ();
			} else {
				if (it.z > startPosition.z && tempGOList [0].transform.position.z < startPosition.z)
					clearAndDestroyAllGO ();
				else if (it.z < startPosition.z && tempGOList [0].transform.position.z > startPosition.z)
					clearAndDestroyAllGO ();
			}
		}
	}
		
	//Utility Method to empty the gameObject Array
	void clearAndDestroyAllGO()
	{
		tempGOList.ForEach(delegate(GameObject obj) {
			Destroy(obj);
		});
		tempGOList.Clear ();
	}


	//destroy
	void destroyObsoleteGO(int pseudoMax){
		int pseudoMaxAbs = Mathf.Abs (pseudoMax);

		for (int i = pseudoMaxAbs  ; i < tempGOList.Count; i++) {
			if (pseudoMax > 0) {
				if (buildingAxis == 1) {
					lastIteration.x -= 1;
				} else {
					lastIteration.z -= 1;
				}
			} else if (pseudoMax < 0) {
				if (buildingAxis == 1) {
					lastIteration.x += 1;
				} else {
					lastIteration.z += 1;
				}
			} else {
				if (buildingAxis == 1) {
					lastIteration.x = startPosition.x;
				} else {
					lastIteration.z = startPosition.z;
				}
			}

			/*if (endPole != null) 
				endPole.transform.position = lastIteration;
			*/

			Destroy (tempGOList [i]);
			tempGOList.RemoveAt (i);
		}
	}

}
