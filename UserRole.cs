namespace MainfreightProject;

// UserRole defines the internal account roles supported by the system.
// Customers do not need login accounts because public shipment tracking is available separately will prtect abstraction .
// Staff and Admin roles are used to control access to internal dashboard workflows.

public enum UserRole
{
    Staff,
    Admin
}