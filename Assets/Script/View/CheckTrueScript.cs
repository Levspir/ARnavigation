using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTrueScript : MonoBehaviour, IARObject {
    
    private ARObject.Type artype = ARObject.Type.Check;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void InitAR()
    {
        gameObject.SetActive(false);
        Debug.Log(transform.parent.name + " attract checkTrue to found dest");
    }

    public ARObject.Type Type
    {
        get
        {
            return artype;
        }
    }
}
