using UnityEngine;
using System.Collections;
using System;


public class QueueParam {
	
	public const string ADD = "ADD";
	public const string REMOVE = "REMOVE";
	
	public string type;
	public string eventType;
	public Action<object[]> callback;
    public string context;

	public QueueParam(string t, string e,Action<object[]> c,string context)
	{
		this.type = t;
		this.eventType = e;
		this.callback = c;
        this.context = context;
	}
}
