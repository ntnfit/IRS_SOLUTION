

CREATE PROCEDURE APZ_D_ImageAlbum_Saves
AS
BEGIN
	
	create local temporary table #APZ_D_ImageAlbumLocal as (select top 0 * from APZ_D_ImageAlbum);
	exec 'drop table #APZ_D_ImageAlbumLocal;';
	exec 'create local temporary table #APZ_D_ImageAlbumLocal as (select * from "#APZ_D_ImageAlbum");';

	delete from APZ_D_ImageAlbum;
	insert into APZ_D_ImageAlbum("Id", "Name", "Remarks") select * from #APZ_D_ImageAlbumLocal;
 	select 0, '' from dummy;

END



