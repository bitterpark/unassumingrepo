using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloatingText : MonoBehaviour 
{	
	public Text numberText;

	public Color healthColor;
	public Color armorColor;
	public Color staminaColor;
	public Color ammoColor;

	public void AssignFloatingText(int value, Transform startPoint, CharacterGraphic.Resource resourceType)
	{
		transform.SetParent(startPoint, false);

		numberText.text=value.ToString();
		if (resourceType == CharacterGraphic.Resource.Health)
			numberText.color = healthColor;
		if (resourceType == CharacterGraphic.Resource.Armor)
			numberText.color = armorColor;
		if (resourceType == CharacterGraphic.Resource.Stamina)
			numberText.color = staminaColor;
		if (resourceType == CharacterGraphic.Resource.Ammo)
			numberText.color = ammoColor;
		StartCoroutine(MoveUpdward());
	}

	IEnumerator MoveUpdward()
	{
		float elapsedMoveTime = 0;
		float requiredMoveTime = CardsScreen.cardPlayAnimationTime;

		float upwardMoveSpeedPerSecond = 35f;

		while (elapsedMoveTime < requiredMoveTime)
		{
			transform.position += new Vector3(0, upwardMoveSpeedPerSecond*Time.deltaTime);
			elapsedMoveTime += Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}
		GameObject.Destroy(this.gameObject);
		yield break;
	}
}
