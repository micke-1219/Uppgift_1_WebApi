CREATE TABLE Customers (
	Id int not null identity(1,1) primary key,
	FirstName nvarchar(50) not null,
	LastName nvarchar(50) not null,
)

CREATE TABLE ServiceWorkers (
	Id int not null identity(1,1) primary key,
	FirstName nvarchar(50) not null,
	LastName nvarchar(50) not null,
	Email varchar(100) not null,
	PasswordHash varbinary(max) not null,
	PasswordSalt varbinary(max) not null
)

GO

CREATE TABLE Cases (
	Id int not null identity(1,1) primary key,
	CustomerId int not null references Customers(Id),
	ServiceWorkerId int not null references ServiceWorkers(Id),
	CaseDate datetime not null,
	CaseResolveDate datetime null,
	CaseDescription nvarchar(max) not null,
	CaseStatus nvarchar(20) not null
)