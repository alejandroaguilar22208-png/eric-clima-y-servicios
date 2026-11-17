<?php
require_once __DIR__ . '/../app/config.php';

// Borra todos los datos de la sesión y la cookie
$_SESSION = [];
session_destroy();
redirigir('login.php');
