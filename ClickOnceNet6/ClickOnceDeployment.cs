using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using ClickOnceNet6.Exceptions;


namespace ClickOnceNet6
{
    /// <summary>
    /// Class ClickOnceDeployment.
    /// </summary>
    public class ClickOnceDeployment
    {

        private string _CurrentAppName;
        private string _CurrentPath;
        private string _PublishPath;
        private bool _IsNetworkDeployed;
        private InstallFromEnum _InstallFromEnum;

        /// 
        public ClickOnceDeployment()
        {
            _PublishPath = ClickOnceInformation.RemoteDirectory.ToString();
            _CurrentPath = ClickOnceInformation.BaseDirectory;
            _IsNetworkDeployed = ClickOnceInformation.IsNetworkDeployed;
            _InstallFromEnum = ClickOnceInformation.InstallForm;
            _CurrentAppName = Assembly.GetEntryAssembly()?.GetName().Name;
            if (string.IsNullOrEmpty(_CurrentAppName))
            {
                throw new ClickOnceDeploymentException("Can't find entry assembly name!");
            }
        }

        private string SearchAppDataDir(string programData, string currentFolderName, int i)
        {
            i++;
            if (i > 100)
            {
                throw new ClickOnceDeploymentException($"Can't find data dir for {currentFolderName} in path: {programData}");
            }
            var subdirectoryEntries = Directory.GetDirectories(programData);
            var result = string.Empty;
            foreach (var dir in subdirectoryEntries)
            {
                if (dir.Contains(currentFolderName))
                {
                    result = Path.Combine(dir, "Data");
                    break;
                }

                result = SearchAppDataDir(Path.Combine(programData, dir), currentFolderName, i);
                if (!string.IsNullOrEmpty(result))
                {
                    break;
                }
            }

            return result;
        }

        public Version CurrentVersion()
        {
            if (!_IsNetworkDeployed)
            {
                throw new ClickOnceDeploymentException("Not deployed by network!");
            }

            if (string.IsNullOrEmpty(_CurrentAppName))
            {
                throw new ClickOnceDeploymentException("Application name is empty!");
            }

            var path = Path.Combine(_CurrentPath, $"{_CurrentAppName}.exe.manifest");
            if (!File.Exists(path))
            {
                throw new ClickOnceDeploymentException($"Can't find manifest file at path {path}");
            }
            var fileContent = File.ReadAllText(path);
            var xmlDoc = XDocument.Parse(fileContent, LoadOptions.None);
            XNamespace nsSys = "urn:schemas-microsoft-com:asm.v1";
            var xmlElement = xmlDoc.Descendants(nsSys + "assemblyIdentity").FirstOrDefault();

            if (xmlElement == null)
            {
                throw new ClickOnceInvalidDeploymentException($"Invalid manifest document for {path}");
            }

            var version = xmlElement.Attribute("version")?.Value;
            if (string.IsNullOrEmpty(version))
            {
                throw new ClickOnceDeploymentException("Version info is empty!");
            }

            return new Version(version);
        }

        public Version ServerVersion()
        {
            string manifestFile;

            if (_InstallFromEnum == InstallFromEnum.Web)
            {
                manifestFile = Methods.ReadManifest.ClearManifestXML(Methods.WebRequest.ReturnString(new HttpClient(), _PublishPath + $"{_CurrentAppName}.application"));
                return Methods.ReadManifest.ReadServerManifest(manifestFile);
            }

            if (_InstallFromEnum == InstallFromEnum.Unc)
            {
                using (var stream = File.OpenRead(Path.Combine($"{_PublishPath}", $"{_CurrentAppName}.application")))
                {
                    StreamReader reader = new(stream);
                    return Methods.ReadManifest.ReadServerManifest(reader.ReadToEnd());
                }
            }

            throw new ClickOnceDeploymentException("No network install was set");
        }

        public async Task<Version> ServerVersionAsync()
        {

            if (_InstallFromEnum == InstallFromEnum.Web)
            {
                using (var client = new HttpClient { BaseAddress = new Uri(_PublishPath) })
                {
                    using (var stream = await client.GetStreamAsync($"{_CurrentAppName}.application"))
                    {
                        return await Methods.ReadManifest.ReadServerManifestAsync(stream);
                    }
                }
            }

            if (_InstallFromEnum == InstallFromEnum.Unc)
            {
                using (var stream = File.OpenRead(Path.Combine($"{_PublishPath}", $"{_CurrentAppName}.application")))
                {
                    return await Methods.ReadManifest.ReadServerManifestAsync(stream);
                }
            }

            throw new ClickOnceDeploymentException("No network install was set");
        }

        public Task<bool> UpdateAvailable()
        {
            var currentVersion = CurrentVersion();
            var serverVersion = ServerVersion();

            return Task.FromResult(currentVersion < serverVersion);
        }

        public async Task<bool> Update()
        {
            var currentVersion = CurrentVersion();
            var serverVersion = ServerVersion();

            if (currentVersion >= serverVersion)
            {
                // Nothing to update
                return false;
            }

            Process proc;
            string setupPath = null;
            if (_InstallFromEnum == InstallFromEnum.Web)
            {

                var uri = new Uri($"{_PublishPath}setup.exe");

                setupPath = Path.Combine(Path.GetTempPath(), $"setup{serverVersion}.exe");

                try
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(uri);
                        using (var fs = new FileStream(setupPath, FileMode.CreateNew))
                        {
                            await response.Content.CopyToAsync(fs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ClickOnceDeploymentDownloadException("Error downloading new update: " + ex.Message);
                }

                proc = OpenUrl(setupPath);
            }
            else if (ClickOnceInformation.InstallForm == InstallFromEnum.Unc)
            {
                proc = OpenUrl(Path.Combine($"{_PublishPath}", $"{_CurrentAppName}.application"));
            }
            else
            {
                throw new ClickOnceDeploymentException("No network install was set");
            }

            if (proc == null)
            {
                throw new ClickOnceDeploymentException("Can't start update process");
            }

            await proc.WaitForExitAsync();

            if (!string.IsNullOrEmpty(setupPath))
            {
                File.Delete(setupPath);
            }

            return true;
        }

        private static Process OpenUrl(string url)
        {
            try
            {
                var info = new ProcessStartInfo(url)
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = false,
                    UseShellExecute = false
                };
                var proc = Process.Start(info);
                return proc;
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                url = url.Replace("&", "^&");
                return Process.Start(new ProcessStartInfo("cmd", $"/c start \"\"{url}\"\"")
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                }
                );
            }
        }

    }
}
