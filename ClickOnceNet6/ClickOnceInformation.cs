using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnceNet6
{
    public static class ClickOnceInformation
    {

        public static string BaseDirectory
        {
            get
            {
                return AppContext.BaseDirectory;
            }
        }

        public static Uri RemoteDirectory
        {
            get
            {
                if (Environment.GetEnvironmentVariable("CLICKONCE_UPDATELOCATION") is { } updateLocationString && Uri.TryCreate(updateLocationString, UriKind.RelativeOrAbsolute, out var updateLocation))
                    return new Uri(updateLocation.ToString().Replace(System.IO.Path.GetFileName(updateLocation?.LocalPath) ?? "", ""));
                else
                    return null;
            }
        }

        public static string TargetFrameworkName
        {
            get
            {
                return AppContext.TargetFrameworkName;
            }
        }

        public static bool IsNetworkDeployed
        {
            get
            {

                //if (!string.IsNullOrEmpty(_CurrentPath) && _CurrentPath.Contains("AppData\\Local\\Apps"))
                //{
                //    return true;
                //}

                //return false;


                if (Environment.GetEnvironmentVariable("CLICKONCE_ISNETWORKDEPLOYED") == bool.TrueString)
                    return true;
                else
                    return false;
            }
        }

        public static Version CurrentVersion
        {
            get
            {
                if (Environment.GetEnvironmentVariable("CLICKONCE_CURRENTVERSION") is { } currentVersionString && Version.TryParse(currentVersionString, out var currentVersion))
                    return currentVersion;
                else
                    return null;

            }
        }


        public static Version UpdatedVersion
        {
            get
            {
                if (Environment.GetEnvironmentVariable("CLICKONCE_UPDATEDVERSION") is { } updatedVersionString && Version.TryParse(updatedVersionString, out var updatedVersion))

                    return updatedVersion;
                else
                    return null;

            }
        }

        public static string UpdateLocation
        {
            get
            {
                if (Environment.GetEnvironmentVariable("CLICKONCE_UPDATELOCATION") is { } updateLocationString)

                    return updateLocationString;
                else
                    return null;

            }
        }

        public static string UpdatedApplicationFullName
        {
            get
            {
                if (Environment.GetEnvironmentVariable("CLICKONCE_UPDATEDAPPLICATIONFULLNAME") is { } updatedApplicationFullName)
                    return updatedApplicationFullName;
                else
                    return null;
            }
        }

        public static DateTime? TimeOfLastUpdateCheck
        {
            get
            {
                if (Environment.GetEnvironmentVariable("CLICKONCE_TIMEOFLASTUPDATECHECK") is { } timeOfLastUpdateCheckString && DateTime.TryParse(timeOfLastUpdateCheckString, null, DateTimeStyles.RoundtripKind, out var timeOfLastUpdateCheck))
                    return timeOfLastUpdateCheck;
                else
                    return null;

            }
        }

        public static Uri ActivationUri
        {
            get
            {
                if (Environment.GetEnvironmentVariable("CLICKONCE_ACTIVATIONURI") is { } activationUriString && Uri.TryCreate(activationUriString, UriKind.RelativeOrAbsolute, out var activationUri))

                    return activationUri;
                else
                    return null;

            }
        }

        public static string DataDirectory
        {
            get
            {
                if (Environment.GetEnvironmentVariable("CLICKONCE_DATADIRECTORY") is { } dataDirectory)

                    return dataDirectory;
                else
                    return null;

            }
        }

        public static string[] ActivationData
        {
            get
            {
                // Not 100%e sure what this is but it is mentioned at https://github.com/dotnet/deployment-tools/pull/135 and https://github.com/dotnet/deployment-tools/issues/113
                // so we can include it. Think it might be about passing command line arguments and/or FileAssociation arguments.

                if (Environment.GetEnvironmentVariable("CLICKONCE_ACTIVATIONDATA_1") is { } activationDataItem)
                {
                    var items = new List<string>();
                    var index = 1;

                    do
                    {
                        items.Add(activationDataItem);

                        activationDataItem = Environment.GetEnvironmentVariable($"CLICKONCE_ACTIVATIONDATA_{++index}");
                    }
                    while (activationDataItem != null);

                    return items.ToArray();
                }
                else return null;
            }
        }

        public static string ApplicationName
        {
            get
            {
                if (Environment.GetEnvironmentVariable("CLICKONCE_UPDATELOCATION") is { } updateLocationString && Uri.TryCreate(updateLocationString, UriKind.RelativeOrAbsolute, out var updateLocation))
                    return new Uri(UpdateLocation)?.Segments[^1].Replace(".application", null, StringComparison.OrdinalIgnoreCase) ?? throw new Exceptions.ClickOnceNotFoundUpdateLocationException(string.Format("Not found Update location at {0}", UpdateLocation));
                else
                    return null;
            }
        }

        public static InstallFromEnum InstallForm
        {
            get
            {
                if (IsNetworkDeployed && !string.IsNullOrEmpty(UpdateLocation))
                {
                    return UpdateLocation.StartsWith("http") ? InstallFromEnum.Web : InstallFromEnum.Unc;
                }
                else
                {
                   return InstallFromEnum.NoNetwork;
                }
            }

        }


    }

}
