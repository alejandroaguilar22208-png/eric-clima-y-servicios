<?php

require_once __DIR__ . '/../app/auth.php';
require_once __DIR__ . '/../app/layout.php';
requerir_login();

$db = db();

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    csrf_verificar();
    $id = (int)($_POST['id'] ?? 0);
    $accion = $_POST['accion'] ?? '';

    $st = $db->prepare('SELECT * FROM solicitudes WHERE id = ?');
    $st->execute([$id]);
    $sol = $st->fetch();

    if (!$sol) {
        flash('La solicitud no existe.', 'danger');
    } elseif ($accion === 'descartada') {
        $st = $db->prepare("DELETE FROM solicitudes WHERE id = ? AND estado IN ('pendiente', 'contactado')");
        $st->execute([$id]);
        flash($st->rowCount() ? 'Solicitud de ' . $sol['nombre'] . ' descartada y borrada.' : 'Esa solicitud ya no se puede descartar.',
              $st->rowCount() ? 'success' : 'warning');
    } elseif ($accion === 'contactado') {
        $db->prepare('UPDATE solicitudes SET estado = ? WHERE id = ?')->execute([$accion, $id]);
        flash('Solicitud actualizada.');
    } elseif ($accion === 'convertir') {
        if ($sol['estado'] === 'convertida') {
            flash('Esa solicitud ya fue convertida.', 'warning');
        } else {
            try {
                $db->beginTransaction();
                $db->prepare('INSERT INTO clientes (nombre, telefono, direccion) VALUES (?, ?, ?)')
                   ->execute([$sol['nombre'], $sol['telefono'], $sol['direccion'] ?: 'A confirmar']);
                $clienteId = (int)$db->lastInsertId();

                $servicioId = $sol['servicio_id'] ?: $db->query('SELECT MIN(id) FROM servicios')->fetchColumn();
                $precio = $db->prepare('SELECT precio_base FROM servicios WHERE id = ?');
                $precio->execute([$servicioId]);
                $db->prepare('INSERT INTO trabajos (cliente_id, servicio_id, total, observaciones) VALUES (?, ?, ?, ?)')
                   ->execute([$clienteId, $servicioId, $precio->fetchColumn() ?: 0, $sol['mensaje']]);

                $db->prepare("UPDATE solicitudes SET estado = 'convertida', cliente_id = ? WHERE id = ?")
                   ->execute([$clienteId, $id]);
                $db->commit();
                flash('Se creó el cliente y un trabajo pendiente. Ahora agendalo en Trabajos.');
            } catch (PDOException $ex) {
                $db->rollBack();
                flash(mensaje_error($ex), 'danger');
            }
        }
    }
    redirigir('solicitudes.php');
}

$solicitudes = $db->query(
    "SELECT so.*, s.nombre AS servicio FROM solicitudes so LEFT JOIN servicios s ON s.id = so.servicio_id
     ORDER BY so.estado = 'pendiente' DESC, so.fecha DESC"
)->fetchAll();

layout_inicio('Solicitudes desde la web', 'solicitudes');
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
                    <td><?= e($s['nombre']) ?><br><small class="text-muted"><?= e($s['direccion']) ?></small></td>
                    <td class="text-nowrap"><a href="https://wa.me/549<?= e(preg_replace('/\D/', '', $s['telefono'])) ?>" target="_blank" rel="noopener"><?= e($s['telefono']) ?></a></td>
                    <td><?= e($s['servicio'] ?? 'Otro') ?></td>
                    <td class="small"><?= e($s['mensaje']) ?></td>
                    <td><?= badge_estado($s['estado']) ?></td>
                    <td class="text-end text-nowrap">
                        <?php if ($s['estado'] === 'pendiente' || $s['estado'] === 'contactado'): ?>
                            <form method="post" class="d-inline">
                                <?= csrf_campo() ?><input type="hidden" name="id" value="<?= $s['id'] ?>">
                                <button name="accion" value="convertir" class="btn btn-sm btn-marca">Convertir en trabajo</button>
                                <?php if ($s['estado'] === 'pendiente'): ?>
                                    <button name="accion" value="contactado" class="btn btn-sm btn-outline-secondary">Contactado</button>
                                <?php endif; ?>
                                <button name="accion" value="descartada" class="btn btn-sm btn-outline-danger" onclick="return confirm(<?= e(json_encode('¿Seguro que querés descartar la solicitud de ' . $s['nombre'] . "?

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
<?php layout_fin();
