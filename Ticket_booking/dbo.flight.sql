CREATE TABLE [dbo].[flight] (
    [fID]           INT            NOT NULL,
    [airline_name]  NVARCHAR (50)  NOT NULL,
    [flight_date]   DATE           NOT NULL,
    [leave_from]    VARCHAR (20)   NOT NULL,
    [going_to]      VARCHAR (20)   NOT NULL,
    [ticket_price]  MONEY          NOT NULL,
    [seat_avilable] BIT            NOT NULL,
    [airline_pic]   NVARCHAR (150) NOT NULL,
    PRIMARY KEY CLUSTERED ([fID] ASC)
);

