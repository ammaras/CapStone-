DROP DATABASE GB_Tasklogtracker_D1;

CREATE DATABASE GB_Tasklogtracker_D1;

USE GB_Tasklogtracker_D1;

CREATE TABLE [Group] (
  groupId int IDENTITY NOT NULL, 
  name    varchar(50) NOT NULL, 
  PRIMARY KEY (groupId));
CREATE TABLE Task (
  taskId            int IDENTITY NOT NULL, 
  primaryContact    int NOT NULL, 
  secondaryContact  int NULL, 
  dateLogged        datetime NOT NULL, 
  dateSubmmited     datetime NOT NULL, 
  serviceTeam       int NOT NULL, 
  serviceGroup      int NOT NULL, 
  platform          varchar(50) NOT NULL, 
  urgency           varchar(50) NOT NULL, 
  businessUnit      int NOT NULL, 
  environment       varchar(50) NOT NULL, 
  category          varchar(50) NOT NULL, 
  application       int NOT NULL, 
  title             varchar(250) NOT NULL, 
  description       varchar(255) NOT NULL, 
  highLevelEstimate varchar(50) NULL, 
  links             varchar(255) NULL, 
  taskStatus        varchar(50) NOT NULL, 
  PRIMARY KEY (taskId));
CREATE TABLE Platform (
  platformCode varchar(50) NOT NULL, 
  PRIMARY KEY (platformCode));
CREATE TABLE Urgency (
  urgencyCode varchar(50) NOT NULL, 
  PRIMARY KEY (urgencyCode));
CREATE TABLE Application (
  applicationId int IDENTITY NOT NULL, 
  name          varchar(50) NOT NULL, 
  PRIMARY KEY (applicationId));
CREATE TABLE BusinessUnit (
  businessUnitId int IDENTITY NOT NULL, 
  description    varchar(50) NOT NULL, 
  PRIMARY KEY (businessUnitId));
CREATE TABLE Reference (
  referenceId   int IDENTITY NOT NULL, 
  referenceNo   varchar(50) NOT NULL, 
  referenceType varchar(50) NOT NULL, 
  PRIMARY KEY (referenceId));
CREATE TABLE ReferenceType (
  referenceTypeCode varchar(50) NOT NULL, 
  PRIMARY KEY (referenceTypeCode));
CREATE TABLE Environment (
  environmentCode varchar(50) NOT NULL, 
  PRIMARY KEY (environmentCode));
CREATE TABLE Category (
  categoryCode varchar(50) NOT NULL, 
  PRIMARY KEY (categoryCode));
CREATE TABLE Employee (
  employeeId  int IDENTITY NOT NULL, 
  team        int NOT NULL, 
  lastName    varchar(50) NOT NULL, 
  firstName   varchar(50) NOT NULL, 
  email       varchar(50) NOT NULL, 
  description varchar(255) NULL, 
  lastChanged datetime NOT NULL, 
  middleName  varchar(50) NULL, 
  phone       varchar(25) NOT NULL, 
  extension   varchar(25) NOT NULL, 
  PRIMARY KEY (employeeId));
CREATE TABLE Team (
  teamId int IDENTITY NOT NULL, 
  name   varchar(50) NOT NULL, 
  PRIMARY KEY (teamId));
CREATE TABLE Account (
  employeeId int NOT NULL, 
  username   varchar(25) NOT NULL, 
  password   varchar(25) NOT NULL, 
  roleCode   varchar(50) NOT NULL, 
  PRIMARY KEY (employeeId));
CREATE TABLE Role (
  roleCode varchar(50) NOT NULL, 
  PRIMARY KEY (roleCode));
CREATE TABLE TaskStatus (
  taskStatusCode varchar(50) NOT NULL, 
  PRIMARY KEY (taskStatusCode));
CREATE TABLE HighLevelEstimate (
  highLevelEstimateCode varchar(50) NOT NULL, 
  PRIMARY KEY (highLevelEstimateCode));
CREATE TABLE Worksheet (
  worksheetId     int IDENTITY NOT NULL, 
  employee        int NOT NULL, 
  task            int NOT NULL, 
  worksheetStatus varchar(50) NOT NULL, 
  dateAssigned    datetime NOT NULL, 
  notes           varchar(255) NOT NULL, 
  timeSpent       decimal(5, 2) NOT NULL, 
  overtime        bit NOT NULL, 
  onCall          bit NOT NULL, 
  links           varchar(255) NULL, 
  PRIMARY KEY (worksheetId));
CREATE TABLE WorksheetStatus (
  worksheetStatusCode varchar(50) NOT NULL, 
  PRIMARY KEY (worksheetStatusCode));
CREATE TABLE WorksheetAudit (
  worksheetAuditId int IDENTITY NOT NULL, 
  worksheet        int NOT NULL, 
  dateLogged       datetime NOT NULL, 
  loggedBy         int NOT NULL, 
  notes            varchar(255) NOT NULL, 
  timeSpent        decimal(5, 2) NOT NULL, 
  worksheetStatus  varchar(50) NOT NULL, 
  PRIMARY KEY (worksheetAuditId));
CREATE TABLE TaskAudit (
  taskAuditId   int IDENTITY NOT NULL, 
  task          int NOT NULL, 
  taskAuditType varchar(50) NOT NULL, 
  dateLogged    datetime NOT NULL, 
  loggedBy      int NOT NULL, 
  notes         varchar(255) NOT NULL, 
  taskStatus    varchar(50) NOT NULL, 
  PRIMARY KEY (taskAuditId));
CREATE TABLE TaskAuditType (
  taskAuditTypeCode varchar(50) NOT NULL, 
  PRIMARY KEY (taskAuditTypeCode));
CREATE TABLE TaskReference (
  taskReferenceId int IDENTITY NOT NULL, 
  task            int NOT NULL, 
  reference       int NOT NULL, 
  PRIMARY KEY (taskReferenceId));

ALTER TABLE Task ADD CONSTRAINT FKTask953350 FOREIGN KEY (serviceGroup) REFERENCES [Group] (groupId);
ALTER TABLE Task ADD CONSTRAINT FKTask291544 FOREIGN KEY (platform) REFERENCES Platform (platformCode);
ALTER TABLE Task ADD CONSTRAINT FKTask326449 FOREIGN KEY (urgency) REFERENCES Urgency (urgencyCode);
ALTER TABLE Task ADD CONSTRAINT FKTask35102 FOREIGN KEY (businessUnit) REFERENCES BusinessUnit (businessUnitId);
ALTER TABLE Reference ADD CONSTRAINT FKReference146955 FOREIGN KEY (referenceType) REFERENCES ReferenceType (referenceTypeCode);
ALTER TABLE Task ADD CONSTRAINT FKTask224529 FOREIGN KEY (environment) REFERENCES Environment (environmentCode);
ALTER TABLE Task ADD CONSTRAINT FKTask346527 FOREIGN KEY (application) REFERENCES Application (applicationId);
ALTER TABLE Task ADD CONSTRAINT FKTask414 FOREIGN KEY (category) REFERENCES Category (categoryCode);
ALTER TABLE Task ADD CONSTRAINT FKTask67458 FOREIGN KEY (serviceTeam) REFERENCES Team (teamId);
ALTER TABLE Employee ADD CONSTRAINT FKEmployee559362 FOREIGN KEY (team) REFERENCES Team (teamId);
ALTER TABLE Task ADD CONSTRAINT FKTask678938 FOREIGN KEY (primaryContact) REFERENCES Employee (employeeId);
ALTER TABLE Task ADD CONSTRAINT FKTask658460 FOREIGN KEY (secondaryContact) REFERENCES Employee (employeeId);
ALTER TABLE Account ADD CONSTRAINT FKAccount517006 FOREIGN KEY (employeeId) REFERENCES Employee (employeeId);
ALTER TABLE Account ADD CONSTRAINT FKAccount300931 FOREIGN KEY (roleCode) REFERENCES Role (roleCode);
ALTER TABLE Task ADD CONSTRAINT FKTask25533 FOREIGN KEY (taskStatus) REFERENCES TaskStatus (taskStatusCode);
ALTER TABLE Task ADD CONSTRAINT FKTask277714 FOREIGN KEY (highLevelEstimate) REFERENCES HighLevelEstimate (highLevelEstimateCode);
ALTER TABLE Worksheet ADD CONSTRAINT FKWorksheet92753 FOREIGN KEY (task) REFERENCES Task (taskId);
ALTER TABLE Worksheet ADD CONSTRAINT FKWorksheet134116 FOREIGN KEY (worksheetStatus) REFERENCES WorksheetStatus (worksheetStatusCode);
ALTER TABLE Worksheet ADD CONSTRAINT FKWorksheet248592 FOREIGN KEY (employee) REFERENCES Employee (employeeId);
ALTER TABLE WorksheetAudit ADD CONSTRAINT FKWorksheetA347076 FOREIGN KEY (worksheet) REFERENCES Worksheet (worksheetId);
ALTER TABLE WorksheetAudit ADD CONSTRAINT FKWorksheetA325029 FOREIGN KEY (worksheetStatus) REFERENCES WorksheetStatus (worksheetStatusCode);
ALTER TABLE WorksheetAudit ADD CONSTRAINT FKWorksheetA619031 FOREIGN KEY (loggedBy) REFERENCES Employee (employeeId);
ALTER TABLE TaskAudit ADD CONSTRAINT FKTaskAudit479842 FOREIGN KEY (task) REFERENCES Task (taskId);
ALTER TABLE TaskAudit ADD CONSTRAINT FKTaskAudit251605 FOREIGN KEY (taskAuditType) REFERENCES TaskAuditType (taskAuditTypeCode);
ALTER TABLE TaskAudit ADD CONSTRAINT FKTaskAudit576958 FOREIGN KEY (taskStatus) REFERENCES TaskStatus (taskStatusCode);
ALTER TABLE TaskAudit ADD CONSTRAINT FKTaskAudit29123 FOREIGN KEY (loggedBy) REFERENCES Employee (employeeId);
ALTER TABLE TaskReference ADD CONSTRAINT FKTaskRefere80681 FOREIGN KEY (task) REFERENCES Task (taskId);
ALTER TABLE TaskReference ADD CONSTRAINT FKTaskRefere248239 FOREIGN KEY (reference) REFERENCES Reference (referenceId);

INSERT INTO Role(roleCode) VALUES('Admin');
INSERT INTO Role(roleCode) VALUES('Employee');
INSERT INTO Role(roleCode) VALUES('Manager');

INSERT INTO Team(name) VALUES('Digital Solutions');
INSERT INTO Team(name) VALUES('DMS');
INSERT INTO Team(name) VALUES('DS - Mobile');
INSERT INTO Team(name) VALUES('DS - Salesforce');
INSERT INTO Team(name) VALUES('Environment Mgmt');
INSERT INTO Team(name) VALUES('Finance GIS Billing');
INSERT INTO Team(name) VALUES('IMIT');
INSERT INTO Team(name) VALUES('Integration API & Web Services');
INSERT INTO Team(name) VALUES('Integration Business Process');
INSERT INTO Team(name) VALUES('Integration Correspondence');
INSERT INTO Team(name) VALUES('Integration Support');
INSERT INTO Team(name) VALUES('Integration Technology Process');
INSERT INTO Team(name) VALUES('ITS - DB Services');
INSERT INTO Team(name) VALUES('ITS - Managed Services');
INSERT INTO Team(name) VALUES('ITS - Service Management');
INSERT INTO Team(name) VALUES('ITS - TAMS');
INSERT INTO Team(name) VALUES('ITS - TPS');
INSERT INTO Team(name) VALUES('QAS - Specialized Testing');
INSERT INTO Team(name) VALUES('QAS - Time Tracker');
INSERT INTO Team(name) VALUES('Release Management');

INSERT INTO Employee(team, lastName, firstName, email, description, lastChanged, middleName, phone, extension) 
VALUES(1, 'Smith', 'John', 'jsmith@manulife.com', 'DBA with 5 years of experience', '02/24/2018', '', '5197923774', '12356');
INSERT INTO Employee(team, lastName, firstName, email, description, lastChanged, middleName, phone, extension) 
VALUES(1, 'Ammara', 'Sheikh', 'asheikh@manulife.com', 'DBA with 1 years of experience', '02/24/2018', '', '5192194567', '12345');
INSERT INTO Employee(team, lastName, firstName, email, description, lastChanged, middleName, phone, extension) 
VALUES(2, 'Linda', 'Paul', 'lpaul@manulife.com', 'BSA with 3 years of experience', '02/24/2018', '', '5192193467', '12344');
INSERT INTO Employee(team, lastName, firstName, email, description, lastChanged, middleName, phone, extension) 
VALUES(2, 'Rob', 'Wise', 'rwise@manulife.com', 'PM with 3 years of experience', '02/24/2018', 'rob', '5192163467', '12334');
INSERT INTO Employee(team, lastName, firstName, email, description, lastChanged, middleName, phone, extension) 
VALUES(3, 'Bill', 'Griffith', 'bgriffith@manulife.com', 'Developer with 3 years of experience', '02/24/2018', '', '5192193467', '12345');
INSERT INTO Employee(team, lastName, firstName, email, description, lastChanged, middleName, phone, extension) 
VALUES(4, 'Lester', 'Stevenson', 'lstevenson@manulife.com', 'QA with 3 years of experience', '02/24/2018', '', '5192193467', '12345');
INSERT INTO Employee(team, lastName, firstName, email, description, lastChanged, middleName, phone, extension) 
VALUES(2, 'Miriam', 'Walters', 'mwalters@manulife.com', 'BSA with 3 years of experience', '02/24/2018', '', '5192193467', '12345');
INSERT INTO Employee(team, lastName, firstName, email, description, lastChanged, middleName, phone, extension) 
VALUES(1, 'Eula', 'Palmer', 'epalmer@manulife.com', 'Integrator with 2 years of experience', '02/24/2018', '', '5192193467', '12345');
INSERT INTO Employee(team, lastName, firstName, email, description, lastChanged, middleName, phone, extension) 
VALUES(4, 'Andrew', 'Bates', 'abates@manulife.com', 'BSA with 3 years of experience', '02/24/2018', '', '5192193467', '12345');
INSERT INTO Employee(team, lastName, firstName, email, description, lastChanged, middleName, phone, extension) 
VALUES(2, 'Phyllis', 'Blake', 'pblake@manulife.com', 'PM with 3 years of experience', '02/24/2018', '', '5192193467', '12345');


INSERT INTO Account VALUES(1, 'admin', 'admin', 'Admin');
INSERT INTO Account VALUES(2, 'asheikh', 'asheikh', 'Manager');
INSERT INTO Account VALUES(3, 'lpaul', 'lpaul', 'Employee');
INSERT INTO Account VALUES(4, 'rwise', 'rwise', 'Admin');

INSERT INTO Application(name) VALUES('Not Listed');
INSERT INTO Application(name) VALUES('AP Pooling');
INSERT INTO Application(name) VALUES('CB Pooling');
INSERT INTO Application(name) VALUES('Datawarehouse');
INSERT INTO Application(name) VALUES('DAD');
INSERT INTO Application(name) VALUES('eTreasury');
INSERT INTO Application(name) VALUES('ETL 9.6.1 Informatica');
INSERT INTO Application(name) VALUES('FIRM');
INSERT INTO Application(name) VALUES('Frontier');
INSERT INTO Application(name) VALUES('GFDW Lawson DataWarehouse');
INSERT INTO Application(name) VALUES('GFEXAD1 Database');
INSERT INTO Application(name) VALUES('DMO');

INSERT INTO BusinessUnit(description) VALUES('ITS - Managed Services');
INSERT INTO Category(categoryCode) VALUES('Support');
INSERT INTO Category(categoryCode) VALUES('Project');
INSERT INTO Category(categoryCode) VALUES('Deployment');
INSERT INTO Category(categoryCode) VALUES('Design');
INSERT INTO Category(categoryCode) VALUES('Consultant');

INSERT INTO Environment(environmentCode) VALUES('Non-Production');
INSERT INTO Environment(environmentCode) VALUES('Production');

INSERT INTO HighLevelEstimate(highLevelEstimateCode) VALUES('days');
INSERT INTO HighLevelEstimate(highLevelEstimateCode) VALUES('hours');

INSERT INTO Platform(platformCode) VALUES('SQL');
INSERT INTO Platform(platformCode) VALUES('MySQL');
INSERT INTO Platform(platformCode) VALUES('Azure');
INSERT INTO Platform(platformCode) VALUES('DB2');
INSERT INTO Platform(platformCode) VALUES('Oracle');


INSERT INTO ReferenceType(referenceTypeCode) VALUES('SNOW');

INSERT INTO Reference(referenceNo, referenceType) VALUES('CH0012345', 'SNOW');
INSERT INTO Reference(referenceNo, referenceType) VALUES('CH0054321', 'SNOW');

INSERT INTO [Group](name) VALUES('DBA');
INSERT INTO [Group](name) VALUES('BSA');
INSERT INTO [Group](name) VALUES('Integrator');
INSERT INTO [Group](name) VALUES('PM');
INSERT INTO [Group](name) VALUES('Developer');
INSERT INTO [Group](name) VALUES('QA');
INSERT INTO [Group](name) VALUES('Technical Consultant');
INSERT INTO [Group](name) VALUES('Data Modeler');


INSERT INTO TaskStatus(taskStatusCode) VALUES('Acknowledged');
INSERT INTO TaskStatus(taskStatusCode) VALUES('Assigned');
INSERT INTO TaskStatus(taskStatusCode) VALUES('Cancelled');
INSERT INTO TaskStatus(taskStatusCode) VALUES('Completed');
INSERT INTO TaskStatus(taskStatusCode) VALUES('In Progress');
INSERT INTO TaskStatus(taskStatusCode) VALUES('Not Assigned');
INSERT INTO TaskStatus(taskStatusCode) VALUES('On Hold');
INSERT INTO TaskStatus(taskStatusCode) VALUES('Pending Signoff');
INSERT INTO TaskStatus(taskStatusCode) VALUES('Re-Work');
INSERT INTO TaskStatus(taskStatusCode) VALUES('Scheduled');
INSERT INTO TaskStatus(taskStatusCode) VALUES('Signed Off');

INSERT INTO Urgency(urgencyCode) VALUES('Medium');
INSERT INTO Task(primaryContact, secondaryContact, dateLogged, dateSubmmited, serviceTeam, serviceGroup, platform, urgency, businessUnit, environment, category, application, title, description, highLevelEstimate, links, taskStatus) 
VALUES(1, 1, '01/30/2018', '01/30/2018', 1, 1, 'SQL', 'Medium', 1, 'Non-Production', 'Support', 1, 'DBA Support needed for DIS_DMO_T10', 'Apply the security from DIS_DMO_A1', 'days', 'https://www.manulife.ca', 'Not Assigned');
INSERT INTO TaskReference(task, reference) VALUES(1, 1);
INSERT INTO TaskReference(task, reference) VALUES(1, 2);

INSERT INTO TaskAuditType(taskAuditTypeCode) VALUES('Communication');
INSERT INTO TaskAuditType(taskAuditTypeCode) VALUES('Field Changes');

INSERT INTO TaskAudit(task, taskAuditType, dateLogged, loggedBy, notes, taskStatus) 
VALUES('1', 'Communication', '01/30/2018', 1, 'Document Created', 'Not Assigned');

INSERT INTO WorksheetStatus VALUES('Assigned');
INSERT INTO WorksheetStatus VALUES('Cancelled');
INSERT INTO WorksheetStatus VALUES('Completed');
INSERT INTO WorksheetStatus VALUES('Pending Signoff');
INSERT INTO WorksheetStatus VALUES('Re-Work');
INSERT INTO WorksheetStatus VALUES('Signed Off');