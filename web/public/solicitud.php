<?php
/**
 * Recibe el formulario de la página pública y guarda la solicitud.
 * Después redirige (patrón POST → redirección → GET) para que
 * al recargar la página no se envíe dos veces.
 */
require_once __DIR__ . '/../app/config.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') redirigir('index.php');
csrf_verificar();

// Campo trampa completado = robot de spam: hacemos como que salió bien
if (!empty($_POST['sitio_web'])) redirigir('index.php?enviado=1#contacto');

$datos = [
    'nombre'      => trim($_POST['nombre'] ?? ''),
    'telefono'    => trim($_POST['telefono'] ?? ''),
    'direccion'   => trim($_POST['direccion'] ?? ''),
    'servicio_id' => $_POST['servicio_id'] ?? '',
    'mensaje'     => trim($_POST['mensaje'] ?? ''),
];

// Validación del lado del servidor (no se confía solo en el navegador)
$error = '';
if (mb_strlen($datos['nombre']) < 3)                          $error = 'Escribí tu nombre y apellido.';
elseif (!preg_match('/^[0-9 +()-]{6,30}$/', $datos['telefono'])) $error = 'Escribí un teléfono válido (solo números).';
elseif (mb_strlen($datos['mensaje']) > 1000)                  $error = 'El mensaje es demasiado largo.';

if ($error) {
    $_SESSION['form_viejo'] = $datos;
    $_SESSION['form_error'] = $error;
    redirigir('index.php#contacto');
}

$st = db()->prepare(
    'INSERT INTO solicitudes (nombre, telefono, direccion, servicio_id, mensaje) VALUES (?, ?, ?, ?, ?)'
);
$st->execute([
    $datos['nombre'],
    $datos['telefono'],
    $datos['direccion'] ?: null,
    ctype_digit((string)$datos['servicio_id']) ? (int)$datos['servicio_id'] : null,
    $datos['mensaje'] ?: null,
]);

redirigir('index.php?enviado=1#contacto');
