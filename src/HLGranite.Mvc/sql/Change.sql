sp_rename 'Nisans.WorItemId', 'WorkItemId', 'COLUMN';
exec sp_columns Nisans;
alter table Nisans alter column Deathm datetime2;