CREATE TABLE PersonPermission
(
    [Name] VARCHAR(200) PRIMARY KEY NOT NULL,
    ArabicName NVARCHAR(200) NOT NULL,
    Grantable BIT NOT NULL,
    Description NVARCHAR(max)  

);

GO;

GO;
CREATE TABLE InstitutionPermission
(
 [Name] VARCHAR(200) PRIMARY KEY NOT NULL,
    ArabicName NVARCHAR(200) NOT NULL,
    Grantable BIT NOT NULL,
    Description NVARCHAR(max)  



);
GO;

CREATE TABLE City
(
    ID              INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    [Name]   NVARCHAR(200)           NOT NULL UNIQUE ,
    PostalCode INT                    NOT NULL UNIQUE
);
GO;

--CREATE TABLE [Admin]
--(
--    ID              INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
--    EmployeeID      INT NOT NULL,
--    SecretKey       VARCHAR(200),
--    LockedDate     DATETIME NOT NULL,
--    Locked    BIT NOT NULL
--);
--GO;
--ALTER TABLE [Admin] 
--ADD FOREIGN KEY (EmployeeID) REFERENCES Employee (ID);
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
ID  INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,

RoleID INT NOT NULL,
PermissionName VARCHAR(200) NOT NULL,
  UNIQUE(RoleID,PermissionName ));
GO;




CREATE TABLE RoleInstitutionPermission
(
ID  INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,

RoleID INT NOT NULL,
PermissionName VARCHAR(200) NOT NULL,
 UNIQUE(RoleID,PermissionName ));
GO;



ALTER TABLE RolePersonPermission
  ADD FOREIGN KEY (RoleID) REFERENCES Role (ID);
      ALTER TABLE RolePersonPermission
  ADD FOREIGN KEY (PermissionName) REFERENCES PersonPermission (Name);
  GO;

  ALTER TABLE RoleInstitutionPermission
  ADD FOREIGN KEY (RoleID) REFERENCES Role (ID);
    ALTER TABLE RoleInstitutionPermission
  ADD FOREIGN KEY (PermissionName) REFERENCES InstitutionPermission (Name);

GO;
CREATE TABLE EmployeeRole
(
	ID              INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
	EmployeeID INT NOT NULL,
	RoleID INT NOT NULL,
	InstitutionID INT NOT NULL,
    ArabicJobDesc       NVARCHAR(max)  ,
	HiringDate             DATETIME                        NOT NULL,
    Active     BIT NOT NULL
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




 CREATE TABLE FileLevel
(

    ID        INT IDENTITY (1,1) PRIMARY KEY NOT NULL,

    Name      NVARCHAR(max) NOT NULL,
    LevelDesc NVARCHAR(max)
);
GO;


  CREATE TABLE FilePermission
(
 [Name] VARCHAR(200) PRIMARY KEY NOT NULL,
    ArabicName NVARCHAR(200) NOT NULL,
    Grantable BIT NOT NULL


);
GO;


CREATE TABLE [File]
(
    ID                INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
    Name NVARCHAR(400) NOT NULL,
    DateCreatedSys     DATETIME   NOT NULL,
    DateCreated    DATETIME   NOT NULL,
    AuthorID            INT        NOT NULL,
    CurrentVersion INT        ,
    Active               BIT        NOT NULL,
    Locked               BIT        NOT NULL,
    LockedUntil         DATETIME,
    LevelID           INT  NOT NULL
);

GO;

ALTER TABLE [File]
    ADD FOREIGN KEY (AuthorID) REFERENCES EmployeeRole (ID);
      ALTER TABLE [File]
ADD FOREIGN KEY (LevelID) REFERENCES  FileLevel(ID);
GO;



CREATE TABLE FilesScope
(
    ID        INT IDENTITY (1,1) PRIMARY KEY NOT NULL,

    LevelID        INT   NOT NULL,
    RoleID INT NOT NULL,
    Permission VARCHAR(200) NOT NULL,
    UNIQUE(LevelID,RoleID,Permission)

)









GO;
ALTER TABLE FilesScope
    ADD FOREIGN KEY (LevelID) REFERENCES FileLevel (ID);
ALTER TABLE FilesScope
    ADD FOREIGN KEY (RoleID) REFERENCES Role (ID);
ALTER TABLE FilesScope
    ADD FOREIGN KEY (Permission) REFERENCES FilePermission (Name);

  GO;


CREATE TABLE FileType
(
    Name VARCHAR(50) NOT NULL PRIMARY KEY,
    Extension VARCHAR(10) NOT NULL UNIQUE,
    ArabicName NVARCHAR(300)
);


GO;


CREATE TABLE FileContent
(
    ID        INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
    Href      VARCHAR(3000)                  NOT NULL,
    FileSize INT                            NOT NULL
);

GO;
CREATE TABLE FileVersion
(
    ID                  INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
    FileID             INT,
    Name        NVARCHAR(200)                   NOT NULL,
    Notes       NVARCHAR(1000),
    Number      FLOAT,
    DateCreatedSys    DATETIME                       NOT NULL,
    DateCreated   DATETIME                       NOT NULL,
    AuthorID           INT                            NOT NULL,
    FileTypeName VARCHAR(50)                    NOT NULL,
    FileContentID     INT                            NOT NULL UNIQUE
);
GO;

    ALTER TABLE [File]
ADD FOREIGN KEY (CurrentVersion) REFERENCES FileVersion (ID);
GO;
ALTER TABLE FileVersion
    ADD FOREIGN KEY (FileID) REFERENCES [File] (ID);
    ALTER TABLE FileVersion
    ADD FOREIGN KEY (AuthorID) REFERENCES EmployeeRole (ID);
    ALTER TABLE FileVersion
    ADD FOREIGN KEY (FileTypeName) REFERENCES FileType (Name);
      ALTER TABLE FileVersion
    ADD FOREIGN KEY (FileContentID) REFERENCES FileContent (ID);

        GO;


CREATE TABLE FileActionLog
(
    ID                      INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    EmployeeID                        INT      NOT NULL,
    FileID                          INT      NOT NULL,
    ActionDate                      DATETIME NOT NULL,
    PermissionName VARCHAR(200)      NOT NULL
);
GO;

ALTER TABLE FileActionLog
    ADD FOREIGN KEY (EmployeeID) REFERENCES EmployeeRole (ID);
ALTER TABLE FileActionLog
    ADD FOREIGN KEY (FileID) REFERENCES [File](ID);
ALTER TABLE FileActionLog
    ADD FOREIGN KEY (PermissionName) REFERENCES FilePermission (Name);
GO;



CREATE TABLE FileMention
(
    ID  INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,

    CreatorID INT NOT NULL,
    FileID INT NOT NULL,
    EmployeeID INT NOT NULL,
    DateCreated DATETIME NOT NULL,
    Seen BIT NOT NULL
    UNIQUE(CreatorID,FileID,EmployeeID)
)
GO;
ALTER TABLE FileMention 
    ADD FOREIGN KEY (FileID) REFERENCES [File] (ID);
    ALTER TABLE FileMention 
    ADD FOREIGN KEY (EmployeeID) REFERENCES Employee (ID);
    ALTER TABLE FileMention 
    ADD FOREIGN KEY (CreatorID) REFERENCES EmployeeRole (ID);
  GO;

 CREATE TABLE Bookmark
(
    ID  INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    FileID INT NOT NULL,
    EmployeeID INT NOT NULL,
    DateCreated DATETIME NOT NULL
    UNIQUE(FileID,EmployeeID)
)
   GO;
ALTER TABLE Bookmark 
    ADD FOREIGN KEY (FileID) REFERENCES [File] (ID);
    ALTER TABLE Bookmark 
    ADD FOREIGN KEY (EmployeeID) REFERENCES EmployeeRole (ID);



CREATE TABLE [Message]
(
ID  INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
SenderID INT NOT NULL,
RecieverID INT NOT NULL,
FileContentID INT NOT NULL,
FileTypeName VARCHAR(50) NOT NULL,
Text NVARCHAR(max),
HeaderText NVARCHAR(300) NOT NULL,
DateCreated DATETIME NOT NULL,
Seen BIT NOT NULL,
DeletedFromSender BIT  NOT NULL,
DeletedFromReciever BIT  NOT NULL

)
GO;
 ALTER TABLE [Message] 
    ADD FOREIGN KEY (SenderID) REFERENCES Employee (ID);
     ALTER TABLE [Message] 
    ADD FOREIGN KEY (RecieverID) REFERENCES Employee (ID);
     ALTER TABLE [Message] 
    ADD FOREIGN KEY (FileContentID) REFERENCES FileContent (ID);
     ALTER TABLE [Message] 
    ADD FOREIGN KEY (FileTypeName) REFERENCES FileType (Name);







INSERT INTO City 
VALUES ('cairo',

        11865
       );



	   

GO;
  INSERT INTO [dbo].[InstitutionPermission] 
  VALUES('DELETE_FILE_LEVEL',N'إنشاء مستوى ملف',0,null)
       INSERT INTO [dbo].[InstitutionPermission] 
  VALUES('EDIT_FILE_LEVEL',N'إنشاء مستوى ملف',0,null)
  INSERT INTO [dbo].[InstitutionPermission] 
  VALUES('CREATE_FILE_LEVEL',N'إنشاء مستوى ملف',0,null)
INSERT INTO InstitutionPermission
VALUES ( 'VIEW_ROLES',N'عرض الوظائف',1,null);
INSERT INTO InstitutionPermission
VALUES ( 'DELETE_ROLE',N'مسح وظيفة',0,null);
          INSERT INTO InstitutionPermission
VALUES ( 'ACTIVATE_INSTITUTION',N'تفعيل',1,null);
   INSERT INTO InstitutionPermission
VALUES ( 'DEACTIVATE_INSTITUTION',N'تعطيل',0,null);
INSERT INTO InstitutionPermission
VALUES ( 'VIEW_PERSONS_IN_INSTITUTION',N'عرض أشخاص في مؤسسة',1,null);
INSERT INTO InstitutionPermission
VALUES ( 'VIEW_INSTITUTION',N'عرض ',1,null);
INSERT INTO InstitutionPermission
VALUES ( 'CREATE_INSTITUTION',N'إنشاء',0,null);
INSERT INTO InstitutionPermission
VALUES ( 'EDIT_INSTITUTION_INFO',N'تعديل معلومات',1,null);
INSERT INTO InstitutionPermission
VALUES ( 'DELETE_INSTITUTION',N'مسح',0,null);


INSERT INTO InstitutionPermission
VALUES ( 'CREATE_INSTITUTION_TYPE',N'إنشاء نوع',0,null);
INSERT INTO InstitutionPermission
VALUES ( 'EDIT_INSTITUTION_TYPE',N'تعديل نوع',0,null);

INSERT INTO InstitutionPermission
VALUES ( 'VIEW_INSTITUTION_TYPES',N'عرض أنواع المؤسسات',0,null);
INSERT INTO InstitutionPermission
VALUES ( 'DELETE_INSTITUTION_TYPE',N'مسح نوع',0,null);



INSERT INTO InstitutionPermission
VALUES ( 'CREATE_ROLE',N'إنشاء مسمى وظيفي',0,null);
INSERT INTO InstitutionPermission
VALUES ( 'EDIT_ROLE',N'تعديل مسمى وظيفي',0,null);
INSERT INTO InstitutionPermission
VALUES ( 'CREATE_FILE',N'إنشاء ملف',1,null);
GO;



INSERT INTO FilePermission VALUES('EDIT_FILE_VERSION',N'تعديل نسخة ملف',1)
INSERT INTO FilePermission VALUES('CREATE_FILE_VERSION',N'إنشاء نسخة ملف',1)
INSERT INTO FilePermission VALUES('VIEW_FILE_CURRENT_VERSION',N'عرض النسخة الحالية',1)
INSERT INTO FilePermission VALUES('VIEW_FILE_ALL_VERSION',N'عرض كل النسخ ',1)
INSERT INTO FilePermission VALUES('DELETE_FILE_VERSION',N'مسح نسخة ملف',1)
INSERT INTO FilePermission VALUES('CREATE_FILE',N'إنشاء ملف',0)


GO;

INSERT INTO PersonPermission
VALUES ( 'DELETE_PERSON',N'مسح حساب شخص',0,null);
INSERT INTO PersonPermission
VALUES ( 'VIEW_PERSON_PROFILE',N'عرض حساب شخص',1,null);
INSERT INTO PersonPermission
VALUES ( 'ATTACH_ROLE_TO_PERSON',N'تعيين وظيفة لشخص',1,null);
INSERT INTO PersonPermission
VALUES ( 'VIEW_ALL_PERSONS_HIERARCHY',N'عرض الهرم الوظيفي',1,null);
INSERT INTO PersonPermission
VALUES ( 'DEACTIVATE_PERSON_WITHIN_INSTITUTION',N'تعطيل حساب شخص',1,null);
INSERT INTO PersonPermission
VALUES ( 'ACTIVATE_PERSON_WITHIN_INSTITUTION',N'تفعيل حساب شخص',1,null);
INSERT INTO PersonPermission
VALUES ( 'CREATE_PERSON_WITHIN_INSTITUTION',N'إنشاء حساب شخص',1,null);
Go;


INSERT INTO Employee VALUES(N'زياد','M',1,'1111111111111111',1,null,'','','','')


GO;


INSERT INTO [Role] VALUES(null,1,N'رئيس الجامعة');

GO;

INSERT INTO [dbo].[RoleInstitutionPermission](RoleID,PermissionName)
 SELECT 1,Name FROM [dbo].[InstitutionPermission]

 INSERT INTO [dbo].[RolePersonPermission](RoleID,PermissionName)
 SELECT 1,Name FROM [dbo].[PersonPermission]
  GO;


GO;
INSERT INTO InstitutionType VALUES('إدارة عامة','');
Insert INTO [dbo].[Institution] VALUES(N'رئاسة الجامعة',1,1,null,'','',1,'','','','')





INSERT INTO FilePermission
VALUES ( 'VIEW_FILE',N'عرض ملف',1);

INSERT INTO FilePermission
VALUES ( 'EDIT_FILE',N'تعديل ملف',1);

INSERT INTO FilePermission
VALUES ( 'ADD_VERSION',N'إضافة نسخة',1);

INSERT INTO FilePermission
VALUES ( 'DELETE_FILE',N'مسح ملف',1);

INSERT INTO FilePermission
VALUES ( 'SET_CURRENT_VERSION',N'تعيين نسخة لملف',1);

  
GO;







 INSERT INTO FileType(Extension,Name,ArabicName) VALUES('docx','Word Document',N'')
 INSERT INTO FileType(Extension,Name,ArabicName) VALUES('txt','Text Document',N'')
 INSERT INTO FileType(Extension,Name,ArabicName) VALUES('pdf','Portable Document',N'')
 INSERT INTO FileType(Extension,Name,ArabicName) VALUES('ppt','Power Point Document',N'')
 INSERT INTO FileType(Extension,Name,ArabicName) VALUES('png','PNG Image',N'')
 INSERT INTO FileType(Extension,Name,ArabicName) VALUES('jpg','JPG Image',N'')
 INSERT INTO FileType(Extension,Name,ArabicName) VALUES('jpeg','JPEG Media',N'')
















--       CREATE TABLE FolderLevel
--(
--    Level      VARCHAR(2) PRIMARY KEY NOT NULL,
--    LevelDesc NVARCHAR(max)
--);
--   GO;
--CREATE TABLE Folder
--(
--    ID                INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
--    ParentID  INT,
--    DateCreatedSys  DATETIME                       NOT NULL,
--    AuthorID         INT                            NOT NULL,
--    Notes      NVARCHAR(1000),
--    DateCreated DATETIME                       NOT NULL,
--    Name              NVARCHAR(200)                   NOT NULL,
--    Active            BIT                            NOT NULL,
--    InstitutionID   INT NOT NULL,
--    Level VARCHAR(2) NOT NULL

--);
--GO;
--ALTER TABLE Folder
--    ADD FOREIGN KEY (ParentID) REFERENCES Folder (ID);
--ALTER TABLE Folder
--    ADD FOREIGN KEY (AuthorID) REFERENCES EmployeeRole (ID);
--ALTER TABLE Folder
--    ADD FOREIGN KEY (InstitutionID) REFERENCES Institution (ID);
--    ALTER TABLE Folder
--    ADD FOREIGN KEY (Level) REFERENCES FolderLevel (Level);

--   GO;


--   CREATE TABLE FolderPermission
--(
-- [Name] VARCHAR(200) PRIMARY KEY NOT NULL,
--    ArabicName NVARCHAR(200) NOT NULL

--);
--GO;
--    CREATE TABLE FoldersScope
--(
--    Level        VARCHAR(2)   NOT NULL,
--    RoleID INT NOT NULL,
--    Permission VARCHAR(200) NOT NULL,

--)

--GO;
--ALTER TABLE FoldersScope
--    ADD FOREIGN KEY (Level) REFERENCES FolderLevel (ID);
--    ALTER TABLE FoldersScope
--    ADD FOREIGN KEY (RoleID) REFERENCES Role (ID);
--    ALTER TABLE FoldersScope
--    ADD FOREIGN KEY (Permission) REFERENCES FolderPermission (ID);
--GO;

--CREATE TABLE FolderActionLog
--(
--    ID                      INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,

--     EmployeeID                        INT      NOT NULL,
--    FolderID                          INT      NOT NULL,
--    ActionDate                      DATETIME NOT NULL,
--    PermissionName INT      NOT NULL
--);
--GO;

-- ALTER TABLE FolderActionLog
--    ADD FOREIGN KEY (EmployeeID) REFERENCES EmployeeRole (ID);
--ALTER TABLE FolderActionLog
--    ADD FOREIGN KEY (FolderID) REFERENCES Folder(ID);
--ALTER TABLE FolderActionLog
--    ADD FOREIGN KEY (PermissionName) REFERENCES FolderPermission (Name);
--GO;



/*ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss*/






GO;
/*ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss*/





--CREATE TABLE NotificationType
--(
--    Name VARCHAR(100) NOT NULL PRIMARY KEY,
--    ArabicName NVARCHAR(300)

--);

--CREATE TABLE PersonNotification
--(
--    ID                    INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
--    EmployeeID             INT                             NOT NULL,
--    NotificationType     VARCHAR(100)                    NOT NULL,
--    Seen                  BIT                             NOT NULL,
--    FileID               INT                             NOT NULL,
--    MakerEmployeeID INT                             NOT NULL,
--    DateCreated          DATETIME                        NOT NULL
--);

--CREATE TABLE Attachments
--(
--    ID                    INT IDENTITY (1, 1) PRIMARY KEY NOT NULL,
--    SenderID             INT                             NOT NULL,
--    RecieverID           INT                             NOT NULL,
--    attachment_file_type  VARCHAR(50)                     NOT NULL,
--    date_sent             DATETIME                        NOT NULL,
--    notes                 VARCHAR(max),
--    attachment_content_id INT                             NOT NULL,
--    seen                  BIT                             NOT NULL
--);





















--                      /*To Delete All Constraints in sql azure*/
--                      SELECT 'ALTER TABLE ' + Table_Name  +' DROP CONSTRAINT ' + Constraint_Name
--FROM Information_Schema.CONSTRAINT_TABLE_USAGE


--/* the views needs to be edited to add role_front_name instead employee_job_desc*/
--CREATE VIEW Person_view
--AS
--SELECT Person.first_name,
--       Person.middle_name,
--       Person.last_name,
--       Person.contact_email,
--       Person.gender,
--       Person.academic_number,
--       City.city_name,
--       PersonContacts.phone_number,
--       PersonContacts.phd_certificate,
--       PersonContacts.bio,
--       PersonContacts.image_ref,
--       PersonContacts.base_faculty,
--       Person.ID
--FROM Person
--         INNER JOIN PersonContacts
--                    ON Person.contact_email = PersonContacts.email
--         INNER JOIN City
--                    ON Person.city_shortcut = City.shortcut;
--GO;
--CREATE VIEW PersonRolesAndPermissions_view AS
--SELECT Employees.person_id,
--       Person_view.contact_email,
--       Employees.role_id,
--       Employees.employee_job_desc,
--       Employees.hiring_date,
--       Employees.active,
--       Employees.institution_id,
--       Roles.role_priority_lvl,
--       Roles.role_front_name,
--       Roles.files_permissions_sum,
--       Roles.folders_permissions_sum,
--       Roles.institutions_permissions_sum,
--       Roles.persons_permissions_sum,
--       Institution.institution_level,
--       Institution.institution_name
--FROM Employees
--         INNER JOIN Roles
--                    ON Employees.role_id = Roles.ID
--         INNER JOIN Person_view
--                    ON Employees.person_id = Person_view.ID
--         INNER JOIN Institution
--                    ON Institution.ID = Employees.institution_id;

--GO;
--CREATE VIEW PersonsHierarchy_view
--AS SELECT Person_view.first_name,
--          Person_view.middle_name,
--          Person_view.last_name,
--          Person_view.contact_email,
--          Person_view.gender,
--          Person_view.city_name,
--          Person_view.phone_number,
--          Person_view.phd_certificate,
--          Person_view.bio,
--          Person_view.image_ref,
--          Person_view.base_faculty,
--          Person_view.ID,
--          Person_view.academic_number,
--    PersonRolesAndPermissions_view.role_id,
--          PersonRolesAndPermissions_view.role_front_name,

--          PersonRolesAndPermissions_view.role_priority_lvl,
--    PersonRolesAndPermissions_view.employee_job_desc,
--    PersonRolesAndPermissions_view.institution_id,
--          PersonRolesAndPermissions_view.institution_name

--   FROM Person_view
--    INNER JOIN PersonRolesAndPermissions_view
--    ON Person_view.ID = PersonRolesAndPermissions_view.person_id;

--GO;
--CREATE VIEW Notifications_view
--AS
--SELECT PersonNotification.ID,
--       PersonNotification.person_id,
--       PersonNotification.notification_type,
--       PersonNotification.seen,
--       PersonNotification.file_id,
--       PersonNotification.notification_maker_id,
--       PersonNotification.date_created,
--       Person_view.contact_email,
--       Person_view.first_name,
--       Person_view.middle_name,
--       Person_view.last_name
--FROM PersonNotification
--         INNER JOIN Person_view
--                    ON PersonNotification.notification_maker_id = Person_view.id;

--GO;
--CREATE VIEW FileWithCurrentVersion_view
--AS
--SELECT [File].ID,
--       [File].parent_folder_id,
--       [File].arch_date_created      AS file_date_created,
--       [File].author_id              AS file_author_id,
--       [File].current_file_version,
--       [File].locked,
--       [File].locked_until,
--       [File].active,
--       FileVersion.version_name,
--       FileVersion.version_notes,
--       FileVersion.version_number,
--       FileVersion.arch_date_created AS version_date_created,
--       FileVersion.author_id         AS version_author_id,
--       FileVersion.file_type_extension,
--       FileVersion.file_content_id,
--       FileContent.file_size         AS file_version_size
--FROM [File]
--         INNER JOIN FileVersion
--                    ON [File].current_file_version = FileVersion.ID
--         INNER JOIN FileContent
--                    On FileVersion.file_content_id = FileContent.ID;

--GO;
--CREATE VIEW FileVersions_Type_Content_view
--AS
--SELECT FileVersion.ID,
--       FileVersion.file_id,
--       FileVersion.version_name,
--       FileVersion.version_notes,
--       FileVersion.version_number,
--       FileVersion.arch_date_created,
--       FileVersion.author_id,
--       FileVersion.file_type_extension,
--       FileVersion.file_content_id,
--       FileContent.file_size
--FROM FileVersion
--         INNER JOIN FileContent
--                    ON FileVersion.file_content_id = FileContent.ID;
--Go;






--DROP VIEW FileVersions_Type_Content_view;
--GO;
--DROP VIEW FileWithCurrentVersion_view;
--GO;
--DROP VIEW Notifications_view;
--GO;
--DROP VIEW PersonsHierarchy_view;
--GO;
--DROP VIEW PersonRolesAndPermissions_view;
--GO;
--DROP VIEW Person_view;




-- DELETION QUIRIES


while(exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME != '__MigrationHistory' AND TABLE_TYPE = 'BASE TABLE')) begin declare @sql nvarchar(2000) SELECT TOP 1 @sql=('DROP TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + ']') FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME != '__MigrationHistory' AND TABLE_TYPE = 'BASE TABLE' exec (@sql) /* you dont need this line, it just shows what was executed */ PRINT @sql end


while(exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='FOREIGN KEY')) begin declare @sql nvarchar(2000) SELECT TOP 1 @sql=('ALTER TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + '] DROP CONSTRAINT [' + CONSTRAINT_NAME + ']') FROM information_schema.table_constraints WHERE CONSTRAINT_TYPE = 'FOREIGN KEY' exec (@sql) PRINT @sql end











