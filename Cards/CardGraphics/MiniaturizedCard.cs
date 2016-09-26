using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IBigCardSpawnable
{
	Transform CreateBigCardGraphic();
}

public class MiniaturizedCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	Transform createdBigCardGraphic;

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
		IBigCardSpawnable bigCardSpawner = GetComponent<CardGraphic>() as IBigCardSpawnable;

		createdBigCardGraphic = bigCardSpawner.CreateBigCardGraphic();
		createdBigCardGraphic.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
		createdBigCardGraphic.transform.SetParent(transform.parent, false);
		createdBigCardGraphic.transform.position = transform.position;
		//createdBigCardGraphic.transform.SetAsLastSibling();
		createdBigCardGraphic.GetComponent<LayoutElement>().ignoreLayout = true;
		Transform canvasTransform = transform.GetComponentInParent<Canvas>().transform;
		createdBigCardGraphic.transform.SetParent(canvasTransform, true);
		createdBigCardGraphic.SetAsLastSibling();
	}

	protected virtual void MouseHoverEnd()
	{
		if (createdBigCardGraphic != null)
		{
			GameObject.Destroy(createdBigCardGraphic.gameObject);
			createdBigCardGraphic = null;
		}
	}

	public bool CanGetCreatedBigCardTransform(out Transform createdBigCard)
	{
		createdBigCard = createdBigCardGraphic;
		if (createdBigCardGraphic != null)
			return true;
		else
			return false;
	}

	public void OnDestroy()
	{
		MouseHoverEnd();
	}
}
