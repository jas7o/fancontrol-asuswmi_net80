using FanControl.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace FanControl.AsusWMI
{
    public class AsusWMIPlugin : IPlugin2
    {
        ManagementObject asusHW;
        Dictionary<uint, List<AsusWMISensor>> sources;

        private readonly IPluginLogger logger;
        private readonly IPluginDialog dialog;

        public AsusWMIPlugin(IPluginDialog dialog)
        {
            this.dialog = dialog;
        }

        public AsusWMIPlugin(IPluginLogger logger, IPluginDialog dialog)
        {
            this.logger = logger;
            this.dialog = dialog;
        }

        public string Name => "Asus WMI";

        public void Close()
        {
            sources = null;
            asusHW.Dispose();
            asusHW = null;
        }

        public void Initialize()
        {
            try
            {
                asusHW = GetInstance(new ManagementScope(@"root\wmi"), "ASUSHW");
            }
            catch
            {
                logger.Log("AsusWMIPlugin Initialization failed.");
            }
        }

        public void Load(IPluginSensorsContainer _container)
        {
            if (asusHW is null)
            {
                return;
            }
            try
            {
                ManagementBaseObject wmiSensorNumResult = asusHW.InvokeMethod("sensor_get_number", null, null);
                uint sensorCount = (uint)wmiSensorNumResult["Data"];
                sources = [];
                for (uint i = 0; i < sensorCount; i++)
                {
                    var parameters = asusHW.GetMethodParameters("sensor_get_info");
                    parameters["Index"] = i;
                    ManagementBaseObject wmiSensorInfoResult = asusHW.InvokeMethod("sensor_get_info", parameters, null);
                    var source = (uint)wmiSensorInfoResult["Source"];
                    var sensorType = (uint)wmiSensorInfoResult["Type"];
                    var sensor = new AsusWMISensor()
                    {
                        WmiIndex = i,
                        WmiDataType = (uint)wmiSensorInfoResult["Data_Type"],
                        Id = $"AsusWMI{i}",
                        Name = wmiSensorInfoResult[nameof(Name)].ToString(),
                    };
                    if (!sources.TryGetValue(source, out List<AsusWMISensor> value))
                    {
                        value = [];
                        sources.Add(source, value);
                    }

                    value.Add(sensor);
                    switch (sensorType)
                    {
                        case 1:
                            _container.TempSensors.Add(sensor);
                            break;
                        case 2:
                            _container.FanSensors.Add(sensor);
                            break;
                    }
                }
                Update();
            }
            catch (ManagementException e)
            {
                logger.Log($"Loading sensors failed: {e.Message}");
            }
        }

        public void Update()
        {
            if (asusHW is null || sources is null)
            {
                return;
            }
            try
            {
                foreach (var source in sources)
                {
                    var sourceParam = asusHW.GetMethodParameters("sensor_update_buffer");
                    sourceParam["Source"] = source.Key;
                    asusHW.InvokeMethod("sensor_update_buffer", sourceParam, null);
                    foreach (var sensor in source.Value)
                    {
                        var indexParam = asusHW.GetMethodParameters("sensor_get_value");
                        indexParam["Index"] = sensor.WmiIndex;
                        ManagementBaseObject wmiSensorValueResult = asusHW.InvokeMethod("sensor_get_value", indexParam, null);
                        sensor.Value = sensor.WmiDataType != 3 ? Convert.ToSingle((uint)wmiSensorValueResult["Data"]) : Convert.ToSingle((uint)wmiSensorValueResult["Data"]) / 1000000f;
                    }
                }
            }
            catch (ManagementException e)
            {
                logger.Log($"Updating sensor values failed: {e.Message}");
            }
        }

        static ManagementObject GetInstance(ManagementScope scope, string path)
        {
            ManagementClass cls = new(scope.Path.Path, path, null);
            foreach (ManagementObject inst in cls.GetInstances().Cast<ManagementObject>())
            {
                return inst;
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            return obj is AsusWMIPlugin plugin &&
                   EqualityComparer<IPluginDialog>.Default.Equals(dialog, plugin.dialog);
        }
    }
}
