<?php

function layout_inicio(string $titulo, string $activo = ''): void
{
    $u = usuario_actual();
    $menu = [
        'index'        => ['Inicio', 'bi-house-door-fill'],
        'solicitudes'  => ['Solicitudes', 'bi-inbox-fill'],
        'clientes'     => ['Clientes', 'bi-people-fill'],
        'trabajos'     => ['Trabajos', 'bi-tools'],
        'materiales'   => ['Stock', 'bi-box-seam-fill'],
        'presupuestos' => ['Presupuestos', 'bi-file-earmark-text-fill'],
        'equipos'      => ['Equipos en venta', 'bi-snow'],
    ];
    if (es_admin()) $menu['reportes'] = ['Reportes', 'bi-bar-chart-fill'];
    ?>
<!doctype html>
<html lang="es">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title><?= e($titulo) ?> · Panel <?= e(EMPRESA) ?></title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700&display=swap" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet">
    <link href="../assets/css/estilos.css" rel="stylesheet">
</head>
<body class="panel">
<nav class="navbar navbar-expand-xl navbar-dark panel-menu">
    <div class="container-fluid">
        <a class="navbar-brand marca-logo claro" href="index.php"><span class="logo-icono"><i class="bi bi-snow2"></i></span><span><b>Eric Clima</b><small>Panel de gestión</small></span></a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#menu">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="menu">
            <ul class="navbar-nav me-auto">
                <?php foreach ($menu as $pagina => [$texto, $icono]): ?>
                    <li class="nav-item">
                        <a class="nav-link <?= $activo === $pagina ? 'active' : '' ?>" href="<?= $pagina ?>.php"><i class="bi <?= $icono ?>"></i><?= $texto ?></a>
                    </li>
                <?php endforeach; ?>
            </ul>
            <span class="navbar-text me-3 text-white"><i class="bi bi-person-circle"></i> <?= e($u['nombre']) ?> <small class="opacity-75">(<?= e($u['rol']) ?>)</small></span>
            <a class="btn btn-sm btn-outline-light me-2" href="../public/index.php" target="_blank"><i class="bi bi-globe2"></i> Ver sitio</a>
            <a class="btn btn-sm btn-acento" href="logout.php"><i class="bi bi-box-arrow-right"></i> Salir</a>
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

function badge_estado(string $estado): string
{
    $colores = ['pendiente' => 'warning', 'agendado' => 'primary', 'realizado' => 'success', 'cancelado' => 'secondary',
                'contactado' => 'info', 'convertida' => 'success', 'descartada' => 'secondary',
                'aceptado' => 'success', 'rechazado' => 'danger', 'vencido' => 'secondary'];
    return '<span class="badge text-bg-' . ($colores[$estado] ?? 'light') . '">' . e(ucfirst($estado)) . '</span>';
}
