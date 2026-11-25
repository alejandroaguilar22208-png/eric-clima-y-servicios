<?php
require_once __DIR__ . '/../includes/funciones.php';
requerirAdmin();

$tipos = $pdo->query('SELECT id, nombre FROM tipo_usuarios ORDER BY id')->fetchAll();
$yo = usuarioActual();

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    verificarCsrf();
    $accion = $_POST['accion'] ?? '';
    $id = (int)($_POST['id'] ?? 0);

    if ($accion === 'activo') {
        if ($id === (int)$yo['id']) {
            flash('No podés desactivar tu propio usuario.', 'warning');
        } else {
            $pdo->prepare('UPDATE usuarios SET activo = 1 - activo WHERE id = ?')->execute([$id]);
            flash('Usuario actualizado.');
        }
        redirigir('usuarios.php');
    }

    $nombre = trim($_POST['nombre'] ?? '');
    $email  = trim($_POST['email'] ?? '');
    $tipo   = (int)($_POST['tipo_usuario_id'] ?? 0);
    $clave  = $_POST['clave'] ?? '';
    $repite = $_POST['clave_repetir'] ?? '';

    $error = '';
    if (mb_strlen($nombre) < 3) $error = 'Escribí el nombre del usuario.';
    elseif (!filter_var($email, FILTER_VALIDATE_EMAIL)) $error = 'El email no es válido.';
    elseif (!in_array($tipo, array_column($tipos, 'id'))) $error = 'Elegí el tipo de usuario.';
    elseif (!$id && mb_strlen($clave) < 8) $error = 'La contraseña tiene que tener al menos 8 caracteres.';
    elseif ($clave !== '' && mb_strlen($clave) < 8) $error = 'La contraseña tiene que tener al menos 8 caracteres.';
    elseif ($clave !== $repite) $error = 'Las contraseñas no coinciden.';
    elseif ($id === (int)$yo['id'] && $tipo !== 1) $error = 'No podés quitarte el permiso de administrador.';

    if ($error) {
        flash($error, 'warning');
        redirigir('usuarios.php' . ($id ? '?editar=' . $id : '?nuevo=1'));
    }

    try {
        if ($id) {
            $pdo->prepare('UPDATE usuarios SET nombre = ?, email = ?, tipo_usuario_id = ? WHERE id = ?')
                ->execute([$nombre, $email, $tipo, $id]);
            if ($clave !== '') {
                $pdo->prepare('UPDATE usuarios SET password_hash = ? WHERE id = ?')
                    ->execute([password_hash($clave, PASSWORD_DEFAULT), $id]);
            }
            flash('Usuario modificado.');
        } else {
            $pdo->prepare('INSERT INTO usuarios (nombre, email, password_hash, tipo_usuario_id) VALUES (?, ?, ?, ?)')
                ->execute([$nombre, $email, password_hash($clave, PASSWORD_DEFAULT), $tipo]);
            flash('Usuario creado. Ya puede entrar al panel y al programa de escritorio.');
        }
    } catch (PDOException $e) {
        flash(($e->errorInfo[1] ?? 0) == 1062 ? 'Ese email ya lo usa otro usuario.' : mensajeError($e), 'danger');
    }
    redirigir('usuarios.php');
}

$usuarios = $pdo->query('SELECT u.*, t.nombre AS tipo FROM usuarios u JOIN tipo_usuarios t ON t.id = u.tipo_usuario_id
                         ORDER BY u.activo DESC, u.nombre')->fetchAll();
$editar = (int)($_GET['editar'] ?? 0);
$actual = ['id' => 0, 'nombre' => '', 'email' => '', 'tipo_usuario_id' => 2];
foreach ($usuarios as $u) if ((int)$u['id'] === $editar) $actual = $u;
$mostrarForm = $editar || isset($_GET['nuevo']);

$titulo = 'Usuarios';
$activo = 'usuarios';
require __DIR__ . '/../includes/header.php';
?>
<?php if ($mostrarForm): ?>
    <form method="post" class="card card-body shadow-sm mb-4" autocomplete="off">
        <h2 class="h5"><?= $actual['id'] ? 'Editar usuario' : 'Nuevo usuario' ?></h2>
        <?= campoCsrf() ?><input type="hidden" name="id" value="<?= (int)$actual['id'] ?>">
        <div class="row g-3">
            <div class="col-md-4"><label class="form-label" for="nombre">Nombre *</label><input class="form-control" id="nombre" name="nombre" maxlength="100" required value="<?= h($actual['nombre']) ?>"></div>
            <div class="col-md-4"><label class="form-label" for="email">Email *</label><input class="form-control" id="email" name="email" type="email" maxlength="120" required value="<?= h($actual['email']) ?>"></div>
            <div class="col-md-4">
                <label class="form-label" for="tipo">Tipo de usuario *</label>
                <select class="form-select" id="tipo" name="tipo_usuario_id">
                    <?php foreach ($tipos as $t): ?>
                        <option value="<?= $t['id'] ?>" <?= (int)$actual['tipo_usuario_id'] === (int)$t['id'] ? 'selected' : '' ?>><?= h($t['nombre']) ?></option>
                    <?php endforeach; ?>
                </select>
            </div>
            <div class="col-md-4"><label class="form-label" for="clave">Contraseña <?= $actual['id'] ? '' : '*' ?></label><input class="form-control" id="clave" name="clave" type="password" autocomplete="new-password"></div>
            <div class="col-md-4"><label class="form-label" for="clave_repetir">Repetir contraseña</label><input class="form-control" id="clave_repetir" name="clave_repetir" type="password" autocomplete="new-password"></div>
            <?php if ($actual['id']): ?><div class="col-md-4 form-text align-self-end">Dejala vacía para no cambiarla.</div><?php endif; ?>
        </div>
        <div class="mt-3">
            <button class="btn btn-marca"><i class="bi bi-check-lg"></i> Guardar</button>
            <a class="btn btn-link" href="usuarios.php">Cancelar</a>
        </div>
    </form>
<?php else: ?>
    <a class="btn btn-marca mb-3" href="usuarios.php?nuevo=1"><i class="bi bi-person-plus"></i> Nuevo usuario</a>
<?php endif; ?>

<div class="card shadow-sm">
    <div class="table-responsive">
        <table class="table align-middle mb-0">
            <thead class="table-light"><tr><th>Nombre</th><th>Email</th><th>Tipo</th><th>Estado</th><th></th></tr></thead>
            <tbody>
            <?php foreach ($usuarios as $u): ?>
                <tr class="<?= $u['activo'] ? '' : 'text-muted' ?>">
                    <td><?= h($u['nombre']) ?></td>
                    <td><?= h($u['email']) ?></td>
                    <td><?= h($u['tipo']) ?></td>
                    <td><?= $u['activo'] ? '<span class="badge text-bg-success">Activo</span>' : '<span class="badge text-bg-secondary">Inactivo</span>' ?></td>
                    <td class="text-end text-nowrap">
                        <a class="btn btn-sm btn-outline-primary" href="usuarios.php?editar=<?= $u['id'] ?>"><i class="bi bi-pencil"></i> Editar</a>
                        <?php if ((int)$u['id'] !== (int)$yo['id']): ?>
                            <form method="post" class="d-inline">
                                <?= campoCsrf() ?><input type="hidden" name="accion" value="activo"><input type="hidden" name="id" value="<?= $u['id'] ?>">
                                <button class="btn btn-sm btn-outline-secondary"><?= $u['activo'] ? 'Desactivar' : 'Activar' ?></button>
                            </form>
                        <?php endif; ?>
                    </td>
                </tr>
            <?php endforeach; ?>
            </tbody>
        </table>
    </div>
</div>
<?php require __DIR__ . '/../includes/footer.php';
