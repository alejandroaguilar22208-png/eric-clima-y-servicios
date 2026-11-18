<?php
/**
 * Un presupuesto listo para imprimir.
 * Con el botón "Imprimir / Guardar PDF" el navegador lo guarda como PDF
 * (el CSS de @media print oculta el menú y los botones).
 */
require_once __DIR__ . '/../app/auth.php';
requerir_login();

$id = (int)($_GET['id'] ?? 0);
$st = db()->prepare(
    'SELECT p.*, c.nombre, c.direccion, c.telefono FROM presupuestos p
     JOIN clientes c ON c.id = p.cliente_id WHERE p.id = ?');
$st->execute([$id]);
$p = $st->fetch();
if (!$p) {
    flash('Ese presupuesto no existe.', 'warning');
    redirigir('presupuestos.php');
}
$st = db()->prepare('SELECT * FROM presupuesto_items WHERE presupuesto_id = ? ORDER BY id');
$st->execute([$id]);
$items = $st->fetchAll();

$numero = str_pad($p['id'], 5, '0', STR_PAD_LEFT);
$fecha  = fn($f) => date('d/m/Y', strtotime($f));
$plata  = fn($n) => '$ ' . number_format((float)$n, 2, ',', '.');
?>
<!doctype html>
<html lang="es">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Presupuesto <?= $numero ?> - <?= e($p['nombre']) ?></title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="../assets/css/estilos.css" rel="stylesheet">
    <style>
        .hoja { max-width: 800px; margin: 24px auto; background: #fff; padding: 40px; box-shadow: 0 2px 12px rgba(0,0,0,.08); }
        .marca { color: #005CAF; }
        .tabla-items thead th { background: #005CAF; color: #fff; }
        @media print {
            .no-imprimir { display: none !important; }
            body { background: #fff !important; }
            .hoja { box-shadow: none; margin: 0; padding: 0; max-width: none; }
            .tabla-items thead th { -webkit-print-color-adjust: exact; print-color-adjust: exact; }
        }
    </style>
</head>
<body class="bg-light">
<div class="container no-imprimir pt-3">
    <?= mostrar_flash() ?>
    <a class="btn btn-outline-secondary btn-sm" href="presupuestos.php">← Volver</a>
    <button class="btn btn-marca btn-sm" onclick="window.print()">Imprimir / Guardar PDF</button>
    <span class="text-muted small ms-2">En la ventana de impresión elegí <b>"Guardar como PDF"</b>.</span>
</div>

<div class="hoja">
    <div class="d-flex justify-content-between">
        <div>
            <div class="fs-3 fw-bold marca"><?= e(EMPRESA) ?></div>
            <div class="text-muted">Instalación, reparación y mantenimiento de aires acondicionados</div>
            <div class="text-muted"><?= e(ZONA) ?> · Tel. <?= e(TELEFONO) ?> · <?= e(EMAIL) ?></div>
        </div>
        <div class="text-end">
            <div class="fs-4 fw-bold">PRESUPUESTO</div>
            <div>N.º <?= $numero ?></div>
            <div>Fecha: <?= $fecha($p['fecha']) ?></div>
            <div class="fw-bold">Válido hasta: <?= $fecha($p['vence']) ?></div>
        </div>
    </div>

    <div class="bg-body-tertiary p-3 my-4">
        <div class="fw-bold marca">Cliente</div>
        <div><?= e($p['nombre']) ?></div>
        <div>Dirección: <?= e($p['direccion']) ?></div>
        <div>Teléfono: <?= e($p['telefono']) ?></div>
    </div>

    <table class="table tabla-items">
        <thead><tr><th>Descripción</th><th class="text-end">Cant.</th><th class="text-end">Precio unit.</th><th class="text-end">Subtotal</th></tr></thead>
        <tbody>
        <?php foreach ($items as $i): ?>
            <tr>
                <td><?= e($i['descripcion']) ?></td>
                <td class="text-end"><?= rtrim(rtrim(number_format((float)$i['cantidad'], 2, ',', '.'), '0'), ',') ?></td>
                <td class="text-end"><?= $plata($i['precio_unitario']) ?></td>
                <td class="text-end"><?= $plata($i['cantidad'] * $i['precio_unitario']) ?></td>
            </tr>
        <?php endforeach; ?>
        </tbody>
    </table>

    <div class="text-end fs-4 fw-bold marca">TOTAL: <?= $plata($p['total']) ?></div>

    <div class="mt-4 small">
        <div class="fw-bold">Condiciones</div>
        <div>• Este presupuesto es válido hasta el <?= $fecha($p['vence']) ?>.</div>
        <div>• Los trabajos realizados tienen 12 meses de garantía.</div>
        <div>• Los precios pueden variar si cambian las condiciones de la instalación.</div>
    </div>
</div>
</body>
</html>
