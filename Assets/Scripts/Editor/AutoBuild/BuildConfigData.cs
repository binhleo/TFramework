using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace AutoBuild
{
    [System.Serializable]
    public class PluginCopy
    {
        public string from;
        public string to;
    }

    [System.Serializable]
    public class PlatformConfig
    {
        public string outputName;
        public string productName;
        public string bundleID;
        public string version;
        public string iosVersionCode;
        public string iosSignTeamID;
        public string androidVersionCode;
        public string buildNo;
        public string orientation;
        public string targetIosVersion;
        public string removeLog;
        public string androidBuildMode;

        public string compileFlags;
        public string keystorePath;
        public string keystorePass;
        public string keystoreAliasName;
        public string keystoreAliasPass;

        public string[] delete;
        public PluginCopy plugins;

        public void Merge(PlatformConfig target)
        {
            if (target.productName != null)
                this.productName = target.productName;

            if (target.bundleID != null)
                this.bundleID = target.bundleID;

            if (target.version != null)
                this.version = target.version;

            if (target.buildNo != null)
                this.buildNo = target.buildNo;

            if (target.orientation != null)
                this.orientation = target.orientation;

            if (target.delete != null)
                this.delete = target.delete;

            if (target.plugins != null)
                this.plugins = target.plugins;

            if (target.targetIosVersion != null)
                this.targetIosVersion = target.targetIosVersion;

            if (target.androidVersionCode != null)
                this.androidVersionCode = target.androidVersionCode;

            if (target.iosVersionCode != null)
                this.iosVersionCode = target.iosVersionCode;

            if (target.iosSignTeamID != null)
                this.iosSignTeamID = target.iosSignTeamID;

            if (target.removeLog != null)
                this.removeLog = target.removeLog;

            if (target.outputName != null)
                this.outputName = target.outputName;

            if (target.androidBuildMode != null)
                this.androidBuildMode = target.androidBuildMode;
        }

        public string GetOutputName(string defaultValue)
        {
            if (outputName == null)
                return defaultValue;
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            System.Text.StringBuilder sb = new System.Text.StringBuilder(outputName);
            sb = sb.Replace("{version}", this.version.Replace(".", string.Empty)); // no dot in name
            sb = sb.Replace("{androidVersionCode}", this.androidVersionCode);
            sb = sb.Replace("{utc}", unixTimestamp.ToString());
            return sb.ToString();
        }
    }

    [System.Serializable]
    public class BuildConfig
    {
        public string[] Scenes;

        public PlatformConfig Common;
        public PlatformConfig Standalone;
        public PlatformConfig iOS;
        public PlatformConfig Android;

        public PlatformConfig SelectConfigByPlatform(BuildTargetGroup target_group)
        {
            switch(target_group)
            {
                case BuildTargetGroup.Standalone:
                    return this.Standalone;
                case BuildTargetGroup.iOS:
                    return this.iOS;
                case BuildTargetGroup.Android:
                    return this.Android;
                default:
                    return null;                        
            }
        }    
    }
}