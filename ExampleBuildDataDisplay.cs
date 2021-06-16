using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ExampleBuildDataDisplay : MonoBehaviour {
	void Start() {
		var versionText = GetComponent<TextMeshProUGUI>();
		if (versionText == null) return;
		versionText.text = "build info missing\ndate goes here";
		
		// try to load build info, may not be present
		var data = Resources.Load<BuildData>("BuildData");
		if (data == null) return;
		versionText.text = $"{data.version} #{data.buildNumber}\n{data.git}\n{data.timestamp}";
	}
}