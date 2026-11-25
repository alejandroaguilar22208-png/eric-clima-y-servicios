<?php
require_once __DIR__ . '/../includes/funciones.php';
requerirAdmin();

$mensual = $pdo->query(
    "SELECT DATE_FORMAT(fecha_realizado, '%Y-%m') AS mes, COUNT(*) AS trabajos, SUM(total) AS ingresos
     FROM trabajos WHERE estado = 'realizado'
     GROUP BY DATE_FORMAT(fecha_realizado, '%Y-%m') ORDER BY mes DESC LIMIT 12"
)->fetchAll();
$porServicio = $pdo->query(
    "SELECT s.nombre, COUNT(t.id) AS cantidad, COALESCE(SUM(t.total), 0) AS ingresos
     FROM servicios s LEFT JOIN trabajos t ON t.servicio_id = s.id AND t.estado = 'realizado'
     GROUP BY s.id, s.nombre ORDER BY ingresos DESC"
)->fetchAll();
$porEstado = $pdo->query('SELECT estado, COUNT(*) AS cantidad FROM trabajos GROUP BY estado')->fetchAll(PDO::FETCH_KEY_PAIR);

$maxServicio = max(array_map('floatval', array_column($porServicio, 'ingresos')) ?: [0]) ?: 1;

$titulo = 'Reportes';
$activo = 'reportes';
require __DIR__ . '/../includes/header.php';
?>
<div class="row g-3 mb-4">
    <?php foreach (['pendiente', 'agendado', 'realizado', 'cancelado'] as $es): ?>
        <div class="col-6 col-lg-3"><div class="card tarjeta-numero shadow-sm"><div class="card-body">
            <div class="numero"><?= (int)($porEstado[$es] ?? 0) ?></div><div class="text-muted">Trabajos <?= $es ?>s</div>
        </div></div></div>
    <?php endforeach; ?>
</div>

<div class="row g-4">
    <div class="col-lg-6">
        <div class="card shadow-sm h-100">
            <div class="card-header fw-semibold">Ingresos por mes (trabajos realizados)</div>
            <table class="table mb-0">
                <thead class="table-light"><tr><th>Mes</th><th>Trabajos</th><th class="text-end">Ingresos</th></tr></thead>
                <tbody>
                <?php foreach ($mensual as $m): ?>
                    <tr><td><?= h($m['mes']) ?></td><td><?= (int)$m['trabajos'] ?></td><td class="text-end"><?= pesos($m['ingresos']) ?></td></tr>
                <?php endforeach; ?>
                <?php if (!$mensual): ?><tr><td colspan="3" class="text-muted">Todavía no hay trabajos realizados.</td></tr><?php endif; ?>
                </tbody>
            </table>
        </div>
    </div>
    <div class="col-lg-6">
        <div class="card shadow-sm h-100">
            <div class="card-header fw-semibold">Ingresos por servicio</div>
            <div class="card-body">
                <?php foreach ($porServicio as $s): ?>
                    <div class="mb-3">
                        <div class="d-flex justify-content-between small">
                            <span><?= h($s['nombre']) ?> (<?= (int)$s['cantidad'] ?>)</span><span><?= pesos($s['ingresos']) ?></span>
                        </div>
                        <div class="barra" style="width: <?= round($s['ingresos'] / $maxServicio * 100) ?>%"></div>
                    </div>
                <?php endforeach; ?>
            </div>
        </div>
    </div>
</div>
<?php require __DIR__ . '/../includes/footer.php';
