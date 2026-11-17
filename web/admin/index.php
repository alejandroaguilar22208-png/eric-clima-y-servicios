<?php
/** Inicio del panel: números del día y alertas. */
require_once __DIR__ . '/../app/auth.php';
require_once __DIR__ . '/../app/layout.php';
requerir_login();

$db = db();
$solicitudesPendientes = $db->query("SELECT COUNT(*) FROM solicitudes WHERE estado = 'pendiente'")->fetchColumn();
$trabajosHoy = $db->query("SELECT COUNT(*) FROM trabajos WHERE estado = 'agendado' AND DATE(fecha_agendada) = CURDATE()")->fetchColumn();
$sinAgendar  = $db->query("SELECT COUNT(*) FROM trabajos WHERE estado = 'pendiente'")->fetchColumn();
$stockBajo   = $db->query('SELECT nombre, stock_actual, stock_minimo, unidad FROM v_stock_bajo')->fetchAll();
$agendaHoy   = $db->query(
    "SELECT t.fecha_agendada, c.nombre AS cliente, c.direccion, s.nombre AS servicio
     FROM trabajos t JOIN clientes c ON c.id = t.cliente_id JOIN servicios s ON s.id = t.servicio_id
     WHERE t.estado = 'agendado' AND DATE(t.fecha_agendada) = CURDATE() ORDER BY t.fecha_agendada"
)->fetchAll();
$garantias   = $db->query('SELECT * FROM v_garantias_por_vencer ORDER BY garantia_hasta')->fetchAll();
$mantenimientos = $db->query('SELECT * FROM v_mantenimiento_pendiente ORDER BY ultimo_trabajo')->fetchAll();

layout_inicio('Inicio', 'index');
?>
<div class="row g-3 mb-4">
    <?php
    $tarjetas = [
        ['Solicitudes nuevas', $solicitudesPendientes, 'solicitudes.php'],
        ['Trabajos hoy', $trabajosHoy, 'trabajos.php?estado=agendado'],
        ['Sin agendar', $sinAgendar, 'trabajos.php?estado=pendiente'],
        ['Materiales con stock bajo', count($stockBajo), 'materiales.php'],
    ];
    foreach ($tarjetas as [$texto, $numero, $link]): ?>
        <div class="col-6 col-lg-3">
            <a href="<?= $link ?>" class="text-decoration-none">
                <div class="card tarjeta-numero shadow-sm h-100"><div class="card-body">
                    <div class="numero"><?= (int)$numero ?></div>
                    <div class="text-muted"><?= $texto ?></div>
                </div></div>
            </a>
        </div>
    <?php endforeach; ?>
</div>

<div class="row g-4">
    <div class="col-lg-6">
        <div class="card shadow-sm h-100">
            <div class="card-header fw-semibold">Agenda de hoy</div>
            <ul class="list-group list-group-flush">
                <?php foreach ($agendaHoy as $t): ?>
                    <li class="list-group-item">
                        <strong><?= date('H:i', strtotime($t['fecha_agendada'])) ?></strong> · <?= e($t['servicio']) ?><br>
                        <small class="text-muted"><?= e($t['cliente']) ?> — <?= e($t['direccion']) ?></small>
                    </li>
                <?php endforeach; ?>
                <?php if (!$agendaHoy): ?><li class="list-group-item text-muted">No hay trabajos agendados para hoy.</li><?php endif; ?>
            </ul>
        </div>
    </div>
    <div class="col-lg-6">
        <div class="card shadow-sm h-100">
            <div class="card-header fw-semibold">Alertas</div>
            <ul class="list-group list-group-flush">
                <?php foreach ($stockBajo as $m): ?>
                    <li class="list-group-item list-group-item-danger">Stock bajo: <?= e($m['nombre']) ?> — quedan <?= (float)$m['stock_actual'] ?> <?= e($m['unidad']) ?></li>
                <?php endforeach; ?>
                <?php foreach ($garantias as $g): ?>
                    <li class="list-group-item list-group-item-warning">Garantía por vencer: <?= e($g['nombre']) ?> (<?= e($g['servicio']) ?>) el <?= date('d/m/Y', strtotime($g['garantia_hasta'])) ?></li>
                <?php endforeach; ?>
                <?php foreach ($mantenimientos as $m): ?>
                    <li class="list-group-item list-group-item-info">Mantenimiento anual: <?= e($m['nombre']) ?> · <?= e($m['telefono']) ?></li>
                <?php endforeach; ?>
                <?php if (!$stockBajo && !$garantias && !$mantenimientos): ?><li class="list-group-item text-muted">No hay alertas.</li><?php endif; ?>
            </ul>
        </div>
    </div>
</div>
<?php layout_fin();
