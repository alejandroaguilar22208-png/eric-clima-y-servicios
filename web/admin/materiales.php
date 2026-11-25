<?php
require_once __DIR__ . '/../includes/funciones.php';
requerirLogin();


if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    verificarCsrf();
    if (!esAdmin()) {
        flash('Solo el administrador puede modificar el stock.', 'warning');
        redirigir('materiales.php');
    }
    $m = [
        'id'     => (int)($_POST['id'] ?? 0),
        'nombre' => trim($_POST['nombre'] ?? ''),
        'unidad' => in_array($_POST['unidad'] ?? '', ['unidad', 'm', 'kg'], true) ? $_POST['unidad'] : 'unidad',
        'actual' => (float)($_POST['stock_actual'] ?? 0),
        'minimo' => (float)($_POST['stock_minimo'] ?? 0),
        'precio' => (float)($_POST['precio'] ?? 0),
    ];
    if ($m['nombre'] === '' || $m['actual'] < 0 || $m['minimo'] < 0 || $m['precio'] < 0) {
        flash('Revisá los datos: nombre obligatorio y sin valores negativos.', 'warning');
    } else {
        try {
            $params = [$m['nombre'], $m['unidad'], $m['actual'], $m['minimo'], $m['precio']];
            if ($m['id']) {
                $pdo->prepare('UPDATE materiales SET nombre=?, unidad=?, stock_actual=?, stock_minimo=?, precio=? WHERE id=?')
                   ->execute([...$params, $m['id']]);
                flash('Material actualizado.');
            } else {
                $pdo->prepare('INSERT INTO materiales (nombre, unidad, stock_actual, stock_minimo, precio) VALUES (?, ?, ?, ?, ?)')
                   ->execute($params);
                flash('Material agregado.');
            }
        } catch (PDOException $ex) {
            flash(mensajeError($ex), 'danger');
        }
    }
    redirigir('materiales.php');
}

$materiales = $pdo->query('SELECT * FROM materiales ORDER BY stock_actual <= stock_minimo DESC, nombre')->fetchAll();
$editar = (int)($_GET['editar'] ?? 0);

$titulo = 'Stock de materiales';
$activo = 'materiales';
require __DIR__ . '/../includes/header.php';
?>
<p class="text-muted">Las filas en rojo tienen stock igual o menor al mínimo. El stock baja solo cuando un trabajo se marca como realizado.</p>

<div class="card shadow-sm">
    <div class="table-responsive">
        <table class="table align-middle mb-0">
            <thead class="table-light"><tr><th>Material</th><th>Unidad</th><th>Stock</th><th>Mínimo</th><th>Precio</th><?= esAdmin() ? '<th></th>' : '' ?></tr></thead>
            <tbody>
            <?php foreach ($materiales as $m): ?>
                <?php if (esAdmin() && $editar === (int)$m['id']): ?>
                    <tr><td colspan="6">
                        <form method="post" class="row g-2 align-items-center">
                            <?= campoCsrf() ?><input type="hidden" name="id" value="<?= $m['id'] ?>">
                            <div class="col-md-3"><input class="form-control form-control-sm" name="nombre" value="<?= h($m['nombre']) ?>"></div>
                            <div class="col-md-2"><select class="form-select form-select-sm" name="unidad">
                                <?php foreach (['unidad', 'm', 'kg'] as $u): ?><option <?= $m['unidad'] === $u ? 'selected' : '' ?>><?= $u ?></option><?php endforeach; ?>
                            </select></div>
                            <div class="col-md-2"><input class="form-control form-control-sm" type="number" step="0.01" min="0" name="stock_actual" value="<?= (float)$m['stock_actual'] ?>"></div>
                            <div class="col-md-2"><input class="form-control form-control-sm" type="number" step="0.01" min="0" name="stock_minimo" value="<?= (float)$m['stock_minimo'] ?>"></div>
                            <div class="col-md-2"><input class="form-control form-control-sm" type="number" step="1" min="0" name="precio" value="<?= (float)$m['precio'] ?>"></div>
                            <div class="col-md-1 text-nowrap"><button class="btn btn-sm btn-marca">Guardar</button></div>
                        </form>
                    </td></tr>
                <?php else: ?>
                    <tr class="<?= $m['stock_actual'] <= $m['stock_minimo'] ? 'fila-stock-bajo' : '' ?>">
                        <td><?= h($m['nombre']) ?></td>
                        <td><?= h($m['unidad']) ?></td>
                        <td><?= (float)$m['stock_actual'] ?></td>
                        <td><?= (float)$m['stock_minimo'] ?></td>
                        <td><?= pesos($m['precio']) ?></td>
                        <?php if (esAdmin()): ?><td class="text-end"><a class="btn btn-sm btn-outline-primary" href="materiales.php?editar=<?= $m['id'] ?>">Editar</a></td><?php endif; ?>
                    </tr>
                <?php endif; ?>
            <?php endforeach; ?>
            </tbody>
        </table>
    </div>
</div>

<?php if (esAdmin()): ?>
    <form method="post" class="card card-body shadow-sm mt-4">
        <h2 class="h6">Agregar material</h2>
        <?= campoCsrf() ?>
        <div class="row g-2">
            <div class="col-md-3"><input class="form-control" name="nombre" placeholder="Nombre *" required></div>
            <div class="col-md-2"><select class="form-select" name="unidad"><option>unidad</option><option>m</option><option>kg</option></select></div>
            <div class="col-md-2"><input class="form-control" type="number" step="0.01" min="0" name="stock_actual" placeholder="Stock"></div>
            <div class="col-md-2"><input class="form-control" type="number" step="0.01" min="0" name="stock_minimo" placeholder="Mínimo"></div>
            <div class="col-md-2"><input class="form-control" type="number" step="1" min="0" name="precio" placeholder="Precio"></div>
            <div class="col-md-1"><button class="btn btn-marca w-100">+</button></div>
        </div>
    </form>
<?php endif; ?>
<?php require __DIR__ . '/../includes/footer.php';
