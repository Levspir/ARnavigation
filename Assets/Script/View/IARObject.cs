using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IARObject
{
	ARObject.Type Type
	{
		get;
	}
    void InitAR();  //init on attract to new marker
}

