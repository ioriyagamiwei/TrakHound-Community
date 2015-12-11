﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Xml;
using System.Data;

using TH_Configuration;
using TH_Database;
using TH_Device_Server;
using TH_Global;
using TH_UserManagement;
using TH_UserManagement.Management;

namespace TrakHound_Server_Core
{
    public partial class Server
    {
        List<Device_Server> Devices = new List<Device_Server>();

        List<Configuration> configurations;

        void LoadDevices()
        {
            if (currentuser != null)
            {
                string[] tablenames = Configurations.GetConfigurationsForUser(currentuser, userDatabaseSettings);

                foreach (string tablename in tablenames)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(LoadConfiguration), tablename);
                }
            }
            // If not logged in Read from File in 'C:\TrakHound\'
            else
            {
                configurations = ReadConfigurationFile();
            }

            DevicesMonitor_Initialize();
        }

        void LoadConfiguration(object o)
        {
            if (o != null)
            {
                string tablename = o.ToString();

                DataTable dt = Configurations.GetConfigurationTable(tablename, userDatabaseSettings);
                if (dt != null)
                {
                    XmlDocument xml = Converter.TableToXML(dt);
                    Configuration config = Configuration.ReadConfigFile(xml);
                    if (config != null)
                    {
                        config.TableName = tablename;

                        if (config.ServerEnabled)
                        {
                            config.Index = Devices.Count;

                            Device_Server server = new Device_Server(config);

                            // Initialize Database Configurations
                            Global.Initialize(server.configuration.Databases_Server);

                            server.Start(false);

                            if (Devices.Find(x => x.configuration.UniqueId == config.UniqueId) == null) Devices.Add(server);
                        }
                    }
                }
            }
        }

        #region "Devices Monitor"

        System.Timers.Timer devicesMonitor_TIMER;

        void DevicesMonitor_Initialize()
        {
            if (devicesMonitor_TIMER != null) devicesMonitor_TIMER.Enabled = false;

            devicesMonitor_TIMER = new System.Timers.Timer();
            devicesMonitor_TIMER.Interval = 5000;
            devicesMonitor_TIMER.Elapsed += devicesMonitor_TIMER_Elapsed;
            devicesMonitor_TIMER.Enabled = true;
        }

        void devicesMonitor_TIMER_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(DevicesMonitor_Worker), Devices.ToList());
        }

        void DevicesMonitor_Worker(object o)
        {
            if (o != null)
            {
                List<Device_Server> devs = (List<Device_Server>)o;

                if (currentuser != null)
                {
                    string[] tablenames = Configurations.GetConfigurationsForUser(currentuser, userDatabaseSettings);
                    if (tablenames != null)
                    {
                        foreach (string tablename in tablenames)
                        {
                            Configuration config = GetConfiguration(tablename);
                            if (config != null)
                            {
                                config.TableName = tablename;

                                Device_Server server = Devices.Find(x => x.configuration.UniqueId == config.UniqueId);
                                if (server != null) // Server is already part of list
                                {
                                    // Check if Configuration has changed
                                    if (server.configuration.ServerUpdateId != config.ServerUpdateId)
                                    {
                                        server.Stop();

                                        server.configuration = config;

                                        if (config.ServerEnabled)
                                        {
                                            // Initialize Database Configurations
                                            Global.Initialize(server.configuration.Databases_Server);

                                            server.Start(false);
                                        }
                                    }
                                }
                                else // Create & Add Device Server
                                {
                                    LoadConfiguration(tablename);
                                }
                            }
                        }

                        // Remove any server that was removed from user
                        foreach (Device_Server server in Devices.ToList())
                        {
                            string ExistingTable = tablenames.ToList().Find(x => x == server.configuration.TableName);
                            if (ExistingTable == null)
                            {
                                server.Stop();
                                int index = Devices.FindIndex(x => x.configuration.UniqueId == server.configuration.UniqueId);
                                if (index >= 0) Devices.RemoveAt(index);
                            }
                        }
                    }
                    else
                    {
                        foreach (Device_Server server in Devices) server.Stop();
                        Devices.Clear();
                    }  
                }
            }
        }

        Configuration GetConfiguration(string tablename)
        {
            if (tablename != null)
            {
                DataTable dt = Configurations.GetConfigurationTable(tablename, userDatabaseSettings);
                if (dt != null)
                {
                    XmlDocument xml = Converter.TableToXML(dt);
                    Configuration config = Configuration.ReadConfigFile(xml);
                    if (config != null) return config;
                }
            }

            return null;
        }

        #endregion

        #region "Xml File"

        List<Configuration> ReadConfigurationFile()
        {
            List<Configuration> result = new List<Configuration>();

            string configPath;

            string localPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + "Configuration.Xml";
            string systemPath = TH_Global.FileLocations.TrakHound + @"\" + "Configuration.Xml";

            // systemPath takes priority (easier for user to navigate to)
            if (File.Exists(systemPath)) configPath = systemPath;
            else configPath = localPath;

            if (System.IO.File.Exists(configPath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configPath);

                foreach (XmlNode Node in doc.DocumentElement.ChildNodes)
                {
                    if (Node.NodeType == XmlNodeType.Element)
                    {
                        switch (Node.Name.ToLower())
                        {
                            case "devices":
                                foreach (XmlNode ChildNode in Node.ChildNodes)
                                {
                                    if (ChildNode.NodeType == XmlNodeType.Element)
                                    {
                                        switch (ChildNode.Name.ToLower())
                                        {
                                            case "device":

                                                Configuration config = GetSettingsFromNode(ChildNode);
                                                if (config != null) result.Add(config);

                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            return result;
        }

        private Configuration GetSettingsFromNode(XmlNode Node)
        {

            Configuration Result = null;

            string configPath = null;

            foreach (XmlNode ChildNode in Node.ChildNodes)
            {
                switch (ChildNode.Name.ToLower())
                {
                    case "configuration_path": configPath = ChildNode.InnerText; break;
                }
            }

            if (configPath != null)
            {
                configPath = GetConfigurationPath(configPath);

                Result = Configuration.ReadConfigFile(configPath);
            }

            return Result;

        }

        static Device_Server ProcessDevice(int index, XmlNode node)
        {
            Device_Server Result = null;

            string configPath = null;

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    if (childNode.Name.ToLower() == "configuration_path")
                    {
                        configPath = childNode.InnerText;
                    }
                }
            }

            if (configPath != null)
            {
                configPath = GetConfigurationPath(configPath);

                Logger.Log("Reading Device Configuration File @ '" + configPath + "'");

                if (File.Exists(configPath))
                {
                    Configuration config = new Configuration();
                    config = Configuration.ReadConfigFile(configPath);

                    if (config != null)
                    {
                        Console.WriteLine("Device Congifuration Read Successfully!");

                        config.Index = index;

                        Device_Server server = new Device_Server(config);
                        server.configurationPath = configPath;
                        server.updateConfigurationFile = false;

                        // Initialize Database Configurations
                        Global.Initialize(server.configuration.Databases_Server);

                        Result = server;
                    }
                    else Logger.Log("Error Occurred While Reading : " + configPath);
                }
                else Logger.Log("Can't find Device Configuration file @ " + configPath);
            }
            else Logger.Log("No Device Congifuration found");

            return Result;

        }

        static string GetConfigurationPath(string path)
        {
            // If not full path, try System Dir ('C:\TrakHound\') and then local App Dir
            if (!System.IO.Path.IsPathRooted(path))
            {
                // Remove initial Backslash if contained in "configuration_path"
                if (path[0] == '\\' && path.Length > 1) path.Substring(1);

                string original = path;

                // Check System Path
                path = TH_Global.FileLocations.TrakHound + "\\Configuration Files\\" + original;
                if (File.Exists(path)) return path;
                else Logger.Log(path + " Not Found");


                // Check local app Path
                path = AppDomain.CurrentDomain.BaseDirectory + "Configuration Files\\" + original;
                if (File.Exists(path)) return path;
                else Logger.Log(path + " Not Found");

                // if no files exist return null
                return null;
            }
            else return path;
        }

        #endregion

    }
}
