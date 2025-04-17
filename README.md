# Volunteer Tracking Project

This is a console-based volunteer tracking system built in **.NET 8** using **Spectre.Console** for a modern terminal UI. It was developed as projectm for CS690 - Software Engineering.

## Features

- User registration and login
- Activity logging with time conflict detection
- View, edit, and cancel upcoming volunteer activities
- Impact reports summarizing volunteer contributions
- Colorful and interactive UI using Spectre.Console

## Related Documents

- [Deployment Guides](https://github.com/rommeldijon/CS690-VolunteerTrackingProject/wiki/Deployment-Guides)
- [User Guides](https://github.com/rommeldijon/CS690-VolunteerTrackingProject/wiki/User-Guides)
- [Developer Guides](https://github.com/rommeldijon/CS690-VolunteerTrackingProject/wiki/Developer-Guides)
  
## Project Structure

- `Program.cs`: Entry point and main menu handler
- `Authentication.cs`: Handles login, registration, and password reset
- `ActivityLogger.cs`, `ActivityViewer.cs`, `ActivityManager.cs`: Manage and display volunteer activities
- `ReportGenerator.cs`: Generates impact summaries
- `Validator.cs`: Input validation logic
- `Tests/`: Unit test files for core components

## Author

**Rommel Dijon** – CS690 Spring 2025  
GitHub: [@rommeldijon](https://github.com/rommeldijon)

## License

This project is for academic use and demonstration purposes.

