CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(255) NOT NULL
);
CREATE TABLE department (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    parentDepartmentId INT,
    FOREIGN KEY (parentDepartmentId) REFERENCES Department(Id)
);
CREATE TABLE job (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    department_id INT,
    FOREIGN KEY (department_id) REFERENCES department(id)
);
CREATE TABLE employee (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT,
    department_id INT,
    job_id INT,
    name VARCHAR(50) NOT NULL,
    surname VARCHAR(50) NOT NULL,
    birthday DATE,
    jmbg INT,
    address VARCHAR(255),
    email VARCHAR(255),
    phone VARCHAR(50),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (department_id) REFERENCES department(id),
    FOREIGN KEY (job_id) REFERENCES job(id)
);
CREATE TABLE location (
    id INT IDENTITY(1,1) PRIMARY KEY,
    gps VARCHAR(50),
    name VARCHAR(100) NOT NULL,
    address VARCHAR(255),
    city VARCHAR(100),
    country VARCHAR(100)
);
CREATE TABLE type (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);
CREATE TABLE project (
    id INT IDENTITY(1,1) PRIMARY KEY,
    location_id INT,
    type_id INT,
	name VARCHAR(100) NOT NULL,
    status VARCHAR(50),
    FOREIGN KEY (location_id) REFERENCES location(id),
    FOREIGN KEY (type_id) REFERENCES type(id)
);
CREATE TABLE employee_project (
    id INT IDENTITY(1,1) PRIMARY KEY,
    project_id INT,
    employee_id INT,
    start_date DATE,
    end_date DATE,
    FOREIGN KEY (project_id) REFERENCES project(id),
    FOREIGN KEY (employee_id) REFERENCES employee(id)
);
CREATE TABLE activity (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);
CREATE TABLE working_card (
    id INT IDENTITY(1,1) PRIMARY KEY,
    employee_id INT,
    project_id INT,
    date DATE,
    hours DECIMAL(5, 2),
    activity_id INT,
    FOREIGN KEY (employee_id) REFERENCES employee(id),
    FOREIGN KEY (project_id) REFERENCES project(id),
    FOREIGN KEY (activity_id) REFERENCES activity(id)
);
ALTER TABLE users
ADD CONSTRAINT unique_username UNIQUE (username);


CREATE TABLE roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    roleName VARCHAR(50) NOT NULL
);

CREATE TABLE user_roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    RoleID INT NOT NULL,
    CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserID) REFERENCES users(id),
    CONSTRAINT FK_UserRoles_Role FOREIGN KEY (RoleID) REFERENCES Roles(id),
    CONSTRAINT UQ_UserRoles UNIQUE (UserID, RoleID)
);

ALTER TABLE employee
ALTER COLUMN jmbg BIGINT;

ALTER TABLE employee_project
ADD manager VARCHAR(3)

ALTER TABLE project
ADD start_date DATE,
    end_date DATE;

ALTER TABLE employee_project
DROP COLUMN start_date,
            end_date;

ALTER TABLE Employee
ADD fullname AS (name + ' ' + surname);

ALTER TABLE working_card
ADD description VARCHAR(MAX);

CREATE TABLE Module (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL UNIQUE
);
CREATE TABLE Permissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ModuleId INT,
    RoleId INT,
    Action NVARCHAR(10) CHECK (Action IN ('Dodaj', 'Citaj', 'Uredi', 'Obrisi')),
    CONSTRAINT FK_Module FOREIGN KEY (ModuleId) REFERENCES Module(Id),
    CONSTRAINT FK_Role FOREIGN KEY (RoleId) REFERENCES roles(Id)
);
ALTER TABLE users
ADD [roles] VARCHAR(255) NULL;














