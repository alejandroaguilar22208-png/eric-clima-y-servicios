<?php

require_once __DIR__ . '/../app/auth.php';
require_once __DIR__ . '/../app/layout.php';
requerir_login();

$db = db();
$errores = [];

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    csrf_verificar();
    $accion = $_POST['accion'] ?? '';

    if ($accion === 'guardar') {
        $c = [
            'id'        => (int)($_POST['id'] ?? 0),
            'nombre'    => trim($_POST['nombre'] ?? ''),
            'telefono'  => trim($_POST['telefono'] ?? ''),
            'email'     => trim($_POST['email'] ?? ''),
            'direccion' => trim($_POST['direccion'] ?? ''),
            'localidad' => trim($_POST['localidad'] ?? '') ?: 'Bahía Blanca',
        ];
        if ($c['nombre'] === '')    $errores['nombre'] = 'El nombre es obligatorio.';
        if ($c['telefono'] === '')  $errores['telefono'] = 'El teléfono es obligatorio.';
        if ($c['direccion'] === '') $errores['direccion'] = 'La dirección es obligatoria.';
        if ($c['email'] !== '' && !filter_var($c['email'], FILTER_VALIDATE_EMAIL)) $errores['email'] = 'El email no es válido.';

        if (!$errores) {
            $params = [$c['nombre'], $c['telefono'], $c['email'] ?: null, $c['direccion'], $c['localidad']];
            if ($c['id']) {
                $db->prepare('UPDATE clientes SET nombre=?, telefono=?, email=?, direccion=?, localidad=? WHERE id=?')
                   ->execute([...$params, $c['id']]);
                flash('Cliente modificado.');
            } else {
                $db->prepare('INSERT INTO clientes (nombre, telefono, email, direccion, localidad) VALUES (?, ?, ?, ?, ?)')
                   ->execute($params);
                flash('Cliente agregado.');
            }
            redirigir('clientes.php');
        }
        $editando = $c;
    } elseif ($accion === 'baja') {
        if (!es_admin()) {
            flash('Solo el administrador puede dar de baja clientes.', 'warning');
        } else {
            $db->prepare('UPDATE clientes SET activo = 0 WHERE id = ?')->execute([(int)$_POST['id']]);
            flash('Cliente dado de baja. Su historial se conserva.');
        }
        redirigir('clientes.php');
    } elseif ($accion === 'equipo') {
        $clienteId = (int)$_POST['cliente_id'];
        $marca = trim($_POST['marca'] ?? '');
        $frigorias = (int)($_POST['frigorias'] ?? 0);
        if ($marca === '' || $frigorias <= 0) {
            flash('Completá marca y frigorías del equipo.', 'warning');
        } else {
            $db->prepare('INSERT INTO equipos (cliente_id, marca, modelo, frigorias, tipo, ubicacion) VALUES (?, ?, ?, ?, ?, ?)')
               ->execute([$clienteId, $marca, trim($_POST['modelo'] ?? '') ?: null, $frigorias, $_POST['tipo'] ?? 'split', trim($_POST['ubicacion'] ?? '') ?: null]);
            flash('Equipo agregado.');
        }
        redirigir('clientes.php?ver=' . $clienteId);
    }
}

if (!isset($editando)) {
    if (isset($_GET['nuevo'])) {
        $editando = ['id' => 0, 'nombre' => '', 'telefono' => '', 'email' => '', 'direccion' => '', 'localidad' => 'Bahía Blanca'];
    } elseif (isset($_GET['editar'])) {
        $st = $db->prepare('SELECT * FROM clientes WHERE id = ? AND activo = 1');
        $st->execute([(int)$_GET['editar']]);
        $editando = $st->fetch() ?: null;
    }
}

$ficha = null;
if (isset($_GET['ver'])) {
    $st = $db->prepare('SELECT * FROM clientes WHERE id = ?');
    $st->execute([(int)$_GET['ver']]);
    $ficha = $st->fetch();
    if ($ficha) {
        $st = $db->prepare('SELECT * FROM equipos WHERE cliente_id = ? ORDER BY id');
        $st->execute([$ficha['id']]);
        $equipos = $st->fetchAll();
        $st = $db->prepare(
            'SELECT t.*, s.nombre AS servicio FROM trabajos t JOIN servicios s ON s.id = t.servicio_id
             WHERE t.cliente_id = ? ORDER BY t.id DESC'
        );
        $st->execute([$ficha['id']]);
        $historial = $st->fetchAll();
    }
}

$q = trim($_GET['q'] ?? '');
$st = $db->prepare(
    'SELECT c.*, (SELECT COUNT(*) FROM equipos e WHERE e.cliente_id = c.id) AS equipos
     FROM clientes c WHERE c.activo = 1 AND (c.nombre LIKE ? OR c.telefono LIKE ? OR c.direccion LIKE ? OR c.localidad LIKE ?)
     ORDER BY c.nombre'
);
$st->execute(array_fill(0, 4, "%$q%"));
$clientes = $st->fetchAll();

function campo(string $nombre, string $etiqueta, array $datos, array $errores, string $tipo = 'text'): string
{
    $inv = isset($errores[$nombre]) ? ' is-invalid' : '';
    return '<div class="col-md-6"><label class="form-label">' . $etiqueta . '</label>'
         . '<input class="form-control' . $inv . '" type="' . $tipo . '" name="' . $nombre . '" value="' . e($datos[$nombre] ?? '') . '">'
         . ($inv ? '<div class="invalid-feedback">' . e($errores[$nombre]) . '</div>' : '') . '</div>';
}

layout_inicio('Clientes', 'clientes');
?>

<?php if (!empty($editando)): ?>
    <div class="card shadow-sm mb-4">
        <div class="card-header fw-semibold"><?= $editando['id'] ? 'Editar cliente' : 'Nuevo cliente' ?></div>
        <form method="post" class="card-body" novalidate>
            <?= csrf_campo() ?>
            <input type="hidden" name="accion" value="guardar">
            <input type="hidden" name="id" value="<?= (int)$editando['id'] ?>">
            <div class="row g-3">
                <?= campo('nombre', 'Nombre *', $editando, $errores) ?>
                <?= campo('telefono', 'Teléfono *', $editando, $errores) ?>
                <?= campo('direccion', 'Dirección *', $editando, $errores) ?>
                <?= campo('localidad', 'Localidad', $editando, $errores) ?>
                <?= campo('email', 'Email', $editando, $errores, 'email') ?>
            </div>
            <div class="mt-3">
                <button class="btn btn-marca">Guardar</button>
                <a href="clientes.php" class="btn btn-outline-secondary">Cancelar</a>
            </div>
        </form>
    </div>
<?php endif; ?>

<?php if ($ficha): ?>
    <div class="card shadow-sm mb-4">
        <div class="card-header d-flex justify-content-between align-items-center">
            <span class="fw-semibold"><?= e($ficha['nombre']) ?></span>
            <a href="clientes.php" class="btn-close" aria-label="Cerrar"></a>
        </div>
        <div class="card-body">
            <p class="mb-3"><?= e($ficha['telefono']) ?> · <?= e($ficha['direccion']) ?>, <?= e($ficha['localidad']) ?> <?= $ficha['email'] ? '· ' . e($ficha['email']) : '' ?></p>
            <div class="row g-4">
                <div class="col-lg-6">
                    <h2 class="h6">Equipos instalados</h2>
                    <ul class="list-group mb-3">
                        <?php foreach ($equipos as $eq): ?>
                            <li class="list-group-item"><?= e($eq['marca']) ?> <?= e($eq['modelo']) ?> · <?= (int)$eq['frigorias'] ?> fg · <?= e($eq['tipo']) ?> <?= $eq['ubicacion'] ? '· ' . e($eq['ubicacion']) : '' ?></li>
                        <?php endforeach; ?>
                        <?php if (!$equipos): ?><li class="list-group-item text-muted">Sin equipos cargados.</li><?php endif; ?>
                    </ul>
                    <form method="post" class="row g-2">
                        <?= csrf_campo() ?>
                        <input type="hidden" name="accion" value="equipo">
                        <input type="hidden" name="cliente_id" value="<?= $ficha['id'] ?>">
                        <div class="col-6"><input class="form-control form-control-sm" name="marca" placeholder="Marca *"></div>
                        <div class="col-6"><input class="form-control form-control-sm" name="modelo" placeholder="Modelo"></div>
                        <div class="col-4"><input class="form-control form-control-sm" name="frigorias" type="number" min="1" placeholder="Frigorías *"></div>
                        <div class="col-4"><select class="form-select form-select-sm" name="tipo">
                            <option value="split">Split</option><option value="ventana">Ventana</option><option value="portatil">Portátil</option>
                            <option value="piso_techo">Piso techo</option><option value="central">Central</option></select></div>
                        <div class="col-4"><input class="form-control form-control-sm" name="ubicacion" placeholder="Ubicación"></div>
                        <div class="col-12"><button class="btn btn-sm btn-outline-primary">Agregar equipo</button></div>
                    </form>
                </div>
                <div class="col-lg-6">
                    <h2 class="h6">Historial de trabajos</h2>
                    <ul class="list-group">
                        <?php foreach ($historial as $t): ?>
                            <li class="list-group-item d-flex justify-content-between">
                                <span>N.º <?= $t['id'] ?> · <?= e($t['servicio']) ?> <?= $t['garantia_hasta'] ? '<small class="text-muted">· garantía hasta ' . date('d/m/Y', strtotime($t['garantia_hasta'])) . '</small>' : '' ?></span>
                                <?= badge_estado($t['estado']) ?>
                            </li>
                        <?php endforeach; ?>
                        <?php if (!$historial): ?><li class="list-group-item text-muted">Sin trabajos.</li><?php endif; ?>
                    </ul>
                </div>
            </div>
        </div>
    </div>
<?php endif; ?>

<div class="d-flex flex-wrap gap-2 mb-3">
    <form class="d-flex gap-2 flex-grow-1" method="get">
        <input class="form-control" name="q" value="<?= e($q) ?>" placeholder="Buscar por nombre, teléfono, dirección o localidad">
        <button class="btn btn-outline-secondary">Buscar</button>
    </form>
    <a href="clientes.php?nuevo=1" class="btn btn-marca">+ Nuevo cliente</a>
</div>

<div class="card shadow-sm">
    <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
            <thead class="table-light"><tr><th>Nombre</th><th>Teléfono</th><th>Dirección</th><th>Equipos</th><th class="text-end">Acciones</th></tr></thead>
            <tbody>
            <?php foreach ($clientes as $c): ?>
                <tr>
                    <td><a href="clientes.php?ver=<?= $c['id'] ?>"><?= e($c['nombre']) ?></a></td>
                    <td><?= e($c['telefono']) ?></td>
                    <td><?= e($c['direccion']) ?>, <?= e($c['localidad']) ?></td>
                    <td><?= (int)$c['equipos'] ?></td>
                    <td class="text-end text-nowrap">
                        <a class="btn btn-sm btn-outline-primary" href="clientes.php?editar=<?= $c['id'] ?>">Editar</a>
                        <?php if (es_admin()): ?>
                            <form method="post" class="d-inline" onsubmit="return confirm('¿Dar de baja a este cliente? Su historial se conserva.')">
                                <?= csrf_campo() ?><input type="hidden" name="accion" value="baja"><input type="hidden" name="id" value="<?= $c['id'] ?>">
                                <button class="btn btn-sm btn-outline-danger">Dar de baja</button>
                            </form>
                        <?php endif; ?>
                    </td>
                </tr>
            <?php endforeach; ?>
            <?php if (!$clientes): ?><tr><td colspan="5" class="text-center text-muted py-4">No se encontraron clientes.</td></tr><?php endif; ?>
            </tbody>
        </table>
    </div>
</div>
<?php layout_fin();
