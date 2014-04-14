#DS4UI

## Idea

To create an easy to use and goodlooking interface for DS4 wrappers

![Demo Screenshot](http://xxxxxxxx.x)

### Status

Under development (Alpha 0.1) for Windows.
 
### Libraries

**Currently used:**
- [MahApps.Metro](http://mahapps.com/MahApps.Metro/) UI Toolkit for WPF
- [TaskbarNotification](http://www.codeproject.com/Articles/36468/WPF-NotifyIcon) NotificationIcon for WPF

## Building

### Dependencies

You will need MahApps.Metro.dll 0.12.1.0 .Net 4.0 version, System.Windows.Interactivity.dll and TaskbarNotification.dll 1.0.5.0

## Any problem?

### Regarding DS4ToolTester crashes
Due to the communication method to send information to the client, if you send too many information all together 
(i.e: moving the battery slider too fast) the Tester will crash. This is already acknowledged and will NOT be
fixed.
At the moment DS4ToolTester require admin rights because it needs to register the service, this will likely going to
change when we move registration during the installation.


## Development
- Run ServiceDestroyer.exe to remove previous version of DS4ToolService and configuration directories
- Run DS4ToolTester.exe with admin rights to register service
- Run DS4Tool.exe and simulate the use with it with DS4ToolTester
- Remember to close DS4Tool.exe before DS4ToolTester.
