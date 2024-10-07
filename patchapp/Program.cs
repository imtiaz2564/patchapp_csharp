using System.Text;
using System.Net;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Management;

namespace patchApp
{
    class PatchApp
    {
        static async Task Main(string[] args)
        {
            // system data from existing machine
            //var sendData = GetSystemInfo();

            // system data from file
            var sendData = GetSystemDataFromFile();
            // Send the data to the Django API
            await SendDataToDjangoApi(sendData);
        }

        // Function to collect data from file (system name, IP address, and OS version)
        static SystemProp GetSystemDataFromFile()
        {
            string filePath = @"F:\project\patchapp\patchapp\file.txt";
            var systemData = SystemInfo.GetSystemDataFromFile(filePath);
            Console.WriteLine($"System Name: {systemData.SystemName}");
            Console.WriteLine($"IP Address: {systemData.IPAddress}");
            Console.WriteLine($"OS Version: {systemData.OSVersion}");

            return systemData;
        }

        // Function to collect existing system data 
        static SystemProp GetSystemInfo()
        {
            string systemName = Environment.MachineName;
            string IP = Dns.GetHostEntry(systemName).AddressList[0].ToString();
            string osVersion = GetOSVersion();

            var systemData = new SystemProp
            {
                SystemName = systemName,
                IPAddress = IP,
                OSVersion = osVersion.ToString()
            };

            Console.WriteLine($"System Name: {systemData.SystemName}");
            Console.WriteLine($"IP Address: {systemData.IPAddress}");
            Console.WriteLine($"OS Version: {systemData.OSVersion}");

            return systemData;
        }
        static string GetOSVersion()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
                    {
                        foreach (ManagementObject os in searcher.Get())
                        {
                            return os["Caption"]?.ToString() ?? "Unknown Windows OS";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving Windows OS version: {ex.Message}");
                    return "Unknown Windows OS";
                }
            }
            else
            {
                // For non-Windows platforms
                return RuntimeInformation.OSDescription;
            }

            return "Unknown OS";
        }

        // Function to send data to the Django API
        static async Task SendDataToDjangoApi(SystemProp sendData)
        {
            using (HttpClient client = new HttpClient())
            {
                string baseUrl = "http://127.0.0.1:8000/";
                client.BaseAddress = new Uri(baseUrl);
                string json = JsonSerializer.Serialize(sendData);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync("api/data/", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response from Django API: " + responseData);
                    }
                    else
                    {
                        Console.WriteLine($"API Request failed with status code: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }
        }
    }
}