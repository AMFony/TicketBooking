CREATE TABLE flight 
(
	fID INT PRIMARY KEY,
	airline_name NVARCHAR(50) not null,
	flight_date DATE not null,
	leave_from VARCHAR(20) not null,
	going_to VARCHAR(20) not null,
	ticket_price MONEY not null,
	seat_avilable BIT not null,
	airline_pic NVARCHAR(150) NOT NULL
)
CREATE TABLE passenger
(
	pID INT PRIMARY KEY,
	pName VARCHAR(30) not null,
	pAddress VARCHAR(50) not null,
	fID INT not null REFERENCES flight (fID)
)
go