<?php
require_once __DIR__ . '/../includes/funciones.php';
requerirLogin();


$pdo->exec("UPDATE presupuestos SET estado = 'vencido' WHERE estado = 'pendiente' AND vence < CURDATE()");

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    verificarCsrf();
    $accion = $_POST['accion'] ?? '';

    if ($accion === 'nuevo') {
        $cliente = (int)($_POST['cliente_id'] ?? 0);
        $vence   = ($_POST['vence'] ?? '') !== '' ? $_POST['vence'] : date('Y-m-d', strtotime('+15 days'));

        $items = [];
        foreach ($_POST['descripcion'] ?? [] as $i => $desc) {
            $desc   = trim($desc);
            $cant   = (float)($_POST['cantidad'][$i] ?? 0);
            $precio = (float)($_POST['precio'][$i] ?? 0);
            if ($desc === '' && $cant == 0 && $precio == 0) continue;
            $items[] = [$desc, $cant, $precio];
        }

        $error = '';
        if ($cliente === 0) $error = 'Elegí un cliente.';
        elseif (!$items) $error = 'Agregá al menos un ítem.';
        else foreach ($items as [$d, $c, $p]) {
            if ($d === '' || $c <= 0 || $p <= 0) { $error = 'Cada ítem necesita descripción, cantidad y precio mayores a 0.'; break; }
        }

        if ($error) {
            flash($error, 'warning');
            redirigir('presupuestos.php?nuevo=1');
        }

        $total = array_sum(array_map(fn($it) => $it[1] * $it[2], $items));
        try {
            $pdo->beginTransaction();
            $pdo->prepare('INSERT INTO presupuestos (cliente_id, fecha, vence, total) VALUES (?, CURDATE(), ?, ?)')
               ->execute([$cliente, $vence, $total]);
            $id = (int)$pdo->lastInsertId();
            $st = $pdo->prepare('INSERT INTO presupuesto_items (presupuesto_id, descripcion, cantidad, precio_unitario) VALUES (?, ?, ?, ?)');
            foreach ($items as $it) $st->execute([$id, ...$it]);
            $pdo->commit();
            flash("Presupuesto N.º $id guardado.");
            redirigir("presupuesto.php?id=$id");
        } catch (PDOException $ex) {
            $pdo->rollBack();
            flash(mensajeError($ex), 'danger');
            redirigir('presupuestos.php?nuevo=1');
        }
    }

    if ($accion === 'estado' && ($_POST['estado'] ?? '') === 'rechazado') {
        $id = (int)($_POST['id'] ?? 0);
        $st = $pdo->prepare("DELETE FROM presupuestos WHERE id = ? AND estado = 'pendiente'");
        $st->execute([$id]);
        flash($st->rowCount() ? 'Presupuesto rechazado y borrado.' : 'Ese presupuesto ya no está pendiente.',
              $st->rowCount() ? 'success' : 'warning');
        redirigir('presupuestos.php');
    }

    if ($accion === 'estado') {
        $nuevo = in_array($_POST['estado'] ?? '', ['aceptado'], true) ? $_POST['estado'] : null;
        $id = (int)($_POST['id'] ?? 0);
        $st = $pdo->prepare("UPDATE presupuestos SET estado = ? WHERE id = ? AND estado = 'pendiente'");
        $st->execute([$nuevo, $id]);
        flash($nuevo && $st->rowCount() ? "Presupuesto marcado como $nuevo." : 'Ese presupuesto ya no está pendiente.',
              $st->rowCount() ? 'success' : 'warning');
        redirigir('presupuestos.php');
    }
}

$presupuestos = $pdo->query(
    'SELECT p.*, c.nombre AS cliente FROM presupuestos p JOIN clientes c ON c.id = p.cliente_id
     ORDER BY p.fecha DESC, p.id DESC')->fetchAll();
$clientes   = $pdo->query('SELECT id, nombre, direccion FROM clientes WHERE activo = 1 ORDER BY nombre')->fetchAll();
$servicios  = $pdo->query('SELECT nombre, precio_base AS precio FROM servicios ORDER BY nombre')->fetchAll();
$materiales = $pdo->query("SELECT CONCAT(nombre, ' (', unidad, ')') AS nombre, precio FROM materiales ORDER BY nombre")->fetchAll();

$titulo = 'Presupuestos';
$activo = 'presupuestos';
require __DIR__ . '/../includes/header.php';
?>
<?php if (isset($_GET['nuevo'])): ?>
    <form method="post" class="card card-body shadow-sm mb-4" id="form-presupuesto">
        <h2 class="h5">Nuevo presupuesto</h2>
        <?= campoCsrf() ?><input type="hidden" name="accion" value="nuevo">
        <div class="row g-3 mb-3">
            <div class="col-md-6">
                <label class="form-label">Cliente *</label>
                <select class="form-select" name="cliente_id" required>
                    <option value="">Elegí…</option>
                    <?php foreach ($clientes as $c): ?>
                        <option value="<?= $c['id'] ?>"><?= h($c['nombre'] . ' - ' . $c['direccion']) ?></option>
                    <?php endforeach; ?>
                </select>
            </div>
            <div class="col-md-3">
                <label class="form-label">Vence</label>
                <input class="form-control" type="date" name="vence">
                <div class="form-text">Vacío = 15 días.</div>
            </div>
            <div class="col-md-3">
                <label class="form-label">Agregar rápido</label>
                <select class="form-select" id="rapido">
                    <option value="">Servicio o material…</option>
                    <optgroup label="Servicios">
                        <?php foreach ($servicios as $s): ?><option data-precio="<?= (float)$s['precio'] ?>"><?= h($s['nombre']) ?></option><?php endforeach; ?>
                    </optgroup>
                    <optgroup label="Materiales">
                        <?php foreach ($materiales as $m): ?><option data-precio="<?= (float)$m['precio'] ?>"><?= h($m['nombre']) ?></option><?php endforeach; ?>
                    </optgroup>
                </select>
            </div>
        </div>

        <table class="table align-middle">
            <thead class="table-light"><tr><th>Descripción</th><th style="width:120px">Cantidad</th><th style="width:170px">Precio unit.</th><th style="width:150px" class="text-end">Subtotal</th><th style="width:50px"></th></tr></thead>
            <tbody id="items"></tbody>
        </table>
        <div class="d-flex justify-content-between align-items-center">
            <button type="button" class="btn btn-outline-primary btn-sm" id="agregar">+ Agregar renglón</button>
            <div class="fs-5 fw-bold">Total: <span id="total">$ 0</span></div>
        </div>
        <div class="mt-3">
            <button class="btn btn-marca">Guardar presupuesto</button>
            <a class="btn btn-link" href="presupuestos.php">Cancelar</a>
        </div>
    </form>

    <template id="fila">
        <tr>
            <td><input class="form-control form-control-sm" name="descripcion[]" maxlength="150"></td>
            <td><input class="form-control form-control-sm" name="cantidad[]" type="number" step="0.01" min="0" value="1"></td>
            <td><input class="form-control form-control-sm" name="precio[]" type="number" step="0.01" min="0"></td>
            <td class="text-end subtotal">$ 0</td>
            <td><button type="button" class="btn btn-sm btn-outline-danger quitar" title="Quitar">×</button></td>
        </tr>
    </template>

    <script>
    const tbody = document.getElementById('items');
    const plantilla = document.getElementById('fila');
    const formato = n => n.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' });

    function agregarFila(desc = '', precio = '') {
        const fila = plantilla.content.cloneNode(true);
        fila.querySelector('[name="descripcion[]"]').value = desc;
        fila.querySelector('[name="precio[]"]').value = precio;
        tbody.appendChild(fila);
        recalcular();
    }

    function recalcular() {
        let total = 0;
        tbody.querySelectorAll('tr').forEach(tr => {
            const cant = parseFloat(tr.querySelector('[name="cantidad[]"]').value) || 0;
            const precio = parseFloat(tr.querySelector('[name="precio[]"]').value) || 0;
            tr.querySelector('.subtotal').textContent = formato(cant * precio);
            total += cant * precio;
        });
        document.getElementById('total').textContent = formato(total);
    }

    document.getElementById('agregar').onclick = () => agregarFila();
    tbody.addEventListener('input', recalcular);
    tbody.addEventListener('click', ev => {
        if (ev.target.classList.contains('quitar')) { ev.target.closest('tr').remove(); recalcular(); }
    });
    document.getElementById('rapido').onchange = ev => {
        const op = ev.target.selectedOptions[0];
        if (op.dataset.precio) agregarFila(op.textContent, op.dataset.precio);
        ev.target.value = '';
    };
    agregarFila();
    </script>
<?php else: ?>
    <a class="btn btn-marca mb-3" href="presupuestos.php?nuevo=1">+ Nuevo presupuesto</a>
<?php endif; ?>

<div class="card shadow-sm">
    <div class="table-responsive">
        <table class="table align-middle mb-0">
            <thead class="table-light"><tr><th>N.º</th><th>Cliente</th><th>Fecha</th><th>Vence</th><th class="text-end">Total</th><th>Estado</th><th></th></tr></thead>
            <tbody>
            <?php foreach ($presupuestos as $p): ?>
                <tr>
                    <td><?= str_pad($p['id'], 5, '0', STR_PAD_LEFT) ?></td>
                    <td><?= h($p['cliente']) ?></td>
                    <td><?= date('d/m/Y', strtotime($p['fecha'])) ?></td>
                    <td><?= $p['vence'] ? date('d/m/Y', strtotime($p['vence'])) : '' ?></td>
                    <td class="text-end"><?= pesos($p['total']) ?></td>
                    <td><?= badgeEstado($p['estado']) ?></td>
                    <td class="text-end text-nowrap">
                        <a class="btn btn-sm btn-outline-primary" href="presupuesto.php?id=<?= $p['id'] ?>">Ver / PDF</a>
                        <?php if ($p['estado'] === 'pendiente'): ?>
                            <form method="post" class="d-inline">
                                <?= campoCsrf() ?><input type="hidden" name="accion" value="estado"><input type="hidden" name="id" value="<?= $p['id'] ?>">
                                <button class="btn btn-sm btn-outline-success" name="estado" value="aceptado">Aceptado</button>
                                <button class="btn btn-sm btn-outline-danger" name="estado" value="rechazado" onclick="return confirm(<?= h(json_encode('¿Seguro que querés rechazar el presupuesto N.º ' . str_pad($p['id'], 5, '0', STR_PAD_LEFT) . ' de ' . $p['cliente'] . "?

Se va a borrar y no va a aparecer más, ni acá ni en el programa de escritorio.", JSON_UNESCAPED_UNICODE)) ?>)">Rechazado</button>
                            </form>
                        <?php endif; ?>
                    </td>
                </tr>
            <?php endforeach; ?>
            <?php if (!$presupuestos): ?><tr><td colspan="7" class="text-muted">Todavía no hay presupuestos.</td></tr><?php endif; ?>
            </tbody>
        </table>
    </div>
</div>
<?php require __DIR__ . '/../includes/footer.php';
