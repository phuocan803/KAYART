USE KayArtDB;
GO

PRINT 'KAYARTDB SCHEMA STRUCTURE';

PRINT '1. Tables and Row Counts:';
PRINT '----------------------------------------';

SELECT 
    t.name AS TableName,
    p.rows AS RowCount,
    CAST(ROUND((SUM(a.total_pages) * 8) / 1024.00, 2) AS NUMERIC(36, 2)) AS TotalSpaceMB
FROM 
    sys.tables t
INNER JOIN      
    sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN 
    sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN 
    sys.allocation_units a ON p.partition_id = a.container_id
WHERE 
    t.is_ms_shipped = 0
    AND i.OBJECT_ID > 255
GROUP BY 
    t.name, p.Rows
ORDER BY 
    t.name;
GO

PRINT '';

PRINT '2. Table Columns:';
PRINT '----------------------------------------';

SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable,
    c.is_identity AS IsIdentity
FROM 
    sys.tables t
INNER JOIN 
    sys.columns c ON t.object_id = c.object_id
INNER JOIN 
    sys.types ty ON c.user_type_id = ty.user_type_id
WHERE 
    t.is_ms_shipped = 0
ORDER BY 
    t.name, c.column_id;
GO

PRINT '';

PRINT '3. Indexes:';
PRINT '----------------------------------------';

SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    COL_NAME(ic.object_id, ic.column_id) AS ColumnName
FROM 
    sys.tables t
INNER JOIN 
    sys.indexes i ON t.object_id = i.object_id
INNER JOIN 
    sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE 
    t.is_ms_shipped = 0
    AND i.name IS NOT NULL
ORDER BY 
    t.name, i.name, ic.key_ordinal;
GO

PRINT '';

PRINT '4. Foreign Key Relationships:';
PRINT '----------------------------------------';

SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn,
    fk.delete_referential_action_desc AS OnDelete
FROM 
    sys.foreign_keys fk
INNER JOIN 
    sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
ORDER BY 
    TableName, ForeignKeyName;
GO

