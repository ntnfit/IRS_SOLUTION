
CREATE PROCEDURE apz_alert_getbyuser(in UserName VARCHAR(100), in MailType int)
AS
BEGIN
	DECLARE hhmm int;
	select cast(TO_VARCHAR(current_date, 'hhmm') as integer) into hhmm from DUMMY;
	select * from apz_fun_alert_getbyuser(:UserName, :MailType);
END




