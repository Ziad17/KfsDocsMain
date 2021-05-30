CREATE TABLE PersonPermission
(
    [Name] VARCHAR(200) PRIMARY KEY NOT NULL,
    ArabicName NVARCHAR(200) NOT NULL
);

GO;

GO;
CREATE TABLE InstitutionPermission
(
 [Name] VARCHAR(200) PRIMARY KEY NOT NULL,
    ArabicName NVARCHAR(200) NOT NULL

);
GO;

CREATE TABLE City
(
    ID              INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    [Name]   NVARCHAR(200)           NOT NULL UNIQUE ,
    PostalCode INT                    NOT NULL UNIQUE
);
GO;


CREATE TABLE Employee
(
	ID              INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    [Name]      NVARCHAR(200)                     NOT NULL ,
    Gender          VARCHAR                         NOT NULL,
	Active                  BIT                             NOT NULL,
 
    AcadmicNumber VARCHAR(16) UNIQUE,
    CityID    INT                   NOT NULL,
    PrimaryRoleID INT,
    PhoneNumber    VARCHAR(11),
    PHD NVARCHAR(200) ,
    ImageURL       VARCHAR(max) ,
    Bio             NVARCHAR(max)
);
GO;
CREATE TABLE EmployeeCredentials
(
    ID                      INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,

Email VARCHAR(300) NOT NULL UNIQUE,
Password VARCHAR(max) NOT NULL,
EmployeeID INT NOT NULL UNIQUE
)
GO;
ALTER TABLE [dbo].[EmployeeCredentials]
ADD FOREIGN KEY (EmployeeID) REFERENCES Employee (ID);


CREATE TABLE [Role]
(
    ID                      INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    ParentID            	INT          ,
    PriorityOrder  INT NOT NULL,
    ArabicName              NVARCHAR(200)   UNIQUE NOT NULL 

);
GO;
ALTER TABLE [Role]
    ADD FOREIGN KEY (ParentID) REFERENCES [Role] (ID);
    GO;

ALTER TABLE Employee
    ADD FOREIGN KEY (CityID) REFERENCES City (ID);
	
GO;


CREATE TABLE RolePersonPermission
(
RoleID INT NOT NULL,
PermissionName VARCHAR(200) NOT NULL,
  PRIMARY KEY(RoleID,PermissionName ));
GO;




CREATE TABLE RoleInstitutionPermission
(
RoleID INT NOT NULL,
PermissionName VARCHAR(200) NOT NULL,
 PRIMARY KEY(RoleID,PermissionName ));
GO;



ALTER TABLE RolePersonPermission
  ADD FOREIGN KEY (RoleID) REFERENCES Role (ID);

  ALTER TABLE RoleInstitutionPermission
  ADD FOREIGN KEY (RoleID) REFERENCES Role (ID);

GO;
CREATE TABLE EmployeeRole
(
	ID              INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
	EmployeeID INT NOT NULL,
	RoleID INT NOT NULL,
	InstitutionID INT NOT NULL,
    ArabicJobDesc       NVARCHAR(max)  ,
	HiringDate             DATETIME                        NOT NULL,
	 UNIQUE(EmployeeID,RoleID,InstitutionID )
);
GO;
ALTER TABLE Employee
ADD FOREIGN KEY (PrimaryRoleID) REFERENCES EmployeeRole (ID);

CREATE TABLE Institution
(
    ID                    INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
    ArabicName      NVARCHAR(300)                   NOT NULL UNIQUE ,
    InstitutionTypeID   INT                   NOT NULL,
    Active    BIT                            NOT NULL,
    ParentID INT  ,
    Website   VARCHAR(max),
    ImageURL       VARCHAR(max),
    InsideCampus         BIT                            NOT NULL,
    PrimaryPhone         VARCHAR(300),
    SecondaryPhone       VARCHAR(300),
    Fax                   VARCHAR(300),
    Email                 VARCHAR(300)
);
GO;
ALTER TABLE EmployeeRole
    ADD FOREIGN KEY (EmployeeID) REFERENCES Employee (ID);
	ALTER TABLE EmployeeRole
    ADD FOREIGN KEY (RoleID) REFERENCES Role (ID);
	ALTER TABLE EmployeeRole
    ADD FOREIGN KEY (InstitutionID) REFERENCES Institution (ID);
GO;

CREATE TABLE InstitutionType
(
    ID                           INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
    ArabicName             NVARCHAR(max) NOT NULL ,
    Description NVARCHAR(max)
);
GO;
ALTER TABLE Institution
    ADD FOREIGN KEY (InstitutionTypeID) REFERENCES InstitutionType (ID);
    ALTER TABLE Institution
    ADD FOREIGN KEY (ParentID) REFERENCES Institution (ID);
GO;




CREATE TABLE InstitutionActionLog
(
    ID                      INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,

    EmployeeID                        INT      NOT NULL,
    InstitutionID                          INT      NOT NULL,
    ActionDate                      DATETIME NOT NULL,
    PermissionName VARCHAR(200)      NOT NULL
);
GO;
CREATE TABLE PersonActionLog
(
    ID                      INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,

    ConductorEmployeeID         INT      NOT NULL,
    AffectedEmployeeID          INT      NOT NULL,
      ActionDate                      DATETIME NOT NULL,
    PermissionName VARCHAR(200)      NOT NULL
);

GO;
ALTER TABLE PersonActionLog
    ADD FOREIGN KEY (ConductorEmployeeID) REFERENCES EmployeeRole (ID);
ALTER TABLE PersonActionLog
    ADD FOREIGN KEY (AffectedEmployeeID) REFERENCES Employee (ID);
ALTER TABLE PersonActionLog
    ADD FOREIGN KEY (PermissionName) REFERENCES PersonPermission (Name);
GO;
ALTER TABLE InstitutionActionLog
    ADD FOREIGN KEY (EmployeeID) REFERENCES EmployeeRole (ID);
ALTER TABLE InstitutionActionLog
    ADD FOREIGN KEY (InstitutionID) REFERENCES Institution (ID);
ALTER TABLE InstitutionActionLog
    ADD FOREIGN KEY (PermissionName) REFERENCES InstitutionPermission (Name);
GO;







INSERT INTO City 
VALUES ('cairo',

        11865
       );



	   
	   INSERT INTO [dbo].[InstitutionType](ArabicName) VALUES(N'إدارة عامة');
INSERT INTO [dbo].[InstitutionType](ArabicName) VALUES(N'إدارة');
INSERT INTO [dbo].[InstitutionType](ArabicName) VALUES(N'قسم');
GO;



INSERT INTO PersonPermission
VALUES ( 'VIEW_PERSON_PROFILE',N'عرض حساب شخص');
INSERT INTO PersonPermission
VALUES ( 'ATTACH_ROLE_TO_PERSON',N'تعيين وظيفة لشخص');
INSERT INTO PersonPermission
VALUES ( 'VIEW_ALL_PERSONS_HIERARCHY',N'عرض الهرم الوظيفي');
INSERT INTO PersonPermission
VALUES ( 'DEACTIVATE_PERSON_WITHIN_INSTITUTION',N'تعطيل حساب شخص');
INSERT INTO PersonPermission
VALUES ( 'CREATE_PERSON_WITHIN_INSTITUTION',N'تفعيل حساب شخص');
Go;


INSERT INTO Employee VALUES(N'زياد','M',1,'',1,null,'','','','')
INSERT INTO Employee VALUES(N'حالد','M',1,'1',1,null,'','','','')
INSERT INTO Employee VALUES(N'ابراهيم','M',1,'2',1,null,'','','','')

INSERT INTO [dbo].[RolePersonPermission] VALUES(1,'VIEW_PERSON_PROFILE')
INSERT INTO [dbo].[RolePersonPermission] VALUES(1,'ATTACH_ROLE_TO_PERSON')
INSERT INTO [dbo].[RolePersonPermission] VALUES(1,'VIEW_ALL_PERSONS_HIERARCHY')
INSERT INTO [dbo].[RolePersonPermission] VALUES(1,'DEACTIVATE_PERSON_WITHIN_INSTITUTION')
INSERT INTO [dbo].[RolePersonPermission] VALUES(1,'CREATE_PERSON_WITHIN_INSTITUTION')






INSERT INTO InstitutionPermission
VALUES ( 'VIEW_PERSONS_IN_INSTITUTION',N'عرض أشخاص في مؤسسة');
INSERT INTO InstitutionPermission
VALUES ( 'VIEW_INSTITUTION',N'عرض مؤسسة');
INSERT INTO InstitutionPermission
VALUES ( 'CREATE_INSTITUTION',N'إنشاء مؤسسة');
INSERT INTO InstitutionPermission
VALUES ( 'EDIT_INSTITUTION_INFO',N'تعديل معلومات مؤسسة');
INSERT INTO InstitutionPermission
VALUES ( 'DELETE_INSTITUTION',N'مسح مؤسسة');
INSERT INTO InstitutionPermission
VALUES ( 'CREATE_INSTITUTION_TYPE',N'إنشاء نوع مؤسسة');
INSERT INTO InstitutionPermission
VALUES ( 'EDIT_INSTITUTION_TYPE',N'تعديل نوع مؤسسة');
INSERT INTO InstitutionPermission
VALUES ( 'CREATE_ROLE',N'إنشاء مسمى وظيفي');
INSERT INTO InstitutionPermission
VALUES ( 'EDIT_ROLE',N'تعديل مسمى وظيفي');
GO;

GO;
Insert INTO [dbo].[Institution] VALUES(N'إدارة الجامعة',1,1,null,'','',1,'','','','')



INSERT INTO [Role] VALUES(null,1,N'مدير عام');                    
INSERT INTO [Role] VALUES(1,2,N'مدير');
INSERT INTO [Role] VALUES(2,3,N'رئيس قسم');







ALTER TABLE FolderActionLog
    ADD FOREIGN KEY (EmployeeID) REFERENCES Employee (ID);
ALTER TABLE FolderActionLog
    ADD FOREIGN KEY (FolderID) REFERENCES Employee (ID);
ALTER TABLE FolderActionLog
    ADD FOREIGN KEY (PermissionName) REFERENCES PersonPermission (Name);
GO;
ALTER TABLE FileActionLog
    ADD FOREIGN KEY (EmployeeID) REFERENCES Employee (ID);
ALTER TABLE FileActionLog
    ADD FOREIGN KEY (FileID) REFERENCES Employee (ID);
ALTER TABLE FileActionLog
    ADD FOREIGN KEY (PermissionName) REFERENCES PersonPermission (Name);
GO;
  ALTER TABLE RoleFilePermission
  ADD FOREIGN KEY (RoleID) REFERENCES Role (ID);
  ALTER TABLE RoleFolderPermission
  ADD FOREIGN KEY (RoleID) REFERENCES Role (ID);
CREATE TABLE FileActionLog
(
    EmployeeID                        INT      NOT NULL,
    FileID                          INT      NOT NULL,
    ActionDate                      DATETIME NOT NULL,
    PermissionName INT      NOT NULL
);
CREATE TABLE RoleFilePermission
(
RoleID INT NOT NULL,
PermissionName VARCHAR(200) NOT NULL,
 PRIMARY KEY(RoleID,PermissionName ));
GO;
CREATE TABLE FilePermission
(
 [Name] VARCHAR(200) PRIMARY KEY NOT NULL,
    ArabicName NVARCHAR(200) NOT NULL

);
CREATE TABLE FolderPermission
(
 [Name] VARCHAR(200) PRIMARY KEY NOT NULL,
    ArabicName NVARCHAR(200) NOT NULL

);
GO;

CREATE TABLE RoleFolderPermission
(
RoleID INT NOT NULL,
PermissionName VARCHAR(200) NOT NULL,
 PRIMARY KEY(RoleID,PermissionName ));
GO;
GO;
CREATE TABLE FolderActionLog
(
     EmployeeID                        INT      NOT NULL,
    FolderID                          INT      NOT NULL,
    ActionDate                      DATETIME NOT NULL,
    PermissionName INT      NOT NULL
);
GO;




















                      /*To Delete All Constraints in sql azure*/
                      SELECT 'ALTER TABLE ' + Table_Name  +' DROP CONSTRAINT ' + Constraint_Name
FROM Information_Schema.CONSTRAINT_TABLE_USAGE











/*



CREATE TABLE NotificationTypes
(
    notification_name VARCHAR(100) NOT NULL PRIMARY KEY
);

CREATE TABLE PersonNotification
(
    ID                    INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    person_id             INT                             NOT NULL,
    notification_type     VARCHAR(100)                    NOT NULL,
    seen                  BIT                             NOT NULL,
    file_id               INT                             NOT NULL,
    notification_maker_id INT                             NOT NULL,
    date_created          DATETIME                        NOT NULL
);

CREATE TABLE Attachments
(
    ID                    INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    sender_id             INT                             NOT NULL,
    receiver_id           INT                             NOT NULL,
    attachment_file_type  VARCHAR(50)                     NOT NULL,
    date_sent             DATETIME                        NOT NULL,
    notes                 VARCHAR(max),
    attachment_content_id INT                             NOT NULL,
    seen                  BIT                             NOT NULL
);

CREATE TABLE Folder
(
    ID                INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
    parent_folder_id  INT,
    sys_date_created  DATETIME                       NOT NULL,
    author_id         INT                            NOT NULL,
    folder_notes      VARCHAR(1000),
    arch_date_created DATETIME                       NOT NULL,

    name              VARCHAR(200)                   NOT NULL,

    active            BIT                            NOT NULL


);

CREATE TABLE [File]
(
    ID
                         INT
        IDENTITY
            (
            1,
            1
            ) PRIMARY KEY           NOT NULL,
    parent_folder_id     INT,
    sys_date_created     DATETIME   NOT NULL,
    arch_date_created    DATETIME   NOT NULL,
    author_id            INT        NOT NULL,
    current_file_version INT        NOT NULL,
    active               BIT        NOT NULL,
    locked               BIT        NOT NULL,
    locked_until         DATETIME,
    file_level           VARCHAR(2) NOT NULL,
    institution_file_id  INT        NOT NULL
);

CREATE TABLE FileLevel
(
    level      VARCHAR(2) PRIMARY KEY NOT NULL,
    level_desc VARCHAR(max)
);

CREATE TABLE FilesScope
(
    file_level        VARCHAR(2)   NOT NULL,
    available_to_role VARCHAR(200) NOT NULL,

)

CREATE TABLE FileType
(
    type_name VARCHAR(50) NOT NULL PRIMARY KEY,
    extension VARCHAR(10) NOT NULL UNIQUE
);

CREATE TABLE FileVersion
(
    ID                  INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
    file_id             INT,
    version_name        VARCHAR(200)                   NOT NULL,
    version_notes       VARCHAR(1000),
    version_number      FLOAT,
    sys_date_created    DATETIME                       NOT NULL,
    arch_date_created   DATETIME                       NOT NULL,

    author_id           INT                            NOT NULL,
    file_type_extension VARCHAR(10)                    NOT NULL,
    file_content_id     INT                            NOT NULL UNIQUE
);

CREATE TABLE FileContent
(
    ID        INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
    href      VARCHAR(3000)                  NOT NULL,
    file_size INT                            NOT NULL
);


/*ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss*/


/* Employees Table*/


/* PersonActionLogs Table*/
ALTER TABLE PersonActionLogs
    ADD FOREIGN KEY (conductor_person_id) REFERENCES Person (ID);
ALTER TABLE PersonActionLogs
    ADD FOREIGN KEY (affected_person_id) REFERENCES Person (ID);
ALTER TABLE PersonActionLogs
    ADD FOREIGN KEY (permission_action_performed) REFERENCES PersonPermissions (bit_value);

/* FileActionLogs Table*/
ALTER TABLE FileActionLogs
    ADD FOREIGN KEY (person_id) REFERENCES Person (ID);
ALTER TABLE FileActionLogs
    ADD FOREIGN KEY (file_id) REFERENCES [File] (ID);
ALTER TABLE FileActionLogs
    ADD FOREIGN KEY (file_permission_action_performed) REFERENCES FilePermissions (bit_value);


/* FolderActionLogs Table*/
ALTER TABLE FolderActionLogs
    ADD FOREIGN KEY (person_id) REFERENCES Person (ID);
ALTER TABLE FolderActionLogs
    ADD FOREIGN KEY (folder_id) REFERENCES Folder (ID);
ALTER TABLE FolderActionLogs
    ADD FOREIGN KEY (folder_permission_action_performed) REFERENCES FolderPermissions (bit_value);

/* InstitutionActionLogs Table*/
ALTER TABLE InstitutionActionLogs
    ADD FOREIGN KEY (person_id) REFERENCES Person (ID);
ALTER TABLE InstitutionActionLogs
    ADD FOREIGN KEY (institution_id) REFERENCES Institution (ID);
ALTER TABLE InstitutionActionLogs
    ADD FOREIGN KEY (institution_permission_action_performed) REFERENCES InstitutionPermissions (bit_value);


/* PersonNotification Table*/
ALTER TABLE PersonNotification
    ADD FOREIGN KEY (person_id) REFERENCES Person (ID);
ALTER TABLE PersonNotification
    ADD FOREIGN KEY (notification_type) REFERENCES NotificationTypes (notification_name);
ALTER TABLE PersonNotification
    ADD FOREIGN KEY (file_id) REFERENCES [File] (ID);
ALTER TABLE PersonNotification
    ADD FOREIGN KEY (notification_maker_id) REFERENCES Person (ID);


/* Attachments Table*/
ALTER TABLE Attachments
    ADD FOREIGN KEY (sender_id) REFERENCES Person (ID);
ALTER TABLE Attachments
    ADD FOREIGN KEY (receiver_id) REFERENCES Person (ID);
ALTER TABLE Attachments
    ADD FOREIGN KEY (attachment_file_type) REFERENCES FileType (type_name);
ALTER TABLE Attachments
    ADD FOREIGN KEY (attachment_content_id) REFERENCES FileContent (ID);


/*Institution Table*/
ALTER TABLE Institution
    ADD FOREIGN KEY (institution_type_id) REFERENCES InstitutionType (ID);
ALTER TABLE Institution
    ADD FOREIGN KEY (institution_parent_id) REFERENCES Institution (ID);


/*Folder Table*/
ALTER TABLE Folder
    ADD FOREIGN KEY (parent_folder_id) REFERENCES Folder (ID);
ALTER TABLE Folder
    ADD FOREIGN KEY (author_id) REFERENCES Person (ID);


/* Files Scope*/
ALTER TABLE FilesScope
    ADD FOREIGN KEY (file_level) REFERENCES FileLevel (level);
ALTER TABLE FilesScope
    ADD FOREIGN KEY (available_to_role) REFERENCES Roles (role_name);


/*File Table*/
ALTER TABLE [File]
    ADD FOREIGN KEY (parent_folder_id) REFERENCES Folder (ID);
ALTER TABLE [File]
    ADD FOREIGN KEY (author_id) REFERENCES Person (ID);
ALTER TABLE [File]
    ADD FOREIGN KEY (current_file_version) REFERENCES FileVersion (ID);
ALTER TABLE [File]
    ADD FOREIGN KEY (file_level) REFERENCES FileLevel (level);
ALTER TABLE [File]
    ADD FOREIGN KEY (institution_file_id) REFERENCES Institution (ID);


/*FileVersion Table*/
ALTER TABLE FileVersion
    ADD FOREIGN KEY (file_id) REFERENCES [File] (ID);
ALTER TABLE FileVersion
    ADD FOREIGN KEY (author_id) REFERENCES Person (ID);
ALTER TABLE FileVersion
    ADD FOREIGN KEY (file_type_extension) REFERENCES FileType (extension);
ALTER TABLE FileVersion
    ADD FOREIGN KEY (file_content_id) REFERENCES FileContent (ID);

GO;
/*ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss*/

/* the views needs to be edited to add role_front_name instead employee_job_desc*/
CREATE VIEW Person_view
AS
SELECT Person.first_name,
       Person.middle_name,
       Person.last_name,
       Person.contact_email,
       Person.gender,
       Person.academic_number,
       City.city_name,
       PersonContacts.phone_number,
       PersonContacts.phd_certificate,
       PersonContacts.bio,
       PersonContacts.image_ref,
       PersonContacts.base_faculty,
       Person.ID
FROM Person
         INNER JOIN PersonContacts
                    ON Person.contact_email = PersonContacts.email
         INNER JOIN City
                    ON Person.city_shortcut = City.shortcut;
GO;
CREATE VIEW PersonRolesAndPermissions_view AS
SELECT Employees.person_id,
       Person_view.contact_email,
       Employees.role_id,
       Employees.employee_job_desc,
       Employees.hiring_date,
       Employees.active,
       Employees.institution_id,
       Roles.role_priority_lvl,
       Roles.role_front_name,
       Roles.files_permissions_sum,
       Roles.folders_permissions_sum,
       Roles.institutions_permissions_sum,
       Roles.persons_permissions_sum,
       Institution.institution_level,
       Institution.institution_name
FROM Employees
         INNER JOIN Roles
                    ON Employees.role_id = Roles.ID
         INNER JOIN Person_view
                    ON Employees.person_id = Person_view.ID
         INNER JOIN Institution
                    ON Institution.ID = Employees.institution_id;

GO;
CREATE VIEW PersonsHierarchy_view
AS SELECT Person_view.first_name,
          Person_view.middle_name,
          Person_view.last_name,
          Person_view.contact_email,
          Person_view.gender,
          Person_view.city_name,
          Person_view.phone_number,
          Person_view.phd_certificate,
          Person_view.bio,
          Person_view.image_ref,
          Person_view.base_faculty,
          Person_view.ID,
          Person_view.academic_number,
    PersonRolesAndPermissions_view.role_id,
          PersonRolesAndPermissions_view.role_front_name,

          PersonRolesAndPermissions_view.role_priority_lvl,
    PersonRolesAndPermissions_view.employee_job_desc,
    PersonRolesAndPermissions_view.institution_id,
          PersonRolesAndPermissions_view.institution_name

   FROM Person_view
    INNER JOIN PersonRolesAndPermissions_view
    ON Person_view.ID = PersonRolesAndPermissions_view.person_id;

GO;
CREATE VIEW Notifications_view
AS
SELECT PersonNotification.ID,
       PersonNotification.person_id,
       PersonNotification.notification_type,
       PersonNotification.seen,
       PersonNotification.file_id,
       PersonNotification.notification_maker_id,
       PersonNotification.date_created,
       Person_view.contact_email,
       Person_view.first_name,
       Person_view.middle_name,
       Person_view.last_name
FROM PersonNotification
         INNER JOIN Person_view
                    ON PersonNotification.notification_maker_id = Person_view.id;

GO;
CREATE VIEW FileWithCurrentVersion_view
AS
SELECT [File].ID,
       [File].parent_folder_id,
       [File].arch_date_created      AS file_date_created,
       [File].author_id              AS file_author_id,
       [File].current_file_version,
       [File].locked,
       [File].locked_until,
       [File].active,
       FileVersion.version_name,
       FileVersion.version_notes,
       FileVersion.version_number,
       FileVersion.arch_date_created AS version_date_created,
       FileVersion.author_id         AS version_author_id,
       FileVersion.file_type_extension,
       FileVersion.file_content_id,
       FileContent.file_size         AS file_version_size
FROM [File]
         INNER JOIN FileVersion
                    ON [File].current_file_version = FileVersion.ID
         INNER JOIN FileContent
                    On FileVersion.file_content_id = FileContent.ID;

GO;
CREATE VIEW FileVersions_Type_Content_view
AS
SELECT FileVersion.ID,
       FileVersion.file_id,
       FileVersion.version_name,
       FileVersion.version_notes,
       FileVersion.version_number,
       FileVersion.arch_date_created,
       FileVersion.author_id,
       FileVersion.file_type_extension,
       FileVersion.file_content_id,
       FileContent.file_size
FROM FileVersion
         INNER JOIN FileContent
                    ON FileVersion.file_content_id = FileContent.ID;
Go;






DROP VIEW FileVersions_Type_Content_view;
GO;
DROP VIEW FileWithCurrentVersion_view;
GO;
DROP VIEW Notifications_view;
GO;
DROP VIEW PersonsHierarchy_view;
GO;
DROP VIEW PersonRolesAndPermissions_view;
GO;
DROP VIEW Person_view;
















