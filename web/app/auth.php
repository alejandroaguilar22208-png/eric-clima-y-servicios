<?php
/**
 * Control de sesión y roles del panel.
 */
require_once __DIR__ . '/config.php';

function usuario_actual(): ?array
{
    return $_SESSION['usuario'] ?? null;
}

function es_admin(): bool
{
    return (usuario_actual()['rol'] ?? '') === 'admin';
}

/** Si no hay sesión, manda al login. Va al principio de cada página del panel. */
function requerir_login(): void
{
    if (!usuario_actual()) redirigir('login.php');
}

/** RN-09: algunas pantallas son solo para el administrador. */
function requerir_admin(): void
{
    requerir_login();
    if (!es_admin()) {
        http_response_code(403);
        flash('Esa sección es solo para el administrador.', 'warning');
        redirigir('index.php');
    }
}

/** Intenta iniciar sesión. Devuelve true si el usuario y la clave son correctos. */
function iniciar_sesion(string $email, string $clave): bool
{
    $st = db()->prepare('SELECT id, nombre, email, password_hash, rol FROM usuarios WHERE email = ? AND activo = 1');
    $st->execute([trim($email)]);
    $u = $st->fetch();

    if (!$u || !password_verify($clave, $u['password_hash'])) return false;

    session_regenerate_id(true); // nueva sesión al ingresar (evita robo de sesión)
    unset($u['password_hash']);
    $_SESSION['usuario'] = $u;
    return true;
}
