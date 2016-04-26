using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProbabilityListValueTypes<T> where T: struct
{
	//Dictionary<,float> probabilities=new Dictionary<Object, float>();
	public Dictionary<T,float> probabilities;
	
	public ProbabilityListValueTypes()
	{
		probabilities=new Dictionary<T, float>();
	}
	
	public void AddProbability(T newItem, float newChance)
	{
		float combinedPositiveProbabilitySpace=0;
		foreach (float chance in probabilities.Values)
		{
			combinedPositiveProbabilitySpace+=chance;
		}
		//If newChance+combinedPositiveProbabilitySpace>1, this will shove the newChance into whatever space is left in the probabilitySpace instead
		probabilities.Add(newItem,Mathf.Max(0,Mathf.Min(1-combinedPositiveProbabilitySpace,newChance)));
		//if (newChance>1-combinedPositiveProbabilitySpace) GameManager.DebugPrint("Probability in list adds up to >1!");
	}
	
	public bool RollProbability(out T pickedItem)
	{
		bool resultFound=false;
		pickedItem=default(T);
		//Setting up intervals
		Dictionary<float,T> intervalDictionary=new Dictionary<float,T>();
		float intervalCeilSoFar=0;
		foreach (T item in probabilities.Keys)
		{
			if (probabilities[item]>0)
			{
				intervalCeilSoFar+=probabilities[item];
				intervalDictionary.Add(intervalCeilSoFar,item);
			}
		}
		//Rolling on intervals
		float roll=Random.value;
		float minIntCeilSoFar=1;
		//Each successfuly rolled interval item will overwrite the next one
		foreach (float chance in intervalDictionary.Keys)
		{
			if (roll<=chance && chance<=minIntCeilSoFar) 
			{
				minIntCeilSoFar=chance;
				pickedItem=intervalDictionary[chance];
				resultFound=true;
			}
		}
		return resultFound;
	}
}

public class ProbabilityList <T> where T: class
{
	//Dictionary<,float> probabilities=new Dictionary<Object, float>();
	public Dictionary<T,float> probabilities;
	
	public ProbabilityList()
	{
		probabilities=new Dictionary<T, float>();
	}
	
	public void AddProbability(T newItem, float newChance)
	{
		float combinedPositiveProbabilitySpace=0;
		foreach (float chance in probabilities.Values)
		{
			combinedPositiveProbabilitySpace+=chance;
		}
		//If newChance+combinedPositiveProbabilitySpace>1, this will shove the newChance into whatever space is left in the probabilitySpace instead
		probabilities.Add(newItem,Mathf.Max(0,Mathf.Min(1-combinedPositiveProbabilitySpace,newChance)));
		//if (newChance>1-combinedPositiveProbabilitySpace) GameManager.DebugPrint("Probability in list adds up to >1!");
	}
	
	public bool RollProbability(out T pickedItem)
	{
		bool resultFound=false;
		pickedItem=null;
		//Setting up intervals
		Dictionary<float,T> intervalDictionary=new Dictionary<float,T>();
		float intervalCeilSoFar=0;
		foreach (T item in probabilities.Keys)
		{
			if (probabilities[item]>0)
			{
				intervalCeilSoFar+=probabilities[item];
				intervalDictionary.Add(intervalCeilSoFar,item);
			}
		}
		//Rolling on intervals
		float roll=Random.value;
		float minIntCeilSoFar=1;
		//Each successfuly rolled interval item will overwrite the next one
		foreach (float chance in intervalDictionary.Keys)
		{
			if (roll<=chance && chance<=minIntCeilSoFar) 
			{
				minIntCeilSoFar=chance;
				pickedItem=intervalDictionary[chance];
				resultFound=true;
			}
		}
		return resultFound;
	}
}

public class PriorityList<T> where T: class
{
	Dictionary<T,int> priorities=new Dictionary<T, int>();

	public int GetCount() {return priorities.Count;}

	public void ClearList()
	{
		priorities.Clear();
	}

	public void Add(T addedObj, int priority)
	{
		priorities.Add(addedObj,priority);
	}

	public void TryRemove(T removedObj)
	{
		if (priorities.ContainsKey(removedObj)) priorities.Remove(removedObj);
	}

	//Gets item with highest priority. If multiple items have the highest priority, picks one at random.
	public T Get(bool removeObj)
	{
		int maxPriority=0;
		T resultObj=null;
		foreach (T obj in priorities.Keys)
		{
			if (priorities[obj]>maxPriority || (priorities[obj]==maxPriority && (Random.value<=0.5f || resultObj==null)))
			{
				maxPriority=priorities[obj];
				resultObj=obj;
			}
		}
		if (removeObj) priorities.Remove(resultObj);
		return resultObj;
	}

	public T Get()
	{
		return Get(false);
	}

}
