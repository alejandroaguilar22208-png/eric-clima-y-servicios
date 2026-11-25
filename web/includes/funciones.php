<?php
session_start();
require_once __DIR__ . '/../config/db.php';

function h($texto): string
{
    return htmlspecialchars((string)$texto, ENT_QUOTES, 'UTF-8');
}

function pesos($numero): string
{
    return '$ ' . number_format((float)$numero, 0, ',', '.');
}

function redirigir(string $url): void
{
    header('Location: ' . $url);
    exit;
}

function flash(string $mensaje, string $tipo = 'success'): void
{
    $_SESSION['flash'] = ['mensaje' => $mensaje, 'tipo' => $tipo];
}

function mostrarFlash(): string
{
    if (empty($_SESSION['flash'])) return '';
    $f = $_SESSION['flash'];
    unset($_SESSION['flash']);
    return '<div class="alert alert-' . h($f['tipo']) . ' alert-dismissible fade show">' . h($f['mensaje'])
         . '<button type="button" class="btn-close" data-bs-dismiss="alert"></button></div>';
}

function tokenCsrf(): string
{
    if (empty($_SESSION['csrf'])) $_SESSION['csrf'] = bin2hex(random_bytes(32));
    return $_SESSION['csrf'];
}

function campoCsrf(): string
{
    return '<input type="hidden" name="csrf" value="' . h(tokenCsrf()) . '">';
}

function verificarCsrf(): void
{
    if (!hash_equals($_SESSION['csrf'] ?? '', $_POST['csrf'] ?? '')) {
        http_response_code(400);
        exit('Formulario vencido. Volvé atrás y recargá la página.');
    }
}

function mensajeError(PDOException $e): string
{
    $codigo = $e->errorInfo[1] ?? 0;
    if ($codigo == 1062) return 'Ya existe un registro con esos datos (por ejemplo, el técnico ya tiene un trabajo en ese horario).';
    if ($codigo == 1451) return 'No se puede borrar: tiene datos relacionados.';
    return 'Error de base de datos.';
}

function usuarioActual(): ?array
{
    return $_SESSION['usuario'] ?? null;
}

function esAdmin(): bool
{
    return (int)(usuarioActual()['tipo_usuario_id'] ?? 0) === 1;
}

function requerirLogin(): void
{
    if (!usuarioActual()) redirigir('login.php');
}

function requerirAdmin(): void
{
    requerirLogin();
    if (!esAdmin()) {
        flash('Esa sección es solo para el administrador.', 'warning');
        redirigir('index.php');
    }
}

function iniciarSesion(string $email, string $clave): bool
{
    global $pdo;
    $st = $pdo->prepare('SELECT u.id, u.nombre, u.email, u.password_hash, u.tipo_usuario_id, t.nombre AS tipo
                         FROM usuarios u JOIN tipo_usuarios t ON t.id = u.tipo_usuario_id
                         WHERE u.email = ? AND u.activo = 1');
    $st->execute([trim($email)]);
    $u = $st->fetch();

    if (!$u || !password_verify($clave, $u['password_hash'])) return false;

    session_regenerate_id(true);
    unset($u['password_hash']);
    $_SESSION['usuario'] = $u;
    return true;
}

function badgeEstado(string $estado): string
{
    $colores = ['pendiente' => 'warning', 'agendado' => 'primary', 'realizado' => 'success', 'cancelado' => 'secondary',
                'contactado' => 'info', 'convertida' => 'success', 'aceptado' => 'success', 'vencido' => 'secondary'];
    return '<span class="badge text-bg-' . ($colores[$estado] ?? 'light') . '">' . h(ucfirst($estado)) . '</span>';
}
