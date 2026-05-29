namespace MainfreightProject;
// IAuthorize defines the role based access checks required by the internal dashboard this will support abstraction.
// Using an interface keeps permission rules abstract and prevents the GUI from depending on hardcoded role logic.
public interface IAuthorize
{
    bool CanUseStaffOperations(UserAccount account);
    bool CanManageShipmentRecords(UserAccount account);
    bool CanRunDepartmentOperations(UserAccount account);
    bool CanManageUserAccess(UserAccount account);
}