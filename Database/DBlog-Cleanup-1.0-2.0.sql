-- Delete duplicate hourly counters (bug)
DELETE HourlyCounter WHERE HourlyCounter_Id IN (
 SELECT hc2.HourlyCounter_Id FROM HourlyCounter hc1, HourlyCounter hc2 WHERE
 hc1.HourlyCounter_Id != hc2.HourlyCounter_Id
 AND hc1.DateTime = hc2.DateTime
)
GO
