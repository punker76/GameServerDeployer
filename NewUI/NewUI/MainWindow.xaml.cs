using System;
using System.Windows;
using System.IO;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace NewUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            serverType.Items.Add("Spigot");
            serverRAM.Items.Add("2G");
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
                    fdbShown = true;
                    if (fdbShown == true)
                    {
                        controller.SetTitle("Ports info");
                        controller.SetProgress(0.2);
                        if (tcpOpen == false && udpOpen == false)
                        {
                            controller.SetMessage("Neither of the two ports (UDP & TCP 25565) are opened, you can always modify the port in the config or open them later.");
                        }
                        if (tcpOpen == false && udpOpen == true)
                        {
                            controller.SetMessage("The TCP port 25565 isn't opened, you can always modify the port in the config or open it later. The UDP port 25565 is opened.");
                        }
                        if (tcpOpen == true && udpOpen == false)
                        {
                            controller.SetMessage("The UDP port 25565 isn't opened, you can always modify the port in the config or open it later. The TCP port 25565 is opened.");
                        }
                        if (tcpOpen == true && udpOpen == true)
                        {
                            controller.SetMessage("All of the ports (UDP & TCP 25565) are opened! :)");
                        }
                        Thread.Sleep(500);


                    }

                if (this.serverType.SelectedItem.ToString() == "Spigot")
                {
                    if (this.serverVersion.SelectedItem.ToString() == "1.8.8")
                    {
                        double MB = double.Parse(GetTotalMemoryInBytes().ToString());
                        if (this.serverRAM.SelectedItem.ToString() == "2G")
                        {
                            if (MB < 2000000)
                            {
                                await this.ShowMessageAsync("Error: not enough RAM", "You don't have enough RAM to run this server", MessageDialogStyle.Affirmative);

                                return;
                            }
                            else
                                {
                                    controller.SetTitle("Downloading files");
                                    controller.SetMessage("Downloading server files: eula.txt");
                                    WebClient webClient = new WebClient();
                                    webClient.DownloadFileAsync(new Uri("https://box.netly.co/ServerDeployer/eula.txt"), fbd.SelectedPath.ToString() + "\\eula.txt");
                                    WebClient webClient2 = new WebClient();
                                    controller.SetProgress(0.4);
                                    controller.SetMessage("Downloading server files: spigot.jar");
                                    webClient2.DownloadFileAsync(new Uri("https://box.netly.co/ServerDeployer/Spigot/1.8.8/spigot.jar"), fbd.SelectedPath.ToString() + "\\spigot.jar");
                                    controller.SetProgress(0.6);
                                    WebClient webClient3 = new WebClient();
                                    controller.SetMessage("Downloading server files: start.cmd");
                                    webClient3.DownloadFileAsync(new Uri("https://box.netly.co/ServerDeployer/Spigot/Start/2G/start.cmd"), fbd.SelectedPath.ToString() + "\\start.cmd");
                                    controller.SetProgress(0.8);
                                    controller.SetTitle("Starting server");
                                    controller.SetMessage("Starting the server for you :)");
                                    controller.SetProgress(0.9);
                                    webClient3.Dispose();

                                    Process.Start(fbd.SelectedPath + "\\start.cmd");
                                    controller.SetTitle("Deployment succesful!");
                                    controller.SetMessage("Your server was succesfully deployed and started. If you want to stop it, enter 'stop' in the console, if you want to start it again, open the folder and launch start.cmd\nEnjoy! :D");
                                    controller.SetProgress(1);
                                    Thread.Sleep(300);
                                    await controller.CloseAsync();



                                }

                        }

















                        }
                    }
                }
            }


            catch (System.NullReferenceException)
            {
                await this.ShowMessageAsync("Error", "Please choose something!", MessageDialogStyle.Affirmative);
            }
            
        }
        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }
    }

}


