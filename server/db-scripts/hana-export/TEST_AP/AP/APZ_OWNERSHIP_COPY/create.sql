

create  procedure apz_ownership_copy(in source NVARCHAR(100), in target NVARCHAR(50000))
AS
BEGIN 
	DELETE from hem5 WHERE  "empID"  IN (SELECT data FROM  Split(:target,','));
	
	INSERT INTO hem5 ("empID","Object","Peer","Manager","Subord","Dept","Branch","Team","AC","Cmpny","EncryptIV")
	SELECT a.Data,"Object","Peer","Manager","Subord","Dept","Branch","Team","AC","Cmpny","EncryptIV" FROM hem5,(SELECT data FROM  Split(:target,',')) a WHERE "empID"=:source;

END 




