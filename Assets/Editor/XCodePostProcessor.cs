using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class XCodePostProcessor : MonoBehaviour
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string buildPath)
    {
        if (target == BuildTarget.iOS)
        {
            ModifyXcodeproj(buildPath);
        }
    }

    private static void ModifyXcodeproj(string buildPath)
    {
        Debug.Log("Configuring Xcode project with environment variables...");

        // Read config from file.
        string projectPath = PBXProject.GetPBXProjectPath(buildPath);
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);

        // Get environment variables.
        bool useAutomaticSigning = Environment.GetEnvironmentVariable("IOS_AUTOMATIC_SIGNING") == "true";
        bool devMode = Environment.GetEnvironmentVariable("IOS_DEV_MODE") == "true";
        string provisioningProfile = Environment.GetEnvironmentVariable("IOS_PROVISIONING_PROFILE_SPECIFIER") ?? "";
        string devTeam = Environment.GetEnvironmentVariable("IOS_DEVELOPMENT_TEAM") ?? "";
        string codeSignIdentity = devMode ? "Apple Development" : "Apple Distribution";

        Debug.Log($"Using automatic signing '{useAutomaticSigning}'.");
        Debug.Log($"Using provisioning profile '{provisioningProfile}'.");
        Debug.Log($"Using development team '{devTeam}'.");

        string iPhoneUnityTarget = project.GetUnityMainTargetGuid();
        string releaseConfigGuid = project.BuildConfigByName(iPhoneUnityTarget, (devMode ? "Debug" : "Release")); 

        if (useAutomaticSigning)
        {
            // Configure automatic signing.
            project.SetBuildPropertyForConfig(releaseConfigGuid, "CODE_SIGN_STYLE", "Automatic");
            project.SetBuildPropertyForConfig(releaseConfigGuid, "CODE_SIGN_IDENTITY", codeSignIdentity);
            // Leave PROVISIONING_PROFILE_SPECIFIER empty, Xcode will handle it automatically.
        }
        else
        {
            // Configure manual signing.
            project.SetBuildPropertyForConfig(releaseConfigGuid, "CODE_SIGN_STYLE", "Manual");
            project.SetBuildPropertyForConfig(releaseConfigGuid, "CODE_SIGN_IDENTITY", codeSignIdentity);
            project.SetBuildPropertyForConfig(releaseConfigGuid, "PROVISIONING_PROFILE_SPECIFIER", provisioningProfile);
        }
        project.SetBuildPropertyForConfig(releaseConfigGuid, "DEVELOPMENT_TEAM", devTeam);

        // Save changes.
        project.WriteToFile(projectPath);
        Debug.Log("Finished configuring Xcode project.");
    }
}
