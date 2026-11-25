<?php

$DB_HOST = 'localhost';
$DB_NAME = 'climagestion';
$DB_USER = 'root';
$DB_PASS = '';

const EMPRESA  = 'Eric Clima y Servicios';
const TELEFONO = '291 533-4545';
const WHATSAPP = '5492915334545';
const EMAIL    = 'alejandroaguilar22208@gmail.com';
const ZONA     = 'Bahía Blanca y alrededores';
const API_KEY  = 'clave-de-prueba-cambiar';

date_default_timezone_set('America/Argentina/Buenos_Aires');

try {
    $pdo = new PDO(
        "mysql:host={$DB_HOST};dbname={$DB_NAME};charset=utf8mb4",
        $DB_USER,
        $DB_PASS,
        [
            PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
        ]
    );
    $pdo->exec("SET time_zone = '-03:00'");
} catch (PDOException $e) {
    die('Error de conexión a la base de datos: ' . htmlspecialchars($e->getMessage()));
}
