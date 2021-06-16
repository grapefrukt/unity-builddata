using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

[CustomEditor(typeof(BuildData))]
public class BuildDataEditor  : Editor {
	
	// this is where the temporary build data asset will go, this will be created
	// when a build runs and deleted immediately after
	internal const string BuildDataPath = "Assets/Resources/BuildData.asset";
	
	public static void UpdateData() {
		
		// i only build on windows/osx and these paths work well there, adjust as needed
		#if UNITY_EDITOR_WIN
			const string gitPath = "C:\\Program Files\\Git\\bin\\git.exe";
		#else
			const string gitPath = "git";
		#endif
		
		// now we run git in the project folder to get the current commit description
		// (this usually ends up being the current tag plus a commit hash)
		
		var gitInfo = new ProcessStartInfo {
			CreateNoWindow = true,
			RedirectStandardError = true,
			RedirectStandardOutput = true,
			FileName = gitPath,
			UseShellExecute = false,
		};
		
		var gitProcess = new Process();
		gitInfo.Arguments = "describe --tags --always"; 
		gitInfo.WorkingDirectory = Directory.GetCurrentDirectory();

		gitProcess.StartInfo = gitInfo;
		gitProcess.Start();

		var stdout = gitProcess.StandardOutput.ReadToEnd();

		gitProcess.WaitForExit();
		gitProcess.Close();

		// once we're done with git, let's make the data file 
		
		var data = CreateInstance<BuildData>();
		
		// parse out the relevant bits of the git call output
		data.git = stdout.Trim();
		
		// set the build number (i use auto-incrementing build numbers that are set externally by my build server) 
		#if UNITY_ANDROID
			data.buildNumber = PlayerSettings.Android.bundleVersionCode;
		#elif UNITY_IOS
			data.buildNumber = int.Parse(PlayerSettings.iOS.buildNumber);
		#else
			data.buildNumber = 0;
		#endif

		// timestamps, regular utc flavor, more or less ISO 8601
		data.timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
		// and a unix timestamp too, for good measure
		data.timestampUnix = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString("F0");
		
		// makes sure old version is deleted as to not make CreateAsset throw an error
		AssetDatabase.DeleteAsset(BuildDataPath);
		AssetDatabase.CreateAsset(data, BuildDataPath);
		AssetDatabase.SaveAssets();
	}
}


// this makes sure the above code runs when a build runs
internal class BuildDataPreprocessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport {
	public int callbackOrder => 2;

	public void OnPreprocessBuild(BuildReport report) {
		BuildDataEditor.UpdateData();
	}

	public void OnPostprocessBuild(BuildReport report) {
		AssetDatabase.DeleteAsset(BuildDataEditor.BuildDataPath);
	}
}

