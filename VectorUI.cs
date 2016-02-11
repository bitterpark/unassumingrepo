using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class VectorUI : MonoBehaviour 
{
	VectorLine myLine;
	public Texture defaultLineTexture;
	
	public void AssignVectorLine(VectorLine newLine, Transform parentObject, bool setToTop)
	{
		myLine=newLine;
		//!!WARNING!! - default texture is thinLine, it gives free AA but the line width MUST BE >=4!!
		myLine.texture=defaultLineTexture;
		//testLine.drawTransform=transform.parent;
		//Canvas newCanvas=Instantiate(testCanvas);
		transform.SetParent(parentObject);
		if (setToTop) transform.SetAsLastSibling();
		else transform.SetAsFirstSibling();
		//newCanvas.enabled=true;
		//newCanvas.overrideSorting=true;
		//newCanvas.sortingOrder=99;
		myLine.SetCanvas(GetComponent<Canvas>());
		myLine.Draw();
		//newCanvas.transform.localScale=new Vector2(1,1);
		foreach(Transform child in transform.GetComponentsInChildren<Transform>()) {child.localScale=new Vector2(1,1);}
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
