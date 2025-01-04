## Orginal work belongs to  Mourdraug (Krzysztof Kowalczyk) I just make it work for .Net 8.0 ##

# FanControl.AsusWMI [![Build status](https://ci.appveyor.com/api/projects/status/mc33hki902w421le?svg=true)](https://ci.appveyor.com/project/Mourdraug/fancontrol-asuswmi)

Plugin for [FanControl .NET 4.8](https://github.com/Rem0o/FanControl.Releases) providing access to Asus motherboard sensors using Asus WMI methods.
...and plugin fork [FanControl .NET 8.0](https://github.com/jas7o/fancontrol-asuswmi_net80/releases) for people like me, who have issuu with plugin using Fancontrol .NET 8.0 Im keep getting error like "Plugin stop work, please refresh etc..."

## Installation

1. Download the latest binaries from:
[NET 4.8] [Releases](https://github.com/Mourdraug/FanControl.AsusWMI/releases)
[NET 8.0] [Releases](https://github.com/jas7o/fancontrol-asuswmi_net80/releases)

2. Copy the FanControl.AsusWMI.dll into FanControl's "Plugins" folder.

## Compatibility 

I tested the plugin with the Asus Crosshair Hero VII (WiFi) motherboard and VII Hero (without WIFI), but it should work with any motherboard using ASUSHW v3 and possibly v2.
The ASUSHW version can be checked with simple Powershell script (ran as administrator):
