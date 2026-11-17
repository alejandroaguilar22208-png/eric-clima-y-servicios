<?php
/**
 * API REST (solo lectura) para las apps móvil y de escritorio.
 *
 * GET api/trabajos.php?estado=agendado
 * Encabezado obligatorio:  X-API-Key: <API_KEY de config.php>
 *
 * Respuesta: JSON con la lista de trabajos.
 */
require_once __DIR__ . '/../app/config.php';

header('Content-Type: application/json; charset=utf-8');

$clave = $_SERVER['HTTP_X_API_KEY'] ?? '';
if (!hash_equals(API_KEY, $clave)) {
    http_response_code(401);
    echo json_encode(['error' => 'Falta la clave de la API o es incorrecta.']);
    exit;
}

$estados = ['pendiente', 'agendado', 'realizado', 'cancelado'];
$estado = $_GET['estado'] ?? '';
if ($estado !== '' && !in_array($estado, $estados, true)) {
    http_response_code(400);
    echo json_encode(['error' => 'Estado inválido. Usá: ' . implode(', ', $estados)]);
    exit;
}

$sql = 'SELECT t.id, t.estado, t.fecha_agendada, t.total, t.garantia_hasta, t.observaciones,
               c.nombre AS cliente, c.telefono, c.direccion, c.localidad,
               s.nombre AS servicio, u.nombre AS tecnico
        FROM trabajos t
        JOIN clientes c ON c.id = t.cliente_id
        JOIN servicios s ON s.id = t.servicio_id
        LEFT JOIN usuarios u ON u.id = t.tecnico_id'
     . ($estado ? ' WHERE t.estado = ?' : '')
     . ' ORDER BY t.fecha_agendada IS NULL, t.fecha_agendada';

$st = db()->prepare($sql);
$st->execute($estado ? [$estado] : []);

echo json_encode(['trabajos' => $st->fetchAll()], JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
