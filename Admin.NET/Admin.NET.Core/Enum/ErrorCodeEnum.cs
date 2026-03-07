// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// System error code
/// </summary>
[ErrorCodeType]
[Description("System error code")]
public enum ErrorCodeEnum
{
    /// <summary>
    /// Verification code error
    /// </summary>
    [ErrorCodeItemMetadata("Verification code error")]
    D0008,

    /// <summary>
    /// Account does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Account does not exist")]
    D0009,

    /// <summary>
    /// Key mismatch
    /// </summary>
    [ErrorCodeItemMetadata("Key mismatch")]
    D0010,

    /// <summary>
    /// The account or password is incorrect
    /// </summary>
    [ErrorCodeItemMetadata("The account or password is incorrect")]
    D1000,

    /// <summary>
    /// Illegal operation! Forbidden to delete yourself
    /// </summary>
    [ErrorCodeItemMetadata("Illegal operation, deleting yourself is prohibited")]
    D1001,

    /// <summary>
    /// Record does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Record does not exist")]
    D1002,

    /// <summary>
    /// Account already exists
    /// </summary>
    [ErrorCodeItemMetadata("The account already exists")]
    D1003,

    /// <summary>
    /// Old password does not match
    /// </summary>
    [ErrorCodeItemMetadata("Old password entered incorrectly")]
    D1004,

    ///// <summary>
    ///// Test data prohibits changing admin password
    ///// </summary>
    //[ErrorCodeItemMetadata("Test data prohibits changing user [admin] password")]
    //D1005,

    /// <summary>
    /// Data already exists
    /// </summary>
    [ErrorCodeItemMetadata("Data already exists")]
    D1006,

    /// <summary>
    /// The data does not exist or contains associated references and is prohibited from deletion.
    /// </summary>
    [ErrorCodeItemMetadata("The data does not exist or contains related references, deletion is prohibited.")]
    D1007,

    /// <summary>
    /// Disable assigning roles to administrators
    /// </summary>
    [ErrorCodeItemMetadata("Prohibit assigning roles to administrators")]
    D1008,

    /// <summary>
    /// Duplicate data or records containing non-existent data
    /// </summary>
    [ErrorCodeItemMetadata("Duplicate data or records containing non-existent data")]
    D1009,

    /// <summary>
    /// Disable assigning permissions to super administrator role
    /// </summary>
    [ErrorCodeItemMetadata("Prohibit assigning permissions to the super administrator role")]
    D1010,

    /// <summary>
    /// Illegal operation, not logged in
    /// </summary>
    [ErrorCodeItemMetadata("Illegal operation, not logged in")]
    D1011,

    /// <summary>
    /// Id cannot be empty
    /// </summary>
    [ErrorCodeItemMetadata("Id cannot be empty")]
    D1012,

    /// <summary>
    /// The affiliated institution is not within the scope of your own data
    /// </summary>
    [ErrorCodeItemMetadata("You do not have permission to operate on this data")]
    D1013,

    /// <summary>
    /// Disable deletion of super administrator
    /// </summary>
    [ErrorCodeItemMetadata("Prohibit deleting the super administrator")]
    D1014,

    /// <summary>
    /// Modification of super administrator status is prohibited
    /// </summary>
    [ErrorCodeItemMetadata("Modification of super administrator status is prohibited")]
    D1015,

    /// <summary>
    /// permission denied
    /// </summary>
    [ErrorCodeItemMetadata("No permission")]
    D1016,

    /// <summary>
    /// Account has been frozen
    /// </summary>
    [ErrorCodeItemMetadata("Account has been frozen")]
    D1017,

    /// <summary>
    /// The role menu permission set under this tenant is empty
    /// </summary>
    [ErrorCodeItemMetadata("The role menu permission set under this tenant is empty")]
    D1019,

    /// <summary>
    /// Disable deletion of default tenant
    /// </summary>
    [ErrorCodeItemMetadata("Deleting the default tenant is prohibited")]
    D1023,

    /// <summary>
    /// The account logged in from other places has been offline.
    /// </summary>
    [ErrorCodeItemMetadata("has beenOtherPlaceLoginAccount numberoffline")]
    D1024,

    /// <summary>
    /// There are accounts under this role that cannot be deleted.
    /// </summary>
    [ErrorCodeItemMetadata("There are accounts under this role that cannot be deleted.")]
    D1025,

    /// <summary>
    /// It is prohibited to modify my account status
    /// </summary>
    [ErrorCodeItemMetadata("Prohibit changing my account status")]
    D1026,

    /// <summary>
    /// The password has been entered incorrectly too many times and the account has been locked. Please try again in half an hour!
    /// </summary>
    [ErrorCodeItemMetadata("The password has been entered incorrectly too many times and the account has been locked. Please try again in half an hour!")]
    D1027,

    /// <summary>
    /// The new password cannot be the same as the old password
    /// </summary>
    [ErrorCodeItemMetadata("The new password cannot be the same as the old password")]
    D1028,

    /// <summary>
    /// The system default account is prohibited from deletion
    /// </summary>
    [ErrorCodeItemMetadata("The system default account cannot be deleted")]
    D1029,

    /// <summary>
    /// Deletion of accounts bound to open interfaces is prohibited
    /// </summary>
    [ErrorCodeItemMetadata("Open interface bound accounts cannot be deleted")]
    D1030,

    /// <summary>
    /// Deletion of the open interface bound tenant is prohibited
    /// </summary>
    [ErrorCodeItemMetadata("Open interface bound tenant deletion prohibited")]
    D1031,

    /// <summary>
    /// Mobile phone number already exists
    /// </summary>
    [ErrorCodeItemMetadata("The phone number already exists")]
    D1032,

    /// <summary>
    /// There is a registration scheme under this role that prohibits deletion
    /// </summary>
    [ErrorCodeItemMetadata("There is a registration scheme under this role that prohibits deletion")]
    D1033,

    /// <summary>
    /// The registration function is not enabled and registration is prohibited.
    /// </summary>
    [ErrorCodeItemMetadata("The registration function is not enabled and registration is prohibited.")]
    D1034,

    /// <summary>
    /// Registration plan does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Registration plan does not exist")]
    D1035,

    /// <summary>
    /// role does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Character does not exist")]
    D1036,

    /// <summary>
    /// Registration of super administrators and system administrators is prohibited
    /// </summary>
    [ErrorCodeItemMetadata("Prohibit the registration of super administrators and system administrators")]
    D1037,

    /// <summary>
    /// Prohibiting unauthorized operating system accounts
    /// </summary>
    [ErrorCodeItemMetadata("Prohibit unauthorized operation of system accounts")]
    D1038,

    /// <summary>
    /// Parent organization does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Parent organization does not exist")]
    D2000,

    /// <summary>
    /// The current organization ID cannot be the same as the parent organization ID
    /// </summary>
    [ErrorCodeItemMetadata("The current organization ID cannot be the same as the parent organization ID")]
    D2001,

    /// <summary>
    /// Already have the same organizational structure, code or name
    /// </summary>
    [ErrorCodeItemMetadata("An organization with the same structure, code, or name already exists")]
    D2002,

    /// <summary>
    /// No authority to operate the organization
    /// </summary>
    [ErrorCodeItemMetadata("No authority to operate the organization")]
    D2003,

    /// <summary>
    /// There are users under this organization who are prohibited from deletion
    /// </summary>
    [ErrorCodeItemMetadata("There are users under this organization who are prohibited from deletion")]
    D2004,

    /// <summary>
    /// Users under affiliated organizations are prohibited from deletion
    /// </summary>
    [ErrorCodeItemMetadata("Users under affiliated organizations are prohibited from deletion")]
    D2005,

    /// <summary>
    /// Only subordinate organizations can be added
    /// </summary>
    [ErrorCodeItemMetadata("Only subordinate organizations can be added")]
    D2006,

    /// <summary>
    /// Users under subordinate organizations are prohibited from deletion
    /// </summary>
    [ErrorCodeItemMetadata("Users under subordinate organizations are prohibited from deletion")]
    D2007,

    /// <summary>
    /// System default organization prohibits deletion
    /// </summary>
    [ErrorCodeItemMetadata("System default organization prohibits deletion")]
    D2008,

    /// <summary>
    /// It is prohibited to add root node institutions
    /// </summary>
    [ErrorCodeItemMetadata("Prohibitedincreaseroot nodemechanism")]
    D2009,

    /// <summary>
    /// There is a registration scheme under this organization that prohibits deletion
    /// </summary>
    [ErrorCodeItemMetadata("thismechanismExists belowRegistration planProhibitedDelete")]
    D2010,

    /// <summary>
    /// Organization does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Organization does not exist")]
    D2011,

    /// <summary>
    /// System default organization prohibits modification
    /// </summary>
    [ErrorCodeItemMetadata("System default organization cannot be modified")]
    D2012,

    /// <summary>
    /// Dictionary type does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Dictionary type does not exist")]
    D3000,

    /// <summary>
    /// Dictionary type already exists
    /// </summary>
    [ErrorCodeItemMetadata("Dictionary type already exists, name or encoding is duplicated")]
    D3001,

    /// <summary>
    /// There are dictionary values ​​under the dictionary type that are prohibited from deletion.
    /// </summary>
    [ErrorCodeItemMetadata("There are dictionary values under the dictionary type that are prohibited from deletion.")]
    D3002,

    /// <summary>
    /// Dictionary value already exists
    /// </summary>
    [ErrorCodeItemMetadata("Dictionary value already exists")]
    D3003,

    /// <summary>
    /// Dictionary value does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Dictionary value does not exist")]
    D3004,

    /// <summary>
    /// Dictionary status error
    /// </summary>
    [ErrorCodeItemMetadata("dictionarystatemistake")]
    D3005,

    /// <summary>
    /// Dictionary encoding cannot end with Enum
    /// </summary>
    [ErrorCodeItemMetadata("Dictionary encoding cannot end with Enum")]
    D3006,

    /// <summary>
    /// It is forbidden to modify the dictionary encoding of enumeration types
    /// </summary>
    [ErrorCodeItemMetadata("Modifying the dictionary codes of enumerated types is prohibited")]
    D3007,

    /// <summary>
    /// Disable migration of enum dictionaries
    /// </summary>
    [ErrorCodeItemMetadata("Prohibit migrating enum dictionary")]
    D3008,

    /// <summary>
    /// Dictionary migration is prohibited in this tenant
    /// </summary>
    [ErrorCodeItemMetadata("The dictionary migration has been disabled for this tenant")]
    D3009,

    /// <summary>
    /// Disabling operating system dictionaries for non-supervisory users
    /// </summary>
    [ErrorCodeItemMetadata("Disabling operating system dictionaries for non-supervisory users")]
    D3010,

    /// <summary>
    /// Incorrect input parameters for obtaining dictionary value set
    /// </summary>
    [ErrorCodeItemMetadata("Incorrect input parameters for obtaining dictionary value set")]
    D3011,

    /// <summary>
    /// Modification of tenant dictionary status is prohibited
    /// </summary>
    [ErrorCodeItemMetadata("Modification of tenant dictionary status is prohibited")]
    D3012,

    /// <summary>
    /// Menu already exists
    /// </summary>
    [ErrorCodeItemMetadata("The menu already exists")]
    D4000,

    /// <summary>
    /// Routing address is empty
    /// </summary>
    [ErrorCodeItemMetadata("The routing address is empty")]
    D4001,

    /// <summary>
    /// Open with empty
    /// </summary>
    [ErrorCodeItemMetadata("Open with empty")]
    D4002,

    /// <summary>
    /// Permission ID format is empty
    /// </summary>
    [ErrorCodeItemMetadata("Permission ID format is empty")]
    D4003,

    /// <summary>
    /// Permission ID format error
    /// </summary>
    [ErrorCodeItemMetadata("The format of the permission identifier is incorrect, such as xxx:xxx")]
    D4004,

    /// <summary>
    /// Permission does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Permission does not exist")]
    D4005,

    /// <summary>
    /// The parent menu cannot be the current node, please reselect the parent menu.
    /// </summary>
    [ErrorCodeItemMetadata("The parent menu cannot be the current node, please reselect the parent menu.")]
    D4006,

    /// <summary>
    /// Cannot move root node
    /// </summary>
    [ErrorCodeItemMetadata("Cannot move root node")]
    D4007,

    /// <summary>
    /// Prevent this node from being the same as its parent node
    /// </summary>
    [ErrorCodeItemMetadata("This node must not be the same as the parent node")]
    D4008,

    /// <summary>
    /// Duplicate route name
    /// </summary>
    [ErrorCodeItemMetadata("Duplicate route name")]
    D4009,

    /// <summary>
    /// The parent node cannot be of button type
    /// </summary>
    [ErrorCodeItemMetadata("The parent node cannot be of button type")]
    D4010,

    /// <summary>
    /// Tenant cannot be empty
    /// </summary>
    [ErrorCodeItemMetadata("Tenant cannot be empty")]
    D4011,

    /// <summary>
    /// Modification of system menu is prohibited
    /// </summary>
    [ErrorCodeItemMetadata("Modification of system menu is prohibited")]
    D4012,

    /// <summary>
    /// System menu is prohibited from deletion
    /// </summary>
    [ErrorCodeItemMetadata("System menu deletion is prohibited")]
    D4013,

    /// <summary>
    /// An application with the same name or encoding already exists
    /// </summary>
    [ErrorCodeItemMetadata("An application with the same name or code already exists")]
    D5000,

    /// <summary>
    /// The default activation system can only have one
    /// </summary>
    [ErrorCodeItemMetadata("There can only be one default active system")]
    D5001,

    /// <summary>
    /// There is a menu under this application that prohibits deletion
    /// </summary>
    [ErrorCodeItemMetadata("This application has menus that cannot be deleted")]
    D5002,

    /// <summary>
    /// An application with the same name or encoding already exists
    /// </summary>
    [ErrorCodeItemMetadata("An application with the same name or code already exists")]
    D5003,

    /// <summary>
    /// A position with the same name or code already exists
    /// </summary>
    [ErrorCodeItemMetadata("A position with the same name or code already exists")]
    D6000,

    /// <summary>
    /// There are users under this position who are prohibited from deletion
    /// </summary>
    [ErrorCodeItemMetadata("Users under this position are prohibited from being deleted")]
    D6001,

    /// <summary>
    /// No authority to modify this position
    /// </summary>
    [ErrorCodeItemMetadata("No authority to modify this position")]
    D6002,

    /// <summary>
    /// Position does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Position does not exist")]
    D6003,

    /// <summary>
    /// There is a registration plan under this position that prohibits deletion
    /// </summary>
    [ErrorCodeItemMetadata("There is a registration scheme under this position that cannot be deleted.")]
    D6004,

    /// <summary>
    /// Notification announcement status error
    /// </summary>
    [ErrorCodeItemMetadata("Notification announcement status error")]
    D7000,

    /// <summary>
    /// Notification announcement deletion failed
    /// </summary>
    [ErrorCodeItemMetadata("Failed to delete notification")]
    D7001,

    /// <summary>
    /// Notification announcement editing failed
    /// </summary>
    [ErrorCodeItemMetadata("Failed to edit the notification announcement; the type must be draft")]
    D7002,

    /// <summary>
    /// The notification operation failed and non-publishers cannot perform the operation.
    /// </summary>
    [ErrorCodeItemMetadata("Notification operation failed; non-publishers cannot perform this operation.")]
    D7003,

    /// <summary>
    /// File does not exist
    /// </summary>
    [ErrorCodeItemMetadata("File does not exist")]
    D8000,

    /// <summary>
    /// Not allowed file types
    /// </summary>
    [ErrorCodeItemMetadata("File type not allowed")]
    D8001,

    /// <summary>
    /// File exceeds allowed size
    /// </summary>
    [ErrorCodeItemMetadata("The file exceeds the allowed size")]
    D8002,

    /// <summary>
    /// File suffix error
    /// </summary>
    [ErrorCodeItemMetadata("File suffix error")]
    D8003,

    /// <summary>
    /// File already exists
    /// </summary>
    [ErrorCodeItemMetadata("The file already exists")]
    D8004,

    /// <summary>
    /// Invalid file name
    /// </summary>
    [ErrorCodeItemMetadata("NoneEffective file name")]
    D8005,

    /// <summary>
    /// Parameter configuration with the same name or encoding already exists
    /// </summary>
    [ErrorCodeItemMetadata("A parameter configuration with the same name or code already exists")]
    D9000,

    /// <summary>
    /// Disable deletion of system parameters
    /// </summary>
    [ErrorCodeItemMetadata("Deleting system parameters is prohibited")]
    D9001,

    /// <summary>
    /// A task schedule with the same name already exists
    /// </summary>
    [ErrorCodeItemMetadata("A file with the same name already existsTask Scheduling")]
    D1100,

    /// <summary>
    /// Task scheduling does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Task scheduling does not exist")]
    D1101,

    /// <summary>
    /// Modification of data is prohibited in the demo environment
    /// </summary>
    [ErrorCodeItemMetadata("Modification of data is prohibited in the demo environment")]
    D1200,

    /// <summary>
    /// A tenant with the same name already exists
    /// </summary>
    [ErrorCodeItemMetadata("A tenant with the same name already exists")]
    D1300,

    /// <summary>
    /// A tenant administrator with the same name already exists
    /// </summary>
    [ErrorCodeItemMetadata("A tenant administrator with the same name already exists")]
    D1301,

    /// <summary>
    /// Tenant slave database configuration error
    /// </summary>
    [ErrorCodeItemMetadata("Tenant slave database configuration error")]
    D1302,

    /// <summary>
    /// A tenant domain name with the same name already exists
    /// </summary>
    [ErrorCodeItemMetadata("A tenant domain name with the same name already exists")]
    D1303,

    /// <summary>
    /// There are duplicate items in the authorization menu
    /// </summary>
    [ErrorCodeItemMetadata("There are duplicate items in the authorization menu")]
    D1304,

    /// <summary>
    /// The table code template has been generated
    /// </summary>
    [ErrorCodeItemMetadata("The table codeTemplatealreadyGeneratepass")]
    D1400,

    /// <summary>
    /// Database configuration does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Database configuration does not exist")]
    D1401,

    /// <summary>
    /// The type does not exist
    /// </summary>
    [ErrorCodeItemMetadata("The type does not exist")]
    D1501,

    /// <summary>
    /// This field does not exist
    /// </summary>
    [ErrorCodeItemMetadata("This field does not exist")]
    D1502,

    /// <summary>
    /// The type is not an enumeration type
    /// </summary>
    [ErrorCodeItemMetadata("The type is not an enumeration type")]
    D1503,

    /// <summary>
    /// The entity does not exist
    /// </summary>
    [ErrorCodeItemMetadata("The entity does not exist")]
    D1504,

    /// <summary>
    /// Parent menu does not exist
    /// </summary>
    [ErrorCodeItemMetadata("FathermenuDoes not exist")]
    D1505,

    /// <summary>
    /// Parent resource does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Parent resource does not exist")]
    D1600,

    /// <summary>
    /// The current resource ID cannot be the same as the parent resource ID
    /// </summary>
    [ErrorCodeItemMetadata("The current resource ID cannot be the same as the parent resource ID")]
    D1601,

    /// <summary>
    /// The same code or name already exists
    /// </summary>
    [ErrorCodeItemMetadata("The same code or name already exists")]
    D1602,

    /// <summary>
    /// Script code cannot be empty
    /// </summary>
    [ErrorCodeItemMetadata("Script code cannot be empty")]
    D1701,

    /// <summary>
    /// The job class in the script code needs to define the [JobDetail] attribute
    /// </summary>
    [ErrorCodeItemMetadata("script codeinHomework type，Needs to be defined [JobDetail] Feature")]
    D1702,

    /// <summary>
    /// The job number needs to be consistent with the job class [JobDetail('jobId')] in the script code
    /// </summary>
    [ErrorCodeItemMetadata("The job number needs to match the job class in the script code [JobDetail('jobId')]")]
    D1703,

    /// <summary>
    /// Modification of job number is prohibited
    /// </summary>
    [ErrorCodeItemMetadata("Modification of job number is prohibited")]
    D1704,

    /// <summary>
    /// Job execution failed
    /// </summary>
    [ErrorCodeItemMetadata("Job execution failed")]
    D1705,

    /// <summary>
    /// A print template with the same name already exists
    /// </summary>
    [ErrorCodeItemMetadata("A print template with the same name already exists")]
    D1800,

    /// <summary>
    /// The same function or program and plug-in with the same name already exists
    /// </summary>
    [ErrorCodeItemMetadata("The same function or program and plug-in with the same name already exists")]
    D1900,

    /// <summary>
    /// Registration scheme name already exists
    /// </summary>
    [ErrorCodeItemMetadata("The registration plan name already exists")]
    D2101,

    /// <summary>
    /// A template with the same name already exists
    /// </summary>
    [ErrorCodeItemMetadata("A template with the same name already exists")]
    T1000,

    /// <summary>
    /// The same encoding template already exists
    /// </summary>
    [ErrorCodeItemMetadata("Already exists the sameEncodingTemplate")]
    T1001,

    /// <summary>
    /// Prohibit deletion of applications with associated tenants
    /// </summary>
    [ErrorCodeItemMetadata("Deleting applications with associated tenants is prohibited")]
    A1001,

    /// <summary>
    /// Prohibit deletion of apps with contextual menus
    /// </summary>
    [ErrorCodeItemMetadata("Deleting applications with associated menus is prohibited")]
    A1002,

    /// <summary>
    /// System app not found
    /// </summary>
    [ErrorCodeItemMetadata("System application not found")]
    A1000,

    /// <summary>
    /// A project with the same name or encoding already exists
    /// </summary>
    [ErrorCodeItemMetadata("A project with the same name or code already exists")]
    xg1000,

    /// <summary>
    /// A person with the same ID number already exists
    /// </summary>
    [ErrorCodeItemMetadata("A person with the same ID number already exists")]
    xg1001,

    /// <summary>
    /// Detection data does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Test data does not exist")]
    xg1002,

    /// <summary>
    /// Please add data columns
    /// </summary>
    [ErrorCodeItemMetadata("Please add a data column")]
    db1000,

    /// <summary>
    /// Data table does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Data table does not exist")]
    db1001,

    /// <summary>
    /// Data table does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Adding the same field name is not allowed")]
    db1002,

    /// <summary>
    /// The entity file does not exist or cannot be matched. If it is a newly generated entity, please restart the service and try again.
    /// </summary>
    [ErrorCodeItemMetadata("The entity file does not exist or cannot be matched. If it is a newly generated entity, please try again after restarting the service.")]
    db1003,

    /// <summary>
    /// Parent node does not exist
    /// </summary>
    [ErrorCodeItemMetadata("Parent node does not exist")]
    R2000,

    /// <summary>
    /// The current node ID cannot be the same as the parent node ID
    /// </summary>
    [ErrorCodeItemMetadata("The current node ID cannot be the same as the parent node ID")]
    R2001,

    /// <summary>
    /// The same code or name already exists
    /// </summary>
    [ErrorCodeItemMetadata("The same code or name already exists")]
    R2002,

    /// <summary>
    /// Administrative district codes can only be 6, 9 or 12 digits
    /// </summary>
    [ErrorCodeItemMetadata("The administrative division code can only be 6, 9, or 12 digits long")]
    R2003,

    /// <summary>
    /// The parent node cannot be its own child node
    /// </summary>
    [ErrorCodeItemMetadata("A parent node cannot be its own child node")]
    R2004,

    /// <summary>
    /// Abnormal synchronization of National Bureau of Statistics data, please try again later.
    /// </summary>
    [ErrorCodeItemMetadata("syncCountryBureau of StatisticsDataAbnormal,Please try again later")]
    R2005,

    /// <summary>
    /// The default tenant status prohibits modification
    /// </summary>
    [ErrorCodeItemMetadata("The default tenant status cannot be modified")]
    Z1001,

    /// <summary>
    /// Creation of this type of database is prohibited
    /// </summary>
    [ErrorCodeItemMetadata("Creating this type of database is prohibited")]
    Z1002,

    /// <summary>
    /// Tenant does not exist or is disabled
    /// </summary>
    [ErrorCodeItemMetadata("Tenant does not exist or is disabled")]
    Z1003,

    /// <summary>
    /// Tenant library connection cannot be empty
    /// </summary>
    [ErrorCodeItemMetadata("Tenant library connection cannot be empty")]
    Z1004,

    /// <summary>
    /// Identity already exists
    /// </summary>
    [ErrorCodeItemMetadata("Identity already exists")]
    O1000,

    /// <summary>
    /// Disable operations by non-super administrators
    /// </summary>
    [ErrorCodeItemMetadata("Disable operations by non-super administrators")]
    SA001
}