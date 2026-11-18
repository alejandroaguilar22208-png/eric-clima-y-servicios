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
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="../assets/css/estilos.css" rel="stylesheet">
</head>
<body class="bg-light d-flex align-items-center min-vh-100">
<div class="container" style="max-width: 400px">
    <div class="card shadow-sm">
        <div class="card-header bg-marca text-white text-center py-3">
            <h1 class="h5 mb-0"><?= e(EMPRESA) ?></h1>
            <small>Panel de gestión</small>
        </div>
        <div class="card-body p-4">
            <?php if ($error): ?><div class="alert alert-danger"><?= e($error) ?></div><?php endif; ?>
            <form method="post">
                <?= csrf_campo() ?>
                <div class="mb-3">
                    <label class="form-label" for="email">Email</label>
                    <input class="form-control" type="email" id="email" name="email" required autofocus value="<?= e($_POST['email'] ?? '') ?>">
                </div>
                <div class="mb-3">
                    <label class="form-label" for="clave">Contraseña</label>
                    <input class="form-control" type="password" id="clave" name="clave" required>
                </div>
                <button class="btn btn-marca w-100" type="submit">Ingresar</button>
            </form>
        </div>
    </div>
    <p class="text-center mt-3"><a href="../public/index.php">← Volver al sitio</a></p>
</div>
</body>
</html>
