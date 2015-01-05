using UnityEngine;
using System.Collections;
using System;


public class CustomEvent {
	public string eventName;
	
	public event Action<object[]> e;
	
	public object[] eventParams;
	
	public CustomEvent(string eventName,params object[] eventParams)
	{
		this.eventName = eventName;
		this.eventParams = eventParams;
	}
	
	public void raise()
	{
		this.e(this.eventParams);
	}
}
