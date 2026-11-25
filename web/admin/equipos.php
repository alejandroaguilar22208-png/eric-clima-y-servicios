<?php
require_once __DIR__ . '/../includes/funciones.php';
requerirLogin();

$tipos = ['split' => 'Split', 'ventana' => 'Ventana', 'portatil' => 'Portátil', 'piso_techo' => 'Piso-techo'];

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    verificarCsrf();
    if (!esAdmin()) {
        flash('Solo el administrador puede modificar los equipos en venta.', 'warning');
        redirigir('equipos.php');
    }
    $accion = $_POST['accion'] ?? 'guardar';
    $id = (int)($_POST['id'] ?? 0);

    if ($accion === 'activo') {
        $pdo->prepare('UPDATE equipos_venta SET activo = 1 - activo WHERE id = ?')->execute([$id]);
        flash('Visibilidad en la web actualizada.');
        redirigir('equipos.php');
    }

    $eq = [
        'marca'       => trim($_POST['marca'] ?? ''),
        'modelo'      => trim($_POST['modelo'] ?? ''),
        'tipo'        => array_key_exists($_POST['tipo'] ?? '', $tipos) ? $_POST['tipo'] : 'split',
        'frigorias'   => (int)($_POST['frigorias'] ?? 0),
        'frio_calor'  => isset($_POST['frio_calor']) ? 1 : 0,
        'inverter'    => isset($_POST['inverter']) ? 1 : 0,
        'precio'      => (float)($_POST['precio'] ?? 0),
        'stock'       => (int)($_POST['stock'] ?? 0),
        'descripcion' => trim($_POST['descripcion'] ?? '') ?: null,
    ];

    if ($eq['marca'] === '' || $eq['modelo'] === '') {
        flash('La marca y el modelo son obligatorios.', 'warning');
    } elseif ($eq['frigorias'] <= 0 || $eq['precio'] <= 0 || $eq['stock'] < 0) {
        flash('Las frigorías y el precio tienen que ser mayores a 0, y el stock no puede ser negativo.', 'warning');
    } else {
        try {
            if ($id) {
                $pdo->prepare('UPDATE equipos_venta SET marca=?, modelo=?, tipo=?, frigorias=?, frio_calor=?, inverter=?, precio=?, stock=?, descripcion=? WHERE id=?')
                   ->execute([...array_values($eq), $id]);
                flash('Equipo actualizado.');
            } else {
                $pdo->prepare('INSERT INTO equipos_venta (marca, modelo, tipo, frigorias, frio_calor, inverter, precio, stock, descripcion) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)')
                   ->execute(array_values($eq));
                flash('Equipo agregado. Ya se ve en la página.');
            }
        } catch (PDOException $ex) {
            flash(mensajeError($ex), 'danger');
        }
    }
    redirigir('equipos.php');
}

$equipos = $pdo->query('SELECT * FROM equipos_venta ORDER BY activo DESC, marca, frigorias')->fetchAll();
$editar = (int)($_GET['editar'] ?? 0);
$actual = ['id' => 0, 'marca' => '', 'modelo' => '', 'tipo' => 'split', 'frigorias' => '', 'frio_calor' => 1, 'inverter' => 0, 'precio' => '', 'stock' => 0, 'descripcion' => ''];
foreach ($equipos as $eq) if ((int)$eq['id'] === $editar) $actual = $eq;
$mostrarForm = esAdmin() && ($editar || isset($_GET['nuevo']));

$titulo = 'Equipos en venta';
$activo = 'equipos';
require __DIR__ . '/../includes/header.php';
?>
<p class="text-muted">Estos son los equipos que se muestran en la sección <b>Equipos en venta</b> de la página. Los que están ocultos no se ven en la web.</p>

<?php if ($mostrarForm): ?>
    <form method="post" class="card card-body shadow-sm mb-4">
        <h2 class="h5"><?= $actual['id'] ? 'Editar equipo' : 'Nuevo equipo' ?></h2>
        <?= campoCsrf() ?><input type="hidden" name="id" value="<?= (int)$actual['id'] ?>">
        <div class="row g-3">
            <div class="col-md-3"><label class="form-label">Marca *</label><input class="form-control" name="marca" maxlength="50" required value="<?= h($actual['marca']) ?>"></div>
            <div class="col-md-4"><label class="form-label">Modelo *</label><input class="form-control" name="modelo" maxlength="80" required value="<?= h($actual['modelo']) ?>"></div>
            <div class="col-md-2">
                <label class="form-label">Tipo</label>
                <select class="form-select" name="tipo">
                    <?php foreach ($tipos as $k => $v): ?><option value="<?= $k ?>" <?= $actual['tipo'] === $k ? 'selected' : '' ?>><?= $v ?></option><?php endforeach; ?>
                </select>
            </div>
            <div class="col-md-3"><label class="form-label">Frigorías *</label><input class="form-control" type="number" name="frigorias" min="1" step="50" required value="<?= h($actual['frigorias']) ?>"></div>
            <div class="col-md-3"><label class="form-label">Precio *</label><input class="form-control" type="number" name="precio" min="1" step="0.01" required value="<?= h($actual['precio']) ?>"></div>
            <div class="col-md-2"><label class="form-label">Stock</label><input class="form-control" type="number" name="stock" min="0" value="<?= (int)$actual['stock'] ?>"></div>
            <div class="col-md-7"><label class="form-label">Descripción</label><input class="form-control" name="descripcion" maxlength="255" value="<?= h($actual['descripcion']) ?>"></div>
            <div class="col-12">
                <div class="form-check form-check-inline"><input class="form-check-input" type="checkbox" name="frio_calor" id="fc" <?= $actual['frio_calor'] ? 'checked' : '' ?>><label class="form-check-label" for="fc">Frío / calor</label></div>
                <div class="form-check form-check-inline"><input class="form-check-input" type="checkbox" name="inverter" id="inv" <?= $actual['inverter'] ? 'checked' : '' ?>><label class="form-check-label" for="inv">Inverter</label></div>
            </div>
        </div>
        <div class="mt-3">
            <button class="btn btn-marca"><i class="bi bi-check-lg"></i> Guardar</button>
            <a class="btn btn-link" href="equipos.php">Cancelar</a>
        </div>
    </form>
<?php elseif (esAdmin()): ?>
    <a class="btn btn-marca mb-3" href="equipos.php?nuevo=1"><i class="bi bi-plus-lg"></i> Nuevo equipo</a>
<?php endif; ?>

<div class="card shadow-sm">
    <div class="table-responsive">
        <table class="table align-middle mb-0">
            <thead class="table-light"><tr><th>Marca</th><th>Modelo</th><th>Tipo</th><th class="text-end">Frigorías</th><th>Características</th><th class="text-end">Precio</th><th class="text-end">Stock</th><th>En la web</th><?= esAdmin() ? '<th></th>' : '' ?></tr></thead>
            <tbody>
            <?php foreach ($equipos as $eq): ?>
                <tr class="<?= $eq['activo'] ? '' : 'text-muted' ?>">
                    <td class="fw-semibold"><?= h($eq['marca']) ?></td>
                    <td><?= h($eq['modelo']) ?></td>
                    <td><?= h($tipos[$eq['tipo']] ?? $eq['tipo']) ?></td>
                    <td class="text-end"><?= number_format((int)$eq['frigorias'], 0, ',', '.') ?></td>
                    <td>
                        <span class="badge text-bg-light"><?= $eq['frio_calor'] ? 'Frío/calor' : 'Solo frío' ?></span>
                        <?php if ($eq['inverter']): ?><span class="badge text-bg-warning">Inverter</span><?php endif; ?>
                    </td>
                    <td class="text-end"><?= pesos($eq['precio']) ?></td>
                    <td class="text-end"><?= (int)$eq['stock'] ?: '<span class="badge text-bg-secondary">A pedido</span>' ?></td>
                    <td><?= $eq['activo'] ? '<span class="badge text-bg-success">Visible</span>' : '<span class="badge text-bg-secondary">Oculto</span>' ?></td>
                    <?php if (esAdmin()): ?>
                        <td class="text-end text-nowrap">
                            <a class="btn btn-sm btn-outline-primary" href="equipos.php?editar=<?= $eq['id'] ?>"><i class="bi bi-pencil"></i> Editar</a>
                            <form method="post" class="d-inline">
                                <?= campoCsrf() ?><input type="hidden" name="accion" value="activo"><input type="hidden" name="id" value="<?= $eq['id'] ?>">
                                <button class="btn btn-sm btn-outline-secondary"><i class="bi bi-eye<?= $eq['activo'] ? '-slash' : '' ?>"></i> <?= $eq['activo'] ? 'Ocultar' : 'Mostrar' ?></button>
                            </form>
                        </td>
                    <?php endif; ?>
                </tr>
            <?php endforeach; ?>
            <?php if (!$equipos): ?><tr><td colspan="9" class="text-muted">Todavía no hay equipos cargados.</td></tr><?php endif; ?>
            </tbody>
        </table>
    </div>
</div>
<?php require __DIR__ . '/../includes/footer.php';
