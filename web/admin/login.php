<?php
require_once __DIR__ . '/../app/auth.php';

if (usuario_actual()) redirigir('index.php');

$error = '';
if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    csrf_verificar();
    if (iniciar_sesion($_POST['email'] ?? '', $_POST['clave'] ?? '')) redirigir('index.php');
    $error = 'Usuario o contraseña incorrectos.';
}
?>
<!doctype html>
<html lang="es">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Ingresar · <?= e(EMPRESA) ?></title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700;800&display=swap" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet">
    <link href="../assets/css/estilos.css" rel="stylesheet">
</head>
<body class="sitio fondo-login d-flex align-items-center min-vh-100">
<div class="container" style="max-width: 420px">
    <div class="tarjeta-login">
        <div class="text-center mb-4">
            <span class="logo-icono grande"><i class="bi bi-snow2"></i></span>
            <h1 class="h4 fw-bold mt-3 mb-0"><?= e(EMPRESA) ?></h1>
            <small class="text-muted">Panel de gestión</small>
        </div>
        <?php if ($error): ?><div class="alert alert-danger"><i class="bi bi-x-circle-fill"></i> <?= e($error) ?></div><?php endif; ?>
        <form method="post" class="tarjeta-formulario p-0 border-0 shadow-none">
            <?= csrf_campo() ?>
            <div class="mb-3">
                <label class="form-label" for="email"><i class="bi bi-envelope"></i> Email</label>
                <input class="form-control" type="email" id="email" name="email" required autofocus value="<?= e($_POST['email'] ?? '') ?>">
            </div>
            <div class="mb-4">
                <label class="form-label" for="clave"><i class="bi bi-lock"></i> Contraseña</label>
                <input class="form-control" type="password" id="clave" name="clave" required>
            </div>
            <button class="btn btn-acento btn-lg w-100 rounded-pill" type="submit">Ingresar</button>
        </form>
    </div>
    <p class="text-center mt-3"><a class="text-white" href="../public/index.php"><i class="bi bi-arrow-left"></i> Volver al sitio</a></p>
</div>
</body>
</html>
