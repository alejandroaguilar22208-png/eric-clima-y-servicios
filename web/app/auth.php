<?php

require_once __DIR__ . '/config.php';

function usuario_actual(): ?array
{
    return $_SESSION['usuario'] ?? null;
}

function es_admin(): bool
{
    return (usuario_actual()['rol'] ?? '') === 'admin';
}

function requerir_login(): void
{
    if (!usuario_actual()) redirigir('login.php');
}

function requerir_admin(): void
{
    requerir_login();
    if (!es_admin()) {
        http_response_code(403);
        flash('Esa sección es solo para el administrador.', 'warning');
        redirigir('index.php');
    }
}

function iniciar_sesion(string $email, string $clave): bool
{
    $st = db()->prepare('SELECT id, nombre, email, password_hash, rol FROM usuarios WHERE email = ? AND activo = 1');
    $st->execute([trim($email)]);
    $u = $st->fetch();

    if (!$u || !password_verify($clave, $u['password_hash'])) return false;

    session_regenerate_id(true);
    unset($u['password_hash']);
    $_SESSION['usuario'] = $u;
    return true;
}
