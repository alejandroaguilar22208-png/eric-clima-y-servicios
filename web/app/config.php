<?php
/**
 * Configuración general y funciones comunes.
 * Todas las páginas incluyen este archivo primero.
 */

session_start();

// ---- Base de datos (la misma que usa el sistema de escritorio) ----
const DB_HOST = 'localhost';
const DB_NAME = 'climagestion';
const DB_USER = 'root';
const DB_PASS = '';

// ---- Datos del negocio (cambiar por los reales) ----
const EMPRESA   = 'Eric Clima y Servicios';
const TELEFONO  = '291 400-0000';          // TODO: teléfono real
const WHATSAPP  = '5492914000000';          // TODO: número real, formato internacional sin + ni espacios
const EMAIL     = 'contacto@ericclima.test'; // TODO: email real
const ZONA      = 'Bahía Blanca y alrededores';

// ---- Clave para la API que usan las apps (cambiar en producción) ----
const API_KEY = 'clave-de-prueba-cambiar';

/** Conexión PDO única (se crea una sola vez por pedido). */
function db(): PDO
{
    static $pdo = null;
    if ($pdo === null) {
        $pdo = new PDO(
            'mysql:host=' . DB_HOST . ';dbname=' . DB_NAME . ';charset=utf8mb4',
            DB_USER,
            DB_PASS,
            [
                PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION, // errores como excepciones
                PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
                PDO::ATTR_EMULATE_PREPARES   => false,                  // consultas preparadas reales
            ]
        );
    }
    return $pdo;
}

/** Escapa texto antes de mostrarlo en HTML (evita XSS). */
function e($texto): string
{
    return htmlspecialchars((string)$texto, ENT_QUOTES, 'UTF-8');
}

/** Formatea un importe en pesos. */
function pesos($n): string
{
    return '$ ' . number_format((float)$n, 0, ',', '.');
}

function redirigir(string $url): void
{
    header('Location: ' . $url);
    exit;
}

/** Mensaje que se muestra una sola vez en la próxima página. */
function flash(string $mensaje, string $tipo = 'success'): void
{
    $_SESSION['flash'] = ['mensaje' => $mensaje, 'tipo' => $tipo];
}

function mostrar_flash(): string
{
    if (empty($_SESSION['flash'])) return '';
    $f = $_SESSION['flash'];
    unset($_SESSION['flash']);
    return '<div class="alert alert-' . e($f['tipo']) . ' alert-dismissible fade show">' . e($f['mensaje'])
         . '<button type="button" class="btn-close" data-bs-dismiss="alert"></button></div>';
}

// ---- Protección CSRF: cada formulario lleva un token secreto ----
function csrf_token(): string
{
    if (empty($_SESSION['csrf'])) $_SESSION['csrf'] = bin2hex(random_bytes(32));
    return $_SESSION['csrf'];
}

function csrf_campo(): string
{
    return '<input type="hidden" name="csrf" value="' . e(csrf_token()) . '">';
}

function csrf_verificar(): void
{
    if (!hash_equals($_SESSION['csrf'] ?? '', $_POST['csrf'] ?? '')) {
        http_response_code(400);
        exit('Formulario vencido. Volvé atrás y recargá la página.');
    }
}

/**
 * Traduce los errores de MySQL a mensajes entendibles.
 * 1644 = mensaje de un TRIGGER (regla de negocio).
 */
function mensaje_error(PDOException $ex): string
{
    $codigo = $ex->errorInfo[1] ?? 0;
    if ($codigo == 1644) return $ex->errorInfo[2];
    if ($codigo == 1062) return 'Ya existe un registro con esos datos (por ejemplo, el técnico ya tiene un trabajo en ese horario).';
    if ($codigo == 1451) return 'No se puede borrar: tiene datos relacionados.';
    return 'Error de base de datos.';
}
