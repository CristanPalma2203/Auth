-- Renombra importador → usuario_externo y actualiza TipoUsuario.
-- Ejecutar en la BD pos (Auth + ERP compartida).

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
SET TipoUsuario = N'usuario-externo'
WHERE TipoUsuario = N'importador';
GO

IF COL_LENGTH('dbo.recibo', 'ImportadorId') IS NOT NULL
   AND COL_LENGTH('dbo.recibo', 'UsuarioExternoId') IS NULL
BEGIN
    EXEC sp_rename 'dbo.recibo.ImportadorId', 'UsuarioExternoId', 'COLUMN';
END
GO

-- Pedido.TerceroId sigue apuntando a la misma tabla (ya renombrada).
IF OBJECT_ID(N'dbo.FK_pedido_tercero', N'F') IS NOT NULL
BEGIN
    ALTER TABLE dbo.pedido DROP CONSTRAINT FK_pedido_tercero;
END
GO
IF OBJECT_ID(N'dbo.usuario_externo', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.FK_pedido_tercero', N'F') IS NULL
BEGIN
    ALTER TABLE dbo.pedido WITH CHECK ADD CONSTRAINT FK_pedido_tercero
        FOREIGN KEY (TerceroId) REFERENCES dbo.usuario_externo(Id);
END
GO

IF OBJECT_ID(N'dbo.FK_recibo_importador', N'F') IS NOT NULL
BEGIN
    ALTER TABLE dbo.recibo DROP CONSTRAINT FK_recibo_importador;
END
GO
IF OBJECT_ID(N'dbo.usuario_externo', N'U') IS NOT NULL
   AND COL_LENGTH('dbo.recibo', 'UsuarioExternoId') IS NOT NULL
   AND OBJECT_ID(N'dbo.FK_recibo_usuario_externo', N'F') IS NULL
BEGIN
    ALTER TABLE dbo.recibo WITH CHECK ADD CONSTRAINT FK_recibo_usuario_externo
        FOREIGN KEY (UsuarioExternoId) REFERENCES dbo.usuario_externo(Id);
END
GO

UPDATE dbo.permiso SET
    Codigo = CASE Codigo
        WHEN N'importadores' THEN N'usuarios-externos'
        WHEN N'gestionar-importador' THEN N'gestionar-usuario-externo'
        WHEN N'listar-importadores' THEN N'listar-usuarios-externos'
        WHEN N'gestionar-accesos-importador' THEN N'gestionar-accesos-usuario-externo'
        WHEN N'importador-ver' THEN N'usuario-externo-ver'
        WHEN N'importador-editar' THEN N'usuario-externo-editar'
        ELSE Codigo END,
    Nombre = CASE Codigo
        WHEN N'importadores' THEN N'Usuarios externos'
        WHEN N'gestionar-importador' THEN N'Gestionar usuario externo'
        WHEN N'listar-importadores' THEN N'Usuarios externos'
        WHEN N'importador-ver' THEN N'Ver usuario externo'
        WHEN N'importador-editar' THEN N'Editar usuario externo'
        ELSE Nombre END,
    Url = REPLACE(Url, N'/importadores', N'/usuarios-externos'),
    Icono = CASE WHEN Icono = N'importador' THEN N'usuario-externo' ELSE Icono END
WHERE Codigo IN (
    N'importadores', N'gestionar-importador', N'listar-importadores',
    N'gestionar-accesos-importador', N'importador-ver', N'importador-editar'
)
OR Url LIKE N'/importadores%';
GO
