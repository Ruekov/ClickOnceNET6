using ClickOnceNet6.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClickOnceNet6.Methods
{
    internal static class ReadManifest
    {
        /// <summary>
        /// Reads the server manifest Async
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Task&lt;Version&gt;.</returns>
        /// <exception cref="PureManApplicationDeployment.ClickOnceInvalidDeploymentException">Invalid manifest document for</exception>
        /// <exception cref="PureManApplicationDeployment.ClickOnceInvalidDeploymentException">Version info is empty!</exception>
        public static async Task<Version> ReadServerManifestAsync(Stream stream)
        {
            var xmlDoc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            XNamespace nsSys = "urn:schemas-microsoft-com:asm.v1";
            var xmlElement = xmlDoc.Descendants(nsSys + "assemblyIdentity").FirstOrDefault();

            if (xmlElement == null)
            {
                throw new ClickOnceInvalidDeploymentException($"Invalid manifest");
            }

            var version = xmlElement.Attribute("version")?.Value;
            if (string.IsNullOrEmpty(version))
            {
                throw new ClickOnceInvalidDeploymentException($"Version info is empty!");
            }

            return new Version(version);
        }

        /// <summary>
        /// Reads the server manifest
        /// </summary>
        /// <param name="stream">The xml string</param>
        /// <returns>Version</returns>
        /// <exception cref="PureManApplicationDeployment.ClickOnceDeploymentException">Invalid manifest document for {_CurrentAppName}.application</exception>
        /// <exception cref="PureManApplicationDeployment.ClickOnceDeploymentException">Version info is empty!</exception>
        public static Version ReadServerManifest(string xmlString)
        {
            var xmlDoc = XDocument.Parse(xmlString, LoadOptions.None);
            XNamespace nsSys = "urn:schemas-microsoft-com:asm.v1";
            var xmlElement = xmlDoc.Descendants(nsSys + "assemblyIdentity").FirstOrDefault();

            if (xmlElement == null)
            {
                throw new ClickOnceInvalidDeploymentException($"Invalid manifest");
            }

            var version = xmlElement.Attribute("version")?.Value;
            if (string.IsNullOrEmpty(version))
            {
                throw new ClickOnceInvalidDeploymentException($"Version info is empty!");
            }

            return new Version(version);
        }

        public static string ClearManifestXML(string receivedXML)
        {
            // Removes the first line from the manifest (the one corresponding to ' <? xml version = "1.0" encoding = "utf-8" ?> ').

            if (receivedXML.IndexOf("\n") >= 0)
            {
                return receivedXML.Substring(receivedXML.IndexOf("\n") + 1);
            }

            return receivedXML;
        }
    }
}
