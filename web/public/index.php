<?php
/**
 * Sitio público de Eric Clima y Servicios.
 * Los servicios y la galería se leen de la base (sitio dinámico).
 */
require_once __DIR__ . '/../app/config.php';

try {
    $servicios = db()->query('SELECT id, nombre, descripcion, precio_base FROM servicios WHERE activo = 1 ORDER BY id')->fetchAll();
    $fotos     = db()->query('SELECT titulo, imagen FROM galeria ORDER BY fecha DESC LIMIT 6')->fetchAll();
} catch (PDOException $ex) {
    $servicios = [];
    $fotos = [];
}

// Datos del formulario si volvió con error (para no hacerle escribir todo de nuevo)
$viejo  = $_SESSION['form_viejo'] ?? [];
$errorF = $_SESSION['form_error'] ?? '';
unset($_SESSION['form_viejo'], $_SESSION['form_error']);
$enviado = isset($_GET['enviado']);

$mensajeWa = rawurlencode('Hola ' . EMPRESA . ', quiero consultar por un aire acondicionado.');
?>
<!doctype html>
<html lang="es">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title><?= e(EMPRESA) ?> · Instalación y reparación de aires acondicionados</title>
    <meta name="description" content="Instalación, reparación, carga de gas y mantenimiento de aires acondicionados en <?= e(ZONA) ?>.">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="../assets/css/estilos.css" rel="stylesheet">
</head>
<body>

<!-- Menú -->
<nav class="navbar navbar-expand-lg navbar-dark bg-marca sticky-top">
    <div class="container">
        <a class="navbar-brand fw-bold" href="#inicio"><?= e(EMPRESA) ?></a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#menu">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="menu">
            <ul class="navbar-nav ms-auto">
                <li class="nav-item"><a class="nav-link" href="#servicios">Servicios</a></li>
                <li class="nav-item"><a class="nav-link" href="#como-trabajamos">Cómo trabajamos</a></li>
                <li class="nav-item"><a class="nav-link" href="#trabajos">Trabajos</a></li>
                <li class="nav-item"><a class="nav-link" href="#preguntas">Preguntas</a></li>
                <li class="nav-item"><a class="nav-link fw-semibold" href="#contacto">Pedir presupuesto</a></li>
            </ul>
        </div>
    </div>
</nav>

<!-- Portada -->
<header class="hero" id="inicio">
    <div class="container">
        <div class="row align-items-center">
            <div class="col-lg-7">
                <h1 class="display-5 fw-bold mb-3">Tu aire acondicionado, bien instalado y funcionando todo el año</h1>
                <p class="lead mb-4">Instalación, reparación, carga de gas y mantenimiento de equipos split y de ventana en <?= e(ZONA) ?>. Presupuesto sin cargo.</p>
                <a href="#contacto" class="btn btn-light btn-lg me-2 mb-2">Pedir presupuesto</a>
                <a href="https://wa.me/<?= WHATSAPP ?>?text=<?= $mensajeWa ?>" class="btn btn-outline-light btn-lg mb-2" target="_blank" rel="noopener">Escribir por WhatsApp</a>
            </div>
            <div class="col-lg-5 mt-4 mt-lg-0">
                <ul class="list-unstyled fs-5 mb-0">
                    <li class="mb-2">✔ Garantía de 12 meses en instalaciones</li>
                    <li class="mb-2">✔ Materiales de primera calidad</li>
                    <li class="mb-2">✔ Trabajo prolijo y puntual</li>
                    <li>✔ Aviso para el mantenimiento anual</li>
                </ul>
            </div>
        </div>
    </div>
</header>

<!-- Servicios (desde la base de datos) -->
<section class="seccion" id="servicios">
    <div class="container">
        <h2 class="text-center fw-bold mb-2">Servicios</h2>
        <p class="text-center text-muted mb-5">Precios orientativos. El valor final depende de cada instalación.</p>
        <div class="row g-4">
            <?php foreach ($servicios as $s): ?>
                <div class="col-md-6 col-lg-4">
                    <div class="card tarjeta-servicio h-100 shadow-sm">
                        <div class="card-body">
                            <h3 class="h5 card-title"><?= e($s['nombre']) ?></h3>
                            <p class="card-text text-muted"><?= e($s['descripcion']) ?></p>
                        </div>
                        <div class="card-footer bg-white border-0 pb-3">
                            <span class="text-marca fw-semibold">Desde <?= pesos($s['precio_base']) ?></span>
                        </div>
                    </div>
                </div>
            <?php endforeach; ?>
            <?php if (!$servicios): ?>
                <p class="text-center text-muted">Pronto vas a ver acá nuestros servicios.</p>
            <?php endif; ?>
        </div>
    </div>
</section>

<!-- Cómo trabajamos -->
<section class="seccion seccion-alt" id="como-trabajamos">
    <div class="container">
        <h2 class="text-center fw-bold mb-5">Cómo trabajamos</h2>
        <div class="row g-4 text-center">
            <?php
            $pasos = [
                ['Nos contactás', 'Por el formulario o por WhatsApp, contanos qué necesitás.'],
                ['Te pasamos el presupuesto', 'Si hace falta, vamos a ver el lugar. El presupuesto es sin cargo.'],
                ['Coordinamos el día', 'Elegimos juntos fecha y horario para el trabajo.'],
                ['Instalamos y probamos', 'Dejamos el equipo funcionando, el lugar limpio y la garantía registrada.'],
            ];
            foreach ($pasos as $i => [$titulo, $texto]): ?>
                <div class="col-sm-6 col-lg-3">
                    <div class="paso-numero mb-3"><?= $i + 1 ?></div>
                    <h3 class="h6 fw-bold"><?= $titulo ?></h3>
                    <p class="text-muted small mb-0"><?= $texto ?></p>
                </div>
            <?php endforeach; ?>
        </div>
    </div>
</section>

<!-- Galería (desde la base de datos) -->
<section class="seccion" id="trabajos">
    <div class="container">
        <h2 class="text-center fw-bold mb-5">Trabajos realizados</h2>
        <div class="row g-3 galeria">
            <?php foreach ($fotos as $f): ?>
                <div class="col-sm-6 col-lg-4">
                    <img src="../uploads/<?= e($f['imagen']) ?>" alt="<?= e($f['titulo']) ?>" loading="lazy">
                    <p class="small text-muted mt-1 mb-0"><?= e($f['titulo']) ?></p>
                </div>
            <?php endforeach; ?>
            <?php if (!$fotos): ?>
                <?php for ($i = 0; $i < 3; $i++): ?>
                    <div class="col-sm-6 col-lg-4"><div class="galeria-vacia">Foto de trabajo próximamente</div></div>
                <?php endfor; ?>
            <?php endif; ?>
        </div>
    </div>
</section>

<!-- Preguntas frecuentes -->
<section class="seccion seccion-alt" id="preguntas">
    <div class="container" style="max-width: 820px">
        <h2 class="text-center fw-bold mb-5">Preguntas frecuentes</h2>
        <div class="accordion" id="faq">
            <?php
            $faq = [
                ['¿Cuántas frigorías necesito?', 'Como referencia, unas 50 frigorías por metro cuadrado en un ambiente normal. Un dormitorio de 12 m² necesita alrededor de 2.250 frigorías; un living de 25 m², unas 3.000 a 4.500. Si el ambiente da al sol o tiene mucho vidrio, conviene un equipo más grande.'],
                ['¿Qué incluye la instalación?', 'Hasta 3 metros de cañería de cobre, soportes para la unidad exterior, conexión eléctrica, vacío del circuito y prueba de funcionamiento.'],
                ['¿Cada cuánto hay que hacer mantenimiento?', 'Recomendamos una limpieza completa una vez al año, idealmente antes del verano. Así el equipo enfría mejor y gasta menos luz.'],
                ['¿Tiene garantía?', 'Sí. Las instalaciones tienen 12 meses de garantía sobre la mano de obra.'],
                ['¿En qué zonas trabajan?', ZONA . '.'],
            ];
            foreach ($faq as $i => [$pregunta, $respuesta]): ?>
                <div class="accordion-item">
                    <h3 class="accordion-header">
                        <button class="accordion-button <?= $i ? 'collapsed' : '' ?>" type="button" data-bs-toggle="collapse" data-bs-target="#faq<?= $i ?>"><?= e($pregunta) ?></button>
                    </h3>
                    <div id="faq<?= $i ?>" class="accordion-collapse collapse <?= $i ? '' : 'show' ?>" data-bs-parent="#faq">
                        <div class="accordion-body"><?= e($respuesta) ?></div>
                    </div>
                </div>
            <?php endforeach; ?>
        </div>
    </div>
</section>

<!-- Contacto y formulario de solicitud -->
<section class="seccion" id="contacto">
    <div class="container">
        <div class="row g-5">
            <div class="col-lg-5">
                <h2 class="fw-bold mb-3">Pedí tu presupuesto</h2>
                <p class="text-muted">Completá el formulario y te contactamos a la brevedad. También podés escribirnos directamente.</p>
                <ul class="list-unstyled">
                    <li class="mb-2"><strong>Teléfono:</strong> <?= e(TELEFONO) ?></li>
                    <li class="mb-2"><strong>Email:</strong> <?= e(EMAIL) ?></li>
                    <li class="mb-2"><strong>Zona:</strong> <?= e(ZONA) ?></li>
                    <li class="mb-2"><strong>Horario:</strong> lunes a sábado de 8 a 20 h</li>
                </ul>
                <a href="https://wa.me/<?= WHATSAPP ?>?text=<?= $mensajeWa ?>" class="btn btn-success" target="_blank" rel="noopener">Escribir por WhatsApp</a>
            </div>
            <div class="col-lg-7">
                <?php if ($enviado): ?>
                    <div class="alert alert-success">¡Gracias! Recibimos tu pedido y te vamos a contactar pronto.</div>
                <?php endif; ?>
                <?php if ($errorF): ?>
                    <div class="alert alert-warning"><?= e($errorF) ?></div>
                <?php endif; ?>
                <form method="post" action="solicitud.php" class="card card-body shadow-sm" novalidate>
                    <?= csrf_campo() ?>
                    <!-- Campo trampa: las personas no lo ven, los robots de spam lo completan -->
                    <input type="text" name="sitio_web" class="d-none" tabindex="-1" autocomplete="off">
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label" for="nombre">Nombre y apellido *</label>
                            <input class="form-control" id="nombre" name="nombre" maxlength="120" required value="<?= e($viejo['nombre'] ?? '') ?>">
                        </div>
                        <div class="col-md-6">
                            <label class="form-label" for="telefono">Teléfono *</label>
                            <input class="form-control" id="telefono" name="telefono" maxlength="30" required value="<?= e($viejo['telefono'] ?? '') ?>">
                        </div>
                        <div class="col-md-6">
                            <label class="form-label" for="direccion">Dirección</label>
                            <input class="form-control" id="direccion" name="direccion" maxlength="150" value="<?= e($viejo['direccion'] ?? '') ?>">
                        </div>
                        <div class="col-md-6">
                            <label class="form-label" for="servicio">Servicio</label>
                            <select class="form-select" id="servicio" name="servicio_id">
                                <option value="">No sé / otro</option>
                                <?php foreach ($servicios as $s): ?>
                                    <option value="<?= $s['id'] ?>" <?= ($viejo['servicio_id'] ?? '') == $s['id'] ? 'selected' : '' ?>><?= e($s['nombre']) ?></option>
                                <?php endforeach; ?>
                            </select>
                        </div>
                        <div class="col-12">
                            <label class="form-label" for="mensaje">Contanos qué necesitás</label>
                            <textarea class="form-control" id="mensaje" name="mensaje" rows="4" maxlength="1000" placeholder="Ej: instalar un split de 3000 frigorías en un dormitorio de planta alta"><?= e($viejo['mensaje'] ?? '') ?></textarea>
                        </div>
                        <div class="col-12">
                            <button class="btn btn-marca btn-lg w-100" type="submit">Enviar pedido</button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</section>

<footer class="bg-dark text-white-50 py-4">
    <div class="container d-flex flex-wrap justify-content-between gap-2">
        <span>© <?= date('Y') ?> <?= e(EMPRESA) ?> · <?= e(ZONA) ?></span>
        <a href="../admin/login.php" class="text-white-50">Acceso al panel</a>
    </div>
</footer>

<a href="https://wa.me/<?= WHATSAPP ?>?text=<?= $mensajeWa ?>" class="boton-whatsapp" target="_blank" rel="noopener">WhatsApp</a>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
<script>
// Validación en el navegador antes de enviar (la del servidor está igual en solicitud.php)
document.querySelector('form[action="solicitud.php"]').addEventListener('submit', function (ev) {
    const nombre = this.nombre.value.trim();
    const tel = this.telefono.value.trim();
    let ok = true;
    this.nombre.classList.toggle('is-invalid', nombre.length < 3);
    this.telefono.classList.toggle('is-invalid', !/^[0-9 +()-]{6,}$/.test(tel));
    if (nombre.length < 3 || !/^[0-9 +()-]{6,}$/.test(tel)) ok = false;
    if (!ok) ev.preventDefault();
});
</script>
</body>
</html>
