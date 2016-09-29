using System;
using System.Windows;
using System.IO;
using System.Drawing;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

namespace NewUI
{
    public static class Globals
    {
        public static bool EulaDL = false;
        public static bool ServerDL = false;
        public static bool StartDL = false;
        public static string fbdSel;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            serverType.Items.Add("Spigot");
            serverRAM.Items.Add("1G");
            serverRAM.Items.Add("2G");
            serverRAM.Items.Add("3G");
            serverRAM.Items.Add("4G");
            serverRAM.Items.Add("5G");
            serverRAM.Items.Add("6G");
            serverRAM.Items.Add("7G");
            serverRAM.Items.Add("8G");
            serverVersion.Items.Add("1.8.8");





        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }


        private async void button_Click_1(object sender, RoutedEventArgs e)
        {
            bool fdbShown = false;
            MessageDialogResult msgRes = await this.ShowMessageAsync("EULA", "Do you accept the Minecraft EULA? \n(https://account.mojang.com/documents/minecraft_eula)", MessageDialogStyle.AffirmativeAndNegative);
            if (msgRes == MessageDialogResult.Negative)
            {
                await this.ShowMessageAsync("EULA", "You need to accept the Minecraft EULA to host your own server.");
                return;
            }

            try
            {


                var controller = await this.ShowProgressAsync("Initialization", "Starting process");
                controller.SetProgress(0);
                bool tcpOpen = false;
                bool udpOpen = false;
                using (TcpClient tcpClient = new TcpClient())
                {
                    try
                    {
                        tcpClient.Connect("localhost", 25565);
                        tcpOpen = true;
                        tcpClient.Close();
                    }
                    catch (Exception)
                    {
                        tcpOpen = false;
                        tcpClient.Close();

                    }
                    using (UdpClient udpClient = new UdpClient())
                        try
                        {
                            udpClient.Connect("localhost", 25565);
                            udpOpen = true;
                            udpClient.Close();
                        }
                        catch (Exception)
                        {
                            udpOpen = false;
                            udpClient.Close();
                        }
                    await this.ShowMessageAsync("Choosing path", "Ok, so you're going to choose the path for your server", MessageDialogStyle.Affirmative);
                    controller.SetTitle("Path");
                    controller.SetMessage("Choosing the path");
                    controller.SetProgress(0.1);

                    FolderBrowserDialog fbd = new FolderBrowserDialog();

                    DialogResult result = fbd.ShowDialog();
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel || fbd.ShowDialog() == System.Windows.Forms.DialogResult.Abort)
                    {
                        await controller.CloseAsync();
                        await this.ShowMessageAsync("Error", "You need to choose a path to install a server.", MessageDialogStyle.Affirmative);
                        Thread.Sleep(2000);
                        return;
                    }
                    fdbShown = true;
                    if (fdbShown == true)
                    {
                        controller.SetTitle("Ports info");
                        controller.SetProgress(0.2);
                        if (tcpOpen == false && udpOpen == false)
                        {
                            await this.ShowMessageAsync("Ports info", "Neither of the two ports (UDP & TCP 25565) are opened, you can always modify the port in the config or open them later.", MessageDialogStyle.Affirmative);
                            
                        }
                        if (tcpOpen == false && udpOpen == true)
                        {
                            await this.ShowMessageAsync("Ports info", "The TCP port 25565 isn't opened, you can always modify the port in the config or open it later. The UDP port 25565 is opened.", MessageDialogStyle.Affirmative);
                            
                        }
                        if (tcpOpen == true && udpOpen == false)
                        {
                            await this.ShowMessageAsync("Ports info", "The UDP port 25565 isn't opened, you can always modify the port in the config or open it later. The TCP port 25565 is opened.");
                            
                        }
                        if (tcpOpen == true && udpOpen == true)
                        {
                            await this.ShowMessageAsync("Ports info", "All of the ports (UDP & TCP 25565) are opened! :)", MessageDialogStyle.Affirmative);
                            
                        }


                    }
                            controller.SetTitle("Downloading files");
                            controller.SetMessage("Downloading server files: eula.txt");
                            WebClient webClient = new WebClient();
                            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(EulaDL);
                            webClient.DownloadFileAsync(new Uri("https://box.netly.co/ServerDeployer/eula.txt"), fbd.SelectedPath.ToString() + "\\eula.txt");
                            WebClient webClient2 = new WebClient();
                            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(serverDLProg);
                            webClient2.DownloadFileCompleted += new AsyncCompletedEventHandler(ServerDL);
                            controller.SetProgress(0.4);
                            controller.SetMessage("Downloading server files: server.jar");
                            webClient2.DownloadFileAsync(new Uri("https://box.netly.co/ServerDeployer/" + this.serverType.SelectedItem.ToString() + "/" + this.serverVersion.SelectedItem.ToString() + "/spigot.jar"), fbd.SelectedPath.ToString() + "\\server.jar");
                            controller.SetProgress(0.5);
                            //WebClient webClient3 = new WebClient();
                            //webClient3.DownloadFileCompleted += new AsyncCompletedEventHandler(StartDL);
                            //controller.SetMessage("Downloading server files: start.cmd");
                            //System.Windows.MessageBox.Show("test1" + this.serverRAM.SelectedItem.ToString() + "");
                            //webClient3.DownloadFileAsync(new Uri("https://box.netly.co/ServerDeployer/Start" + this.serverRAM.SelectedItem.ToString() + "/start.cmd"), fbd.SelectedPath.ToString() + "\\start.cmd");
                            //System.Windows.MessageBox.Show("test2" + this.serverRAM.SelectedItem.ToString() + "");
                            //controller.SetProgress(0.8);
                            controller.SetMessage("Copying files: start.cmd");
                            controller.SetProgress(0.6);
                            File.Copy(System.Windows.Forms.Application.StartupPath + "\\Start\\" + this.serverRAM.SelectedItem.ToString() + "\\start.cmd", fbd.SelectedPath.ToString() + "/start.cmd");
                            
                            Globals.StartDL = true;
                            Globals.fbdSel = fbd.SelectedPath.ToString();
                            await controller.CloseAsync();
                }


            }


            catch (System.NullReferenceException)
            {
                await this.ShowMessageAsync("Error", "Please choose something!", MessageDialogStyle.Affirmative);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("" + ex.ToString() + ".");
            }

        }
        async private void serverDLProg(object sender, DownloadProgressChangedEventArgs e)
        {
            var controller = await this.ShowProgressAsync("Downloading server files", "Downloading files: server.jar");
            controller.SetProgress(0.8);
            if (e.ProgressPercentage >= 99)
            {
                controller.CloseAsync();
            }
            else
            {

            }
        }
        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }
        private void EulaDL(object sender, AsyncCompletedEventArgs e)
        {
            Globals.EulaDL = true;
        }
        async private void ServerDL(object sender, AsyncCompletedEventArgs e)
        {
            Globals.ServerDL = true;
            if (Globals.EulaDL == true && Globals.ServerDL == true && Globals.StartDL == true)
            {
                Process.Start(Globals.fbdSel.ToString() + "\\start.cmd");
                var controller = await this.ShowProgressAsync("Starting server", "Starting the server for you!");
                controller.SetProgress(0.9);
                string strCmdText = "/C cd " + Globals.fbdSel.ToString() + " & start.cmd";
                Process.Start("CMD.exe", strCmdText);
                await this.ShowMessageAsync("Deployment successful", "Your server was succesfully deployed and started. If you want to stop it, enter 'stop' in the console, if you want to start it again, open the folder and launch start.cmd\nEnjoy! :D");
                await controller.CloseAsync();
            }
        }
        
        }
    }



