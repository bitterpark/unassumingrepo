using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class InteractableCard: MonoBehaviour//MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
	protected void SetupButton()
	{
		GetComponent<Button>().onClick.AddListener(() =>CardClicked());
	}

	protected abstract void CardClicked();
}
