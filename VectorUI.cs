using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
using UnityEngine.UI;

public class VectorUI : MonoBehaviour 
{
	VectorLine myLine;
	public Texture defaultLineTexture;
	
	public void AssignVectorLine(VectorLine newLine, Transform parentObject, bool setToTop)
	{
		myLine=newLine;
		//!!WARNING!! - default texture is thinLine, it gives free AA but the line width MUST BE >=4!!
		myLine.texture=defaultLineTexture;

		transform.SetParent(parentObject);
		if (setToTop) transform.SetAsLastSibling();
		else transform.SetAsFirstSibling();

		myLine.SetCanvas(GetComponent<Canvas>());//parentObject.GetComponentsInParent<Canvas>()[0]);//GetComponent<Canvas>());
		myLine.Draw();
		//This distinction is important to make sure the canvas does not scale itself to 1,1
		foreach(VectorObject2D child in transform.GetComponentsInChildren<VectorObject2D>()) 
		{
			child.transform.localScale=new Vector2(1,1);
		}
	}
	
	public void AssignVectorLine(string vectorName,Transform parentObject, bool setToTop, List<Vector2> points, float width, Color col)
	{
		VectorLine createdLine=new VectorLine(vectorName,points,width);
		createdLine.color=col;
		AssignVectorLine(createdLine,parentObject,setToTop);
	}
	
	void OnDestroy()
	{
		VectorLine.Destroy(ref myLine);
	}
}
