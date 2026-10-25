
CREATE FUNCTION STUFF (
	STRING nvarchar(5000),	-- Required. The string to be modified
	STAT INT,				-- Required. The position in string to start to delete some characters
	LENG INT,				-- Required. The number of characters to delete from string
	STUFF_STRING nvarchar(5000)		-- Required. The new string to insert into string at the start position
)
RETURNS result nvarchar(5000)
AS
BEGIN
	result := SUBSTRING(:STRING, 1, :STAT - 1) ||  :STUFF_STRING || SUBSTRING(:STRING, :STAT + :LENG, LENGTH(:STRING));
END

