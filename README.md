# Employee Management System

A clean, modern Employee Management System built with ASP.NET Core MVC, showcasing clean architecture, layered design, comprehensive validation, structured logging, and containerized deployment.

## 🚀 Features

### Core Functionality
- **Employee CRUD Operations**: Complete Create, Read, Update, Delete functionality for employees
- **Department Management**: Full department lifecycle management with validation
- **Dashboard Analytics**: Real-time insights with employee counts, department distribution, and recent hires
- **Advanced Search & Filtering**: Search employees by name, email, or department
- **Pagination & Sorting**: Efficient data browsing with customizable page sizes and sorting options

### Technical Features
- **Clean Architecture**: Layered design with separation of concerns
- **Data Validation**: Comprehensive server-side validation with user-friendly error messages
- **Global Exception Handling**: Centralized error handling with structured logging
- **Responsive Design**: Mobile-first design using Bootstrap 5
- **Container Ready**: Multi-stage Docker build for production deployment

## 🛠 Tech Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Runtime**: .NET 8.0
- **Web Server**: Kestrel (containerized)
- **UI Framework**: ASP.NET MVC Razor Views + Bootstrap 5
- **Data Storage**: JSON file-based storage with thread-safe file locking
- **Logging**: Serilog with file and console outputs
- **Containerization**: Docker with multi-stage builds
- **Charts**: Chart.js for data visualization

## 📋 Prerequisites

- **Docker**: Docker Desktop 4.0+ (for containerized deployment)
- **Development**: 
  - .NET 8.0 SDK
  - Visual Studio 2022 17.10+ or Visual Studio Code
  
## 🚀 Quick Start

### Option 1: Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EmployeeManagementSystem
   ```

2. **Build and run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

3. **Access the application**
   - Open your browser and navigate to: `http://localhost:8080`

### Option 2: Local Development

1. **Clone and restore packages**
   ```bash
   git clone <repository-url>
   cd EmployeeManagementSystem
   dotnet restore
   ```

2. **Run the application**
   ```bash
   dotnet run
   ```

3. **Access the application**
   - Navigate to: `https://localhost:5001` or `http://localhost:5000`

## 🏗 Architecture Overview

### Project Structure
```
EmployeeManagementSystem/
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs
│   ├── EmployeesController.cs
│   └── DepartmentsController.cs
├── Models/               # Data Models and ViewModels
│   ├── Employee.cs
│   ├── Department.cs
│   └── DashboardViewModel.cs
├── Services/             # Business Logic Layer
│   ├── EmployeeService.cs
│   └── DepartmentService.cs
├── Repositories/         # Data Access Layer
│   ├── JsonEmployeeRepository.cs
│   └── JsonDepartmentRepository.cs
├── Middleware/           # Custom Middleware
│   └── GlobalExceptionMiddleware.cs
├── Views/               # Razor Views
│   ├── Home/
│   ├── Employees/
│   ├── Departments/
│   └── Shared/
├── App_Data/            # JSON Data Storage
├── Logs/               # Application Logs
└── wwwroot/            # Static Assets
```

### Architecture Principles

**Clean Architecture**: The application follows clean architecture principles with clear separation between:
- **Presentation Layer**: MVC Controllers and Views
- **Business Logic Layer**: Services containing business rules
- **Data Access Layer**: Repositories handling data persistence
- **Cross-cutting Concerns**: Middleware for logging and error handling

**Dependency Injection**: All dependencies are injected through the built-in DI container, promoting loose coupling and testability.

**Repository Pattern**: Data access is abstracted through repository interfaces, allowing for easy swapping of data storage mechanisms.

## 📊 Data Model

### Employee Entity
```csharp
public class Employee
{
    public int Id { get; set; }                    // Auto-increment primary key
    public string FirstName { get; set; }          // Required
    public string LastName { get; set; }           // Required  
    public string Email { get; set; }              // Required, valid email format
    public DateTime HireDate { get; set; }         // Cannot be in future
    public decimal Salary { get; set; }            // Must be > 0
    public int DepartmentId { get; set; }          // Foreign key to Department
}
```

### Department Entity
```csharp
public class Department
{
    public int Id { get; set; }                    // Auto-increment primary key
    public string Name { get; set; }               // Required
    public string Description { get; set; }        // Optional
}
```

## 🔒 Validation Rules

### Employee Validation
- **FirstName & LastName**: Required fields
- **Email**: Required, must be valid email format
- **HireDate**: Required, cannot be in the future
- **Salary**: Required, must be greater than 0
- **DepartmentId**: Required, must reference existing department

### Department Validation
- **Name**: Required field
- **Description**: Optional field
- **Deletion**: Cannot delete department with active employees

## 📝 Logging & Error Handling

### Global Exception Middleware
- Catches all unhandled exceptions
- Logs detailed error information to JSON files
- Returns user-friendly error pages
- Includes error tracking with unique error IDs

### Structured Logging
- **File Logging**: Daily rolling log files in `Logs/log-YYYY-MM-DD.json`
- **Console Logging**: Real-time logging during development
- **Log Levels**: Error, Warning, Information, Debug

### Error Log Format
```json
{
  "ErrorId": "guid",
  "Timestamp": "2025-01-XX",
  "Message": "Exception message",
  "StackTrace": "Full stack trace",
  "RequestPath": "/controller/action",
  "RequestMethod": "GET/POST",
  "UserAgent": "Browser info",
  "IPAddress": "Client IP"
}
```

## 🐳 Docker Configuration

### Multi-Stage Build
The Dockerfile uses a multi-stage build process:

1. **Base Stage**: Sets up the runtime environment
2. **Build Stage**: Compiles the application
3. **Publish Stage**: Creates the deployment package
4. **Final Stage**: Creates the minimal production image

### Volume Mapping
- **App_Data**: Persistent storage for JSON data files
- **Logs**: Persistent storage for application logs

### Health Checks
- Configured health check endpoint
- 30-second intervals with 3 retry attempts
- 40-second startup grace period

## 🌟 Key Features Walkthrough

### Dashboard
- **Total Employee Count**: Live employee statistics
- **Department Distribution**: Interactive bar chart showing employee distribution
- **Recent Hires**: Last 30 days hiring activity
- **Quick Search**: Real-time employee search functionality

### Employee Management
- **List View**: Paginated table with sorting and filtering
- **Create/Edit**: Form validation with user-friendly error messages
- **Details View**: Comprehensive employee information display
- **Delete Confirmation**: Protected deletion with confirmation dialog

### Department Management
- **Card-Based Layout**: Visual department overview
- **CRUD Operations**: Complete department lifecycle management
- **Employee Protection**: Prevents deletion of departments with active employees
- **Department Linking**: Quick navigation to department employees

## 🧪 Testing the Application

### Sample Data
The application automatically initializes with sample departments:
- Human Resources
- Information Technology  
- Finance
- Marketing
- Operations

### Test Scenarios
1. **Create Employee**: Add new employee with validation testing
2. **Department Constraints**: Try deleting department with employees
3. **Search Functionality**: Test employee search across different fields
4. **Pagination**: Test with different page sizes and navigation
5. **Error Handling**: Test invalid data submission

## 🔧 Development Setup

### Running Tests
```bash
dotnet test
```

### Development Mode
```bash
dotnet run --environment Development
```

### Production Build
```bash
dotnet publish -c Release
```

## 📦 Deployment

### Docker Production Deployment
```bash
# Build production image
docker build -t employee-management-system .

# Run container
docker run -d \
  --name emp-mgmt \
  -p 8080:8080 \
  -v emp_data:/app/App_Data \
  -v emp_logs:/app/Logs \
  employee-management-system
```

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Set to `Production` for production deployment
- `ASPNETCORE_URLS`: Configure binding URLs (default: `http://+:8080`)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📋 Todo / Future Enhancements

- [ ] Add unit tests with xUnit
- [ ] Implement database storage option (Entity Framework)
- [ ] Add authentication and authorization
- [ ] Export functionality (PDF, Excel)
- [ ] Email notifications for new hires
- [ ] Employee photo upload
- [ ] Advanced reporting features
- [ ] API endpoints for mobile app integration

## 📄 License

This project is created for educational and assessment purposes.


## 🙋‍♂️ Support

For questions or support, please open an issue in the GitHub repository.

---

**Built with ❤️ using ASP.NET Core 8.0**