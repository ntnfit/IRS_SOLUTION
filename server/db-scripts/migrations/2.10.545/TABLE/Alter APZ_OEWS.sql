
ALTER TABLE APZ_OEWS ALTER("WorkCode" nvarchar(250));

GO

ALTER TABLE APZ_OEWS ALTER("AbsentCode" nvarchar(250));

GO

ALTER TABLE APZ_OEWS ADD("CalcHld" char(1) NULL);

GO

ALTER TABLE APZ_OEWS ADD("HldCode" nvarchar(250) NULL);
