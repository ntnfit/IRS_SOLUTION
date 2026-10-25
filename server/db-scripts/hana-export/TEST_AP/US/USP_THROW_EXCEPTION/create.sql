create procedure USP_THROW_EXCEPTION(in message nvarchar(254))
as
begin
	SIGNAL SQL_ERROR_CODE 10002
	SET MESSAGE_TEXT = :message;
end
