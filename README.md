# BroadlinkSharp

C# class library to control Broadlink devices.

The code in the lib is based on the Python implementation at https://github.com/mjg59/python-broadlink

Usage
-----

Use the static BroadlinkDiscovery class to discover all Broadlink devices on your network:

`List<BroadlinkDevice> devices = BroadLinkDiscovery.Discover(2, null);`

Use the Authorize method on the device classes to authorize access to the device, e.g.:

`dooyaCurtainMotor.Authorize();`


Use the methods in the BroadlinkDevice classes to trigger actions or get information on the device state, e.g. for a Dooya curtain motor:

`dooyaCurtainMotor.Open();`

Check the code of the BroadLinkSharpTest project for more examples.


Warning
-------
The only tested device class in this project is the class for Dooya curtain motors. The code of the other classes has been ported from Python, but never tested since I dont own these devices.

There is certainly a lot of room for improvments of this code. Please feel free to contribute to this project or to fork and extend the project.
