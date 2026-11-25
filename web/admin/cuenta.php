<?php
require_once __DIR__ . '/../includes/funciones.php';
requerirLogin();

$u = usuarioActual();

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    verificarCsrf();
    $nombre = trim($_POST['nombre'] ?? '');
    $email  = trim($_POST['email'] ?? '');
    $actual = $_POST['clave_actual'] ?? '';
    $nueva  = $_POST['clave_nueva'] ?? '';
    $repite = $_POST['clave_repetir'] ?? '';

    $st = $pdo->prepare('SELECT password_hash FROM usuarios WHERE id = ?');
    $st->execute([$u['id']]);
    $hash = $st->fetchColumn();

    $error = '';
    if (mb_strlen($nombre) < 3) $error = 'Escribí tu nombre.';
    elseif (!filter_var($email, FILTER_VALIDATE_EMAIL)) $error = 'El email no es válido.';
    elseif (!$hash || !password_verify($actual, $hash)) $error = 'La contraseña actual no es correcta.';
    elseif ($nueva !== '' && mb_strlen($nueva) < 8) $error = 'La contraseña nueva tiene que tener al menos 8 caracteres.';
    elseif ($nueva !== $repite) $error = 'La contraseña nueva y la repetición no coinciden.';

    if ($error) {
        flash($error, 'warning');
        redirigir('cuenta.php');
    }

    try {
        if ($nueva !== '') {
            $pdo->prepare('UPDATE usuarios SET nombre = ?, email = ?, password_hash = ? WHERE id = ?')
               ->execute([$nombre, $email, password_hash($nueva, PASSWORD_BCRYPT), $u['id']]);
        } else {
            $pdo->prepare('UPDATE usuarios SET nombre = ?, email = ? WHERE id = ?')
               ->execute([$nombre, $email, $u['id']]);
        }
        $_SESSION['usuario']['nombre'] = $nombre;
        $_SESSION['usuario']['email'] = $email;
        flash($nueva !== '' ? 'Datos y contraseña actualizados. La próxima vez entrá con los datos nuevos.' : 'Datos actualizados.');
    } catch (PDOException $ex) {
        flash(($ex->errorInfo[1] ?? 0) == 1062 ? 'Ese email ya lo usa otro usuario.' : mensajeError($ex), 'danger');
    }
    redirigir('cuenta.php');
}

$u = usuarioActual();
$titulo = 'Mi cuenta';
$activo = 'cuenta';
require __DIR__ . '/../includes/header.php';
?>
<div class="row">
    <div class="col-lg-7">
        <form method="post" class="card card-body shadow-sm" autocomplete="off">
            <?= campoCsrf() ?>
            <h2 class="h5"><i class="bi bi-person-circle text-marca"></i> Tus datos</h2>
            <p class="text-muted small">Con este email y contraseña entrás al panel web y al programa de escritorio.</p>
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label" for="nombre">Nombre</label>
                    <input class="form-control" id="nombre" name="nombre" maxlength="100" required value="<?= h($u['nombre']) ?>">
                </div>
                <div class="col-md-6">
                    <label class="form-label" for="email">Email (usuario para entrar)</label>
                    <input class="form-control" id="email" name="email" type="email" maxlength="120" required value="<?= h($u['email']) ?>">
                </div>
                <div class="col-md-6">
                    <label class="form-label" for="clave_nueva">Contraseña nueva</label>
                    <input class="form-control" id="clave_nueva" name="clave_nueva" type="password" minlength="8" autocomplete="new-password">
                    <div class="form-text">Dejala vacía si no la querés cambiar. Mínimo 8 caracteres.</div>
                </div>
                <div class="col-md-6">
                    <label class="form-label" for="clave_repetir">Repetir contraseña nueva</label>
                    <input class="form-control" id="clave_repetir" name="clave_repetir" type="password" autocomplete="new-password">
                </div>
                <div class="col-12"><hr class="my-1"></div>
                <div class="col-md-6">
                    <label class="form-label" for="clave_actual">Contraseña actual *</label>
                    <input class="form-control" id="clave_actual" name="clave_actual" type="password" required autocomplete="current-password">
                    <div class="form-text">Para confirmar que sos vos.</div>
                </div>
            </div>
            <div class="mt-3">
                <button class="btn btn-marca"><i class="bi bi-check-lg"></i> Guardar cambios</button>
            </div>
        </form>
    </div>
</div>
<?php require __DIR__ . '/../includes/footer.php';
