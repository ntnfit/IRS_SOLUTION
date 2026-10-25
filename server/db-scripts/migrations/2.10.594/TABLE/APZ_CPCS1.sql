
CREATE TABLE APZ_CPCS1
(
	"DocEntry" Int NOT NULL,
	"LineNum" Int NOT NULL,
	"ColName" Nvarchar(50) NULL,
	"ColText" Nvarchar(254) NULL,
	"VisOrder" Int NULL,
	PRIMARY KEY("DocEntry","LineNum")
);