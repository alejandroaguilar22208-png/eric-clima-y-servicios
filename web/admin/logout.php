<?php
require_once __DIR__ . '/../app/config.php';

$_SESSION = [];
session_destroy();
redirigir('login.php');
