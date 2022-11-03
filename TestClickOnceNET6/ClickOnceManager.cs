using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TestClickOnceNET6
{
    public class ClickOnceManager
    {

        #region Enums

        public enum RemoteVersionStatusEnum
        {
            SameAsLocal = 0,
            HigerThanLocal = 1,
            LowerThanLocal = -1
        }

        #endregion

        #region private properties

        private static Uri UriUpdateLocation { get; set; }
        private readonly static ClickOnceNet6.ClickOnceDeployment clickOnceDeployment = new();

        #endregion

        #region "public Properties"

        public static Uri UpdateLocation
        {
            get
            {
                if (UriUpdateLocation == null)
                {
                    try
                    {
                        UriUpdateLocation = new Uri(ClickOnceNet6.ClickOnceInformation.UpdateLocation);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Error obteniendo la URL de descarga de actualizaciones de ClickOnce: {0}", ex.Message));
                    }
                }

                return UriUpdateLocation;
            }
        }

        public static Version ClickOnceVersion
        {
            get

            {
                return ClickOnceNet6.ClickOnceInformation.IsNetworkDeployed ? ClickOnceNet6.ClickOnceInformation.CurrentVersion : new Version(0, 0, 0, 0);

            }
        }


        #endregion

        #region public static methods

        public static RemoteVersionStatusEnum CheckForUpdate(ref Version remoteVersionNumber)
        {

            // If not in ClickOnce execution, returns False.
            if (!ClickOnceNet6.ClickOnceInformation.IsNetworkDeployed)
            {
                return RemoteVersionStatusEnum.SameAsLocal;
            }

            try
            {
                remoteVersionNumber =  clickOnceDeployment.ServerVersion();
            }
            catch 
            {
                throw;
            }

            //Returns 'True' if the current version differs from the remote version number.
            return remoteVersionNumber == ClickOnceManager.ClickOnceVersion ? RemoteVersionStatusEnum.SameAsLocal : remoteVersionNumber > ClickOnceManager.ClickOnceVersion ? RemoteVersionStatusEnum.HigerThanLocal : RemoteVersionStatusEnum.LowerThanLocal;


        }


        public async static Task<Boolean> Update()
        {

            //If not in ClickOnce execution, returns False.
            if (!ClickOnceNet6.ClickOnceInformation.IsNetworkDeployed) return false;

            return await clickOnceDeployment.Update();
        }



        #endregion

    }
}
