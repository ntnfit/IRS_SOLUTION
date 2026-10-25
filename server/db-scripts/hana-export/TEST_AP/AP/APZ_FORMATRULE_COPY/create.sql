
CREATE PROCEDURE APZ_FormatRule_Copy
(
in ItemId VARCHAR(50),
in FormId VARCHAR(254),
in UserName VARCHAR(20),
in CopyUser VARCHAR(4000),
in CopyAll CHAR(1)
)
AS 
BEGIN
	 declare code int =  0;
	select Max("ExpressionId") into code from APZ_FCRE ;

		DELETE FROM APZ_FCRE WHERE "UserCode"<>:UserName AND "UserCode" in (select Data from Split(:CopyUser, ',')) AND "ItemId"=:ItemId AND "FormId"=:FormId;

		INSERT INTO APZ_FCRE
		        ( 
		        "FormId" ,
		          "ItemId" ,
		         "UserCode" ,
		          "ExpressionId" ,
		          "Expression" ,
		          "ColumnId" ,
		          "ApplyToRow" ,
		          "BackColor" ,
		          "BackColor2",
				  "BorderColor", 
				  "Font"
		        )
		        (
		SELECT "FormId","ItemId",x.Data ,:code+ ROW_NUMBER() OVER( ORDER BY "ExpressionId"),"Expression","ColumnId","ApplyToRow","BackColor","BackColor2", "BorderColor", "Font" FROM APZ_FCRE a
			inner join (select Data from split(:CopyUser,',')) x on 1=1
		WHERE "UserCode"=:UserName  AND "ItemId"=:ItemId AND "FormId"=:FormId AND (x.Data<>:UserName ));
END;



