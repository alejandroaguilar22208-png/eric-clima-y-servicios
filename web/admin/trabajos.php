<?php

require_once __DIR__ . '/../includes/funciones.php';
requerirLogin();


if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    verificarCsrf();
    $accion = $_POST['accion'] ?? '';
    $id = (int)($_POST['id'] ?? 0);
    $volver = 'trabajos.php' . ($id ? '?ver=' . $id : '');

    $st = $pdo->prepare('SELECT * FROM trabajos WHERE id = ?');
    $st->execute([$id]);
    $trabajo = $st->fetch();
    $cerrado = $trabajo && in_array($trabajo['estado'], ['realizado', 'cancelado'], true);

    try {
        if ($accion !== 'nuevo' && !$trabajo) {
            flash('Ese trabajo no existe.', 'danger');
            redirigir('trabajos.php');
        }
        if ($accion !== 'nuevo' && $cerrado) {
            flash('Un trabajo realizado o cancelado no se puede modificar.', 'warning');
            redirigir($volver);
        }
        switch ($accion) {
            case 'nuevo':
                $clienteId = (int)($_POST['cliente_id'] ?? 0);
                $servicioId = (int)($_POST['servicio_id'] ?? 0);
                $total = (float)($_POST['total'] ?? 0);
                if (!$clienteId || !$servicioId) { flash('Elegí cliente y servicio.', 'warning'); break; }
                if ($total < 0) { flash('El total no puede ser negativo.', 'warning'); break; }
                $pdo->prepare('INSERT INTO trabajos (cliente_id, servicio_id, total, observaciones) VALUES (?, ?, ?, ?)')
                   ->execute([$clienteId, $servicioId, $total, trim($_POST['observaciones'] ?? '') ?: null]);
                flash('Trabajo creado. Ahora agendalo.');
                $volver = 'trabajos.php?ver=' . $pdo->lastInsertId();
                break;

            case 'agendar':
                $fecha = $_POST['fecha'] ?? '';
                $tecnico = (int)($_POST['tecnico_id'] ?? 0);
                if (!$fecha || !$tecnico) { flash('Elegí fecha y técnico.', 'warning'); break; }
                if (strtotime($fecha) < strtotime('today')) { flash('La fecha no puede ser anterior a hoy.', 'warning'); break; }
                $pdo->prepare("UPDATE trabajos SET fecha_agendada = ?, tecnico_id = ?, estado = 'agendado' WHERE id = ?")
                   ->execute([str_replace('T', ' ', $fecha) . ':00', $tecnico, $id]);
                flash('Trabajo agendado.');
                break;

            case 'material':
                $materialId = (int)($_POST['material_id'] ?? 0);
                $cantidad = (float)($_POST['cantidad'] ?? 0);
                if (!$materialId || $cantidad <= 0) { flash('Elegí material y una cantidad mayor a 0.', 'warning'); break; }
                $st = $pdo->prepare('SELECT m.stock_actual, COALESCE(tm.cantidad, 0) AS ya_cargado
                                     FROM materiales m LEFT JOIN trabajo_material tm ON tm.material_id = m.id AND tm.trabajo_id = ?
                                     WHERE m.id = ?');
                $st->execute([$id, $materialId]);
                $stock = $st->fetch();
                if (!$stock || $stock['ya_cargado'] + $cantidad > $stock['stock_actual']) {
                    flash('Stock insuficiente para el material solicitado.', 'warning');
                    break;
                }
                $pdo->prepare(
                    'INSERT INTO trabajo_material (trabajo_id, material_id, cantidad) VALUES (?, ?, ?)
                     ON DUPLICATE KEY UPDATE cantidad = cantidad + VALUES(cantidad)'
                )->execute([$id, $materialId, $cantidad]);
                flash('Material agregado al trabajo.');
                break;

            case 'realizado':
                if (!$trabajo['fecha_agendada'] || !$trabajo['tecnico_id']) {
                    flash('Para marcarlo realizado, el trabajo necesita fecha y técnico.', 'warning');
                    break;
                }
                $st = $pdo->prepare('SELECT m.nombre, m.stock_actual, tm.cantidad
                                     FROM trabajo_material tm JOIN materiales m ON m.id = tm.material_id
                                     WHERE tm.trabajo_id = ?');
                $st->execute([$id]);
                $usados = $st->fetchAll();
                foreach ($usados as $u) {
                    if ($u['cantidad'] > $u['stock_actual']) {
                        flash('No alcanza el stock de ' . $u['nombre'] . '.', 'warning');
                        redirigir($volver);
                    }
                }
                $pdo->beginTransaction();
                $pdo->prepare('UPDATE materiales m JOIN trabajo_material tm ON tm.material_id = m.id
                               SET m.stock_actual = m.stock_actual - tm.cantidad WHERE tm.trabajo_id = ?')->execute([$id]);
                $pdo->prepare("UPDATE trabajos SET estado = 'realizado', fecha_realizado = CURDATE(),
                               garantia_hasta = CURDATE() + INTERVAL 12 MONTH WHERE id = ?")->execute([$id]);
                $pdo->commit();
                flash('Trabajo realizado. Se descontó el stock y la garantía corre 12 meses.');
                break;

            case 'cancelar':
                $pdo->prepare("UPDATE trabajos SET estado = 'cancelado' WHERE id = ?")->execute([$id]);
                flash('Trabajo cancelado.');
                break;
        }
    } catch (PDOException $ex) {
        if ($pdo->inTransaction()) $pdo->rollBack();
        flash(mensajeError($ex), 'danger');
    }
    redirigir($volver);
}

$estados = ['pendiente', 'agendado', 'realizado', 'cancelado'];
$filtro = in_array($_GET['estado'] ?? '', $estados, true) ? $_GET['estado'] : '';

$sql = 'SELECT t.*, c.nombre AS cliente, c.direccion, s.nombre AS servicio, u.nombre AS tecnico
        FROM trabajos t
        JOIN clientes c ON c.id = t.cliente_id
        JOIN servicios s ON s.id = t.servicio_id
        LEFT JOIN usuarios u ON u.id = t.tecnico_id';
$st = $pdo->prepare($sql . ($filtro ? ' WHERE t.estado = ?' : '') . ' ORDER BY t.fecha_agendada IS NULL, t.fecha_agendada, t.id DESC');
$st->execute($filtro ? [$filtro] : []);
$trabajos = $st->fetchAll();

$clientes   = $pdo->query('SELECT id, nombre FROM clientes WHERE activo = 1 ORDER BY nombre')->fetchAll();
$servicios  = $pdo->query('SELECT id, nombre, precio_base FROM servicios WHERE activo = 1 ORDER BY nombre')->fetchAll();
$tecnicos   = $pdo->query('SELECT id, nombre FROM usuarios WHERE activo = 1 ORDER BY nombre')->fetchAll();
$materiales = $pdo->query('SELECT id, nombre, stock_actual, unidad FROM materiales ORDER BY nombre')->fetchAll();

$detalle = null;
if (isset($_GET['ver'])) {
    $st = $pdo->prepare($sql . ' WHERE t.id = ?');
    $st->execute([(int)$_GET['ver']]);
    $detalle = $st->fetch();
    if ($detalle) {
        $st = $pdo->prepare('SELECT m.nombre, m.unidad, tm.cantidad FROM trabajo_material tm JOIN materiales m ON m.id = tm.material_id WHERE tm.trabajo_id = ?');
        $st->execute([$detalle['id']]);
        $usados = $st->fetchAll();
    }
}

$titulo = 'Trabajos';
$activo = 'trabajos';
require __DIR__ . '/../includes/header.php';
?>

<?php if ($detalle):
    $cerrado = in_array($detalle['estado'], ['realizado', 'cancelado'], true); ?>
    <div class="card shadow-sm mb-4">
        <div class="card-header d-flex justify-content-between align-items-center">
            <span class="fw-semibold">Trabajo N.º <?= $detalle['id'] ?> · <?= h($detalle['servicio']) ?> <?= badgeEstado($detalle['estado']) ?></span>
            <a href="trabajos.php" class="btn-close" aria-label="Cerrar"></a>
        </div>
        <div class="card-body">
            <p class="mb-1"><strong>Cliente:</strong> <?= h($detalle['cliente']) ?> — <?= h($detalle['direccion']) ?></p>
            <p class="mb-1"><strong>Fecha:</strong> <?= $detalle['fecha_agendada'] ? date('d/m/Y H:i', strtotime($detalle['fecha_agendada'])) : 'sin agendar' ?>
               · <strong>Técnico:</strong> <?= h($detalle['tecnico'] ?? '-') ?> · <strong>Total:</strong> <?= pesos($detalle['total']) ?></p>
            <?php if ($detalle['garantia_hasta']): ?><p class="mb-1"><strong>Garantía hasta:</strong> <?= date('d/m/Y', strtotime($detalle['garantia_hasta'])) ?></p><?php endif; ?>
            <?php if ($detalle['observaciones']): ?><p class="mb-1 text-muted"><?= h($detalle['observaciones']) ?></p><?php endif; ?>

            <h2 class="h6 mt-3">Materiales</h2>
            <ul class="mb-3">
                <?php foreach ($usados as $u): ?><li><?= h($u['nombre']) ?>: <?= (float)$u['cantidad'] ?> <?= h($u['unidad']) ?></li><?php endforeach; ?>
                <?php if (!$usados): ?><li class="text-muted">Sin materiales cargados.</li><?php endif; ?>
            </ul>

            <?php if (!$cerrado): ?>
                <div class="row g-3">
                    <div class="col-lg-5">
                        <form method="post" class="border rounded p-3 h-100">
                            <?= campoCsrf() ?><input type="hidden" name="accion" value="agendar"><input type="hidden" name="id" value="<?= $detalle['id'] ?>">
                            <h3 class="h6">1. Agendar</h3>
                            <input class="form-control form-control-sm mb-2" type="datetime-local" name="fecha" required
                                   value="<?= $detalle['fecha_agendada'] ? date('Y-m-d\TH:i', strtotime($detalle['fecha_agendada'])) : date('Y-m-d\T09:00', strtotime('+1 day')) ?>">
                            <select class="form-select form-select-sm mb-2" name="tecnico_id">
                                <?php foreach ($tecnicos as $tc): ?><option value="<?= $tc['id'] ?>" <?= $detalle['tecnico_id'] == $tc['id'] ? 'selected' : '' ?>><?= h($tc['nombre']) ?></option><?php endforeach; ?>
                            </select>
                            <button class="btn btn-sm btn-marca">Agendar</button>
                        </form>
                    </div>
                    <div class="col-lg-4">
                        <form method="post" class="border rounded p-3 h-100">
                            <?= campoCsrf() ?><input type="hidden" name="accion" value="material"><input type="hidden" name="id" value="<?= $detalle['id'] ?>">
                            <h3 class="h6">2. Cargar material</h3>
                            <select class="form-select form-select-sm mb-2" name="material_id">
                                <?php foreach ($materiales as $m): ?><option value="<?= $m['id'] ?>"><?= h($m['nombre']) ?> (hay <?= (float)$m['stock_actual'] ?> <?= h($m['unidad']) ?>)</option><?php endforeach; ?>
                            </select>
                            <input class="form-control form-control-sm mb-2" type="number" step="0.01" min="0.01" name="cantidad" value="1">
                            <button class="btn btn-sm btn-outline-primary">Agregar</button>
                        </form>
                    </div>
                    <div class="col-lg-3">
                        <div class="border rounded p-3 h-100">
                            <h3 class="h6">3. Cerrar</h3>
                            <form method="post" onsubmit="return confirm('¿Marcar como realizado? Se descuenta el stock y no se podrá modificar.')">
                                <?= campoCsrf() ?><input type="hidden" name="id" value="<?= $detalle['id'] ?>">
                                <button name="accion" value="realizado" class="btn btn-sm btn-success w-100 mb-2">Marcar realizado</button>
                            </form>
                            <form method="post" onsubmit="return confirm('¿Cancelar este trabajo?')">
                                <?= campoCsrf() ?><input type="hidden" name="id" value="<?= $detalle['id'] ?>">
                                <button name="accion" value="cancelar" class="btn btn-sm btn-outline-secondary w-100">Cancelar trabajo</button>
                            </form>
                        </div>
                    </div>
                </div>
            <?php endif; ?>
        </div>
    </div>
<?php endif; ?>

<div class="d-flex flex-wrap gap-2 mb-3 align-items-center">
    <div class="btn-group flex-wrap">
        <a href="trabajos.php" class="btn btn-sm <?= $filtro === '' ? 'btn-marca' : 'btn-outline-secondary' ?>">Todos</a>
        <?php foreach ($estados as $es): ?>
            <a href="trabajos.php?estado=<?= $es ?>" class="btn btn-sm <?= $filtro === $es ? 'btn-marca' : 'btn-outline-secondary' ?>"><?= ucfirst($es) ?></a>
        <?php endforeach; ?>
    </div>
    <button class="btn btn-marca ms-auto" data-bs-toggle="collapse" data-bs-target="#nuevoTrabajo">+ Nuevo trabajo</button>
</div>

<div class="collapse mb-3" id="nuevoTrabajo">
    <form method="post" class="card card-body shadow-sm">
        <?= campoCsrf() ?><input type="hidden" name="accion" value="nuevo">
        <div class="row g-2">
            <div class="col-md-4"><label class="form-label small">Cliente *</label>
                <select class="form-select" name="cliente_id" required><option value="">Elegir…</option>
                    <?php foreach ($clientes as $c): ?><option value="<?= $c['id'] ?>"><?= h($c['nombre']) ?></option><?php endforeach; ?>
                </select></div>
            <div class="col-md-3"><label class="form-label small">Servicio *</label>
                <select class="form-select" name="servicio_id" required onchange="document.getElementById('total').value = this.selectedOptions[0].dataset.precio || 0">
                    <option value="">Elegir…</option>
                    <?php foreach ($servicios as $s): ?><option value="<?= $s['id'] ?>" data-precio="<?= (float)$s['precio_base'] ?>"><?= h($s['nombre']) ?></option><?php endforeach; ?>
                </select></div>
            <div class="col-md-2"><label class="form-label small">Total ($)</label>
                <input class="form-control" type="number" min="0" step="1" name="total" id="total" value="0"></div>
            <div class="col-md-3"><label class="form-label small">Observaciones</label>
                <input class="form-control" name="observaciones"></div>
        </div>
        <div class="mt-2"><button class="btn btn-marca">Crear trabajo</button></div>
    </form>
</div>

<div class="card shadow-sm">
    <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
            <thead class="table-light"><tr><th>N.º</th><th>Cliente</th><th>Servicio</th><th>Fecha</th><th>Técnico</th><th>Total</th><th>Estado</th><th></th></tr></thead>
            <tbody>
            <?php foreach ($trabajos as $t): ?>
                <tr>
                    <td><?= $t['id'] ?></td>
                    <td><?= h($t['cliente']) ?></td>
                    <td><?= h($t['servicio']) ?></td>
                    <td class="text-nowrap"><?= $t['fecha_agendada'] ? date('d/m/Y H:i', strtotime($t['fecha_agendada'])) : '-' ?></td>
                    <td><?= h($t['tecnico'] ?? '-') ?></td>
                    <td class="text-nowrap"><?= pesos($t['total']) ?></td>
                    <td><?= badgeEstado($t['estado']) ?></td>
                    <td class="text-end"><a class="btn btn-sm btn-outline-primary" href="trabajos.php?ver=<?= $t['id'] ?>">Abrir</a></td>
                </tr>
            <?php endforeach; ?>
            <?php if (!$trabajos): ?><tr><td colspan="8" class="text-center text-muted py-4">No hay trabajos.</td></tr><?php endif; ?>
            </tbody>
        </table>
    </div>
</div>
<?php require __DIR__ . '/../includes/footer.php';
