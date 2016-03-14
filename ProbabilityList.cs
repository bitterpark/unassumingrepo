using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		probabilities.Add(newItem,Mathf.Min(1-combinedPositiveProbabilitySpace,newChance));
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
