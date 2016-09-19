using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class InteractableCard: MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{

	Transform createdBigCardGraphic;

	protected void SetupButton()
	{
		GetComponent<Button>().onClick.AddListener(() =>CardClicked());
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		MouseHoverStart();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		MouseHoverEnd();
	}


	protected virtual void MouseHoverStart()
	{
		
		createdBigCardGraphic = CreateBigCardGraphic();
		createdBigCardGraphic.transform.localScale = new Vector3(1.1f,1.1f,1f);
		createdBigCardGraphic.transform.SetParent(transform.parent, false);
		createdBigCardGraphic.transform.position = transform.position;
		createdBigCardGraphic.transform.SetAsLastSibling();
		createdBigCardGraphic.GetComponent<LayoutElement>().ignoreLayout = true;
	}

	protected abstract Transform CreateBigCardGraphic();

	protected virtual void MouseHoverEnd()
	{
		if (createdBigCardGraphic != null)
		{
			GameObject.Destroy(createdBigCardGraphic.gameObject);
			createdBigCardGraphic = null;
		}
	}

	protected abstract void CardClicked();
}
