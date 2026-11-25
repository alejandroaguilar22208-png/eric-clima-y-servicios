<?php

require_once __DIR__ . '/../includes/funciones.php';
requerirLogin();


if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    verificarCsrf();
    $id = (int)($_POST['id'] ?? 0);
    $accion = $_POST['accion'] ?? '';

    $st = $pdo->prepare('SELECT * FROM solicitudes WHERE id = ?');
    $st->execute([$id]);
    $sol = $st->fetch();

    if (!$sol) {
        flash('La solicitud no existe.', 'danger');
    } elseif ($accion === 'descartada') {
        $st = $pdo->prepare("DELETE FROM solicitudes WHERE id = ? AND estado IN ('pendiente', 'contactado')");
        $st->execute([$id]);
        flash($st->rowCount() ? 'Solicitud de ' . $sol['nombre'] . ' descartada y borrada.' : 'Esa solicitud ya no se puede descartar.',
              $st->rowCount() ? 'success' : 'warning');
    } elseif ($accion === 'contactado') {
        $pdo->prepare('UPDATE solicitudes SET estado = ? WHERE id = ?')->execute([$accion, $id]);
        flash('Solicitud actualizada.');
    } elseif ($accion === 'convertir') {
        if ($sol['estado'] === 'convertida') {
            flash('Esa solicitud ya fue convertida.', 'warning');
        } else {
            try {
                $pdo->beginTransaction();
                $pdo->prepare('INSERT INTO clientes (nombre, telefono, direccion) VALUES (?, ?, ?)')
                   ->execute([$sol['nombre'], $sol['telefono'], $sol['direccion'] ?: 'A confirmar']);
                $clienteId = (int)$pdo->lastInsertId();

                $servicioId = $sol['servicio_id'] ?: $pdo->query('SELECT MIN(id) FROM servicios')->fetchColumn();
                $precio = $pdo->prepare('SELECT precio_base FROM servicios WHERE id = ?');
                $precio->execute([$servicioId]);
                $pdo->prepare('INSERT INTO trabajos (cliente_id, servicio_id, total, observaciones) VALUES (?, ?, ?, ?)')
                   ->execute([$clienteId, $servicioId, $precio->fetchColumn() ?: 0, $sol['mensaje']]);

                $pdo->prepare("UPDATE solicitudes SET estado = 'convertida', cliente_id = ? WHERE id = ?")
                   ->execute([$clienteId, $id]);
                $pdo->commit();
                flash('Se creó el cliente y un trabajo pendiente. Ahora agendalo en Trabajos.');
            } catch (PDOException $ex) {
                $pdo->rollBack();
                flash(mensajeError($ex), 'danger');
            }
        }
    }
    redirigir('solicitudes.php');
}

$solicitudes = $pdo->query(
    "SELECT so.*, s.nombre AS servicio FROM solicitudes so LEFT JOIN servicios s ON s.id = so.servicio_id
     ORDER BY so.estado = 'pendiente' DESC, so.fecha DESC"
)->fetchAll();

$titulo = 'Solicitudes desde la web';
$activo = 'solicitudes';
require __DIR__ . '/../includes/header.php';
?>
<div class="card shadow-sm">
    <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
                <tr><th>Fecha</th><th>Nombre</th><th>Teléfono</th><th>Servicio</th><th>Mensaje</th><th>Estado</th><th class="text-end">Acciones</th></tr>
            </thead>
            <tbody>
            <?php foreach ($solicitudes as $s): ?>
                <tr>
                    <td class="text-nowrap"><?= date('d/m H:i', strtotime($s['fecha'])) ?></td>
                    <td><?= h($s['nombre']) ?><br><small class="text-muted"><?= h($s['direccion']) ?></small></td>
                    <td class="text-nowrap"><a href="https://wa.me/549<?= h(preg_replace('/\D/', '', $s['telefono'])) ?>" target="_blank" rel="noopener"><?= h($s['telefono']) ?></a></td>
                    <td><?= h($s['servicio'] ?? 'Otro') ?></td>
                    <td class="small"><?= h($s['mensaje']) ?></td>
                    <td><?= badgeEstado($s['estado']) ?></td>
                    <td class="text-end text-nowrap">
                        <?php if ($s['estado'] === 'pendiente' || $s['estado'] === 'contactado'): ?>
                            <form method="post" class="d-inline">
                                <?= campoCsrf() ?><input type="hidden" name="id" value="<?= $s['id'] ?>">
                                <button name="accion" value="convertir" class="btn btn-sm btn-marca">Convertir en trabajo</button>
                                <?php if ($s['estado'] === 'pendiente'): ?>
                                    <button name="accion" value="contactado" class="btn btn-sm btn-outline-secondary">Contactado</button>
                                <?php endif; ?>
                                <button name="accion" value="descartada" class="btn btn-sm btn-outline-danger" onclick="return confirm(<?= h(json_encode('¿Seguro que querés descartar la solicitud de ' . $s['nombre'] . "?

Se va a borrar y no va a aparecer más, ni acá ni en el programa de escritorio.", JSON_UNESCAPED_UNICODE)) ?>)">Descartar</button>
                            </form>
                        <?php endif; ?>
                    </td>
                </tr>
            <?php endforeach; ?>
            <?php if (!$solicitudes): ?><tr><td colspan="7" class="text-center text-muted py-4">No hay solicitudes.</td></tr><?php endif; ?>
            </tbody>
        </table>
    </div>
</div>
<?php require __DIR__ . '/../includes/footer.php';
