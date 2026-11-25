<?php
require_once __DIR__ . '/../includes/funciones.php';
requerirLogin();

$solicitudesPendientes = $pdo->query("SELECT COUNT(*) FROM solicitudes WHERE estado = 'pendiente'")->fetchColumn();
$trabajosHoy = $pdo->query("SELECT COUNT(*) FROM trabajos WHERE estado = 'agendado' AND DATE(fecha_agendada) = CURDATE()")->fetchColumn();
$sinAgendar  = $pdo->query("SELECT COUNT(*) FROM trabajos WHERE estado = 'pendiente'")->fetchColumn();
$stockBajo   = $pdo->query('SELECT nombre, stock_actual, stock_minimo, unidad FROM materiales WHERE stock_actual <= stock_minimo ORDER BY nombre')->fetchAll();
$agendaHoy   = $pdo->query(
    "SELECT t.fecha_agendada, c.nombre AS cliente, c.direccion, s.nombre AS servicio
     FROM trabajos t JOIN clientes c ON c.id = t.cliente_id JOIN servicios s ON s.id = t.servicio_id
     WHERE t.estado = 'agendado' AND DATE(t.fecha_agendada) = CURDATE() ORDER BY t.fecha_agendada"
)->fetchAll();
$garantias   = $pdo->query(
    "SELECT c.nombre, s.nombre AS servicio, t.garantia_hasta
     FROM trabajos t JOIN clientes c ON c.id = t.cliente_id JOIN servicios s ON s.id = t.servicio_id
     WHERE t.garantia_hasta BETWEEN CURDATE() AND CURDATE() + INTERVAL 30 DAY ORDER BY t.garantia_hasta"
)->fetchAll();
$mantenimientos = $pdo->query(
    "SELECT c.nombre, c.telefono, MAX(t.fecha_realizado) AS ultimo_trabajo
     FROM clientes c JOIN trabajos t ON t.cliente_id = c.id AND t.estado = 'realizado'
     WHERE c.activo = 1
     GROUP BY c.id, c.nombre, c.telefono
     HAVING MAX(t.fecha_realizado) <= CURDATE() - INTERVAL 12 MONTH
     ORDER BY ultimo_trabajo"
)->fetchAll();

$titulo = 'Inicio';
$activo = 'index';
require __DIR__ . '/../includes/header.php';
?>
<div class="row g-3 mb-4">
    <?php
    $tarjetas = [
        ['Solicitudes nuevas', $solicitudesPendientes, 'solicitudes.php', 'bi-inbox-fill', 'azul'],
        ['Trabajos hoy', $trabajosHoy, 'trabajos.php?estado=agendado', 'bi-calendar-check-fill', 'verde'],
        ['Sin agendar', $sinAgendar, 'trabajos.php?estado=pendiente', 'bi-hourglass-split', 'naranja'],
        ['Materiales con stock bajo', count($stockBajo), 'materiales.php', 'bi-exclamation-triangle-fill', 'rojo'],
    ];
    foreach ($tarjetas as [$texto, $numero, $link, $icono, $color]): ?>
        <div class="col-6 col-lg-3">
            <a href="<?= $link ?>" class="text-decoration-none">
                <div class="card tarjeta-numero tarjeta-<?= $color ?> shadow-sm h-100"><div class="card-body d-flex align-items-center gap-3">
                    <div class="icono-numero"><i class="bi <?= $icono ?>"></i></div>
                    <div><div class="numero"><?= (int)$numero ?></div><div class="text-muted small"><?= $texto ?></div></div>
                </div></div>
            </a>
        </div>
    <?php endforeach; ?>
</div>

<div class="row g-4">
    <div class="col-lg-6">
        <div class="card shadow-sm h-100">
            <div class="card-header fw-semibold"><i class="bi bi-calendar-day text-marca"></i> Agenda de hoy</div>
            <ul class="list-group list-group-flush">
                <?php foreach ($agendaHoy as $t): ?>
                    <li class="list-group-item">
                        <strong><?= date('H:i', strtotime($t['fecha_agendada'])) ?></strong> · <?= h($t['servicio']) ?><br>
                        <small class="text-muted"><?= h($t['cliente']) ?> — <?= h($t['direccion']) ?></small>
                    </li>
                <?php endforeach; ?>
                <?php if (!$agendaHoy): ?><li class="list-group-item text-muted">No hay trabajos agendados para hoy.</li><?php endif; ?>
            </ul>
        </div>
    </div>
    <div class="col-lg-6">
        <div class="card shadow-sm h-100">
            <div class="card-header fw-semibold"><i class="bi bi-bell-fill text-marca"></i> Alertas</div>
            <ul class="list-group list-group-flush">
                <?php foreach ($stockBajo as $m): ?>
                    <li class="list-group-item list-group-item-danger">Stock bajo: <?= h($m['nombre']) ?> — quedan <?= (float)$m['stock_actual'] ?> <?= h($m['unidad']) ?></li>
                <?php endforeach; ?>
                <?php foreach ($garantias as $g): ?>
                    <li class="list-group-item list-group-item-warning">Garantía por vencer: <?= h($g['nombre']) ?> (<?= h($g['servicio']) ?>) el <?= date('d/m/Y', strtotime($g['garantia_hasta'])) ?></li>
                <?php endforeach; ?>
                <?php foreach ($mantenimientos as $m): ?>
                    <li class="list-group-item list-group-item-info">Mantenimiento anual: <?= h($m['nombre']) ?> · <?= h($m['telefono']) ?></li>
                <?php endforeach; ?>
                <?php if (!$stockBajo && !$garantias && !$mantenimientos): ?><li class="list-group-item text-muted">No hay alertas.</li><?php endif; ?>
            </ul>
        </div>
    </div>
</div>
<?php require __DIR__ . '/../includes/footer.php';
