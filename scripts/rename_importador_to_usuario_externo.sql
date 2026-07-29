-- Renombra importador → usuario_externo y actualiza TipoUsuario.
-- Ejecutar en la BD corelux (Auth + Backend compartida).

IF OBJECT_ID(N'dbo.importador', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.usuario_externo', N'U') IS NULL
BEGIN
    EXEC sp_rename 'dbo.importador', 'usuario_externo';
END
GO

IF OBJECT_ID(N'dbo.usuario_externo', N'U') IS NOT NULL
   AND COL_LENGTH('dbo.usuario_externo', 'Apellidos') IS NULL
BEGIN
    ALTER TABLE dbo.usuario_externo ADD Apellidos nvarchar(max) NULL;
END
GO
IF OBJECT_ID(N'dbo.importador', N'U') IS NOT NULL
   AND COL_LENGTH('dbo.importador', 'Apellidos') IS NULL
BEGIN
    ALTER TABLE dbo.importador ADD Apellidos nvarchar(max) NULL;
END
GO

UPDATE dbo.usuario
SET TipoUsuario = N'external-user'
WHERE TipoUsuario = N'importador';
GO

IF COL_LENGTH('dbo.receipt', 'ImportadorId') IS NOT NULL
   AND COL_LENGTH('dbo.receipt', 'UsuarioExternoId') IS NULL
BEGIN
    EXEC sp_rename 'dbo.receipt.ImportadorId', 'UsuarioExternoId', 'COLUMN';
END
GO

-- Pedido.TerceroId sigue apuntando a la misma tabla (ya renombrada).
IF OBJECT_ID(N'dbo.FK_salesOrder_tercero', N'F') IS NOT NULL
BEGIN
    ALTER TABLE dbo.pedido DROP CONSTRAINT FK_salesOrder_tercero;
END
GO
IF OBJECT_ID(N'dbo.usuario_externo', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.FK_salesOrder_tercero', N'F') IS NULL
BEGIN
    ALTER TABLE dbo.pedido WITH CHECK ADD CONSTRAINT FK_salesOrder_tercero
        FOREIGN KEY (TerceroId) REFERENCES dbo.usuario_externo(Id);
END
GO

IF OBJECT_ID(N'dbo.FK_receipt_importador', N'F') IS NOT NULL
BEGIN
    ALTER TABLE dbo.receipt DROP CONSTRAINT FK_receipt_importador;
END
GO
IF OBJECT_ID(N'dbo.usuario_externo', N'U') IS NOT NULL
   AND COL_LENGTH('dbo.receipt', 'UsuarioExternoId') IS NOT NULL
   AND OBJECT_ID(N'dbo.FK_receipt_usuario_externo', N'F') IS NULL
BEGIN
    ALTER TABLE dbo.receipt WITH CHECK ADD CONSTRAINT FK_receipt_usuario_externo
        FOREIGN KEY (UsuarioExternoId) REFERENCES dbo.usuario_externo(Id);
END
GO

UPDATE dbo.permiso SET
    Codigo = CASE Codigo
        WHEN N'importadores' THEN N'external-users'
        WHEN N'manage-external-user' THEN N'manage-external-user'
        WHEN N'listar-importadores' THEN N'external-user-list'
        WHEN N'manage-external-user-access' THEN N'manage-external-user-access'
        WHEN N'importador-ver' THEN N'external-user-view'
        WHEN N'external-user-edit' THEN N'external-user-edit'
        ELSE Codigo END,
    Nombre = CASE Codigo
        WHEN N'importadores' THEN N'Usuarios externos'
        WHEN N'manage-external-user' THEN N'Gestionar usuario externo'
        WHEN N'listar-importadores' THEN N'Usuarios externos'
        WHEN N'importador-ver' THEN N'Ver usuario externo'
        WHEN N'external-user-edit' THEN N'Editar usuario externo'
        ELSE Nombre END,
    Url = REPLACE(Url, N'/importadores', N'/external-users'),
    Icono = CASE WHEN Icono = N'importador' THEN N'external-user' ELSE Icono END
WHERE Codigo IN (
    N'importadores', N'manage-external-user', N'listar-importadores',
    N'manage-external-user-access', N'importador-ver', N'external-user-edit'
)
OR Url LIKE N'/importadores%';
GO
