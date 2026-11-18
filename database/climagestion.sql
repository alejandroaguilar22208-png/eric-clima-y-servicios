-- =====================================================================
--  ClimaGestión - Base de datos
--  Sistema de gestión para servicio técnico de climatización
--  Motor: MySQL 8 / MariaDB 10.4+ (XAMPP)   Cotejamiento: utf8mb4
--  Compartida por: Web (PHP), Escritorio (C#) y Móvil
--
--  Usuario de prueba (solo desarrollo, cambiar en producción):
--    email:      admin@climagestion.test
--    contraseña: Clima2026!
--  El hash es bcrypt ($2y$): lo valida password_verify() en PHP
--  y BCrypt.Net.BCrypt.Verify() en C#.
-- =====================================================================

SET NAMES utf8mb4; -- el archivo está en UTF-8 (evita acentos rotos al importar)

DROP DATABASE IF EXISTS climagestion;
CREATE DATABASE climagestion CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE climagestion;

-- ---------------------------------------------------------------------
-- USUARIOS: quienes ingresan al sistema (RN-09, RN-10)
-- ---------------------------------------------------------------------
CREATE TABLE usuarios (
    id             INT AUTO_INCREMENT PRIMARY KEY,
    nombre         VARCHAR(100) NOT NULL,
    email          VARCHAR(120) NOT NULL UNIQUE,
    password_hash  VARCHAR(255) NOT NULL,
    rol            ENUM('admin','tecnico') NOT NULL DEFAULT 'tecnico',
    activo         TINYINT(1) NOT NULL DEFAULT 1,
    fecha_alta     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- CLIENTES (RN-08: no se borran si tienen trabajos, se desactivan)
-- ---------------------------------------------------------------------
CREATE TABLE clientes (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    nombre      VARCHAR(120) NOT NULL,
    telefono    VARCHAR(30)  NOT NULL,
    email       VARCHAR(120) NULL,
    direccion   VARCHAR(150) NOT NULL,
    localidad   VARCHAR(80)  NOT NULL DEFAULT 'Bahía Blanca',
    activo      TINYINT(1) NOT NULL DEFAULT 1,
    fecha_alta  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_clientes_nombre (nombre),
    INDEX idx_clientes_telefono (telefono)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- EQUIPOS instalados en cada cliente
-- ---------------------------------------------------------------------
CREATE TABLE equipos (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id  INT NOT NULL,
    marca       VARCHAR(60) NOT NULL,
    modelo      VARCHAR(60) NULL,
    frigorias   INT NOT NULL,
    tipo        ENUM('split','ventana','portatil','piso_techo','central') NOT NULL DEFAULT 'split',
    ubicacion   VARCHAR(80) NULL COMMENT 'Ej: dormitorio, living',
    CONSTRAINT fk_equipos_cliente FOREIGN KEY (cliente_id) REFERENCES clientes(id)
        ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- SERVICIOS que ofrece el negocio (se muestran en la web pública)
-- ---------------------------------------------------------------------
CREATE TABLE servicios (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    nombre       VARCHAR(80) NOT NULL UNIQUE,
    descripcion  TEXT NULL,
    precio_base  DECIMAL(12,2) NOT NULL DEFAULT 0,
    activo       TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- SOLICITUDES que llegan desde el formulario de la web
-- cliente_id se completa cuando la solicitud se convierte en cliente
-- ---------------------------------------------------------------------
CREATE TABLE solicitudes (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id   INT NULL,
    nombre       VARCHAR(120) NOT NULL,
    telefono     VARCHAR(30)  NOT NULL,
    direccion    VARCHAR(150) NULL,
    servicio_id  INT NULL,
    mensaje      TEXT NULL,
    fecha        DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    estado       ENUM('pendiente','contactado','convertida','descartada') NOT NULL DEFAULT 'pendiente',
    CONSTRAINT fk_solicitudes_cliente  FOREIGN KEY (cliente_id)  REFERENCES clientes(id)  ON DELETE SET NULL,
    CONSTRAINT fk_solicitudes_servicio FOREIGN KEY (servicio_id) REFERENCES servicios(id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- TRABAJOS (RN-01, RN-03, RN-04, RN-05, RN-07)
-- ---------------------------------------------------------------------
CREATE TABLE trabajos (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id       INT NOT NULL,
    equipo_id        INT NULL,
    servicio_id      INT NOT NULL,
    tecnico_id       INT NULL,
    fecha_agendada   DATETIME NULL,
    fecha_realizado  DATE NULL,
    estado           ENUM('pendiente','agendado','realizado','cancelado') NOT NULL DEFAULT 'pendiente',
    total            DECIMAL(12,2) NOT NULL DEFAULT 0,
    garantia_hasta   DATE NULL,
    observaciones    TEXT NULL,
    CONSTRAINT fk_trabajos_cliente  FOREIGN KEY (cliente_id)  REFERENCES clientes(id)  ON DELETE RESTRICT,
    CONSTRAINT fk_trabajos_equipo   FOREIGN KEY (equipo_id)   REFERENCES equipos(id)   ON DELETE SET NULL,
    CONSTRAINT fk_trabajos_servicio FOREIGN KEY (servicio_id) REFERENCES servicios(id) ON DELETE RESTRICT,
    CONSTRAINT fk_trabajos_tecnico  FOREIGN KEY (tecnico_id)  REFERENCES usuarios(id)  ON DELETE SET NULL,
    -- RN-07: un técnico no puede tener dos trabajos en la misma fecha y hora
    UNIQUE KEY uq_tecnico_horario (tecnico_id, fecha_agendada),
    INDEX idx_trabajos_estado (estado)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- MATERIALES y stock (RN-02, RN-11)
-- ---------------------------------------------------------------------
CREATE TABLE materiales (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    nombre        VARCHAR(100) NOT NULL UNIQUE,
    unidad        VARCHAR(20) NOT NULL COMMENT 'm, kg, unidad',
    stock_actual  DECIMAL(10,2) NOT NULL DEFAULT 0,
    stock_minimo  DECIMAL(10,2) NOT NULL DEFAULT 0,
    precio        DECIMAL(12,2) NOT NULL DEFAULT 0,
    CONSTRAINT chk_stock_no_negativo CHECK (stock_actual >= 0)
) ENGINE=InnoDB;

-- Materiales usados en cada trabajo (relación N a N)
CREATE TABLE trabajo_material (
    trabajo_id   INT NOT NULL,
    material_id  INT NOT NULL,
    cantidad     DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (trabajo_id, material_id),
    CONSTRAINT fk_tm_trabajo  FOREIGN KEY (trabajo_id)  REFERENCES trabajos(id)   ON DELETE CASCADE,
    CONSTRAINT fk_tm_material FOREIGN KEY (material_id) REFERENCES materiales(id) ON DELETE RESTRICT,
    CONSTRAINT chk_tm_cantidad CHECK (cantidad > 0)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- PROVEEDORES de materiales (relación N a N con materiales)
-- ---------------------------------------------------------------------
CREATE TABLE proveedores (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    nombre     VARCHAR(120) NOT NULL,
    cuit       VARCHAR(13)  NULL UNIQUE,
    telefono   VARCHAR(30)  NULL,
    email      VARCHAR(120) NULL,
    direccion  VARCHAR(150) NULL
) ENGINE=InnoDB;

CREATE TABLE proveedor_material (
    proveedor_id   INT NOT NULL,
    material_id    INT NOT NULL,
    precio_compra  DECIMAL(12,2) NOT NULL,
    PRIMARY KEY (proveedor_id, material_id),
    CONSTRAINT fk_pm_proveedor FOREIGN KEY (proveedor_id) REFERENCES proveedores(id) ON DELETE CASCADE,
    CONSTRAINT fk_pm_material  FOREIGN KEY (material_id)  REFERENCES materiales(id)  ON DELETE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- PRESUPUESTOS e ítems (RN-06: vencen a los 15 días)
-- ---------------------------------------------------------------------
CREATE TABLE presupuestos (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id  INT NOT NULL,
    fecha       DATE NOT NULL,
    vence       DATE NULL COMMENT 'si se deja vacío, el trigger pone fecha + 15 días',
    total       DECIMAL(12,2) NOT NULL DEFAULT 0,
    estado      ENUM('pendiente','aceptado','rechazado','vencido') NOT NULL DEFAULT 'pendiente',
    CONSTRAINT fk_presupuestos_cliente FOREIGN KEY (cliente_id) REFERENCES clientes(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE presupuesto_items (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    presupuesto_id   INT NOT NULL,
    descripcion      VARCHAR(150) NOT NULL,
    cantidad         DECIMAL(10,2) NOT NULL DEFAULT 1,
    precio_unitario  DECIMAL(12,2) NOT NULL,
    CONSTRAINT fk_items_presupuesto FOREIGN KEY (presupuesto_id) REFERENCES presupuestos(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- GALERIA de trabajos para la web pública
-- ---------------------------------------------------------------------
CREATE TABLE galeria (
    id      INT AUTO_INCREMENT PRIMARY KEY,
    titulo  VARCHAR(100) NOT NULL,
    imagen  VARCHAR(255) NOT NULL COMMENT 'ruta dentro de /uploads',
    fecha   DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- =====================================================================
--  TRIGGERS de reglas de negocio
-- =====================================================================
DELIMITER $$

-- RN-02: no usar más material del que hay en stock
CREATE TRIGGER trg_tm_validar_stock
BEFORE INSERT ON trabajo_material
FOR EACH ROW
BEGIN
    DECLARE disponible DECIMAL(10,2);
    SELECT stock_actual INTO disponible FROM materiales WHERE id = NEW.material_id;
    IF NEW.cantidad > disponible THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Stock insuficiente para el material solicitado';
    END IF;
END$$

-- RN-03, RN-04, RN-05: reglas al cambiar el estado de un trabajo
CREATE TRIGGER trg_trabajos_estado
BEFORE UPDATE ON trabajos
FOR EACH ROW
BEGIN
    IF OLD.estado IN ('realizado','cancelado') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Un trabajo realizado o cancelado no puede modificarse';
    END IF;
    IF NEW.estado = 'realizado' AND OLD.estado <> 'realizado' THEN
        IF NEW.fecha_agendada IS NULL OR NEW.tecnico_id IS NULL THEN
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Para marcar Realizado el trabajo necesita fecha y técnico';
        END IF;
        IF NEW.fecha_realizado IS NULL THEN
            SET NEW.fecha_realizado = CURDATE();
        END IF;
        SET NEW.garantia_hasta = DATE_ADD(NEW.fecha_realizado, INTERVAL 12 MONTH);
    END IF;
END$$

-- RN-04: al quedar Realizado se descuentan los materiales del stock
CREATE TRIGGER trg_trabajos_descontar_stock
AFTER UPDATE ON trabajos
FOR EACH ROW
BEGIN
    IF NEW.estado = 'realizado' AND OLD.estado <> 'realizado' THEN
        UPDATE materiales m
        JOIN trabajo_material tm ON tm.material_id = m.id
        SET m.stock_actual = m.stock_actual - tm.cantidad
        WHERE tm.trabajo_id = NEW.id;
    END IF;
END$$

-- RN-06: el presupuesto vence a los 15 días si no se indica otra fecha
CREATE TRIGGER trg_presupuestos_vence
BEFORE INSERT ON presupuestos
FOR EACH ROW
BEGIN
    IF NEW.vence IS NULL THEN
        SET NEW.vence = DATE_ADD(NEW.fecha, INTERVAL 15 DAY);
    END IF;
END$$

DELIMITER ;

-- =====================================================================
--  VISTAS para alertas y reportes
-- =====================================================================

-- RN-11: materiales con stock bajo
CREATE VIEW v_stock_bajo AS
SELECT id, nombre, unidad, stock_actual, stock_minimo
FROM materiales
WHERE stock_actual <= stock_minimo;

-- RN-12: clientes a los que les toca mantenimiento (último trabajo hace 12 meses o más)
CREATE VIEW v_mantenimiento_pendiente AS
SELECT c.id AS cliente_id, c.nombre, c.telefono, c.direccion,
       MAX(t.fecha_realizado) AS ultimo_trabajo
FROM clientes c
JOIN trabajos t ON t.cliente_id = c.id AND t.estado = 'realizado'
WHERE c.activo = 1
GROUP BY c.id, c.nombre, c.telefono, c.direccion
HAVING MAX(t.fecha_realizado) <= DATE_SUB(CURDATE(), INTERVAL 12 MONTH);

-- Garantías que vencen en los próximos 30 días
CREATE VIEW v_garantias_por_vencer AS
SELECT t.id AS trabajo_id, c.nombre, c.telefono, s.nombre AS servicio, t.garantia_hasta
FROM trabajos t
JOIN clientes c  ON c.id = t.cliente_id
JOIN servicios s ON s.id = t.servicio_id
WHERE t.garantia_hasta BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 30 DAY);

-- Ingresos y cantidad de trabajos por mes
CREATE VIEW v_ingresos_mensuales AS
SELECT DATE_FORMAT(fecha_realizado, '%Y-%m') AS mes,
       COUNT(*) AS trabajos,
       SUM(total) AS ingresos
FROM trabajos
WHERE estado = 'realizado'
GROUP BY DATE_FORMAT(fecha_realizado, '%Y-%m');

-- =====================================================================
--  DATOS DE PRUEBA
-- =====================================================================
INSERT INTO usuarios (nombre, email, password_hash, rol) VALUES
('Administrador', 'admin@climagestion.test', '$2y$10$FxefwA.g4V8JPKTJzKDUROPCVDSbCVdmBrGWiKESYFF2pJC5k6dWC', 'admin');

INSERT INTO servicios (nombre, descripcion, precio_base) VALUES
('Instalación de split',     'Instalación completa con hasta 3 m de cañería, soportes y prueba de funcionamiento.', 120000),
('Reparación',               'Diagnóstico y reparación de fallas eléctricas o de refrigeración.',                     45000),
('Carga de gas',             'Detección de fugas y carga de gas refrigerante.',                                       60000),
('Mantenimiento preventivo', 'Limpieza de filtros, serpentinas y control de presiones.',                               35000),
('Desinstalación',           'Retiro del equipo recuperando el gas.',                                                 40000);

INSERT INTO materiales (nombre, unidad, stock_actual, stock_minimo, precio) VALUES
('Caño de cobre 1/4"',        'm',      30, 10,  9000),
('Caño de cobre 3/8"',        'm',      25, 10, 12000),
('Gas R410A',                 'kg',      8,  3, 25000),
('Gas R22',                   'kg',      2,  2, 30000),
('Ménsula para condensadora', 'unidad',  6,  2, 15000),
('Cable tipo taller 4x1.5',   'm',      40, 15,  2500),
('Cinta vinílica',            'unidad', 12,  5,  1500);

INSERT INTO proveedores (nombre, cuit, telefono) VALUES
('Proveedor de prueba Refrigeración', '30-00000001-0', '291-4500001'),
('Proveedor de prueba Electricidad',  '30-00000002-0', '291-4500002');

INSERT INTO proveedor_material (proveedor_id, material_id, precio_compra) VALUES
(1, 1, 7000), (1, 2, 9500), (1, 3, 20000), (1, 4, 24000), (1, 5, 11000),
(2, 6, 1800), (2, 7, 1000);

INSERT INTO clientes (nombre, telefono, email, direccion) VALUES
('Cliente de prueba Uno',  '291-4000001', 'uno@ejemplo.test',  'Calle Falsa 123'),
('Cliente de prueba Dos',  '291-4000002', NULL,                'Avenida Siempreviva 742'),
('Cliente de prueba Tres', '291-4000003', 'tres@ejemplo.test', 'Pasaje Prueba 45');

INSERT INTO equipos (cliente_id, marca, modelo, frigorias, tipo, ubicacion) VALUES
(1, 'Marca A', 'Modelo 3000', 3000, 'split', 'Dormitorio'),
(1, 'Marca B', 'Modelo 4500', 4500, 'split', 'Living'),
(2, 'Marca A', 'Modelo 2250', 2250, 'split', 'Dormitorio'),
(3, 'Marca C', 'Modelo 6000', 6000, 'piso_techo', 'Local comercial');

INSERT INTO solicitudes (nombre, telefono, direccion, servicio_id, mensaje) VALUES
('Visitante de prueba', '291-4000009', 'Calle de Prueba 999', 1, 'Quiero instalar un split de 3000 frigorías.');

INSERT INTO trabajos (cliente_id, equipo_id, servicio_id, tecnico_id, fecha_agendada, estado, total) VALUES
(1, 1, 1, 1, '2026-11-20 09:00:00', 'agendado',  150000),
(2, 3, 4, 1, '2026-11-21 15:00:00', 'agendado',   35000),
(3, 4, 2, NULL, NULL,               'pendiente',  45000);

INSERT INTO trabajo_material (trabajo_id, material_id, cantidad) VALUES
(1, 1, 3), (1, 2, 3), (1, 5, 1), (1, 6, 5);

INSERT INTO presupuestos (cliente_id, fecha, vence, total) VALUES
(3, CURDATE(), NULL, 105000);

INSERT INTO presupuesto_items (presupuesto_id, descripcion, cantidad, precio_unitario) VALUES
(1, 'Reparación de placa electrónica', 1, 75000),
(1, 'Carga de gas R410A (kg)',         1.2, 25000);
