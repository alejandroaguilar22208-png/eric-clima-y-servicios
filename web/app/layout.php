<?php
/**
 * Plantilla del panel de gestión: encabezado con menú y pie.
 * Uso: layout_inicio('Clientes', 'clientes'); ... layout_fin();
 */

function layout_inicio(string $titulo, string $activo = ''): void
{
    $u = usuario_actual();
    $menu = [
        'index'       => 'Inicio',
        'solicitudes' => 'Solicitudes',
        'clientes'    => 'Clientes',
        'trabajos'    => 'Trabajos',
        'materiales'  => 'Stock',
        'presupuestos' => 'Presupuestos',
    ];
    if (es_admin()) $menu['reportes'] = 'Reportes';
    ?>
<!doctype html>
<html lang="es">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title><?= e($titulo) ?> · Panel <?= e(EMPRESA) ?></title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="../assets/css/estilos.css" rel="stylesheet">
</head>
<body class="bg-light">
<nav class="navbar navbar-expand-lg navbar-dark bg-marca">
    <div class="container-fluid">
        <a class="navbar-brand fw-bold" href="index.php"><?= e(EMPRESA) ?></a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#menu">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="menu">
            <ul class="navbar-nav me-auto">
                <?php foreach ($menu as $pagina => $texto): ?>
                    <li class="nav-item">
                        <a class="nav-link <?= $activo === $pagina ? 'active fw-semibold' : '' ?>" href="<?= $pagina ?>.php"><?= $texto ?></a>
                    </li>
                <?php endforeach; ?>
            </ul>
            <span class="navbar-text me-3"><?= e($u['nombre']) ?> (<?= e($u['rol']) ?>)</span>
            <a class="btn btn-sm btn-outline-light me-2" href="../public/index.php" target="_blank">Ver sitio</a>
            <a class="btn btn-sm btn-light" href="logout.php">Salir</a>
        </div>
    </div>
</nav>
<main class="container py-4">
    <h1 class="h3 mb-4"><?= e($titulo) ?></h1>
    <?= mostrar_flash() ?>
<?php
}

function layout_fin(): void
{
    ?>
</main>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
<?php
}

/** Etiqueta de color para cada estado de trabajo. */
function badge_estado(string $estado): string
{
    $colores = ['pendiente' => 'warning', 'agendado' => 'primary', 'realizado' => 'success', 'cancelado' => 'secondary',
                'contactado' => 'info', 'convertida' => 'success', 'descartada' => 'secondary',
                'aceptado' => 'success', 'rechazado' => 'danger', 'vencido' => 'secondary'];
    return '<span class="badge text-bg-' . ($colores[$estado] ?? 'light') . '">' . e(ucfirst($estado)) . '</span>';
}
