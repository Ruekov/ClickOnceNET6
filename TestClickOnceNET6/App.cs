using ClickOnceNet6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestClickOnceNET6
{
    public static class App
    {
        public static Form MainForm { get; set; }


        public static Boolean InClickOnceExecution
        {
            get
            {

                return ClickOnceInformation.IsNetworkDeployed;
            }
        }

        public async static Task<bool> OnProcessAction()
        {

            // Checks whether there is any application update in ClickOnce. If the application has been updated (and must restart), returns //ExitToRestartApplication//.
            if (await App.CheckForClickOnceUpdates()) return false;

            // Reloads any outdated setting.

            return true;

        }

        public async static Task<Boolean> CheckForClickOnceUpdates()
        {
            string errMessage = "";

            try
            {

                if (App.InClickOnceExecution)
                {

                    ClickOnceManager.RemoteVersionStatusEnum updateStatus;
                    Version remoteVersionNumber = null;

                    try
                    {
                        // Checks the remote app version.
                        updateStatus = ClickOnceManager.CheckForUpdate(ref remoteVersionNumber);
                    }
                    catch (ClickOnceNet6.Exceptions.ClickOnceDeploymentDownloadException ex)
                    {

                        // This exception occurs if a network error or disk error occurs when downloading the deployment.
                        errMessage = string.Format("The application cannot check for the existence of a new version at this time. " +
                                                   "\n" + "\n" + "Please check your network connection, or try again later. Message: " + ex.Message);

                        MessageBox.Show(errMessage, "App_Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    catch (ClickOnceNet6.Exceptions.ClickOnceInvalidDeploymentException ex)
                    {

                        errMessage = string.Format("The application cannot check for an update. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Message: " + ex.Message);
                        //    Logger.logger.Error(errMessage, ex);
                        MessageBox.Show(errMessage, "App_Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }

                    try
                    {

                        switch (updateStatus)
                        // * There is a higher / lower version to update. *
                        {
                            case ClickOnceManager.RemoteVersionStatusEnum.HigerThanLocal:
                            case ClickOnceManager.RemoteVersionStatusEnum.LowerThanLocal:

                                MessageBox.Show(remoteVersionNumber.ToString(), "App_Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                try
                                {
                                    // Tries updating the application programmatically.
                                    await ClickOnceManager.Update();
                                    // If the update was OK, shows a message and restarts the application.
                                    MessageBox.Show("App_ApplicationUpdated", "pp_Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    //Program.Restart = true;

                                    if (MainForm.InvokeRequired)
                                    {
                                        MainForm.Invoke(new Action(() => Application.Restart()));
                                    }
                                    else
                                    {
                                        Application.Restart();
                                    }

                                }
                                catch (System.NullReferenceException ex)
                                {
                                    // If there was a 'NullReferenceException' (ClickOnce bug?), restarts the application in order to try updating it again once restarted.
                                    errMessage = ex.Message;


                                    if (MainForm.InvokeRequired)
                                    {
                                        MainForm.Invoke(new Action(() => Application.Restart()));
                                    }
                                    else
                                    {
                                        Application.Restart();
                                    }

                                }
                                catch (Exception ex)
                                {
                                    // Couldn't update itself programmatically,
                                    errMessage = ex.Message;
                                    MessageBox.Show(errMessage, "App_Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //MessageBox.Show(Properties.Resources.App_ApplicationUpdateError_WillRetryAfterStartingAgain, Properties.Resources.App_Warning,
                                    //MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Exits the application. To try updating when the user manually starts it.
                                    Application.Exit();
                                }

                                return true; // Returns 'true' to inform that the application must exit.

                            default:
                                return false;
                        }

                    }
                    catch (Exception ex1)//catch (DeploymentDownloadException ex1)
                    {

                        MessageBox.Show("Cannot install the latest version of the application. " + "\n" + "\n" + "Please check your network connection, or try again later." + ex1.Message);
                    }

                }

            }
            catch (Exception ex2)
            {
                errMessage = ex2.Message;
                MessageBox.Show(errMessage, "App_Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;

        }
    }
}
