using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatCreatedScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			ChangeMat();
		}
	}

	public void ChangeMat()
	{
		Material matt = new Material(Shader.Find("Unlit/UnlitShader01"));
		matt.mainTexture = (Texture) Resources.Load("floor4");
		Renderer rend = GetComponent<Renderer>();
		rend.material = matt;
		//rend.material.mainTexture = (Texture) Resources.Load("floor4");
	}
}
