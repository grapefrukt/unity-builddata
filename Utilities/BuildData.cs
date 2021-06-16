using UnityEngine;

public class BuildData : ScriptableObject {
    public string git         = "";
    public int    buildNumber = 0;
    public string version => Application.version;
    public string timestamp     = "";
    public string timestampUnix = "";
    
    public static string platform => Application.platform.ToString();
    
    public override string ToString() {
	    return $"git: {git}\n" +
	           $"buildNumber: {buildNumber}\n" +
	           $"version: {version}\n" +
	           $"timestamp: {timestamp}\n" +
	           $"timestampUnix: {timestampUnix}\n" +
	           $"platform: {platform}";
    }
}
	