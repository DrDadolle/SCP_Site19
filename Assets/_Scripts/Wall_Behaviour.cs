using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Behaviour : MonoBehaviour {

	//Handle Trigger Enter
	void OnTriggerEnter(Collider other){
		//If it is an already installed wall, do not do anything
		if (!gameObject.layer.Equals (CreateWall.VALUE_OF_WALLS_LAYER)) {
			CreateWall.changePropertiesOfWall (gameObject, CreateWall.ghostly_red, true);
		}
	}

	//Handle Trigger Exit
	void OnTriggerExit (Collider other)
	{
		if(!gameObject.layer.Equals(CreateWall.VALUE_OF_WALLS_LAYER)){
			CreateWall.changePropertiesOfWall (gameObject, CreateWall.ghostly_blue, true);
		}
	}
}
