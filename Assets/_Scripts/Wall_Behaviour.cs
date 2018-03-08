using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Behaviour : MonoBehaviour {

	//Handle Trigger Enter
	void OnTriggerEnter(Collider other){
		
		//If the game is a ghost wall
		if (gameObject.layer.Equals (CreateWall.VALUE_OF_GHOST_WALLS_LAYER)){
			//Colliding with anything but a placed wall
			if(!other.gameObject.layer.Equals(CreateWall.VALUE_OF_WALLS_LAYER)){
				CreateWall.changePropertiesOfWall (gameObject, CreateWall.ghostly_red, true);
			//or the placed wall it is colliding with occupies the same place
			} else if(other.gameObject.transform.position.Equals(gameObject.transform.position)){
				CreateWall.changePropertiesOfWall (gameObject, CreateWall.ghostly_red, true);
			}
		}
	}

	//Handle Trigger Exit
	void OnTriggerExit (Collider other){
		
		//If the game is a ghost wall
		if (gameObject.layer.Equals (CreateWall.VALUE_OF_GHOST_WALLS_LAYER)){
			//Colliding with anything but a placed wall
			if(!other.gameObject.layer.Equals(CreateWall.VALUE_OF_WALLS_LAYER)){
				CreateWall.changePropertiesOfWall (gameObject, CreateWall.ghostly_blue, true);
			//or the placed wall it is colliding with occupies the same place
			} else if(other.gameObject.transform.position.Equals(gameObject.transform.position)){
				CreateWall.changePropertiesOfWall (gameObject, CreateWall.ghostly_blue, true);
			}
		}
	}
}
