using UnityEngine;

namespace OxUE
{
	public class MatchScreenSize : MonoBehaviour {
		RectTransform rt;
		// Use this for initialization
		void Start () {
			rt = GetComponent<RectTransform>();
			MatchSize();
		}
	
		// Update is called once per frame
		void MatchSize () {
			float aspectRatio = (float)Screen.width / (float)Screen.height;
			float aspectRatio16x9 = 1280.0f / 720.0f;

			if (aspectRatio / aspectRatio16x9 > 1)
				rt.sizeDelta = new Vector2(1280 * (aspectRatio/ aspectRatio16x9), rt.sizeDelta.y);
		}
	}
}
