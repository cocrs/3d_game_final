using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarUI : MonoBehaviour, Observer {
	protected ObservableRatio observingObject;
	protected Transform bar;
	protected Image barImage;
	protected Image shadowImage;	//Optional shadow effect
	protected Text text;

	public virtual void Initialize(ObservableRatio observable) {
		observingObject = observable;
		observingObject.AddObserver (this);
		Transform canvas = transform.Find ("Canvas");
		bar = canvas.Find ("MainBar").transform;
		barImage = bar.GetComponent<Image> ();

		Transform shadowTransform = bar.Find ("Shadow");
		if (shadowTransform != null) shadowImage = shadowTransform.GetComponent<Image> ();
		Transform textTransform = canvas.Find ("Text");
		if (textTransform != null) text = textTransform.GetComponent<Text> ();
	}

	public virtual void Notify() {
		if (bar == null) {
			return;
		}
		float ratio = observingObject.GetRatio ();
		barImage.fillAmount = ratio;
		if (shadowImage != null) shadowImage.fillAmount = ratio;
		if (text != null) text.text = observingObject.GetAbsoluteAmount ().ToString ();
	}

}
