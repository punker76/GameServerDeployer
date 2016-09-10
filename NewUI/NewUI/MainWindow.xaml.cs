using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Net.Sockets;
using System.Net;
using System.Threading;
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
            try
            {
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

                                        //MetroFramework.MetroMessageBox.Show(this, "The TCP port 25565 isn't opened. If you want to use another port, you can always modify it in the config. The server deployer will continue.", "TCP Port not opened", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    catch (Exception)
                                    {
                                        tcpOpen = false;
                                        tcpClient.Close();
                                        
                                    }
                                    using (UdpClient udpClient = new UdpClient())
                                        try
                                        {

                                            udpClient.Ttl = 10;
                                            udpClient.Connect("localhost", 25565);
                                            udpOpen = true;
                                            udpClient.Close();

                                            

                                        }
                                        catch (Exception)
                                        {
                                            udpOpen = false;
                                            udpClient.Close();
                                            
                                        }
                                    controller.SetTitle("Ports info");
                                    controller.SetProgress(0.1);
                                    if (tcpOpen == false && udpOpen == false)
                                    {
                                        
                                        controller.SetMessage("Neither of the two ports (UDP & TCP 25565) are opened, you can always modify the port in the config or open them later.");

                                    }
                                    if (tcpOpen == false && udpOpen == true)
                                    {
                                        controller.SetMessage("The TCP port 25565 isn't opened, you can always modify the port in the config or open it later.");
                                    }
                                    if (tcpOpen == true && udpOpen == false)
                                    {
                                        controller.SetMessage("The UDP port 25565 isn't opened, you can always modify the port in the config or open it later.");
                                    }
                                    if (tcpOpen == true && udpOpen == true)
                                    {
                                        controller.SetMessage("All of the ports (UDP & TCP 25565) are opened! :)");
                                    }
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
            //var controller = await this.ShowProgressAsync("wesh wesh", "wesh");
            //controller.SetProgress(0);
            //controller.SetMessage("Bonjour");
            //controller.SetProgress(0.9);
            
        }
        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }
    }

}


