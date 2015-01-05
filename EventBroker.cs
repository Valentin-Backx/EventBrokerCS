using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class EventBroker : MonoBehaviour {

    public const string EVENT_BROKER_DEFAULT_CONTEXT = "eventBrokerDefaultContext";

    private Dictionary<string, Dictionary<string, Action<object[]>>> _contexts;

	public static EventBroker instance;
	
	private bool _lockQueue = false;
	
	private List<QueueParam> _itemsDuringLock;

    

	void Awake()
	{

		if(instance)
		{
			Destroy(gameObject);
		}else{
            DontDestroyOnLoad(this.gameObject);
            _contexts = new Dictionary<string, Dictionary<string, Action<object[]>>>();
            _contexts[EVENT_BROKER_DEFAULT_CONTEXT] = new Dictionary<string, Action<object[]>>();
			_itemsDuringLock = new List<QueueParam>();
			EventBroker.instance = this;
		}
	}
	
	
	static public void subscribe(string eventType,Action<object[]> callback,string context = EVENT_BROKER_DEFAULT_CONTEXT)
	{
		EventBroker.instance.addSubscriber(eventType,callback,context);
	}

    static public void unsubscribe(string eventType, Action<object[]> callback, string context = EVENT_BROKER_DEFAULT_CONTEXT)
	{
		instance.removeSubscriber(eventType,callback,context);
	}

    static public void broadcast(CustomEvent e, string context = EVENT_BROKER_DEFAULT_CONTEXT)
	{
		EventBroker.instance.broadcastMessage(e,context);
	}
	
	private void addSubscriber(string eventType,Action<object[]> callback,string context)
	{
		//prevent items from being added while broadcasting
		if(_lockQueue)
		{
			_itemsDuringLock.Add(new QueueParam(QueueParam.ADD,eventType,callback,context));
			return;
		}
		
		if(_contexts.ContainsKey(context) && _contexts[context].ContainsKey(eventType))
		{
			Action<object[]> callbackList = _contexts[context][eventType];
			
			callbackList = callbackList==null?callback:(Action<object[]>)Delegate.Combine(callbackList,callback);
			
			_contexts[context][eventType] = callbackList;

		}else{
            if(!this._contexts.ContainsKey(context)) this._contexts.Add(context,new Dictionary<string,Action<object[]>>());
			_contexts[context][eventType] = callback;
		}	
	}
	
	private void removeSubscriber(string eventType,Action<object[]> callback, string context)
	{
		if(_lockQueue)
		{
			_itemsDuringLock.Add(new QueueParam(QueueParam.REMOVE,eventType,callback,context));
			return;
		}
		
		if(_contexts.ContainsKey(context) && _contexts[context].ContainsKey(eventType))
		{
			Action<object[]> callbackList = _contexts[context][eventType];
			
			callbackList = (Action<object[]>)callbackList - callback;
			
			_contexts[context][eventType] = callbackList;
		}
		
		
	}
	
	private void broadcastMessage(CustomEvent e,string context)
	{
		_lockQueue = true;
		if(_contexts.ContainsKey(context) && _contexts[context].ContainsKey(e.eventName))
		{
			Action<object[]> callbackList = _contexts[context][e.eventName];
			e.e += (Action<object[]>)(callbackList);
			if(callbackList != null) e.raise();
		}
		
		_lockQueue = false;
		
		if(_itemsDuringLock.Count > 0) this.clearQueue();
	}
	
	private void clearQueue()
	{
		int len = _itemsDuringLock.Count;
		for (int i = 0; i < len; i++) {
			QueueParam item = _itemsDuringLock[i];
			switch(item.type)
			{
			case QueueParam.ADD:
				this.addSubscriber(item.eventType,item.callback,item.context);
				break;
			case QueueParam.REMOVE:
				break;
			}
		}
		
		_itemsDuringLock = new List<QueueParam>();
	}
}
