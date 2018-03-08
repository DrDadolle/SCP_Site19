using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Behaviour : MonoBehaviour {

	//Handle Trigger Enter
	void OnTriggerEnter(Collider other){

		if (gameObject.layer.Equals (CreateWall.VALUE_OF_GHOST_WALLS_LAYER)){
			if(!other.gameObject.layer.Equals(CreateWall.VALUE_OF_WALLS_LAYER)){
				CreateWall.changePropertiesOfWall (gameObject, CreateWall.ghostly_red, true);
			} else if(other.gameObject.transform.position.Equals(gameObject.transform.position)){
				CreateWall.changePropertiesOfWall (gameObject, CreateWall.ghostly_red, true);
			}
		}
	}

	//Handle Trigger Exit
	void OnTriggerExit (Collider other)
	{
		if (gameObject.layer.Equals (CreateWall.VALUE_OF_GHOST_WALLS_LAYER)){
			if(!other.gameObject.layer.Equals(CreateWall.VALUE_OF_WALLS_LAYER)){
				CreateWall.changePropertiesOfWall (gameObject, CreateWall.ghostly_blue, true);
			} else if(other.gameObject.transform.position.Equals(gameObject.transform.position)){
				CreateWall.changePropertiesOfWall (gameObject, CreateWall.ghostly_blue, true);
			}
		}
	}
}
