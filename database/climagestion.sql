SET NAMES utf8mb4;

CREATE DATABASE IF NOT EXISTS climagestion CHARACTER SET utf8mb4 COLLATE utf8mb4_spanish_ci;
USE climagestion;

DROP VIEW IF EXISTS v_stock_bajo, v_mantenimiento_pendiente, v_garantias_por_vencer, v_ingresos_mensuales;

DROP TABLE IF EXISTS presupuesto_items;
DROP TABLE IF EXISTS presupuestos;
DROP TABLE IF EXISTS trabajo_material;
DROP TABLE IF EXISTS trabajos;
DROP TABLE IF EXISTS solicitudes;
DROP TABLE IF EXISTS equipos_venta;
DROP TABLE IF EXISTS proveedor_material;
DROP TABLE IF EXISTS proveedores;
DROP TABLE IF EXISTS materiales;
DROP TABLE IF EXISTS servicios;
DROP TABLE IF EXISTS equipos;
DROP TABLE IF EXISTS clientes;
DROP TABLE IF EXISTS usuarios;
DROP TABLE IF EXISTS tipo_usuarios;

CREATE TABLE tipo_usuarios (
    id      INT AUTO_INCREMENT PRIMARY KEY,
    nombre  VARCHAR(30) NOT NULL UNIQUE
) ENGINE=InnoDB;

CREATE TABLE usuarios (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    nombre           VARCHAR(100) NOT NULL,
    email            VARCHAR(120) NOT NULL UNIQUE,
    password_hash    VARCHAR(255) NOT NULL,
    tipo_usuario_id  INT NOT NULL,
    activo           TINYINT(1) NOT NULL DEFAULT 1,
    fecha_alta       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_usuarios_tipo FOREIGN KEY (tipo_usuario_id) REFERENCES tipo_usuarios(id)
) ENGINE=InnoDB;

CREATE TABLE clientes (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    nombre      VARCHAR(120) NOT NULL,
    telefono    VARCHAR(30)  NOT NULL,
    email       VARCHAR(120) NULL,
    direccion   VARCHAR(150) NOT NULL,
    localidad   VARCHAR(80)  NOT NULL DEFAULT 'Bahía Blanca',
    activo      TINYINT(1) NOT NULL DEFAULT 1,
    fecha_alta  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE equipos (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id  INT NOT NULL,
    marca       VARCHAR(60) NOT NULL,
    modelo      VARCHAR(60) NULL,
    frigorias   INT NOT NULL,
    tipo        ENUM('split','ventana','portatil','piso_techo') NOT NULL DEFAULT 'split',
    ubicacion   VARCHAR(80) NULL,
    CONSTRAINT fk_equipos_cliente FOREIGN KEY (cliente_id) REFERENCES clientes(id)
) ENGINE=InnoDB;

CREATE TABLE servicios (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    nombre       VARCHAR(80) NOT NULL UNIQUE,
    descripcion  VARCHAR(255) NULL,
    precio_base  DECIMAL(12,2) NOT NULL DEFAULT 0,
    activo       TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB;

CREATE TABLE materiales (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    nombre        VARCHAR(100) NOT NULL UNIQUE,
    unidad        VARCHAR(20) NOT NULL,
    stock_actual  DECIMAL(10,2) NOT NULL DEFAULT 0,
    stock_minimo  DECIMAL(10,2) NOT NULL DEFAULT 0,
    precio        DECIMAL(12,2) NOT NULL DEFAULT 0,
    CONSTRAINT chk_materiales_stock CHECK (stock_actual >= 0 AND stock_minimo >= 0)
) ENGINE=InnoDB;

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

CREATE TABLE equipos_venta (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    marca        VARCHAR(50)  NOT NULL,
    modelo       VARCHAR(80)  NOT NULL,
    tipo         ENUM('split','ventana','portatil','piso_techo') NOT NULL DEFAULT 'split',
    frigorias    INT          NOT NULL,
    frio_calor   TINYINT(1)   NOT NULL DEFAULT 1,
    inverter     TINYINT(1)   NOT NULL DEFAULT 0,
    precio       DECIMAL(12,2) NOT NULL,
    stock        INT          NOT NULL DEFAULT 0,
    descripcion  VARCHAR(255) NULL,
    activo       TINYINT(1)   NOT NULL DEFAULT 1,
    CONSTRAINT chk_equipos_venta CHECK (frigorias > 0 AND precio > 0 AND stock >= 0)
) ENGINE=InnoDB;

CREATE TABLE solicitudes (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    nombre       VARCHAR(120) NOT NULL,
    telefono     VARCHAR(30)  NOT NULL,
    direccion    VARCHAR(150) NULL,
    servicio_id  INT NULL,
    mensaje      TEXT NULL,
    fecha        DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    estado       ENUM('pendiente','contactado','convertida') NOT NULL DEFAULT 'pendiente',
    cliente_id   INT NULL,
    CONSTRAINT fk_solicitudes_servicio FOREIGN KEY (servicio_id) REFERENCES servicios(id) ON DELETE SET NULL,
    CONSTRAINT fk_solicitudes_cliente  FOREIGN KEY (cliente_id)  REFERENCES clientes(id)  ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE trabajos (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id       INT NOT NULL,
    servicio_id      INT NOT NULL,
    equipo_id        INT NULL,
    tecnico_id       INT NULL,
    fecha_agendada   DATETIME NULL,
    fecha_realizado  DATE NULL,
    estado           ENUM('pendiente','agendado','realizado','cancelado') NOT NULL DEFAULT 'pendiente',
    total            DECIMAL(12,2) NOT NULL DEFAULT 0,
    garantia_hasta   DATE NULL,
    observaciones    TEXT NULL,
    CONSTRAINT fk_trabajos_cliente  FOREIGN KEY (cliente_id)  REFERENCES clientes(id),
    CONSTRAINT fk_trabajos_servicio FOREIGN KEY (servicio_id) REFERENCES servicios(id),
    CONSTRAINT fk_trabajos_equipo   FOREIGN KEY (equipo_id)   REFERENCES equipos(id)  ON DELETE SET NULL,
    CONSTRAINT fk_trabajos_tecnico  FOREIGN KEY (tecnico_id)  REFERENCES usuarios(id) ON DELETE SET NULL,
    CONSTRAINT uq_tecnico_horario UNIQUE (tecnico_id, fecha_agendada)
) ENGINE=InnoDB;

CREATE TABLE trabajo_material (
    trabajo_id   INT NOT NULL,
    material_id  INT NOT NULL,
    cantidad     DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (trabajo_id, material_id),
    CONSTRAINT fk_tm_trabajo  FOREIGN KEY (trabajo_id)  REFERENCES trabajos(id) ON DELETE CASCADE,
    CONSTRAINT fk_tm_material FOREIGN KEY (material_id) REFERENCES materiales(id),
    CONSTRAINT chk_tm_cantidad CHECK (cantidad > 0)
) ENGINE=InnoDB;

CREATE TABLE presupuestos (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id  INT NOT NULL,
    fecha       DATE NOT NULL,
    vence       DATE NOT NULL,
    total       DECIMAL(12,2) NOT NULL DEFAULT 0,
    estado      ENUM('pendiente','aceptado','vencido') NOT NULL DEFAULT 'pendiente',
    CONSTRAINT fk_presupuestos_cliente FOREIGN KEY (cliente_id) REFERENCES clientes(id)
) ENGINE=InnoDB;

CREATE TABLE presupuesto_items (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    presupuesto_id   INT NOT NULL,
    descripcion      VARCHAR(150) NOT NULL,
    cantidad         DECIMAL(10,2) NOT NULL DEFAULT 1,
    precio_unitario  DECIMAL(12,2) NOT NULL,
    CONSTRAINT fk_items_presupuesto FOREIGN KEY (presupuesto_id) REFERENCES presupuestos(id) ON DELETE CASCADE
) ENGINE=InnoDB;

INSERT INTO tipo_usuarios (id, nombre) VALUES
(1, 'Administrador'),
(2, 'Técnico');

INSERT INTO usuarios (nombre, email, password_hash, tipo_usuario_id) VALUES
('Administrador', 'admin@climagestion.test', '$2y$10$FxefwA.g4V8JPKTJzKDUROPCVDSbCVdmBrGWiKESYFF2pJC5k6dWC', 1);

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
('Refrigeración del Sur', '30-71234567-1', '291 450-0001'),
('Electricidad Bahía',    '30-71234568-9', '291 450-0002');

INSERT INTO proveedor_material (proveedor_id, material_id, precio_compra) VALUES
(1, 1, 7000), (1, 2, 9500), (1, 3, 20000), (1, 4, 24000), (1, 5, 11000),
(2, 6, 1800), (2, 7, 1000);

INSERT INTO equipos_venta (marca, modelo, tipo, frigorias, frio_calor, inverter, precio, stock, descripcion) VALUES
('BGH',     'Silent Air BS26WCCR',   'split',      2250, 1, 0,  780000, 3, 'Ideal para dormitorios de hasta 12 m².'),
('Surrey',  'Pría Inverter 3200',    'split',      2750, 1, 1, 1050000, 2, 'Inverter: gasta hasta un 40% menos de luz.'),
('Philco',  'PHS32HA4AN',            'split',      2800, 1, 0,  820000, 4, 'Buena relación precio y calidad para ambientes medianos.'),
('Samsung', 'WindFree AR12',         'split',      3000, 1, 1, 1390000, 2, 'Inverter, muy silencioso y con control por Wi-Fi.'),
('LG',      'Dual Inverter S4-W18',  'split',      4500, 1, 1, 1780000, 1, 'Para livings y comedores de hasta 30 m².'),
('Midea',   'Xtreme Save 5400',      'split',      5400, 1, 1, 1950000, 1, 'Alta potencia para ambientes grandes.'),
('BGH',     'Ventana BC30CP',        'ventana',    3000, 0, 0,  690000, 2, 'Solo frío. Fácil de instalar en una abertura de pared.'),
('Midea',   'Portátil MPPD-09',      'portatil',   2250, 0, 0,  620000, 3, 'Sin instalación: se enchufa y se mueve de ambiente.'),
('Surrey',  'Piso-techo 15000',      'piso_techo', 15000, 1, 0, 4900000, 0, 'Para locales y oficinas. Consultar disponibilidad.');

INSERT INTO clientes (nombre, telefono, email, direccion, localidad) VALUES
('Laura Gómez',  '291 555-0303', 'laura.gomez@ejemplo.test', 'Brown 420',    'Bahía Blanca'),
('Carlos Díaz',  '291 555-0404', NULL,                       'Sarmiento 77', 'Bahía Blanca'),
('Ana Ruiz',     '291 555-0505', 'ana.ruiz@ejemplo.test',    'Vieytes 1500', 'Punta Alta');

INSERT INTO equipos (cliente_id, marca, modelo, frigorias, tipo, ubicacion) VALUES
(1, 'BGH',     'Silent Air', 3000, 'split',      'Dormitorio'),
(1, 'Samsung', 'WindFree',   4500, 'split',      'Living'),
(2, 'Surrey',  'Pría',       2250, 'split',      'Dormitorio'),
(3, 'LG',      'Piso techo', 6000, 'piso_techo', 'Local comercial');

INSERT INTO solicitudes (nombre, telefono, direccion, servicio_id, mensaje) VALUES
('María López', '291 555-0101', 'Zelarrayán 850', 1,    'Quiero instalar un split en el living.'),
('Jorge Pérez', '291 555-0202', 'Alem 1200',      NULL, 'Consulto por el Samsung WindFree de 3000 frigorías.');

INSERT INTO trabajos (cliente_id, servicio_id, equipo_id, tecnico_id, fecha_agendada, fecha_realizado, estado, total, garantia_hasta) VALUES
(2, 1, 3, 1, CONCAT(CURDATE() - INTERVAL 40 DAY, ' 10:00:00'), CURDATE() - INTERVAL 40 DAY, 'realizado', 120000, CURDATE() - INTERVAL 40 DAY + INTERVAL 12 MONTH);

INSERT INTO trabajos (cliente_id, servicio_id, equipo_id, tecnico_id, fecha_agendada, estado, total) VALUES
(1, 1, 1, 1, CONCAT(CURDATE(), ' 10:00:00'),                     'agendado', 120000),
(1, 4, 2, 1, CONCAT(CURDATE() + INTERVAL 1 DAY, ' 15:00:00'),    'agendado',  35000);

INSERT INTO trabajos (cliente_id, servicio_id, equipo_id, estado, total, observaciones) VALUES
(3, 2, 4, 'pendiente', 45000, 'No enfría.');

INSERT INTO trabajo_material (trabajo_id, material_id, cantidad) VALUES
(1, 2, 3), (1, 5, 1),
(2, 1, 3), (2, 2, 3), (2, 5, 1), (2, 6, 5);

INSERT INTO presupuestos (cliente_id, fecha, vence, total) VALUES
(3, CURDATE(), CURDATE() + INTERVAL 15 DAY, 105000);

INSERT INTO presupuesto_items (presupuesto_id, descripcion, cantidad, precio_unitario) VALUES
(1, 'Reparación de placa electrónica', 1,   75000),
(1, 'Carga de gas R410A (kg)',         1.2, 25000);
