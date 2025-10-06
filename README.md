# NotifyPanel

A Windows Forms application that receives and displays notification messages from a Firebird SQL Server.

#### Check NotifyClient for sending notifications to this panel

Firebird 3.0 User Defined Routine (UDR) for sending messages over TCP to the calling SQL client or any other IP client.

https://github.com/huh66/NotifyClient

## Features

- **TCP Server**: Listens for incoming messages on a configurable port (default: 1526)
- **Message Display**: Shows received messages in a popup dialog with different colors based on message level
- **Message Logging**: Saves all received messages to a log file
- **System Tray**: Minimizes to system tray with notification icon
- **Settings Dialog**: Configure port, minimize to tray behavior, and view message log
- **Multi-language Support**: English and German language support

## Message Format

The application expects JSON messages with the following structure:

```json
{
    "HEADER": "Message Header",
    "SUBJECT": "Message Subject", 
    "REFERENZ": 12345,
    "MESSAGE": "The actual message content",
    "LEVEL": "INFORMATION" // Optional: ERROR, WARN, INFO, INFORMATION
}
```

## Message Levels

- **ERROR**: Red background (Pastel red)
- **WARN**: Yellow background (Pastel yellow)  
- **INFO/INFORMATION**: White background (default)

## Configuration

Settings are stored in the Windows Registry under `HKEY_CURRENT_USER\Software\NotifyPanel`:

- **Port**: TCP port to listen on (1024-65535, default: 1526)
- **MinimizeToTray**: Whether to minimize to system tray (0/1)

## Log File

Message history is saved to: `%APPDATA%\NotifyPanel\message_history.log`

Format: `DateTime;Header;Subject;Reference;Message`

## Requirements

- .NET 8.0 Windows Runtime
- Windows operating system

## Usage

1. Start the application
2. The application will listen on the configured port (default: 1526)
3. Send JSON messages via TCP to the application
4. Messages will be displayed as popup dialogs
5. Use the system tray icon or File menu to access settings
6. Right-click the system tray icon for quick access to settings and exit

## Development

This is a C# Windows Forms application built with .NET 8.0.

### Project Structure

- `Form1.cs` - Main application form and TCP server
- `MessageForm.cs` - Message display dialog
- `SettingsDialog.cs` - Settings configuration dialog
- `Program.cs` - Application entry point
- `Properties/` - Application resources and settings

### Building

```bash
dotnet build
```

### Running

```bash
dotnet run
```

## License

Copyright (c) HUH
